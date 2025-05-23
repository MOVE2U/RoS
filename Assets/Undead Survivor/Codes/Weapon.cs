using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
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

    void Update()
    {
        if (!GameManager.instance.isLive)
            return;

        timer += Time.deltaTime;

        if (timer > speed)
        {
            timer = 0;
            Attack();
        }
    }
    public void Init(ItemData data)
    {
        // �⺻ ����
        name = "Weapon " + data.itemId;
        transform.parent = player.transform;
        transform.localPosition = Vector3.zero;

        // ������ ����
        id = data.itemId;
        damage = data.baseDamage * Character.Damage;
        count = data.baseCount + Character.Count;
        speed = 1f;

        for (int index = 0; index < GameManager.instance.pool.prefabs.Length; index++)
        {
            if (data.projectile == GameManager.instance.pool.prefabs[index])
            {
                prefabId = index;
                break;
            }
        }

        // �̹� �ʱ�ȭ�� player�� hands �迭�� �������� �� ���̹Ƿ� ���� �ʱ�ȭ�� �ʿ����� �ʴ�.
        Hand hand = player.hands[(int)data.itemType];
        hand.spriter.sprite = data.hand;
        // hand.gameObject.SetActive(true);

        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
    }
    void Attack()
    {
        List<Vector2Int> tiles = GetAttackTiles();
        Debug.Log("���� Ÿ�� ���: " + string.Join(", ", tiles));

        foreach (var tile in tiles)
        {
            GameObject obj = GridManager.instance.GetObject(tile);
            if (obj != null && obj.TryGetComponent<Enemy>(out var enemy))
            {
                enemy.Attacked(damage);
            }
        }

        StartCoroutine(ShowEffect());
    }
    List<Vector2Int> GetAttackTiles()
    {
        List<Vector2Int> tiles = new List<Vector2Int>();

        if(!player.isMoving)
        {
            Vector2Int playerGridPos = GridManager.instance.WorldToGrid(player.transform.position);
            Vector2Int targetGrisPos = playerGridPos + player.lastStopDir * player.grid;
            tiles.Add(targetGrisPos);
        }
        else
        {
            Vector2Int startGridPos = GridManager.instance.WorldToGrid(player.moveStartPos);
            Vector2Int targetGrisPos1 = startGridPos + player.lastMoveDir * player.grid;
            tiles.Add(targetGrisPos1);

            Vector2Int endGridPos = GridManager.instance.WorldToGrid(player.moveEndPos);
            Vector2Int targetGrisPos2 = endGridPos + player.lastMoveDir * player.grid;
            tiles.Add(targetGrisPos2);
        }
            return tiles;
    }
    IEnumerator ShowEffect()
    {
        var effects = new List<GameObject>();

        // 1) ���� count ��ŭ ���� �����ؼ� ����Ʈ�� ���
        for (int i = 0; i < count; i++)
        {
            Transform effect = GameManager.instance.pool.Get(prefabId).transform;
            effect.parent = transform;
            effect.localPosition = new Vector3(1 * (1 + i), 0, 0);
            effects.Add(effect.gameObject);
        }

        // 2) �� ���� ���
        yield return new WaitForSeconds(0.3f);

        // 3) ����Ʈ�� ��� ��� ����Ʈ�� ���ÿ� ��Ȱ��ȭ
        foreach (var go in effects)
            go.SetActive(false);
    }

    //void Update()
    //{
    //    if (!GameManager.instance.isLive)
    //        return;

    //    switch (id)
    //    {
    //        case 0:
    //            transform.Rotate(Vector3.back * speed * Time.deltaTime);
    //            break;
    //        default:
    //            timer += Time.deltaTime;

    //            if(timer > speed)
    //            {
    //                timer = 0;
    //                Fire();
    //            }
    //            break;
    //    }
    //}

    public void LevelUp(float damage, int count)
    {
        this.damage = damage * Character.Damage;
        this.count += count;

        //if (id == 0)
        //{
        //    Batch();
        //}
        //// ������ ��ũ��Ʈ�� �����ͼ� ������ ������ ¥���� ���� �θ�.GetComponentsInChildren�� foreach�� �̿�
        //// ������ ��ũ��Ʈ���� �޼��常 �����Ű�� �� ���� BroadcastMessage�� �̿�
        //player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
    }

    //public void Init(ItemData data)
    //{
    //    // �⺻ ����
    //    name = "Weapon " + data.itemId;
    //    transform.parent = player.transform;
    //    transform.localPosition = Vector3.zero;

    //    // ������ ����
    //    id = data.itemId;
    //    damage = data.baseDamage * Character.Damage;
    //    count = data.baseCount + Character.Count;

    //    for(int index = 0; index < GameManager.instance.pool.prefabs.Length; index++)
    //    {
    //        if (data.projectile == GameManager.instance.pool.prefabs[index])
    //        {
    //            prefabId = index;
    //            break;
    //        }
    //    }

    //    switch (id)
    //    {
    //        case 0:
    //            speed = 150 * Character.WeaponSpeed;
    //            Batch();
    //            break;
    //        default:
    //            speed = 0.5f * Character.WeaponRate;
    //            break;
    //    }
    //    // �̹� �ʱ�ȭ�� player�� hands �迭�� �������� �� ���̹Ƿ� ���� �ʱ�ȭ�� �ʿ����� �ʴ�.
    //    Hand hand = player.hands[(int)data.itemType];
    //    hand.spriter.sprite = data.hand;
    //    hand.gameObject.SetActive(true);

    //    player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
    //}

    //void Batch()
    //{
    //    switch (id)
    //    {
    //        case 0:


    //            ȸ�� ���� �ڵ�

    //            for (int index = 0; index < count; index++)
    //            {
    //                Transform bullet;

    //                if (index < transform.childCount)
    //                {
    //                    bullet = transform.GetChild(index);
    //                }
    //                else
    //                {
    //                    bullet = GameManager.instance.pool.Get(prefabId).transform;
    //                    bullet.parent = transform;
    //                }
    //                // �ʱ�ȭ
    //                bullet.localPosition = Vector3.zero;
    //                bullet.localRotation = Quaternion.identity;

    //                bullet.Rotate(Vector3.forward * index * 360 / count);
    //                // Vector3.up�� ���� ��ǥ�� ���� ����. Translate.up(���⼱ bullet.up�� ���)�� ���� ��ǥ�� ����.
    //                // Translate�� �ι�° ���ڴ� ���� ��ǥ��, ���� ��ǥ�踦 ����. Space.World �Ǵ� Space.Self.
    //                // ���⼱ �̹� bullet.up���� ���� ��ǥ�踦 ����߱� ������, Space.World�� ����ص�, Space.Self�� ����ص� ���� ����� ���´�.
    //                bullet.Translate(bullet.up * 1.5f, Space.World);
    //                // bullet�� GameObject�� ������ �ƴ϶� ������Ʈ�� Transform��. �׷����� �ұ��ϰ� �ٸ� ������Ʈ�� �Ʒ�ó�� ������ �� ����.
    //                // ��, GetComponent<t>()�� �ٸ� ������Ʈ������ ������ �� ����.
    //                bullet.GetComponent<Bullet>().Init(damage, -100, Vector3.zero); // -100 is Infinity Per.
    //            }
    //            break;
    //        default:
    //            break;
    //    }
    //}

    //void Fire()
    //{
    //    if (!player.scanner.nearestTarget)
    //        return;

    //    Vector3 targetPos = player.scanner.nearestTarget.position;
    //    Vector3 dir = (targetPos - transform.position).normalized;

    //    Transform bullet = GameManager.instance.pool.Get(prefabId).transform;
    //    bullet.position = transform.position;
    //    bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);
    //    bullet.GetComponent<Bullet>().Init(damage, count, dir);

    //    AudioManager.instance.PlaySfx(AudioManager.Sfx.Range);
    //}
}
