using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int id;
    public int prefabId;
    public float damage;
    public int count;
    public float speed;

    private void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        switch (id)
        {
            case 0:
                transform.Rotate(Vector3.back * speed * Time.deltaTime);
                break;
            default:
                break;
        }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            LevelUp(5);
        }
    }

    public void LevelUp(int count)
    {
        this.count = count;
        Batch();
    }

    public void Init()
    {
        switch (id)
        {
            case 0:
                speed = 150;
                Batch();
                break;
            default:
                break;
        }
    }

    void Batch()
    {
        for (int index = 0; index < count; index++)
        {
            Transform bullet = GameManager.instance.pool.Get(prefabId).transform;
            bullet.parent = transform;
            bullet.Rotate(Vector3.forward * index * 360 / count);

            // Vector3.up은 월드 좌표계 기준 위쪽. Translate.up(여기선 bullet.up을 사용)은 로컬 좌표계 기준.
            // Translate의 두번째 인자는 월드 좌표계, 로컬 좌표계를 결정. Space.World 또는 Space.Self.
            // 여기선 이미 bullet.up에서 로컬 좌표계를 사용했기 때문에, Space.World를 사용해도, Space.Self를 사용해도 같은 결과가 나온다.
            bullet.Translate(bullet.up * 1.5f, Space.World);

            // bullet은 GameObject형 변수가 아니라 컴포넌트인 Transform임. 그럼에도 불구하고 다른 컴포넌트에 아래처럼 접근할 수 있음.
            // 즉, GetComponent<t>()는 다른 컴포넌트에서도 접근할 수 있음.
            bullet.GetComponent<Bullet>().Init(damage, -1); // -1 is Infinity Per.
        }
    }
}
