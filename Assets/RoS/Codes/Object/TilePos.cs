using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilePos : MonoBehaviour, Iinteractable
{
    public enum TileType { Ground, Prob, Silently, Rustle }
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
        tilePos.Clear();
        GetTilePos();

        switch (type)
        {
            case TileType.Prob:
                foreach (Vector2Int pos in tilePos)
                {
                    GridManager.instance.RegisterOccupant(pos, gameObject);
                }
                break;
            case TileType.Ground:
            case TileType.Silently:
            case TileType.Rustle:
                foreach (Vector2Int pos in tilePos)
                {
                    GridManager.instance.RegisterTrigger(pos, gameObject);
                }
                break;
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

    public void OnInteract()
    {
        switch (type)
        {
            case TileType.Ground:
                TurnManager.instance.MoveCountChange(1);
                foreach(Enemy e in Spawner.instance.activeEnemies)
                {
                    e.AwakeEnemy(0.15f);
                }
                break;
            case TileType.Silently:
                TurnManager.instance.MoveCountChange(-1);
                break;
            case TileType.Rustle:
                TurnManager.instance.MoveCountChange(1);
                foreach (Enemy e in Spawner.instance.activeEnemies)
                {
                    e.AwakeEnemy(0.2f);
                }
                break;
        }
    }
}
