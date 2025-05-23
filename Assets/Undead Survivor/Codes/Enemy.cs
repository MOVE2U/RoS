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
    Vector2Int enemyGridPos;
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
    }
    private void Update()
    {
        playerPos = GameManager.instance.player.transform.position;
        playerGridPos = GridManager.instance.WorldToGrid(playerPos);
        enemyGridPos = GridManager.instance.WorldToGrid(transform.position);

        if (!GameManager.instance.isLive || !TurnManager.instance.isEnemyTurn || isMoving)
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
        isLive = true;
        spriter.sortingOrder = 2;
        anim.SetBool("Dead", false);
        health = maxHealth;
    }

    public void Init(SpawnData data)
    {
        anim.runtimeAnimatorController = animCon[data.spriteType];
        wait = data.wait;
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
    //void FixedUpdate()
    //{
    //    if (!GameManager.instance.isLive)
    //        return;

    //    if (!isLive || anim.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
    //        return;
    //    // 시간, 속도, 힘, 충돌 등을 반영하는 물리적인 이동에는 위치를 rigid.position으로 받아야 한다. transform.position은 좌표 그 자체로 이동한다.
    //    Vector2 dirVec = target.position - rigid.position;
    //    Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
    //    rigid.MovePosition(rigid.position + nextVec);
    //    rigid.linearVelocity = nextVec;
    //}

    //// 렌더링 이후 호출되어 시각적 처리를 한다.
    //void LateUpdate()
    //{
    //    if (!GameManager.instance.isLive)
    //        return;

    //    if (!isLive)
    //        return;
    //    spriter.flipX = rigid.position.x > target.position.x;  
    //}
    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if(!collision.CompareTag("Bullet") || !isLive)
    //        return;
    //    health -= collision.GetComponent<Bullet>().damage;
    //    // 코루틴의 실행. 다른 메서드처럼 메서드명을 바로 쓰지 않고 StartCoroutine으로 실행한다.
    //    StartCoroutine(KnockBack());

    //    if (health>0)
    //    {
    //        anim.SetTrigger("Hit");
    //        AudioManager.instance.PlaySfx(AudioManager.Sfx.Hit);
    //    }
    //    else
    //    {
    //        // dead
    //        isLive = false;
    //        rigid.simulated = false;
    //        coll.enabled = false;
    //        spriter.sortingOrder = 1;
    //        anim.SetBool("Dead", true);
    //        GameManager.instance.kill++;
    //        GameManager.instance.GetExp();

    //        if(GameManager.instance.isLive)
    //        {
    //            AudioManager.instance.PlaySfx(AudioManager.Sfx.Dead);
    //        }
    //    }
    // }
    // 코루틴 메서드는 IEnumerator로 선언한다.
    //IEnumerator KnockBack()
    //{
    //    yield return wait; // 다음 하나의 프레임까지 대기
    //    Vector3 playerPos = GameManager.instance.player.transform.position;
    //    Vector3 dirVec = transform.position - playerPos;
    //    // AddForce의 첫번째 인자는 방향과 힘
    //    // 두번째 인자가 없으면 ForceMode.Force로, 프레임마다 힘을 가하기 때문에 가속이 붙는다.
    //    // ForceMode.Impulse는 첫번째 프레임에만 힘을 빡! 가한다.
    //    rigid.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse);
    //}
    //void Dead()
    //{
    //    gameObject.SetActive(false);
    //}
}
