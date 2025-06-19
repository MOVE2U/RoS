using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
    [Header("Spawn Data")]
    [SerializeField] private int[] spawnCounts = { 30, 35, 40, 45, 50 };

    private List<Vector2Int> spawnPoint = new List<Vector2Int>();

    public SpawnData[] spawnData;

    public void RandomSpawn(int index)
    {
        int count = spawnCounts[Mathf.Min(index - 1, spawnCounts.Length - 1)];

        GetSpawnPoints(count);
        MonsterSpawn();
    }

    public void GetSpawnPoints(int count)
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

    public void MonsterSpawn()
    {
        foreach(Vector2Int point in spawnPoint)
        {
            GameObject enemy = GameManager.instance.pool.Get(0);
            enemy.transform.position = new Vector3(point.x, point.y, 0);
            enemy.TryGetComponent<Enemy>(out var e);
            e.enemyGridPos = point;
            GridManager.instance.Register(point, enemy);

            enemy.GetComponent<Enemy>().Init(spawnData[TurnManager.instance.TurnCount % 2]);
            GameManager.instance.spawnCountTotal++;
        }
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
