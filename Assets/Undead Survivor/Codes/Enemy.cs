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

    // Awake������ ������Ʈ �Ҵ�. ������Ʈ �Ҵ��� '���۷��� �ʱ�ȭ'��, �ʱ�ȭ�� ���ֿ� ���Ѵ�.
    // Start������ ������ �ʱ�ȭ.
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
    //    // �ð�, �ӵ�, ��, �浹 ���� �ݿ��ϴ� �������� �̵����� ��ġ�� rigid.position���� �޾ƾ� �Ѵ�. transform.position�� ��ǥ �� ��ü�� �̵��Ѵ�.
    //    Vector2 dirVec = target.position - rigid.position;
    //    Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
    //    rigid.MovePosition(rigid.position + nextVec);
    //    rigid.linearVelocity = nextVec;
    //}

    //// ������ ���� ȣ��Ǿ� �ð��� ó���� �Ѵ�.
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
    //    // �ڷ�ƾ�� ����. �ٸ� �޼���ó�� �޼������ �ٷ� ���� �ʰ� StartCoroutine���� �����Ѵ�.
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
    // �ڷ�ƾ �޼���� IEnumerator�� �����Ѵ�.
    //IEnumerator KnockBack()
    //{
    //    yield return wait; // ���� �ϳ��� �����ӱ��� ���
    //    Vector3 playerPos = GameManager.instance.player.transform.position;
    //    Vector3 dirVec = transform.position - playerPos;
    //    // AddForce�� ù��° ���ڴ� ����� ��
    //    // �ι�° ���ڰ� ������ ForceMode.Force��, �����Ӹ��� ���� ���ϱ� ������ ������ �ٴ´�.
    //    // ForceMode.Impulse�� ù��° �����ӿ��� ���� ��! ���Ѵ�.
    //    rigid.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse);
    //}
    //void Dead()
    //{
    //    gameObject.SetActive(false);
    //}
}
