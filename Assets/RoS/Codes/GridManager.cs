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
    // 있음? 여부
    public bool IsOccupant(Vector2Int gridPos)
    {
        return occupants.ContainsKey(gridPos);
    }

    public bool IsOccupant(Vector3 worldPos)
    {
        Vector2Int gridPos = WorldToGrid(worldPos);
        return IsOccupant(gridPos);
    }

    // 있으면 줘
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

    // 등록
    public void RegisterOccupant(Vector2Int gridPos, GameObject obj)
    {
        occupants[gridPos] = obj;
    }

    public void RegisterOccupant(Vector3 worldPos, GameObject obj)
    {
        Vector2Int gridPos = WorldToGrid(worldPos);
        RegisterOccupant(gridPos, obj);
    }

    // 등록 해제
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

    // 이동
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
    // 있음? 여부
    public bool HasTriggers(Vector2Int gridPos)
    {
        return triggers.ContainsKey(gridPos);
    }

    public bool HasTriggers(Vector3 worldPos)
    {
        Vector2Int gridPos = WorldToGrid(worldPos);
        return HasTriggers(gridPos);
    }

    // 있으면 줘
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

    // 등록
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

    // 등록 해제
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
    
    private void OnDrawGizmos()
    {
        // tileObject 딕셔너리가 초기화되지 않았으면 아무것도 하지 않음 (안전장치)
        if (occupants == null)
        {
            return;
        }
   
        // 기즈모의 색상을 빨간색으로 설정
        Gizmos.color = Color.red;
   
        // tileObject 딕셔너리에 등록된 모든 항목에 대해 루프를 돕니다.
        foreach (var tile in occupants)
        {
            // tile.Key는 그리드 좌표 (Vector2Int)
            // tile.Value는 등록된 게임 오브젝트 (GameObject)
   
            if (tile.Value != null) // 혹시 모를 null 값에 대비
            {
                // 그리드 좌표를 실제 월드 좌표로 변환
                Vector3 worldPos = GridToWorld(tile.Key);
   
                // 해당 월드 좌표에 반투명한 큐브를 그립니다.
                // 사이즈는 타일 크기에 맞게 조절하세요. (예: new Vector3(1, 1, 0.1f))
                Gizmos.DrawCube(worldPos, Vector3.one* 0.8f);
            }
        }
    }
}
