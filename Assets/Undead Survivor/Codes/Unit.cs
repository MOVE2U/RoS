using System.Collections;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [Header("value")]
    [SerializeField] protected int grid = 1;

    [Header("for check")]
    [SerializeField] protected bool isMoving;
    [SerializeField] protected float moveTime;
    [SerializeField] protected float wait;
    [SerializeField] protected Vector2Int inputDir;
    [SerializeField] protected Vector2Int moveDir;

    [Header("sprites")]
    [SerializeField] private Sprite spriteRight;
    [SerializeField] private Sprite spriteLeft;
    [SerializeField] private Sprite spriteDown;
    [SerializeField] private Sprite spriteUp;

    private SpriteRenderer spriteRenderer;

    public Vector2Int gridPos;

    public int Grid => grid;
    public bool IsMoving => isMoving;
    public Vector2Int InputDir => inputDir;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        Debug.Log($"spriteRenderer: {spriteRenderer}");

    }

    private void OnEnable()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        Debug.Log($"OnEnable spriteRenderer: {spriteRenderer}");

        // 기타 활성화 시 초기화
        gridPos = GridManager.instance.WorldToGrid(transform.position);
        isMoving = false;
        inputDir = Vector2Int.zero;
    }

    protected bool TryMove(Vector2Int inputDir)
    {
        Debug.Log("trymove 실행은 됐음");

        if (isMoving)
        {
            Debug.Log("움직이고 있어서 리턴");
            return false;
        }

        Debug.Log("움직이고 있지 않은 경우");

        // 이동에 성공을 안하더라도 움직이는 중이 아니라면 스프라이트 방향은 바꿔준다.
        SetSprite(inputDir);
        Debug.Log("스프라이트 세팅 성공");
        Vector2Int nextGridPos = gridPos + inputDir * grid;
        if (GridManager.instance.IsObject(nextGridPos))
        {
            Debug.Log("다음 좌표에 오브젝트가 있어서 리턴");
            return false;
        }

        Debug.Log("이동 성공한 경우");

        // 이동한다면 이동 방향을 moveDir에 캡쳐
        moveDir = inputDir;
        StartCoroutine(Move(moveDir));
        return true;
    }

    protected IEnumerator Move(Vector2Int dir)
    {
        isMoving = true;

        // 논리 좌표 업데이트
        Vector2Int startGridPos = gridPos;
        gridPos += dir * grid;

        // GridManager에 좌표 변경 알림
        GridManager.instance.Change(startGridPos, gridPos, gameObject);

        // 이동 애니메이션을 위한 월드 좌표 계산
        Vector3 startPos = GridManager.instance.GridToWorld(startGridPos);
        Vector3 endPos = GridManager.instance.GridToWorld(gridPos);

        // 이동 애니메이션
        float elapsedTime = 0;
        while (elapsedTime < moveTime)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / moveTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = endPos;
        yield return new WaitForSeconds(wait);

        isMoving = false;
    }

    protected void SetSprite(Vector2Int dir)
    {
        if (dir == Vector2Int.right)
        {
            spriteRenderer.sprite = spriteRight;
        }
        else if (dir == Vector2Int.left)
        {
            spriteRenderer.sprite = spriteLeft;
        }
        else if (dir == Vector2Int.up)
        {
            spriteRenderer.sprite = spriteUp;
        }
        else
        {
            spriteRenderer.sprite = spriteDown;
        }
    }
}
