using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
    [Header("spawn data")]
    [SerializeField] private SpawnData[] spawnData;
    [SerializeField] private int[] spawnCounts = { 30, 35, 40, 45, 50 };
    
    private List<Vector2Int> spawnPoint = new List<Vector2Int>();

    public void RandomSpawn(int index)
    {
        int count = spawnCounts[Mathf.Min(index - 1, spawnCounts.Length - 1)];

        GetSpawnPoints(count);
        MonsterSpawn();
    }

    private void GetSpawnPoints(int count)
    {
        spawnPoint.Clear();

        List<Vector2Int> emptyPoint = new List<Vector2Int>();

        for(int x = -15; x <= 15; x++)
        {
            for (int y = -9; y <= 9; y++)
            {
                Vector3 playerPos = GameManager.instance.player.transform.position;
                Vector2Int playerGridPos = GridManager.instance.WorldToGrid(playerPos);

                Vector2Int point = new Vector2Int(playerGridPos.x + x, playerGridPos.y + y);
                if(!GridManager.instance.IsObject(point))
                {
                    emptyPoint.Add(point);
                }
            }
        }

        for(int i = 0; i < Mathf.Min(count,emptyPoint.Count); i++)
        {
            int index = Random.Range(0, emptyPoint.Count);
            spawnPoint.Add(emptyPoint[index]);
            emptyPoint.RemoveAt(index);
        }
    }

    private void MonsterSpawn()
    {
        foreach(Vector2Int point in spawnPoint)
        {
            GameObject enemy = GameManager.instance.pool.Get(0);
            
            // �� ��ǥ ������Ʈ
            enemy.TryGetComponent<Enemy>(out var e);
            e.gridPos = point;
            GridManager.instance.Register(point, enemy);

            // ���� ��ǥ ������Ʈ
            enemy.transform.position = new Vector3(e.gridPos.x, e.gridPos.y, 0);

            // �����ɶ� Init�ϳ�, enable�� �� �ش� ������ ������ ���� �ʳ�? ��������
            enemy.GetComponent<Enemy>().Init(spawnData[TurnManager.instance.TurnCount % 2]);
            GameManager.instance.spawnCountTotal++;
        }
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
