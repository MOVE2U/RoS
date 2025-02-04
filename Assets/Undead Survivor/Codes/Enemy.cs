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

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriter = GetComponent<SpriteRenderer>();
    }
    void FixedUpdate()
    {
        if (!isLive)
            return;
        // 시간, 속도, 힘, 충돌 등을 반영하는 물리적인 이동에는 위치를 rigid.position으로 받아야 한다. transform.position은 좌표 그 자체로 이동한다.
        Vector2 dirVec = target.position - rigid.position;
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position +  nextVec);
        rigid.linearVelocity = nextVec;
    }

    // 렌더링 이후 호출되어 시각적 처리를 한다.
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

        if(health>0)
        {
            // live. hit action
        }
        else
        {
            // dead
            Dead();
        }
    }

    void Dead()
    {
        gameObject.SetActive(false);
    }
}
