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
    public float speed; // 나중에 Gear에서 변경하는 메서드를 Weapon에서 만들고 private set으로 변경 필요

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
        // 기본 설정
        name = "Weapon " + data.itemId;
        transform.parent = player.transform;
        transform.localPosition = Vector3.zero;

        // 데이터 설정
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
        // 1. 공격 타일 계산
        List<Vector2Int> attackTiles = GetAttackTiles();
        // Debug.Log("공격 타일 목록: " + string.Join(", ", tiles));

        // 2. 공격 타일에 있는 적에게 대미지 적용
        foreach (var attackTile in attackTiles)
        {
            GameObject obj = GridManager.instance.GetOccupant(attackTile);
            if (obj != null && obj.TryGetComponent<Enemy>(out var enemy))
            {
                enemy.Attacked(damage);
            }
        }

        // 3. 동일한 타일에 이펙트 표시
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

        // 1. 전달받은 타일 리스트에 이펙트 생성
        foreach (var tile in tiles)
        {
            Vector3 effectPos = GridManager.instance.GridToWorld(tile);

            Transform effect = GameManager.instance.pool.Get(prefabId).transform;
            effect.position = effectPos;
            effects.Add(effect.gameObject);
        }

        // 2. 0.3초 후 모든 이펙트 비활성화
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
        //// 형제의 스크립트를 가져와서 복잡한 로직을 짜야할 때는 부모.GetComponentsInChildren와 foreach를 이용
        //// 형제의 스크립트에서 메서드만 실행시키면 될 때는 BroadcastMessage를 이용
        //player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
    }

    //public void Init(ItemData data)
    //{
    //    // 기본 설정
    //    name = "Weapon " + data.itemId;
    //    transform.parent = player.transform;
    //    transform.localPosition = Vector3.zero;

    //    // 데이터 설정
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
    //    // 이미 초기화된 player의 hands 배열을 가져오는 것 뿐이므로 별도 초기화가 필요하지 않다.
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


    //            회전 무기 코드

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
    //                // 초기화
    //                bullet.localPosition = Vector3.zero;
    //                bullet.localRotation = Quaternion.identity;

    //                bullet.Rotate(Vector3.forward * index * 360 / count);
    //                // Vector3.up은 월드 좌표계 기준 위쪽. Translate.up(여기선 bullet.up을 사용)은 로컬 좌표계 기준.
    //                // Translate의 두번째 인자는 월드 좌표계, 로컬 좌표계를 결정. Space.World 또는 Space.Self.
    //                // 여기선 이미 bullet.up에서 로컬 좌표계를 사용했기 때문에, Space.World를 사용해도, Space.Self를 사용해도 같은 결과가 나온다.
    //                bullet.Translate(bullet.up * 1.5f, Space.World);
    //                // bullet은 GameObject형 변수가 아니라 컴포넌트인 Transform임. 그럼에도 불구하고 다른 컴포넌트에 아래처럼 접근할 수 있음.
    //                // 즉, GetComponent<t>()는 다른 컴포넌트에서도 접근할 수 있음.
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
