using System.Collections;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public bool isMoving;
    public int grid;
    public float moveTime;
    public float wait;
    public Vector3 moveStartPos;
    public Vector3 moveEndPos;
    public Vector2Int moveDir;

    public SpriteRenderer spriter;

    protected bool TryMove(Vector2Int curGridPos, Vector2Int moveDir)
    {
        transform.right = new Vector3(moveDir.x, moveDir.y, 0f);

        Vector2Int nextGridPos = curGridPos + moveDir * grid;
        if (!GridManager.instance.IsObject(nextGridPos))
        {
            StartCoroutine(MoveRoutine(moveDir));
            return true;
        }
        return false;
    }
    protected IEnumerator MoveRoutine(Vector2Int dir)
    {
        isMoving = true;

        Vector3 startPos = transform.position;
        moveStartPos = startPos;
        Vector2Int startGridPos = GridManager.instance.WorldToGrid(startPos);

        Vector3 dirWorld = new Vector3(dir.x, dir.y, 0);
        Vector3 endPos = startPos + dirWorld * grid;
        moveEndPos = endPos;
        Vector2Int endGridPos = startGridPos + dir * grid;

        GridManager.instance.Change(startGridPos, endGridPos, gameObject);

        float elapsedTime = 0;

        while (elapsedTime < moveTime)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / moveTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = GridManager.instance.GridToWorld(endGridPos);
        yield return new WaitForSeconds(wait);

        isMoving = false;
    }
}
