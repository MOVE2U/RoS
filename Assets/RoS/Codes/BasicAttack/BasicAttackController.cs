using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAttackController : MonoBehaviour
{
    [Header("for check")]
    private int id;
    private GameObject prefab;
    private float damage;
    private int count;
    public float speed; // ���߿� Gear���� �����ϴ� �޼��带 Weapon���� ����� private set���� ���� �ʿ�

    [Header("color")]


    private float timer;
    private Player player;

    public int Id => id;

    public ItemData itemData;


    private void Awake()
    {
        player = GameManager.instance.player;
        Init(itemData);
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
        // ������ ����
        id = data.itemId;
        damage = data.baseDamage * Character.Damage;
        count = data.baseCount + Character.Count;
        speed = 0.7f;
        prefab = data.projectile;

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

            Transform effect = GameManager.instance.pool.Get(prefab).transform;
            effect.position = effectPos;
            effects.Add(effect.gameObject);
        }

        // 2. 0.3�� �� ��� ����Ʈ ��Ȱ��ȭ
        yield return new WaitForSeconds(0.3f);

        foreach (var effect in effects)
            effect.SetActive(false);
    }

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

    public void ColorLevelUp(ColorData colorData)
    {

    }
}
