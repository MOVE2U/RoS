using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("for check")]
    private int id;
    private int prefabId;
    private float damage;
    private int count;
    public float speed; // ���߿� Gear���� �����ϴ� �޼��带 Weapon���� ����� private set���� ���� �ʿ�

    private float timer;
    private Player player;

    public int Id => id;

    private void Awake()
    {
        player = GameManager.instance.player;
    }

    private void Update()
    {
        if (TurnManager.instance.CurState == TurnState.EnemyTurn 
            && GameManager.instance.isLive)
        {
            timer += Time.deltaTime;

            if (timer > speed)
            {
                timer = 0;
                Attack();
            }
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


        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
    }

    void Attack()
    {
        // 1. ���� Ÿ�� ���
        List<Vector2Int> attackTiles = GetAttackTiles();
        // Debug.Log("���� Ÿ�� ���: " + string.Join(", ", tiles));

        // 2. ���� Ÿ�Ͽ� �ִ� ������ ����� ����
        foreach (var attackTile in attackTiles)
        {
            GameObject obj = GridManager.instance.GetOccupant(attackTile);
            if (obj != null && obj.TryGetComponent<Enemy>(out var enemy))
            {
                enemy.Attacked(damage);
            }
        }

        // 3. ������ Ÿ�Ͽ� ����Ʈ ǥ��
        StartCoroutine(ShowEffect(attackTiles));
    }

    List<Vector2Int> GetAttackTiles()
    {
        List<Vector2Int> tiles = new List<Vector2Int>();

        Vector2Int playerGridPos = GridManager.instance.WorldToGrid(player.transform.position);
        for (int i = 1; i <= count; i++)
        {
            Vector2Int targetGrisPos = player.gridPos + player.LastInputDir * i * player.Grid;
            tiles.Add(targetGrisPos);
        }

        return tiles;
    }

    IEnumerator ShowEffect(List<Vector2Int> tiles)
    {
        List<GameObject> effects = new List<GameObject>();

        // 1. ���޹��� Ÿ�� ����Ʈ�� ����Ʈ ����
        foreach (var tile in tiles)
        {
            Vector3 effectPos = GridManager.instance.GridToWorld(tile);

            Transform effect = GameManager.instance.pool.Get(prefabId).transform;
            effect.position = effectPos;
            effects.Add(effect.gameObject);
        }

        // 2. 0.3�� �� ��� ����Ʈ ��Ȱ��ȭ
        yield return new WaitForSeconds(0.3f);

        foreach (var effect in effects)
            effect.SetActive(false);
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
