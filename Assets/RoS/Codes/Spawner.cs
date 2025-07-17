using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
    public static Spawner instance;

    [Header("spawn data")]
    [SerializeField] private SpawnData[] spawnData;
    [SerializeField] private int[] spawnCounts = { 5, 20, 30, 40, 50 };
    [SerializeField] private int maxAttempts = 5;

    private List<Vector2Int> spawnPoints = new List<Vector2Int>();
    
    public List<Enemy> activeEnemies = new List<Enemy>();

    private void Awake()
    {
        instance = this;
    }

    public void RandomSpawn(int index, int minSpawnDistance = 14, int maxSpawnDistance = 18)
    {
        int count = spawnCounts[Mathf.Min(index - 1, spawnCounts.Length - 1)];

        GetSpawnPoints(count, minSpawnDistance, maxSpawnDistance);
        MonsterSpawn();
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
            Vector3 spawnPoint = GameManager.instance.player.transform.position + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * Random.Range(minSpawnDistance, maxSpawnDistance);

            // 3. 해당 위치가 비어있고, spawnPoints에 없다면 추가
            if (!GridManager.instance.IsOccupant(spawnPoint) && !spawnPoints.Contains(GridManager.instance.WorldToGrid(spawnPoint)))
            {
                spawnPoints.Add(GridManager.instance.WorldToGrid(spawnPoint));
            }
        }
    }

    private void MonsterSpawn()
    {
        foreach(Vector2Int point in spawnPoints)
        {
            GameObject enemyObj = GameManager.instance.pool.Get(0);
            
            // 논리 좌표 업데이트
            enemyObj.TryGetComponent<Enemy>(out var enemy);
            enemy.gridPos = point;
            GridManager.instance.RegisterOccupant(point, enemyObj);

            // 월드 좌표 업데이트
            enemyObj.transform.position = new Vector3(enemy.gridPos.x, enemy.gridPos.y, 0);

            // 초기화
            enemy.Init(spawnData[TurnManager.instance.TurnCount % 2]);

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

// 데이터를 그룹화해서 부모 클래스의 인스펙터에서 데이터를 관리하는 방식. 간단한 데이터에 사용
[System.Serializable]
public class SpawnData
{
    public int spriteType;
    // Enemy에서는 health를 float으로 선언했는데, float형 변수에 int형 변수를 넣어도 문제 없이 자동 변환이 일어난다.
    // 초기값을 float으로 선언하면 부동소수점 오차가 발생할 수 있으니, 초기값은 int로 선언하는 것이 장점이 있다.
    public int health;
}
