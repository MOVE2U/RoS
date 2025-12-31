using UnityEngine;
using System;
using System.Collections;

public abstract class EnemyAbstract : Unit, ISpawnable
{
    [Header("for check")]
    [SerializeField] private Vector2Int targetGridPos;
    [SerializeField] protected bool isArrived;

    [Header("Status Condition")]
    public int pointillismStacks = 0;
    public bool canMove = true;
    private Player player;
    private Spawner spawner;

    [Header("Intent")]
    public EnemyIntent intent;
    public EnemyIntentState intentState;

    public abstract void IntentCalculate();
    public abstract void IntentExecute();

    #region LifeCycle
    private new void Awake()
    {
        base.Awake();
        intentState = EnemyIntentState.None;
    }

    // 활성화 될 때
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
    }

    // 외부에서 전달받은 데이터로 세팅
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

    // 비활성화 될 때
    private void OnDisable()
    {
        GridManager.instance.UnRegisterOccupant(gridPos);
        spawner.RemoveEnemy(this);
    }

    // 죽었을 때만 호출
    public override void Dead()
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

        // 스테이지 종료 조건 체크
        StageManager.instance.CheckStageClear(isLastEnemy);
    }
    #endregion

    #region Move
    // 어디로 어떻게 갈지 판단 후 이동 시도
    public void EnemyMoveJudge()
    {
        if (!canMove) return; // canMove는 유화 효과 때문에 쓰던것. 지금은 안씀

        wait = TurnManager.instance.enemyTurnDelay; // 이동 후 턴 넘김을 위한 대기 시간이라 TurnManager에서 관리

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

    // 플레이어 상하좌우 중 가장 가까운 빈 칸을 목표로 설정
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

    // 목표한 좌표로 이동할 방향을 설정
    private void GetInputDir()
    {
        Vector2Int dir = targetGridPos - gridPos;

        // X축과 Y축 중 0이 아닌 것만 후보로 선택
        bool canMoveX = dir.x != 0;
        bool canMoveY = dir.y != 0;

        if (canMoveX && canMoveY)
        {
            // 둘 다 이동 가능하면 랜덤 선택
            if (UnityEngine.Random.value < 0.5f)
            {
                inputDir = new Vector2Int(Math.Sign(dir.x), 0);
            }
            else
            {
                inputDir = new Vector2Int(0, Math.Sign(dir.y));
            }
        }
        else if (canMoveX)
        {
            // X축만 이동 가능
            inputDir = new Vector2Int(Math.Sign(dir.x), 0);
        }
        else if (canMoveY)
        {
            // Y축만 이동 가능
            inputDir = new Vector2Int(0, Math.Sign(dir.y));
        }
        else
        {
            // 둘 다 0인 경우 (이미 목표 위치에 있음)
            inputDir = Vector2Int.zero;
        }
    }

    // 내 스크립트 안에 있는 IEnumerator만 StartCoroutine로 직접 호출할 수 있다.
    // 아래는 Player에서 Enemy의 StartCoroutine을 호출할 필요가 있어서 별도의 메서드를 만든 것.
    public void PushedMove(Vector2Int dir)
    {
        StartCoroutine(ExecuteMove(dir));
    }
    #endregion

    #region Archive
    public void Attacked(BasicAttackController attack, float damage)
    {
        health -= damage;

        if (health > 0)
        {
            // StartCoroutine(base.AttackedVFX());
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

    public void AwakeEnemy(float shakePower)
    {
        StartCoroutine(AwakeVFX(shakePower));
    }

    private IEnumerator AwakeVFX(float shakePower)
    {
        if (TurnManager.instance.CurState != TurnState.PlayerTurn)
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

    public IEnumerator StopMove(BasicAttackController attack, float time)
    {
        canMove = false;

        yield return new WaitForSeconds(time);

        canMove = true;
    }
    #endregion
}
