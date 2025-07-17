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
            // 1. ��ǥ�ϴ� ������ ���������� Ż�� (for���� '�õ�'�ϴ� ���̱� ������, ��ǥ�� count�� 5��� �õ��Ѵ�)
            if (spawnPoints.Count >= count) break;

            // 2. ���� ��ġ ����
            float angle = Random.Range(0f, 360f);
            Vector3 spawnPoint = GameManager.instance.player.transform.position + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * Random.Range(minSpawnDistance, maxSpawnDistance);

            // 3. �ش� ��ġ�� ����ְ�, spawnPoints�� ���ٸ� �߰�
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
            
            // �� ��ǥ ������Ʈ
            enemyObj.TryGetComponent<Enemy>(out var enemy);
            enemy.gridPos = point;
            GridManager.instance.RegisterOccupant(point, enemyObj);

            // ���� ��ǥ ������Ʈ
            enemyObj.transform.position = new Vector3(enemy.gridPos.x, enemy.gridPos.y, 0);

            // �ʱ�ȭ
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

// �����͸� �׷�ȭ�ؼ� �θ� Ŭ������ �ν����Ϳ��� �����͸� �����ϴ� ���. ������ �����Ϳ� ���
[System.Serializable]
public class SpawnData
{
    public int spriteType;
    // Enemy������ health�� float���� �����ߴµ�, float�� ������ int�� ������ �־ ���� ���� �ڵ� ��ȯ�� �Ͼ��.
    // �ʱⰪ�� float���� �����ϸ� �ε��Ҽ��� ������ �߻��� �� ������, �ʱⰪ�� int�� �����ϴ� ���� ������ �ִ�.
    public int health;
}
