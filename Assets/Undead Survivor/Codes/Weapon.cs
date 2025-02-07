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

            // Vector3.up�� ���� ��ǥ�� ���� ����. Translate.up(���⼱ bullet.up�� ���)�� ���� ��ǥ�� ����.
            // Translate�� �ι�° ���ڴ� ���� ��ǥ��, ���� ��ǥ�踦 ����. Space.World �Ǵ� Space.Self.
            // ���⼱ �̹� bullet.up���� ���� ��ǥ�踦 ����߱� ������, Space.World�� ����ص�, Space.Self�� ����ص� ���� ����� ���´�.
            bullet.Translate(bullet.up * 1.5f, Space.World);

            // bullet�� GameObject�� ������ �ƴ϶� ������Ʈ�� Transform��. �׷����� �ұ��ϰ� �ٸ� ������Ʈ�� �Ʒ�ó�� ������ �� ����.
            // ��, GetComponent<t>()�� �ٸ� ������Ʈ������ ������ �� ����.
            bullet.GetComponent<Bullet>().Init(damage, -1); // -1 is Infinity Per.
        }
    }
}
