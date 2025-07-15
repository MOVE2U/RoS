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

    // 존재 여부(Vector2Int)
    public bool IsOccupant(Vector2Int gridPos)
    {
        return occupants.ContainsKey(gridPos);
    }

    // 존재 여부(Vector3)
    public bool IsOccupant(Vector3 worldPos)
    {
        Vector2Int gridPos = WorldToGrid(worldPos);
        return IsOccupant(gridPos);
    }

    // 등록(Vector2Int)
    public void RegisterOccupant(Vector2Int gridPos, GameObject obj)
    {
        occupants[gridPos] = obj;
    }

    // 등록(Vector3)
    public void RegisterOccupant(Vector3 worldPos, GameObject obj)
    {
        Vector2Int gridPos = WorldToGrid(worldPos);
        RegisterOccupant(gridPos, obj);
    }

    // 제거(Vector2Int)
    public void UnregisterOccupant(Vector2Int gridPos)
    {
        if (occupants.ContainsKey(gridPos))
            occupants.Remove(gridPos);
    }

    // 제거(Vector3)
    public void UnregisterOccupant(Vector3 worldPos)
    {
        Vector2Int gridPos = WorldToGrid(worldPos);
        UnregisterOccupant(gridPos);
    }

    // 이동(Vector2Int)
    public void ChangeOccupant(Vector2Int from, Vector2Int to, GameObject obj)
    {
        UnregisterOccupant(from);
        RegisterOccupant(to, obj);
    }

    // 이동(Vector3)
    public void ChangeOccupant(Vector3 worldFrom, Vector3 worldTo, GameObject obj)
    {
        Vector2Int fromGrid = WorldToGrid(worldFrom);
        Vector2Int toGrid = WorldToGrid(worldTo);
        ChangeOccupant(fromGrid, toGrid, obj);
    }

    // 조회(Vector2Int)
    public List<GameObject> GetTriggerList(Vector2Int gridPos)
    {
        if (triggerLists.TryGetValue(gridPos, out var obj))
            return obj;
        return null;
    }

    // 조회(Vector3)
    public List<GameObject> GetTriggerList(Vector3 worldPos)
    {
        Vector2Int gridPos = WorldToGrid(worldPos);
        return GetTriggerList(gridPos);
    }

    // 존재 여부(Vector2Int)
    public bool IsTriggerList(Vector2Int gridPos)
    {
        return triggerLists.ContainsKey(gridPos);
    }

    // 존재 여부(Vector3)
    public bool IsTriggerList(Vector3 worldPos)
    {
        Vector2Int gridPos = WorldToGrid(worldPos);
        return IsTriggerList(gridPos);
    }

    // 등록(Vector2Int)
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

    // 등록(Vector3)
    public void RegisterTriggerList(Vector3 worldPos, GameObject obj)
    {
        Vector2Int gridPos = WorldToGrid(worldPos);
        RegisterTriggerList(gridPos, obj);
    }

    // 제거(Vector2Int)
    public void UnregisterTriggerList(Vector2Int gridPos)
    {
        if (triggerLists.ContainsKey(gridPos))
        {
            List<GameObject> existingList = GetTriggerList(gridPos);
            if (existingList.Count > 1)
            {
                existingList.RemoveAt(0); // 첫 번째 요소 제거
            }
            else
            {
                triggerLists.Remove(gridPos);
            }
        }
    }

    // 제거(Vector3)
    public void UnregisterTriggerList(Vector3 worldPos)
    {
        Vector2Int gridPos = WorldToGrid(worldPos);
        UnregisterTriggerList(gridPos);
    }

    // 조회(Vector2Int)
    public GameObject GetOccupant(Vector2Int gridPos)
    {
        if (occupants.TryGetValue(gridPos, out var obj))
            return obj;
        return null;
    }

    // 조회(Vector3)
    public GameObject GetOccupant(Vector3 worldPos)
    {
        Vector2Int gridPos = WorldToGrid(worldPos);
        return GetOccupant(gridPos);
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
