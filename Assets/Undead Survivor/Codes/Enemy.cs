using UnityEngine;
using System;

public class Enemy : Unit
{
    [Header("value")]
    [SerializeField] private float health;
    [SerializeField] private float maxHealth;

    [Header("for check")]
    [SerializeField] private Vector2Int targetGridPos;
    [SerializeField] private bool isArrived;

    [Header("external ref")]
    [SerializeField] private Spawner spawner;

    private Player player;

    private void Awake()
    {
        // Unit 공통 변수 초기화
        moveTime = 0.3f;
    }

    // 싱글톤 참조는 start
    private void Start()
    {
        player = GameManager.instance.player;
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

            if (GridManager.instance.IsObject(candidate))
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

    public void Attacked(float damage)
    {
        health -= damage;

        if (health > 0)
        {
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Hit);
        }
        else
        {
            GridManager.instance.Unregister(gridPos);
            Dead();
            GameManager.instance.kill++;
            GameManager.instance.GetExp();

            if (GameManager.instance.isLive)
            {
                AudioManager.instance.PlaySfx(AudioManager.Sfx.Dead);
            }
        }
    }

    public void AutoMove(float randomWait)
    {
        wait = randomWait;
        EnemyMoveJudge();
    }

    // 초기화 - 외부에서 전달받은 데이터로 세팅하는 항목
    public void Init(SpawnData data)
    {
        health = data.health;
        maxHealth = data.health;
    }

    // 초기화 - 활성화 될 때 공통
    private void OnEnable()
    {
        spawner.AddEnemy(this);
        isMoving = false;
        health = maxHealth;
    }

    // 정리 - 죽었을 때만 호출되는 항목
    void Dead()
    {
        gameObject.SetActive(false);
    }

    // 정리 - 비활성화 될 때 공통
    private void OnDisable()
    {
        GridManager.instance.Unregister(gridPos);
        spawner.RemoveEnemy(this);
    }
}
