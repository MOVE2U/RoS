using UnityEngine;

[CreateAssetMenu(fileName = "SpawnData", menuName = "Scriptable Objects/SpawnData")]
public class SpawnData : ScriptableObject
{
    public GameObject prefab;
    public int[] spawnCounts;
    public int minSpawnDistance;
    public int maxSpawnDistance;
    public int health;
}
