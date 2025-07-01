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
    }

    private void OnEnable()
    {
        gridPos = GridManager.instance.WorldToGrid(transform.position);
        isMoving = false;
        inputDir = Vector2Int.zero;
    }

    protected bool TryMove(Vector2Int inputDir)
    {
        if (isMoving)
            return false;

        // �̵��� ������ ���ϴ��� �����̴� ���� �ƴ϶�� ��������Ʈ ������ �ٲ��ش�.
        SetSprite(inputDir);

        Vector2Int nextGridPos = gridPos + inputDir * grid;
        if (!GridManager.instance.IsObject(nextGridPos))
        {
            // �̵��Ѵٸ� �̵� ������ moveDir�� ĸ��
            moveDir = inputDir;
            StartCoroutine(Move(moveDir));
            return true;
        }
        return false;
    }

    protected IEnumerator Move(Vector2Int dir)
    {
        isMoving = true;

        // �� ��ǥ ������Ʈ
        Vector2Int startGridPos = gridPos;
        gridPos += dir * grid;

        // GridManager�� ��ǥ ���� �˸�
        GridManager.instance.Change(startGridPos, gridPos, gameObject);

        // �̵� �ִϸ��̼��� ���� ���� ��ǥ ���
        Vector3 startPos = GridManager.instance.GridToWorld(startGridPos);
        Vector3 endPos = GridManager.instance.GridToWorld(gridPos);

        // �̵� �ִϸ��̼�
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
        else if (dir == Vector2Int.down)
        {
            spriteRenderer.sprite = spriteDown;
        }
    }
}
