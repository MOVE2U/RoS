using UnityEngine;

public class Spawner : MonoBehaviour
{
    // Transform 컴포넌트를 배열에 담는데
    public Transform[] spawnPoint;
    public SpawnData[] spawnData;

    int level;
    float timer;
    
    void Awake()
    {
        // 현재 오브젝트와 그 자식들의 Transform 컴포넌트를 가져온다.
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
            // GetComponentsInChildren은 현재 오브젝트도 가져오기 때문에 0은 현재 오브젝트가 된다. 자식만 가져오기 위해 1부터 시작한것
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
