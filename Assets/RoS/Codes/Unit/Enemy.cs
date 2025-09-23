using UnityEngine;
using System;
using System.Collections;

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
        moveTime = 0.3f;
    }

    // 초기화 - 활성화 될 때 공통
    private void OnEnable()
    {
        // 오브젝트 풀링은 OnEnable에서 초기화하기도 함. 싱글톤 참조는 비용이 적음
        player = GameManager.instance.player;
        spawner = Spawner.instance;

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
        gameObject.SetActive(false);
        GameManager.instance.kill++;
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Dead);
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

    public void AutoMove(float randomWait)
    {
        if(canMove)
        {
            wait = randomWait;
            EnemyMoveJudge();
        }
    }

    // 내 스크립트 안에 있는 IEnumerator만 StartCoroutine로 직접 호출할 수 있다. 아래는 Player에서 Enemy의 StartCoroutine을 호출할 필요가 있어서 별도의 메서드를 만든 것.
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
