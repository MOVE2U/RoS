using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
    public SpawnData[] spawnData;

    List<Vector3> spawnPoint = new List<Vector3>();

    public void GetSpawnPoints(Vector3 playerPos, int count)
    {
        spawnPoint.Clear();
        List<Vector3> emptyPoint = new List<Vector3>();

        for(int x = -11; x <= 11; x++)
        {
            for (int y = -6; y <= 6; y++)
            {
                Vector3 point = new Vector3(playerPos.x + x, playerPos.y + y, 0);
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
        foreach(Vector3 point in spawnPoint)
        {
            GameObject enemy = GameManager.instance.pool.Get(0);
            enemy.transform.position = point;
            GridManager.instance.Register(enemy.transform.position, enemy);
            enemy.GetComponent<Enemy>().Init(spawnData[1]);
        }
    }
    public void RandomSpawn(Vector3 playerPos, int count)
    {
        GetSpawnPoints(playerPos, count);
        MonsterSpawn();
    }
}

[System.Serializable]
public class SpawnData
{
    public float spawnTime;
    public int spriteType;
    // Enemy������ health�� float���� �����ߴµ�, float�� ������ int�� ������ �־ ���� ���� �ڵ� ��ȯ�� �Ͼ��.
    // �ʱⰪ�� float���� �����ϸ� �ε��Ҽ��� ������ �߻��� �� ������, �ʱⰪ�� int�� �����ϴ� ���� ������ �ִ�.
    public int health;
    public float speed;
}
