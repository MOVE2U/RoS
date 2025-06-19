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
    [SerializeField] protected Vector2Int moveDir;
    [SerializeField] protected Vector2Int gridPosition;

    [Header("sprites")]
    [SerializeField] private Sprite spriteRight;
    [SerializeField] private Sprite spriteLeft;
    [SerializeField] private Sprite spriteDown;
    [SerializeField] private Sprite spriteUp;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        gridPosition = GridManager.instance.WorldToGrid(transform.position);
    }

    protected bool TryMove(Vector2Int inputDir)
    {
        if (isMoving)
            return false;

        // �̵��� ������ ���ϴ��� �����̴� ���� �ƴ϶�� ��������Ʈ ������ �ٲ��ش�.
        SetSprite(inputDir);

        Vector2Int nextGridPosition = gridPosition + inputDir * grid;
        if (!GridManager.instance.IsObject(nextGridPosition))
        {
            // �̵��Ѵٸ� �̵� ������ moveDir�� ĸ��
            moveDir = inputDir;
            StartCoroutine(MoveRoutine(moveDir));
            return true;
        }
        return false;
    }
    protected IEnumerator MoveRoutine(Vector2Int dir)
    {
        isMoving = true;

        // �� ��ǥ ������Ʈ
        Vector2Int startGridPos = gridPosition;
        gridPosition += dir * grid;

        // GridManager�� ��ǥ ���� �˸�
        GridManager.instance.Change(startGridPos, gridPosition, gameObject);

        // �̵� �ִϸ��̼��� ���� ���� ��ǥ ���
        Vector3 startPos = GridManager.instance.GridToWorld(startGridPos);
        Vector3 endPos = GridManager.instance.GridToWorld(gridPosition);

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

    private void SetSprite(Vector2Int dir)
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
