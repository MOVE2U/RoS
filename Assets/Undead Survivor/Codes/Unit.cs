using System.Collections;
using System.Net;
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

    protected void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        // 기타 활성화 시 초기화
        gridPos = GridManager.instance.WorldToGrid(transform.position);
        isMoving = false;
        inputDir = Vector2Int.zero;
    }

    protected bool TryMove(Vector2Int inputDir)
    {
        // 1. 이동 중이면 이동 안함
        if (isMoving)
        {
            return false;
        }

        // 2. 이동 성공 여부와 관계 없이 스프라이트 방향은 일단 바꿈
        SetSprite(inputDir);

        // 3. 이동 방향에 뭐 있는지 검사하고 처리
        Vector2Int nextGridPos = gridPos + inputDir * grid;
        GameObject nextObject = GridManager.instance.GetObject(nextGridPos);
        if(nextObject != null)
        {
            return ObjectEncounter(nextObject, inputDir);
        }

        // 4. 이동 실행
        StartCoroutine(ExecuteMove(inputDir));
        return true;
    }

    protected virtual bool ObjectEncounter(GameObject obj, Vector2Int dir)
    {
        return false;
    }

    protected IEnumerator ExecuteMove(Vector2Int dir)
    {
        isMoving = true;

        // 이동 방향 캡쳐
        moveDir = dir;

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
