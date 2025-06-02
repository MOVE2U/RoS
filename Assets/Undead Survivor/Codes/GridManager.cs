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

    // 존재 여부(Vector2Int)
    public bool IsObject(Vector2Int gridPos)
    {
        return tileObject.ContainsKey(gridPos);
    }

    // 존재 여부(Vector3)
    public bool IsObject(Vector3 worldPos)
    {
        Vector2Int gridPos = WorldToGrid(worldPos);
        return IsObject(gridPos);
    }

    // 등록(Vector2Int)
    public void Register(Vector2Int gridPos, GameObject obj)
    {
        tileObject[gridPos] = obj;
    }

    // 등록(Vector3)
    public void Register(Vector3 worldPos, GameObject obj)
    {
        Vector2Int gridPos = WorldToGrid(worldPos);
        Register(gridPos, obj);
    }

    // 제거(Vector2Int)
    public void Unregister(Vector2Int gridPos)
    {
        if (tileObject.ContainsKey(gridPos))
            tileObject.Remove(gridPos);
    }

    // 제거(Vector3)
    public void Unregister(Vector3 worldPos)
    {
        Vector2Int gridPos = WorldToGrid(worldPos);
        Unregister(gridPos);
    }

    // 이동(Vector2Int)
    public void Change(Vector2Int from, Vector2Int to, GameObject obj)
    {
        Unregister(from);
        Register(to, obj);
    }

    // 이동(Vector3)
    public void Change(Vector3 worldFrom, Vector3 worldTo, GameObject obj)
    {
        Vector2Int fromGrid = WorldToGrid(worldFrom);
        Vector2Int toGrid = WorldToGrid(worldTo);
        Change(fromGrid, toGrid, obj);
    }

    // 조회(Vector2Int)
    public GameObject GetObject(Vector2Int gridPos)
    {
        if (tileObject.TryGetValue(gridPos, out var obj))
            return obj;
        return null;
    }

    // 조회(Vector3)
    public GameObject GetObject(Vector3 worldPos)
    {
        Vector2Int gridPos = WorldToGrid(worldPos);
        return GetObject(gridPos);
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
