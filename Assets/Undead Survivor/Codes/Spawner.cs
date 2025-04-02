using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Transform[] spawnPoint;
    public SpawnData[] spawnData;
    public float levelTime;

    int level;
    float timer;
    
    void Awake()
    {
        // ���� ������Ʈ�� �� �ڽĵ��� Transform ������Ʈ�� �����´�.
        spawnPoint = GetComponentsInChildren<Transform>();
    }
    private void Start()
    {
        levelTime = GameManager.instance.maxGameTime / spawnData.Length;
    }
    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.isLive)
            return;

        timer += Time.deltaTime;
        // ����Ƽ���� spawnData�� ������ ���س�������. �̰� ���� ������ ������ ������. �׷��� �ð��� ��� �帣�ϱ� �ƽ��� ���س��� ���� ���� ������ ������ ������ �ȳѰ� �ϴ� ��ġ
        // �迭�� 0���� �����ϴϱ� �迭�� �ε��� ������ ���� -1��.
        level = Mathf.Min(Mathf.FloorToInt(GameManager.instance.gameTime / levelTime),spawnData.Length-1);

        if (timer > spawnData[level].spawnTime)
        {
            timer = 0;
            Spawn();
        }

        void Spawn()
        {
            // public PoolManager pool;�� ���� �����ϰ�, Awake���� pool = GetComponent<PoolManager>();�� ���� �ʱ�ȭ�ؼ� pool.Get(0);���ε� ����� �� �ִ�. �ٵ� �̱����� ����ϴ°� �� ȿ�����̴�.
            // MonoBehaviour�� ��ӹ��� Class�� ��� ������Ʈ�̱� ������ GetComponent<PoolManager>ó�� �� �� �ִ�.
            GameObject enemy = GameManager.instance.pool.Get(0);

            // GetComponentsInChildren�� ���� ������Ʈ�� �������� ������ 0�� ���� ������Ʈ�� �ȴ�. �ڽĸ� �������� ���� 1���� �����Ѱ�
            enemy.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position;
            enemy.GetComponent<Enemy>().Init(spawnData[level]);
        }

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
