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
    
    private void OnDrawGizmos()
    {
        // tileObject 딕셔너리가 초기화되지 않았으면 아무것도 하지 않음 (안전장치)
        if (tileObject == null)
        {
            return;
        }
   
        // 기즈모의 색상을 빨간색으로 설정
        Gizmos.color = Color.red;
   
        // tileObject 딕셔너리에 등록된 모든 항목에 대해 루프를 돕니다.
        foreach (var tile in tileObject)
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
