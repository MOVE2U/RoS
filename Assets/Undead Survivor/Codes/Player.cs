using System.Collections;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public Vector2 inputVec;
    public Vector2 moveDir;
    public float moveTime;
    public float grid;
    public float speed;
    public bool isMoving;

    public Scanner scanner;
    public Hand[] hands;
    public RuntimeAnimatorController[] animCon;

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
        scanner = GetComponent<Scanner>();
        // 비활성화된 오브젝트도 검색하려면 true를 인자로 넣는다.
        hands = GetComponentsInChildren<Hand>(true);
        isMoving = false;
        moveDir = Vector2.zero;
    }
    private void Update()
    {
        if (!GameManager.instance.isLive)
            return;
        if (!isMoving && moveDir != Vector2.zero && TurnManager.instance.isPlayerTurn)
        {   
            StartCoroutine(MoveRoutine(moveDir));
        }
    }
    private void OnEnable()
    {
        if(GameManager.instance == null)
            return;

        speed *= Character.Speed;
        anim.runtimeAnimatorController = animCon[GameManager.instance.playerId];
    }
    void OnMove(InputValue value)
    {
        inputVec = value.Get<Vector2>();

        if(inputVec.x != 0 && Mathf.Abs(inputVec.x) >= Mathf.Abs(inputVec.y))
        {
            moveDir = new Vector2(Mathf.Sign(inputVec.x), 0);
        }
        else if (Mathf.Abs(inputVec.x) < Mathf.Abs(inputVec.y))
        {
            moveDir = new Vector2(0, Mathf.Sign(inputVec.y));
        }
        else
        {
            moveDir = Vector2.zero;
        }
    }
    IEnumerator MoveRoutine(Vector2 dir)
    {
        isMoving = true;

        if (dir.x != 0)
        {
            spriter.flipX = dir.x < 0;
        }

        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + (Vector3)dir * grid;
        float elapsedTime = 0;

        while (elapsedTime < moveTime)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / moveTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;
        TurnManager.instance.playerTurnCount--;
        isMoving = false;
    }
    private void LateUpdate()
    {
        if (!GameManager.instance.isLive)
            return;

        anim.SetFloat("Speed", inputVec.magnitude);
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!GameManager.instance.isLive)
            return;

        GameManager.instance.health -= 10 * Time.deltaTime;
        if(GameManager.instance.health <= 0)
        {
            for(int index = 2; index < transform.childCount; index++)
            {
                transform.GetChild(index).gameObject.SetActive(false);
            }
            anim.SetTrigger("Dead");
            GameManager.instance.GameOver();
        }
    }
}