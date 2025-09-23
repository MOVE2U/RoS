using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Unit
{
    [Header("external ref")]
    [SerializeField] private GridManager gridManager;

    private Vector2 inputVec;
    public Vector2Int lastInputDir { get; private set; }
    public BasicAttackController basicAttackController;

    private new void Awake()
    {
        base.Awake();

        // Unit 공통 변수 초기화
        moveTime = 0.3f;
        wait = 0.1f;
    }

    private void Start()
    {
        basicAttackController = GetComponentInChildren<BasicAttackController>();
    }

    private void Update()
    {
        if (TurnManager.instance.CurState == TurnState.MoveTurn
            && inputDir != Vector2Int.zero)
        {
            TryMove(inputDir);
        }
    }

    private void OnMove(InputValue value)
    {
        if(GameManager.instance.isLive == false)
        {
            return;
        }

        inputVec = value.Get<Vector2>();

        if(inputVec.x != 0 && Mathf.Abs(inputVec.x) >= Mathf.Abs(inputVec.y))
        {
            inputDir = new Vector2Int((int)Mathf.Sign(inputVec.x), 0);
            lastInputDir = inputDir;
            SetSprite(lastInputDir);
        }
        else if (Mathf.Abs(inputVec.x) < Mathf.Abs(inputVec.y))
        {
            inputDir = new Vector2Int(0, (int)Mathf.Sign(inputVec.y));
            lastInputDir = inputDir;
            SetSprite(lastInputDir);
        }
        // 입력이 없을 때 이동 방향 초기화
        else
        {
            inputDir = Vector2Int.zero;
        }
    }

    protected override bool ObjectEncounter(GameObject obj, Vector2Int dir)
    {
        // 1. 만난 오브젝트가 Enemy 라면
        if(obj.TryGetComponent<Enemy>(out var encountEnemy))
        {
            // TryPush한 결과를 그대로 return
            return TryPush(encountEnemy, dir);
        }

        // 2. 나중에 다른거 만난 경우를 추가

        // 99. 기본적으론 뭔갈 만나면 false를 리턴해서 움직이지 않음
        return false;
    }

    private bool TryPush(Enemy firstEnemy, Vector2Int dir)
    {
        // 1. 밀려날 유닛들을 담기
        List<Enemy> pushChain = new List<Enemy>();
        pushChain.Add(firstEnemy);
        Enemy lastEnemy = firstEnemy;

        while(true)
        {
            Vector2Int nextGridPos = lastEnemy.gridPos + dir * grid;
            GameObject nextObject = GridManager.instance.GetOccupant(nextGridPos);

            // case1: 다응 칸이 빈칸이면 Push 가능. 루프 탈출
            if (nextObject == null)
            {
                break;
            }

            // case2: 다음 칸이 Enemy면 pushChain에 추가하고 계속 진행
            else if (nextObject.TryGetComponent<Enemy>(out var nextEnemy))
            {
                pushChain.Add(nextEnemy);
                lastEnemy = nextEnemy;
            }

            // case3: 다음 칸이 다른 장애물이면 Push 실패
            else
            {
                return false;
            }
        }

        // 2. 이동 실행
        for (int i = pushChain.Count - 1; i >= 0; i--)
        {
            pushChain[i].PushedMove(dir);
        }

        this.StartCoroutine(ExecuteMove(dir));

        return true;
    }

    protected override void TriggerEncounter(Vector2Int gridPos)
    {
        // 이동 지점에 트리거 확인하고 처리
        if (GridManager.instance.HasTriggers(gridPos))
        {
            List<GameObject> triggers = GridManager.instance.GetTriggers(gridPos);

            foreach (GameObject trigger in new List<GameObject>(triggers))
            {
                if (trigger.TryGetComponent<Iinteractable>(out var iinteractable))
                {
                    iinteractable.OnInteract();
                }
            }
        }
    }
}