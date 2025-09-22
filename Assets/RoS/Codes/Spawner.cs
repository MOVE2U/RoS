using UnityEngine;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
    public static Spawner instance;

    [Header("spawn data")]
    [SerializeField] private int maxAttempts = 5;

    private List<Vector2Int> spawnPoints = new List<Vector2Int>();
    
    public List<Enemy> activeEnemies = new List<Enemy>();

    private void Awake()
    {
        instance = this;
    }

    public void FixedSpawn(List<Vector2Int> fixedPoints, SpawnData spawnData)
    {
        spawnPoints = new List<Vector2Int>(fixedPoints);
        Spawn(spawnData);
    }

    public void RandomSpawn(int index, SpawnData spawnData)
    {
        int count = spawnData.spawnCounts[Mathf.Min(index - 1, spawnData.spawnCounts.Length - 1)];
        int minSpawnDistance = spawnData.minSpawnDistance;
        int maxSpawnDistance = spawnData.maxSpawnDistance;
        GameObject prefab = spawnData.prefab;

        GetSpawnPoints(count, minSpawnDistance, maxSpawnDistance);
        Spawn(spawnData);
    }

    private void GetSpawnPoints(int count, int minSpawnDistance, int maxSpawnDistance)
    {
        spawnPoints.Clear();

        for (int i = 0; i < count * maxAttempts; i++)
        {
            // 1. 목표하는 개수에 도달했으면 탈출 (for문은 '시도'하는 것이기 때문에, 목표한 count의 5배로 시도한다)
            if (spawnPoints.Count >= count) break;

            // 2. 랜덤 위치 생성
            float angle = Random.Range(0f, 360f);
            Vector3 spawnPoint = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * Random.Range(minSpawnDistance, maxSpawnDistance);

            // 3. 해당 위치가 비어있고, spawnPoints에 없다면 추가
            if (!GridManager.instance.IsOccupant(spawnPoint) && !spawnPoints.Contains(GridManager.instance.WorldToGrid(spawnPoint)))
            {
                spawnPoints.Add(GridManager.instance.WorldToGrid(spawnPoint));
            }
        }
    }

    private void Spawn(SpawnData spawnData)
    {
        foreach(Vector2Int point in spawnPoints)
        {
            GameObject Object = GameManager.instance.pool.Get(spawnData.prefab);

            // 초기화
            ISpawnable spawnable = Object.GetComponent<ISpawnable>();
            if (spawnable != null)
            {
                spawnable.OnSpawn(spawnData, point);
            }

            GameManager.instance.spawnCountTotal++;
        }
    }

    public void AddEnemy(Enemy e)
    {
        if (!activeEnemies.Contains(e))
        {
            activeEnemies.Add(e);
        }
    }

    public void RemoveEnemy(Enemy e)
    {
        activeEnemies.Remove(e);
    }
}
