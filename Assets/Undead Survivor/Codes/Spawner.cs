using UnityEngine;

public class Spawner : MonoBehaviour
{
    // Transform ������Ʈ�� �迭�� ��µ�
    public Transform[] spawnPoint;
    public SpawnData[] spawnData;

    int level;
    float timer;
    
    void Awake()
    {
        // ���� ������Ʈ�� �� �ڽĵ��� Transform ������Ʈ�� �����´�.
        spawnPoint= GetComponentsInChildren<Transform>();
    }
    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        level = Mathf.FloorToInt(GameManager.instance.gameTime / 10f);

        if (timer > 0.2f)
        {
            timer = 0;
            Spawn();
        }

        void Spawn()
        {
            GameObject enemy = GameManager.instance.pool.Get(level);
            // GetComponentsInChildren�� ���� ������Ʈ�� �������� ������ 0�� ���� ������Ʈ�� �ȴ�. �ڽĸ� �������� ���� 1���� �����Ѱ�
            enemy.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position;
        }

    }
}

[System.Serializable]
public class SpawnData
{
    public int spriteType;
    public float spawnTime;
    public int health;
    public float speed;
}
