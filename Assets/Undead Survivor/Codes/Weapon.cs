using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int id;
    public int prefabId;
    public float damage;
    public int count;
    public float speed;

    float timer;
    Player player;

    private void Awake()
    {
        player = GetComponentInParent<Player>();
    }

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
                timer += Time.deltaTime;

                if(timer > speed)
                {
                    timer = 0;
                    Fire();
                }
                break;
        }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            LevelUp(10,1);
        }
    }

    public void LevelUp(float damage, int count)
    {
        this.damage += damage;
        this.count += count;
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
                speed = 0.5f;
                break;
        }
    }

    void Batch()
    {
        switch (id)
        {
            case 0:
                for (int index = 0; index < count; index++)
                {
                    Transform bullet;

                    if (index < transform.childCount)
                    {
                        bullet = transform.GetChild(index);
                    }
                    else
                    {
                        bullet = GameManager.instance.pool.Get(prefabId).transform;
                        bullet.parent = transform;
                    }
                    // �ʱ�ȭ
                    bullet.localPosition = Vector3.zero;
                    bullet.localRotation = Quaternion.identity;

                    bullet.Rotate(Vector3.forward * index * 360 / count);
                    // Vector3.up�� ���� ��ǥ�� ���� ����. Translate.up(���⼱ bullet.up�� ���)�� ���� ��ǥ�� ����.
                    // Translate�� �ι�° ���ڴ� ���� ��ǥ��, ���� ��ǥ�踦 ����. Space.World �Ǵ� Space.Self.
                    // ���⼱ �̹� bullet.up���� ���� ��ǥ�踦 ����߱� ������, Space.World�� ����ص�, Space.Self�� ����ص� ���� ����� ���´�.
                    bullet.Translate(bullet.up * 1.5f, Space.World);
                    // bullet�� GameObject�� ������ �ƴ϶� ������Ʈ�� Transform��. �׷����� �ұ��ϰ� �ٸ� ������Ʈ�� �Ʒ�ó�� ������ �� ����.
                    // ��, GetComponent<t>()�� �ٸ� ������Ʈ������ ������ �� ����.
                    bullet.GetComponent<Bullet>().Init(damage, -1, Vector3.zero); // -1 is Infinity Per.
                }
                break;
            default:
                break;
        }
    }

    void Fire()
    {
        if (!player.scanner.nearestTarget)
            return;

        Vector3 targetPos = player.scanner.nearestTarget.position;
        Vector3 dir = (targetPos - transform.position).normalized;

        Transform bullet = GameManager.instance.pool.Get(prefabId).transform;
        bullet.position = transform.position;
        bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);
        bullet.GetComponent<Bullet>().Init(damage, count, dir);
    }
}
