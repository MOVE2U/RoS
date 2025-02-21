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
    Animator anim;
    SpriteRenderer spriter;
    // �ڷ�ƾ ��ü
    WaitForFixedUpdate wait;

    // Awake������ ������Ʈ �Ҵ�. ������Ʈ �Ҵ��� '���۷��� �ʱ�ȭ'��, �ʱ�ȭ�� ���ֿ� ���Ѵ�.
    // Start������ ������ �ʱ�ȭ.
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriter = GetComponent<SpriteRenderer>();
        // ������Ʈ�� �ƴ����� �ѹ��� �ʱ�ȭ�ϸ� �Ǳ� ������ Awake���� ��
        wait = new WaitForFixedUpdate();
    }

    void FixedUpdate()
    {
        if (!isLive)
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
        if(!isLive)
            return;
        spriter.flipX = rigid.position.x > target.position.x;  
    }

    void OnEnable()
    {
        target = GameManager.instance.player.GetComponent<Rigidbody2D>();
        isLive = true;
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
        if(!collision.CompareTag("Bullet"))
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
            Dead();
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
