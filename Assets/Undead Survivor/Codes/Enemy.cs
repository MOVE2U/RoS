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
    // 코루틴 객체
    WaitForFixedUpdate wait;

    // Awake에서는 컴포넌트 할당. 컴포넌트 할당은 '레퍼런스 초기화'로, 초기화의 범주에 속한다.
    // Start에서는 변수의 초기화.
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        spriter = GetComponent<SpriteRenderer>();
        // 컴포넌트는 아니지만 한번만 초기화하면 되기 때문에 Awake에서 함
        wait = new WaitForFixedUpdate();
    }

    void FixedUpdate()
    {
        if (!GameManager.instance.isLive)
            return;

        if (!isLive || anim.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
            return;
        // 시간, 속도, 힘, 충돌 등을 반영하는 물리적인 이동에는 위치를 rigid.position으로 받아야 한다. transform.position은 좌표 그 자체로 이동한다.
        Vector2 dirVec = target.position - rigid.position;
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
        rigid.linearVelocity = nextVec;
    }

    // 렌더링 이후 호출되어 시각적 처리를 한다.
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
        // 코루틴의 실행. 다른 메서드처럼 메서드명을 바로 쓰지 않고 StartCoroutine으로 실행한다.
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
    // 코루틴 메서드는 IEnumerator로 선언한다.
    IEnumerator KnockBack()
    {
        yield return wait; // 다음 하나의 프레임까지 대기
        Vector3 playerPos = GameManager.instance.player.transform.position;
        Vector3 dirVec = transform.position - playerPos;
        // AddForce의 첫번째 인자는 방향과 힘
        // 두번째 인자가 없으면 ForceMode.Force로, 프레임마다 힘을 가하기 때문에 가속이 붙는다.
        // ForceMode.Impulse는 첫번째 프레임에만 힘을 빡! 가한다.
        rigid.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse);
    }
    void Dead()
    {
        gameObject.SetActive(false);
    }
}
