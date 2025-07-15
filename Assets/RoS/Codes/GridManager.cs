using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager instance;
    private Dictionary<Vector2Int, GameObject> occupants = new Dictionary<Vector2Int, GameObject>();
    private Dictionary<Vector2Int, List<GameObject>> triggerLists = new Dictionary<Vector2Int, List<GameObject>>();

    private void Awake()
    {
        instance = this;
    }

    // ���� ����(Vector2Int)
    public bool IsOccupant(Vector2Int gridPos)
    {
        return occupants.ContainsKey(gridPos);
    }

    // ���� ����(Vector3)
    public bool IsOccupant(Vector3 worldPos)
    {
        Vector2Int gridPos = WorldToGrid(worldPos);
        return IsOccupant(gridPos);
    }

    // ���(Vector2Int)
    public void RegisterOccupant(Vector2Int gridPos, GameObject obj)
    {
        occupants[gridPos] = obj;
    }

    // ���(Vector3)
    public void RegisterOccupant(Vector3 worldPos, GameObject obj)
    {
        Vector2Int gridPos = WorldToGrid(worldPos);
        RegisterOccupant(gridPos, obj);
    }

    // ����(Vector2Int)
    public void UnregisterOccupant(Vector2Int gridPos)
    {
        if (occupants.ContainsKey(gridPos))
            occupants.Remove(gridPos);
    }

    // ����(Vector3)
    public void UnregisterOccupant(Vector3 worldPos)
    {
        Vector2Int gridPos = WorldToGrid(worldPos);
        UnregisterOccupant(gridPos);
    }

    // �̵�(Vector2Int)
    public void ChangeOccupant(Vector2Int from, Vector2Int to, GameObject obj)
    {
        UnregisterOccupant(from);
        RegisterOccupant(to, obj);
    }

    // �̵�(Vector3)
    public void ChangeOccupant(Vector3 worldFrom, Vector3 worldTo, GameObject obj)
    {
        Vector2Int fromGrid = WorldToGrid(worldFrom);
        Vector2Int toGrid = WorldToGrid(worldTo);
        ChangeOccupant(fromGrid, toGrid, obj);
    }

    // ��ȸ(Vector2Int)
    public List<GameObject> GetTriggerList(Vector2Int gridPos)
    {
        if (triggerLists.TryGetValue(gridPos, out var obj))
            return obj;
        return null;
    }

    // ��ȸ(Vector3)
    public List<GameObject> GetTriggerList(Vector3 worldPos)
    {
        Vector2Int gridPos = WorldToGrid(worldPos);
        return GetTriggerList(gridPos);
    }

    // ���� ����(Vector2Int)
    public bool IsTriggerList(Vector2Int gridPos)
    {
        return triggerLists.ContainsKey(gridPos);
    }

    // ���� ����(Vector3)
    public bool IsTriggerList(Vector3 worldPos)
    {
        Vector2Int gridPos = WorldToGrid(worldPos);
        return IsTriggerList(gridPos);
    }

    // ���(Vector2Int)
    public void RegisterTriggerList(Vector2Int gridPos, GameObject obj)
    {
        if(IsTriggerList(gridPos))
        {
            List<GameObject> existingList = GetTriggerList(gridPos);
            existingList.Add(obj);
        }
        else
        {
            triggerLists[gridPos] = new List<GameObject> { obj };
        }
    }

    // ���(Vector3)
    public void RegisterTriggerList(Vector3 worldPos, GameObject obj)
    {
        Vector2Int gridPos = WorldToGrid(worldPos);
        RegisterTriggerList(gridPos, obj);
    }

    // ����(Vector2Int)
    public void UnregisterTriggerList(Vector2Int gridPos)
    {
        if (triggerLists.ContainsKey(gridPos))
        {
            List<GameObject> existingList = GetTriggerList(gridPos);
            if (existingList.Count > 1)
            {
                existingList.RemoveAt(0); // ù ��° ��� ����
            }
            else
            {
                triggerLists.Remove(gridPos);
            }
        }
    }

    // ����(Vector3)
    public void UnregisterTriggerList(Vector3 worldPos)
    {
        Vector2Int gridPos = WorldToGrid(worldPos);
        UnregisterTriggerList(gridPos);
    }

    // ��ȸ(Vector2Int)
    public GameObject GetOccupant(Vector2Int gridPos)
    {
        if (occupants.TryGetValue(gridPos, out var obj))
            return obj;
        return null;
    }

    // ��ȸ(Vector3)
    public GameObject GetOccupant(Vector3 worldPos)
    {
        Vector2Int gridPos = WorldToGrid(worldPos);
        return GetOccupant(gridPos);
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
