using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Enemy : Unit, ISpawnable
{
    [Header("value")]
    [SerializeField] private float health;
    [SerializeField] private float maxHealth;

    [Header("for check")]
    [SerializeField] private Vector2Int targetGridPos;
    [SerializeField] private bool isArrived;

    [Header("Status Condition")]
    public int pointillismStacks = 0;
    public bool canMove = true;

    private Player player;
    private Spawner spawner;

    private new void Awake()
    {
        base.Awake();

        // Unit 공통 변수 초기화
        moveTime = 0.2f;
    }

    // 초기화 - 활성화 될 때 공통
    private void OnEnable()
    {
        // 오브젝트 풀링은 OnEnable에서 초기화하기도 함. 싱글톤 참조는 비용이 적음
        player = GameManager.instance.player;
        spawner = Spawner.instance;
        GetComponent<SpriteRenderer>().color = Color.white;

        if (spawner == null)
            spawner = Spawner.instance;
        spawner.AddEnemy(this);
        isMoving = false;
        health = maxHealth;
    }

    // 초기화 - 외부에서 전달받은 데이터로 세팅하는 항목
    public void OnSpawn(SpawnData spawnData, Vector2Int pos)
    {
        health = spawnData.health;
        maxHealth = spawnData.health;

        // 논리 좌표 업데이트
        gridPos = pos;
        GridManager.instance.RegisterOccupant(pos, this.gameObject);

        // 월드 좌표 업데이트
        transform.position = new Vector3(gridPos.x, gridPos.y, 0);
    }

    // 정리 - 비활성화 될 때 공통
    private void OnDisable()
    {
        GridManager.instance.UnRegisterOccupant(gridPos);
        spawner.RemoveEnemy(this);
    }

    // 정리 - 죽었을 때만 호출되는 항목
    void Dead()
    {
        GridManager.instance.UnRegisterOccupant(gridPos);
        
        // gameObject.SetActive(false) 이전에 activeEnemies.Count 체크 필요
        // (SetActive(false) -> OnDisable -> RemoveEnemy 순서로 실행되어 Count가 변경됨)
        bool isLastEnemy = (spawner.activeEnemies.Count == 1); // 자기 자신만 남은 경우
        
        gameObject.SetActive(false); // OnDisable 호출됨 -> RemoveEnemy 실행
        GameManager.instance.kill++;
        // GameManager.instance.GetExp();
        // Spawner.instance.FixedSpawn(new List<Vector2Int> { gridPos }, TurnManager.instance.coinDrop);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Dead);
        
        // 모든 적이 제거되었으면 업그레이드 창 띄우기
        if (isLastEnemy)
        {
            GameManager.instance.StartCoroutine(ShowUpgradePanelNextFrame());
        }
    }

    private IEnumerator ShowUpgradePanelNextFrame()
    {
        yield return null; // 다음 프레임까지 대기
        GameManager.instance.uiLevelUp.Show(0);
    }

    private void EnemyMoveJudge()
    {
        GetTargetGridPos();

        if (!isArrived)
        {
            GetInputDir();

            if (!TryMove(inputDir))
            {
                Vector2Int inputDirCW = new Vector2Int(moveDir.y, -moveDir.x);
                Vector2Int inputDirCCW = new Vector2Int(-moveDir.y, moveDir.x);

                if (UnityEngine.Random.value < 0.5f)
                {
                    TryMove(inputDirCW);
                }
                else
                {
                    TryMove(inputDirCCW);
                }
            }
        }
        else
        {
            Vector2Int dir = player.gridPos - this.gridPos;
            SetSprite(dir);
        }
    }

    private void GetTargetGridPos()
    {
        targetGridPos = Vector2Int.zero;
        isArrived = false;

        Vector2Int[] candidates = new Vector2Int[]
        {
            player.gridPos + Vector2Int.right,
            player.gridPos + Vector2Int.left,
            player.gridPos + Vector2Int.down,
            player.gridPos + Vector2Int.up,
        };

        float minDistance = float.MaxValue;

        foreach (Vector2Int candidate in candidates)
        {
            if (gridPos == candidate)
            {
                isArrived = true;
                break;
            }

            if (GridManager.instance.IsOccupant(candidate))
            {
                continue;
            }

            float distance = Vector2Int.Distance(candidate, gridPos);
            if (distance < minDistance)
            {
                minDistance = distance;
                targetGridPos = candidate;
            }
        }
    }

    private void GetInputDir()
    {
        Vector2Int dir = targetGridPos - gridPos;

        if (UnityEngine.Random.value < 0.5f)
        {
            inputDir = new Vector2Int(Math.Sign(dir.x), 0);
        }
        else
        {
            inputDir = new Vector2Int(0, Math.Sign(dir.y));
        }

    }

    // 나중에 스킬 생기면 BasicAttackController의 상위 클래스를 만들어서, 상위 클래스를 매개변수로 변경 필요
    public void Attacked(BasicAttackController attack, float damage)
    {
        health -= damage;

        if (health > 0)
        {
            StartCoroutine(AttackedVFX());
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Hit);

            if (attack.curTexture == TextureData.TextureType.Pointillism)
            {
                pointillismStacks++;
                if (pointillismStacks >= attack.textureValue)
                {
                    Dead();
                }
            }
            else if (attack.curTexture == TextureData.TextureType.OilPainting)
            {
                StartCoroutine(StopMove(attack, attack.textureValue));
            }
        }
        else
        {
            Dead();
        }
    }

    private IEnumerator AttackedVFX()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.color = new Color(1, 0.5f, 0.5f);
        yield return new WaitForSeconds(0.15f);
        sr.color = Color.white;
    }

    public void AwakeEnemy(float shakePower)
    {
        StartCoroutine(AwakeVFX(shakePower));
    }

    private IEnumerator AwakeVFX(float shakePower)
    {
        if(TurnManager.instance.CurState != TurnState.PlayerTurn)
        {
            yield return null;
        }

        Vector3 originalPos = transform.position;
        float elapsedTime = 0f;
        float shakeTimer = 0f;

        while (elapsedTime < 0.3f) // 0.3초 동안 흔들림
        {
            elapsedTime += Time.deltaTime;
            shakeTimer += Time.deltaTime;

            if (shakeTimer >= 0.1f) // 0.15초마다 위치 변경. 높을수록 묵직하게 흔들림
            {
                Vector2 shakeOffset = UnityEngine.Random.insideUnitCircle * shakePower;
                transform.position = originalPos + new Vector3(shakeOffset.x, shakeOffset.y, 0);

                shakeTimer = 0f;
            }

            yield return null;
        }

        transform.position = originalPos;
    }

    public void AutoMove(float enemyWait)
    {
        if(canMove)
        {
            wait = enemyWait;
            EnemyMoveJudge();
        }
    }

    // 내 스크립트 안에 있는 IEnumerator만 StartCoroutine로 직접 호출할 수 있다.
    // 아래는 Player에서 Enemy의 StartCoroutine을 호출할 필요가 있어서 별도의 메서드를 만든 것.
    public void PushedMove(Vector2Int dir)
    {
        StartCoroutine(ExecuteMove(dir));
    }

    public IEnumerator StopMove(BasicAttackController attack, float time)
    {
        canMove = false;

        yield return new WaitForSeconds(time);

        canMove = true;

    }
}
