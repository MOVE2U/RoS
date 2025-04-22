using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
    public SpawnData[] spawnData;
    public int[] spawnCount = { 10, 20, 30, 40, 50 };

    List<Vector2Int> spawnPoint = new List<Vector2Int>();

    public void RandomSpawn(Vector2Int playerGirdPos, int count)
    {
        GetSpawnPoints(playerGirdPos, count);
        MonsterSpawn();
    }

    public void GetSpawnPoints(Vector2Int playerGirdPos, int count)
    {
        spawnPoint.Clear();

        List<Vector2Int> emptyPoint = new List<Vector2Int>();

        for(int x = -11; x <= 11; x++)
        {
            for (int y = -6; y <= 6; y++)
            {
                Vector2Int point = new Vector2Int(playerGirdPos.x + x, playerGirdPos.y + y);
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

    public void MonsterSpawn()
    {
        foreach(Vector2Int point in spawnPoint)
        {
            GameObject enemy = GameManager.instance.pool.Get(0);
            enemy.transform.position = new Vector3(point.x, point.y, 0);
            GridManager.instance.Register(point, enemy);

            enemy.GetComponent<Enemy>().Init(spawnData[TurnManager.instance.enemyTurnCount % 2]);
            GameManager.instance.spawnCountTotal++;
        }
    }

}

[System.Serializable]
public class SpawnData
{
    public int spriteType;
    // Enemy������ health�� float���� �����ߴµ�, float�� ������ int�� ������ �־ ���� ���� �ڵ� ��ȯ�� �Ͼ��.
    // �ʱⰪ�� float���� �����ϸ� �ε��Ҽ��� ������ �߻��� �� ������, �ʱⰪ�� int�� �����ϴ� ���� ������ �ִ�.
    public int health;
    public float wait;
}
