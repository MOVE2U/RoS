using UnityEngine;

public interface ISpawnable
{
    void OnSpawn(SpawnData spawnData, Vector2Int pos);
}
