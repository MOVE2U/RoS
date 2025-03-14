using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed;
    public float health;
    public float maxHealth;
    public RuntimeAnimatorController[] animCon;
    public Rigidbody2D target;

    bool isLive;

    Rigidbody2D rigid;
    Collider2D coll;
    Animator anim;
    SpriteRenderer spriter;
    // �ڷ�ƾ ��ü
    WaitForFixedUpdate wait;

    // Awake������ ������Ʈ �Ҵ�. ������Ʈ �Ҵ��� '���۷��� �ʱ�ȭ'��, �ʱ�ȭ�� ���ֿ� ���Ѵ�.
    // Start������ ������ �ʱ�ȭ.
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        spriter = GetComponent<SpriteRenderer>();
        // ������Ʈ�� �ƴ����� �ѹ��� �ʱ�ȭ�ϸ� �Ǳ� ������ Awake���� ��
        wait = new WaitForFixedUpdate();
    }

    void FixedUpdate()
    {
        if (!GameManager.instance.isLive)
            return;

        if (!isLive || anim.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
            return;
        // �ð�, �ӵ�, ��, �浹 ���� �ݿ��ϴ� �������� �̵����� ��ġ�� rigid.position���� �޾ƾ� �Ѵ�. transform.position�� ��ǥ �� ��ü�� �̵��Ѵ�.
        Vector2 dirVec = target.position - rigid.position;
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
        rigid.linearVelocity = nextVec;
    }

    // ������ ���� ȣ��Ǿ� �ð��� ó���� �Ѵ�.
    void LateUpdate()
    {
        if (!GameManager.instance.isLive)
            return;

        if (!isLive)
            return;
        spriter.flipX = rigid.position.x > target.position.x;  
    }

    void OnEnable()
    {
        target = GameManager.instance.player.GetComponent<Rigidbody2D>();
        isLive = true;
        rigid.simulated = true;
        coll.enabled = true;
        spriter.sortingOrder = 2;
        anim.SetBool("Dead", false);
        health = maxHealth;
    }

    public void Init(SpawnData data)
    {
        anim.runtimeAnimatorController = animCon[data.spriteType];
        speed = data.speed;
        health = data.health;
        maxHealth = data.health;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!collision.CompareTag("Bullet") || !isLive)
            return;
        health -= collision.GetComponent<Bullet>().damage;
        // �ڷ�ƾ�� ����. �ٸ� �޼���ó�� �޼������ �ٷ� ���� �ʰ� StartCoroutine���� �����Ѵ�.
        StartCoroutine(KnockBack());

        if (health>0)
        {
            anim.SetTrigger("Hit");
        }
        else
        {
            // dead
            isLive = false;
            rigid.simulated = false;
            coll.enabled = false;
            spriter.sortingOrder = 1;
            anim.SetBool("Dead", true);
            GameManager.instance.kill++;
            GameManager.instance.GetExp();
        }
    }
    // �ڷ�ƾ �޼���� IEnumerator�� �����Ѵ�.
    IEnumerator KnockBack()
    {
        yield return wait; // ���� �ϳ��� �����ӱ��� ���
        Vector3 playerPos = GameManager.instance.player.transform.position;
        Vector3 dirVec = transform.position - playerPos;
        // AddForce�� ù��° ���ڴ� ����� ��
        // �ι�° ���ڰ� ������ ForceMode.Force��, �����Ӹ��� ���� ���ϱ� ������ ������ �ٴ´�.
        // ForceMode.Impulse�� ù��° �����ӿ��� ���� ��! ���Ѵ�.
        rigid.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse);
    }
    void Dead()
    {
        gameObject.SetActive(false);
    }
}
