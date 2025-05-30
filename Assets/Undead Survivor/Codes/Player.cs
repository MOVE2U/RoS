using System.Collections;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Unit
{
    public RuntimeAnimatorController[] animCon;
    Animator anim;

    public Scanner scanner;
    public GridManager gridManager;
    public HUD hud;
    public Hand[] hands;

    public Vector2 inputVec;
    public Vector2Int lastMoveDir;
    public Vector2Int lastStopDir;

    // 유니티는 게임 오브젝트를 씬에 로드하면서 메모리 생성을 함
    // 오브젝트가 생성된 후 가장 먼저 호출되는 메서드가 Awake. 이 시점에 오브젝트와 연결된 모든 컴포넌트들이 초기화되어 있음.
    void Awake()
    {
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        scanner = GetComponent<Scanner>();
        // 비활성화된 오브젝트도 검색하려면 true를 인자로 넣는다.
        hands = GetComponentsInChildren<Hand>(true);
        isMoving = false;
        moveDir = Vector2Int.zero;
        lastMoveDir = Vector2Int.right;
        lastStopDir = Vector2Int.right;
        wait = 0;
    }
    private void Update()
    {
        if (!GameManager.instance.isLive)
            return;

        if (!isMoving && moveDir != Vector2Int.zero && TurnManager.instance.isPlayerTurn)
        {
            Vector2Int playerGridPos = GridManager.instance.WorldToGrid(transform.position);
            
            if(TryMove(playerGridPos, moveDir))
            {
                TurnManager.instance.playerMoveCount++;
                hud.UsePlayerTurn(TurnManager.instance.playerMoveCount);
            }
        }
    }
    private void OnEnable()
    {
        if(GameManager.instance == null)
            return;

        anim.runtimeAnimatorController = animCon[GameManager.instance.playerId];
    }
    void OnMove(InputValue value)
    {
        inputVec = value.Get<Vector2>();

        if(inputVec.x != 0 && Mathf.Abs(inputVec.x) >= Mathf.Abs(inputVec.y))
        {
            moveDir = new Vector2Int((int)Mathf.Sign(inputVec.x), 0);
            lastMoveDir = moveDir;
        }
        else if (Mathf.Abs(inputVec.x) < Mathf.Abs(inputVec.y))
        {
            moveDir = new Vector2Int(0, (int)Mathf.Sign(inputVec.y));
            lastMoveDir = moveDir;
        }
        else
        {
            moveDir = Vector2Int.zero;
        }

        if(moveDir != Vector2Int.zero && TurnManager.instance.isPlayerTurn)
        {
            lastStopDir = moveDir;
        }
    }
    private void LateUpdate()
    {
        if (!GameManager.instance.isLive)
            return;

        anim.SetFloat("Speed", inputVec.magnitude);
    }
    //private void OnCollisionStay2D(Collision2D collision)
    //{
    //    if (!GameManager.instance.isLive)
    //        return;

    //    GameManager.instance.health -= 10 * Time.deltaTime;
    //    if(GameManager.instance.health <= 0)
    //    {
    //        for(int index = 2; index < transform.childCount; index++)
    //        {
    //            transform.GetChild(index).gameObject.SetActive(false);
    //        }
    //        anim.SetTrigger("Dead");
    //        GameManager.instance.GameOver();
    //    }
    //}
}