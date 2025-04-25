using System.Collections;
using UnityEngine;

public class Enemy : Unit
{
    public float health;
    public float maxHealth;
    public RuntimeAnimatorController[] animCon;
    public Vector2Int targetGridPos;

    bool isLive;
    bool isArrived;

    Animator anim;

    // Awake������ ������Ʈ �Ҵ�. ������Ʈ �Ҵ��� '���۷��� �ʱ�ȭ'��, �ʱ�ȭ�� ���ֿ� ���Ѵ�.
    // Start������ ������ �ʱ�ȭ.
    void Awake()
    {
        anim = GetComponent<Animator>();
        spriter = GetComponent<SpriteRenderer>();
        isMoving = false;
        moveDir = Vector2Int.zero;
        grid = 1;
        moveTime = 0.3f;
    }
    private void Update()
    {
        if (!GameManager.instance.isLive)
            return;

        GetTargetPos();
        GetMoveDir();

        if (!isMoving && moveDir != Vector2Int.zero && TurnManager.instance.isEnemyTurn)
        {
            Vector2Int enemyGridPos = GridManager.instance.WorldToGrid(transform.position);
            Vector2Int nextGridPos = enemyGridPos + moveDir * grid;
            if (!GridManager.instance.IsObject(nextGridPos))
            {
                StartCoroutine(MoveRoutine(moveDir));
            }
        }
    }
    public void GetMoveDir()
    {
        Vector2Int enemyGridPos = GridManager.instance.WorldToGrid(transform.position);
        Vector2 dirVec = targetGridPos - enemyGridPos;

        if (dirVec.x == 0)
        {
            moveDir = new Vector2Int(0, (int)Mathf.Sign(dirVec.y));
        }
        else if (dirVec.y == 0)
        {
            moveDir = new Vector2Int((int)Mathf.Sign(dirVec.x), 0);
        }
        else
        {
            if (Random.value < 0.5f)
            {
                moveDir = new Vector2Int((int)Mathf.Sign(dirVec.x), 0);
            }
            else
            {
                moveDir = new Vector2Int(0, (int)Mathf.Sign(dirVec.y));
            }
        }
    }
    public void GetTargetPos()
    {
        Vector3 playerPos = GameManager.instance.player.transform.position;
        Vector2Int playerGirdPos = GridManager.instance.WorldToGrid(playerPos);
        Vector2Int enemyGridPos = GridManager.instance.WorldToGrid(transform.position);

        Vector2Int[] candidates = new Vector2Int[]
        {
            playerGirdPos + Vector2Int.up,
            playerGirdPos + Vector2Int.down,
            playerGirdPos + Vector2Int.left,
            playerGirdPos + Vector2Int.right
        };

        float minDistance = float.MaxValue;
        Vector2Int nearestGirdPos = playerGirdPos;

        foreach(Vector2Int candidate in candidates)
        {
            if(enemyGridPos ==  candidate)
            {
                isArrived = true;
                nearestGirdPos = candidate;
                break;
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
