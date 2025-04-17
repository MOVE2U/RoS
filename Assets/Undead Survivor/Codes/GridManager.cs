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
    // ������Ʈ�� �ִ��� Ȯ��
    public bool IsObject(Vector3 pos)
    {
        Vector2Int gridPos = WorldToGrid(pos);
        return tileObject.ContainsKey(gridPos);
    }

    // ������Ʈ ���
    public void Register(Vector3 pos, GameObject obj)
    {
        Vector2Int gridPos = WorldToGrid(pos);
        tileObject[gridPos] = obj;
    }

    // ������Ʈ ����
    public void Unregister(Vector3 pos)
    {
        Vector2Int gridPos = WorldToGrid(pos);
        if (IsObject(pos))
        {
            tileObject.Remove(gridPos);
        }
    }

    // ������Ʈ ����(�����ϰ� ���)
    public void Change(Vector3 from, Vector3 to, GameObject obj)
    {
        Unregister(from);
        Register(to, obj);
    }

    // ������Ʈ Ȯ��
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
