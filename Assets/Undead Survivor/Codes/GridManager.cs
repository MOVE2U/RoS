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

    // ���� ����(Vector2Int)
    public bool IsObject(Vector2Int gridPos)
    {
        return tileObject.ContainsKey(gridPos);
    }

    // ���� ����(Vector3)
    public bool IsObject(Vector3 worldPos)
    {
        Vector2Int gridPos = WorldToGrid(worldPos);
        return IsObject(gridPos);
    }

    // ���(Vector2Int)
    public void Register(Vector2Int gridPos, GameObject obj)
    {
        tileObject[gridPos] = obj;
    }

    // ���(Vector3)
    public void Register(Vector3 worldPos, GameObject obj)
    {
        Vector2Int gridPos = WorldToGrid(worldPos);
        Register(gridPos, obj);
    }

    // ����(Vector2Int)
    public void Unregister(Vector2Int gridPos)
    {
        if (tileObject.ContainsKey(gridPos))
            tileObject.Remove(gridPos);
    }

    // ����(Vector3)
    public void Unregister(Vector3 worldPos)
    {
        Vector2Int gridPos = WorldToGrid(worldPos);
        Unregister(gridPos);
    }

    // �̵�(Vector2Int)
    public void Change(Vector2Int from, Vector2Int to, GameObject obj)
    {
        Unregister(from);
        Register(to, obj);
    }

    // �̵�(Vector3)
    public void Change(Vector3 worldFrom, Vector3 worldTo, GameObject obj)
    {
        Vector2Int fromGrid = WorldToGrid(worldFrom);
        Vector2Int toGrid = WorldToGrid(worldTo);
        Change(fromGrid, toGrid, obj);
    }

    // ��ȸ(Vector2Int)
    public GameObject GetObject(Vector2Int gridPos)
    {
        if (tileObject.TryGetValue(gridPos, out var obj))
            return obj;
        return null;
    }

    // ��ȸ(Vector3)
    public GameObject GetObject(Vector3 worldPos)
    {
        Vector2Int gridPos = WorldToGrid(worldPos);
        return GetObject(gridPos);
    }

    // �׸��� ��ǥ�� ��ȯ
    public Vector2Int WorldToGrid(Vector3 pos)
    {
        return Vector2Int.RoundToInt(pos);
    }
    
    // ���� ��ǥ�� ��ȯ
    public Vector3 GridToWorld(Vector2Int pos)
    {
        return new Vector3(pos.x, pos.y, 0);
    }
}
