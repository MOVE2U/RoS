using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager instance;
    private Dictionary<Vector2Int, GameObject> tileObject = new Dictionary<Vector2Int, GameObject>();

    private void Awake()
    {
        instance = this;
    }
    // 오브젝트가 있는지 확인
    public bool IsObject(Vector3 pos)
    {
        Vector2Int gridPos = WorldToGrid(pos);
        return tileObject.ContainsKey(gridPos);
    }

    // 오브젝트 등록
    public void Register(Vector3 pos, GameObject obj)
    {
        Vector2Int gridPos = WorldToGrid(pos);
        tileObject[gridPos] = obj;
    }

    // 오브젝트 제거
    public void Unregister(Vector3 pos)
    {
        Vector2Int gridPos = WorldToGrid(pos);
        if (IsObject(pos))
        {
            tileObject.Remove(gridPos);
        }
    }

    // 오브젝트 변경(제거하고 등록)
    public void Change(Vector3 from, Vector3 to, GameObject obj)
    {
        Unregister(from);
        Register(to, obj);
    }

    // 오브젝트 확인
    public GameObject GetObject(Vector3 pos)
    {
        Vector2Int gridPos = WorldToGrid(pos);
        if (IsObject(pos))
        {
            tileObject.TryGetValue(gridPos, out var obj);
            return obj;
        }
        return null;
    }

    // 그리드 좌표로 변환
    public Vector2Int WorldToGrid(Vector3 pos)
    {
        return Vector2Int.RoundToInt(pos);
    }
    
    // 월드 좌표로 변환
    public Vector3 GridToWorld(Vector2Int pos)
    {
        return new Vector3(pos.x, pos.y, 0);
    }
}
