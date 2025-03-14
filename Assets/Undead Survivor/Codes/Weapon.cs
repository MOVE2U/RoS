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
        player = GameManager.instance.player;
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
    }

    public void LevelUp(float damage, int count)
    {
        this.damage = damage;
        this.count += count;

        if(id == 0)
        {
            Batch();
        }
        // ������ ��ũ��Ʈ�� �����ͼ� ������ ������ ¥���� ���� �θ�.GetComponentsInChildren�� foreach�� �̿�
        // ������ ��ũ��Ʈ���� �޼��常 �����Ű�� �� ���� BroadcastMessage�� �̿�
        player.BroadcastMessage("ApplyGear",SendMessageOptions.DontRequireReceiver);
    }

    public void Init(ItemData data)
    {
        // �⺻ ����
        name = "Weapon " + data.itemId;
        transform.parent = player.transform;
        transform.localPosition = Vector3.zero;

        // ������ ����
        id = data.itemId;
        damage = data.baseDamage;
        count = data.baseCount;

        for(int index = 0; index < GameManager.instance.pool.prefabs.Length; index++)
        {
            if (data.projectile == GameManager.instance.pool.prefabs[index])
            {
                prefabId = index;
                break;
            }
        }

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
        // �̹� �ʱ�ȭ�� player�� hands �迭�� �������� �� ���̹Ƿ� ���� �ʱ�ȭ�� �ʿ����� �ʴ�.
        Hand hand = player.hands[(int)data.itemType];
        hand.spriter.sprite = data.hand;
        hand.gameObject.SetActive(true);

        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
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
