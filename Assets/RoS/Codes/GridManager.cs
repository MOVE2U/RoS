using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager instance;
    private Dictionary<Vector2Int, GameObject> occupants = new Dictionary<Vector2Int, GameObject>();
    private Dictionary<Vector2Int, List<GameObject>> triggers = new Dictionary<Vector2Int, List<GameObject>>();

    private void Awake()
    {
        instance = this;
    }

    #region occupant
    // ����? ����
    public bool IsOccupant(Vector2Int gridPos)
    {
        return occupants.ContainsKey(gridPos);
    }

    public bool IsOccupant(Vector3 worldPos)
    {
        Vector2Int gridPos = WorldToGrid(worldPos);
        return IsOccupant(gridPos);
    }

    // ������ ��
    public GameObject GetOccupant(Vector2Int gridPos)
    {
        if (occupants.TryGetValue(gridPos, out var obj))
            return obj;
        return null;
    }

    public GameObject GetOccupant(Vector3 worldPos)
    {
        Vector2Int gridPos = WorldToGrid(worldPos);
        return GetOccupant(gridPos);
    }

    // ���
    public void RegisterOccupant(Vector2Int gridPos, GameObject obj)
    {
        occupants[gridPos] = obj;
    }

    public void RegisterOccupant(Vector3 worldPos, GameObject obj)
    {
        Vector2Int gridPos = WorldToGrid(worldPos);
        RegisterOccupant(gridPos, obj);
    }

    // ��� ����
    public void UnregisterOccupant(Vector2Int gridPos)
    {
        if (occupants.ContainsKey(gridPos))
            occupants.Remove(gridPos);
    }

    public void UnregisterOccupant(Vector3 worldPos)
    {
        Vector2Int gridPos = WorldToGrid(worldPos);
        UnregisterOccupant(gridPos);
    }

    // �̵�
    public void MoveOccupant(Vector2Int from, Vector2Int to, GameObject obj)
    {
        UnregisterOccupant(from);
        RegisterOccupant(to, obj);
    }

    public void MoveOccupant(Vector3 worldFrom, Vector3 worldTo, GameObject obj)
    {
        Vector2Int fromGrid = WorldToGrid(worldFrom);
        Vector2Int toGrid = WorldToGrid(worldTo);
        MoveOccupant(fromGrid, toGrid, obj);
    }
    #endregion

    #region trigger
    // ����? ����
    public bool HasTriggers(Vector2Int gridPos)
    {
        return triggers.ContainsKey(gridPos);
    }

    public bool HasTriggers(Vector3 worldPos)
    {
        Vector2Int gridPos = WorldToGrid(worldPos);
        return HasTriggers(gridPos);
    }

    // ������ ��
    public List<GameObject> GetTriggers(Vector2Int gridPos)
    {
        if (triggers.TryGetValue(gridPos, out var list))
            return list;
        return null;
    }

    public List<GameObject> GetTriggers(Vector3 worldPos)
    {
        Vector2Int gridPos = WorldToGrid(worldPos);
        return GetTriggers(gridPos);
    }

    // ���
    public void RegisterTrigger(Vector2Int gridPos, GameObject obj)
    {
        if(triggers.TryGetValue(gridPos, out var existingList))
        {
            existingList.Add(obj);
        }
        else
        {
            triggers[gridPos] = new List<GameObject> { obj };
        }
    }

    public void RegisterTrigger(Vector3 worldPos, GameObject obj)
    {
        Vector2Int gridPos = WorldToGrid(worldPos);
        RegisterTrigger(gridPos, obj);
    }

    // ��� ����
    public void UnregisterTrigger(Vector2Int gridPos, GameObject obj)
    {
        if (triggers.TryGetValue(gridPos, out var existingList))
        {
            existingList.Remove(obj);

            if (existingList.Count == 0)
            {
                triggers.Remove(gridPos);
            }
        }
    }

    public void UnregisterTrigger(Vector3 worldPos, GameObject obj)
    {
        Vector2Int gridPos = WorldToGrid(worldPos);
        UnregisterTrigger(gridPos, obj);
    }
    #endregion

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
    
    private void OnDrawGizmos()
    {
        // tileObject ��ųʸ��� �ʱ�ȭ���� �ʾ����� �ƹ��͵� ���� ���� (������ġ)
        if (occupants == null)
        {
            return;
        }
   
        // ������� ������ ���������� ����
        Gizmos.color = Color.red;
   
        // tileObject ��ųʸ��� ��ϵ� ��� �׸� ���� ������ ���ϴ�.
        foreach (var tile in occupants)
        {
            // tile.Key�� �׸��� ��ǥ (Vector2Int)
            // tile.Value�� ��ϵ� ���� ������Ʈ (GameObject)
   
            if (tile.Value != null) // Ȥ�� �� null ���� ���
            {
                // �׸��� ��ǥ�� ���� ���� ��ǥ�� ��ȯ
                Vector3 worldPos = GridToWorld(tile.Key);
   
                // �ش� ���� ��ǥ�� �������� ť�긦 �׸��ϴ�.
                // ������� Ÿ�� ũ�⿡ �°� �����ϼ���. (��: new Vector3(1, 1, 0.1f))
                Gizmos.DrawCube(worldPos, Vector3.one* 0.8f);
            }
        }
    }
}
