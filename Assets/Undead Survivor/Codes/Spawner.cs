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
        // 현재 오브젝트와 그 자식들의 Transform 컴포넌트를 가져온다.
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
        // 유니티에서 spawnData의 개수를 정해놨을꺼임. 이게 내가 정의한 레벨의 개수임. 그런데 시간은 계속 흐르니까 맥스를 정해놓기 위해 내가 정의한 레벨의 개수를 안넘게 하는 장치
        // 배열은 0부터 시작하니까 배열의 인덱스 개수는 길이 -1임.
        level = Mathf.Min(Mathf.FloorToInt(GameManager.instance.gameTime / levelTime),spawnData.Length-1);

        if (timer > spawnData[level].spawnTime)
        {
            timer = 0;
            Spawn();
        }

        void Spawn()
        {
            // public PoolManager pool;과 같이 선언하고, Awake에서 pool = GetComponent<PoolManager>();과 같이 초기화해서 pool.Get(0);으로도 사용할 수 있다. 근데 싱글톤을 사용하는게 더 효율적이다.
            // MonoBehaviour를 상속받은 Class는 모두 컴포넌트이기 때문에 GetComponent<PoolManager>처럼 쓸 수 있다.
            GameObject enemy = GameManager.instance.pool.Get(0);

            // GetComponentsInChildren은 현재 오브젝트도 가져오기 때문에 0은 현재 오브젝트가 된다. 자식만 가져오기 위해 1부터 시작한것
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
    // Enemy에서는 health를 float으로 선언했는데, float형 변수에 int형 변수를 넣어도 문제 없이 자동 변환이 일어난다.
    // 초기값을 float으로 선언하면 부동소수점 오차가 발생할 수 있으니, 초기값은 int로 선언하는 것이 장점이 있다.
    public int health;
    public float speed;
}
