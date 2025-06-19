using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Unit
{
    [Header("for check")]
    [SerializeField] private Vector2Int inputDir;

    [Header("external ref")]
    [SerializeField] private GridManager gridManager;

    private Vector2 inputVec;

    private void Awake()
    {
        // Unit 공통 변수 초기화
        isMoving = false;
        moveTime = 0.3f;
        wait = 0.1f;

        // Player 전용 변수 초기화
        inputDir = Vector2Int.zero;
    }
    private void Update()
    {
        if (TurnManager.instance.CurState == TurnState.PlayerTurn
            && inputDir != Vector2Int.zero)
        {            
            if(TryMove(inputDir))
            {
                TurnManager.instance.MoveCountInc();
            }
        }
    }

    private void OnMove(InputValue value)
    {
        inputVec = value.Get<Vector2>();

        if(inputVec.x != 0 && Mathf.Abs(inputVec.x) >= Mathf.Abs(inputVec.y))
        {
            inputDir = new Vector2Int((int)Mathf.Sign(inputVec.x), 0);
        }
        else if (Mathf.Abs(inputVec.x) < Mathf.Abs(inputVec.y))
        {
            inputDir = new Vector2Int(0, (int)Mathf.Sign(inputVec.y));
        }
        else
        {
            inputDir = Vector2Int.zero;
        }
    }
}