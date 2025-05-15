using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilePos : MonoBehaviour
{
    public enum TileType { Ground, Prob }
    public TileType type;

    Tilemap tilemap;
    List<Vector2Int> boundPos = new List<Vector2Int>();
    List<Vector2Int> tilePos = new List<Vector2Int>();

    private void Awake()
    {
        tilemap = GetComponent<Tilemap>();
    }
    void Start()
    {
        if (type == TileType.Prob)
        {
            tilePos.Clear();
            GetTilePos();
            foreach (Vector2Int pos in tilePos)
            {
                GridManager.instance.Register(pos, gameObject);
            }
        }
    }
    void GetBoundPos()
    {
        foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin)
        {
            boundPos.Add(new Vector2Int(pos.x, pos.y));
        }
    }
    void GetTilePos()
    {
        foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin)
        {
            TileBase tile = tilemap.GetTile(pos);

            if (tile != null)
                tilePos.Add(new Vector2Int(pos.x, pos.y));
        }
    }
}
