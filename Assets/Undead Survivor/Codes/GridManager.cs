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
    public bool IsObject(Vector2Int gridPos)
    {
        return tileObject.ContainsKey(gridPos);
    }

    // ������Ʈ ���
    public void Register(Vector2Int gridPos, GameObject obj)
    {
        tileObject[gridPos] = obj;
    }

    // ������Ʈ ����
    public void Unregister(Vector2Int gridPos)
    {
        if (IsObject(gridPos))
        {
            tileObject.Remove(gridPos);
        }
    }

    // ������Ʈ ����(�����ϰ� ���)
    public void Change(Vector2Int from, Vector2Int to, GameObject obj)
    {
        Unregister(from);
        Register(to, obj);
    }

    // ������Ʈ Ȯ��
    public GameObject GetObject(Vector2Int gridPos)
    {
        if (IsObject(gridPos))
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
