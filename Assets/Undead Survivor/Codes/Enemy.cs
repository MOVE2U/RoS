using UnityEngine;
using System;
using System.Collections;
using static UnityEngine.EventSystems.EventTrigger;

public class Enemy : Unit
{
    public float health;
    public float maxHealth;
    public RuntimeAnimatorController[] animCon;
    public Vector2Int targetGridPos;
    public Vector2Int dirVec;
    public bool isLive;
    public bool isLookAt;

    Vector3 playerPos;
    Vector2Int playerGridPos;
    public Vector2Int enemyGridPos;
    bool isArrived;

    Animator anim;

    // Awake에서는 컴포넌트 할당. 컴포넌트 할당은 '레퍼런스 초기화'로, 초기화의 범주에 속한다.
    // Start에서는 변수의 초기화.
    void Awake()
    {
        anim = GetComponent<Animator>();
        spriter = GetComponent<SpriteRenderer>();
        grid = 1;
        moveTime = 0.3f;

        enemyGridPos = GridManager.instance.WorldToGrid(transform.position);
    }
    private void Update()
    {
        playerPos = GameManager.instance.player.transform.position;
        playerGridPos = GridManager.instance.WorldToGrid(playerPos);
        enemyGridPos = GridManager.instance.WorldToGrid(transform.position);
    }
    public void Move()
    {
        if (!GameManager.instance.isLive || isMoving)
            return;

        GetTargetGridPos();

        if (!isArrived )
        {
            isLookAt = false;
            GetMoveDir();

            transform.right = new Vector3(moveDir.x, moveDir.y, 0f);

            if (moveDir != Vector2Int.zero)
            {
                if(!TryMove(enemyGridPos, moveDir))
                {
                    Vector2Int moveDirCW = new Vector2Int(moveDir.y, -moveDir.x);
                    Vector2Int moveDirCCW = new Vector2Int(-moveDir.y, moveDir.x);

                    if (UnityEngine.Random.value < 0.5f)
                    {
                        TryMove(enemyGridPos, moveDirCW);
                    }
                    else
                    {
                        TryMove(enemyGridPos, moveDirCCW);
                    }
                }                
            }
        }
        else
        {
            Vector2Int dir = playerGridPos - enemyGridPos;

            if (transform.right == new Vector3(dir.x, dir.y, 0f))
            {
                isLookAt = true;
            }
            else
            {
                transform.right = new Vector3(dir.x, dir.y, 0f);
                isLookAt = true;
            }
        }
    }
    void GetTargetGridPos()
    {
        targetGridPos = Vector2Int.zero;
        isArrived = false;

        Vector2Int[] candidates = new Vector2Int[]
        {
            playerGridPos + Vector2Int.up,
            playerGridPos + Vector2Int.down,
            playerGridPos + Vector2Int.left,
            playerGridPos + Vector2Int.right,
        };

        float minDistance = float.MaxValue;
        Vector2Int nearestGirdPos = playerGridPos;

        foreach(Vector2Int candidate in candidates)
        {
            if(enemyGridPos ==  candidate)
            {
                isArrived = true;
                break;
            }
            if(GridManager.instance.IsObject(candidate))
            {
                continue;
            }
            float distance = Vector2Int.Distance(candidate, enemyGridPos);
            if(distance < minDistance)
            {
                minDistance = distance;
                nearestGirdPos = candidate;
            }
        }
        targetGridPos = nearestGirdPos;
    }
    void GetMoveDir()
    {
        moveDir = Vector2Int.zero;

        dirVec = targetGridPos - enemyGridPos;

        if (UnityEngine.Random.value < 0.5f)
        {
            moveDir = new Vector2Int(Math.Sign(dirVec.x), 0);
        }
        else
        {
            moveDir = new Vector2Int(0, Math.Sign(dirVec.y));
        }

    }

    void OnEnable()
    {
        TurnManager.instance.AddEnemy(this);
        isLive = true;
        isMoving = false;
        spriter.sortingOrder = 2;
        anim.SetBool("Dead", false);
        health = maxHealth;
    }
    private void OnDisable()
    {
        TurnManager.instance.RemoveEnemy(this);
        GridManager.instance.Unregister(enemyGridPos);
    }

    public void Init(SpawnData data)
    {
        anim.runtimeAnimatorController = animCon[data.spriteType];
        health = data.health;
        maxHealth = data.health;
    }
    public void Attacked(float damage)
    {
        if (!isLive)
            return;

        health -= damage;

        if(health > 0)
        {
            anim.SetTrigger("Hit");
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Hit);
        }
        else
        {
            // dead
            isLive = false;
            Debug.Log(enemyGridPos);
            GridManager.instance.Unregister(enemyGridPos);
            Dead();
            // spriter.sortingOrder = 1;
            // anim.SetBool("Dead", true);
            GameManager.instance.kill++;
            GameManager.instance.GetExp();

            if (GameManager.instance.isLive)
            {
                AudioManager.instance.PlaySfx(AudioManager.Sfx.Dead);
            }
        }
    }
    void Dead()
    {
        gameObject.SetActive(false);
    }
}
