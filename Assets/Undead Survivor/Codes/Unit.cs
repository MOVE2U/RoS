using System.Collections;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public bool isMoving;
    public float grid;
    public float moveTime;

    public SpriteRenderer spriter;
    public IEnumerator MoveRoutine(Vector2 dir)
    {
        isMoving = true;

        if (dir.x != 0)
        {
            spriter.flipX = dir.x < 0;
        }

        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + (Vector3)dir * grid;
        GridManager.instance.Change(startPos, endPos, gameObject);
        float elapsedTime = 0;

        while (elapsedTime < moveTime)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / moveTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;
        isMoving = false;
    }


}
