using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public Vector2 inputVec;
    public float speed;

    Rigidbody2D rigid;
    SpriteRenderer spriter;
    Animator anim;

    // 유니티는 게임 오브젝트를 씬에 로드하면서 메모리 생성을 함
    // 오브젝트가 생성된 후 가장 먼저 호출되는 메서드가 Awake. 이 시점에 오브젝트와 연결된 모든 컴포넌트들이 초기화되어 있음.
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        Vector2 nextVec = inputVec.normalized * speed * Time.fixedDeltaTime;
        //1. 힘을 준다.
        //rigid.AddForce (inputVec);

        //2. 속도 제어
        //rigid.velocity = inputVec;

        //3. 위치 변경
        rigid.MovePosition(rigid.position + nextVec);
    }

    void OnMove(InputValue value)
    {
        inputVec = value.Get<Vector2>();
    }

    private void LateUpdate()
    {
        anim.SetFloat("Speed", inputVec.magnitude);

        if(inputVec.x != 0)
        {
            spriter.flipX = inputVec.x < 0;
        }
    }
}