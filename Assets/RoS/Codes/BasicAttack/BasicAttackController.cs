using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAttackController : MonoBehaviour
{
    [Header("Base Stat")]
    public float baseDamage = 10.0f;
    public float baseAttackSpeed = 0.7f;
    public int baseRange = 1;
    public float finalDamage = baseDamage;
    public GameObject effectPrefab;

    [Header("Upgrade State")]
    // ���� �� Ȯ�� ������ ���� private�� �ٲ� ��
    public Dictionary<ColorData.ColorType, int> colorState = new Dictionary<ColorData.ColorType, int>();

    private float timer;
    private Player player;




    private void Start()
    {
        player = GameManager.instance.player;
    }

    private void Update()
    {
        if (TurnManager.instance.CurState == TurnState.EnemyTurn 
            && GameManager.instance.isLive)
        {
            timer += Time.deltaTime;

            if (timer > baseAttackSpeed)
            {
                timer = 0;
                Attack();
            }
        }
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
                enemy.Attacked(baseDamage);
            }
        }

        // 3. ������ Ÿ�Ͽ� ����Ʈ ǥ��
        StartCoroutine(ShowEffect(attackTiles));
    }

    List<Vector2Int> GetAttackTiles()
    {
        List<Vector2Int> tiles = new List<Vector2Int>();

        Vector2Int playerGridPos = GridManager.instance.WorldToGrid(player.transform.position);
        for (int i = 1; i <= baseRange; i++)
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

            Transform effect = GameManager.instance.pool.Get(effectPrefab).transform;
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
        this.baseDamage = damage * Character.Damage;
        this.baseRange += count;

        //if (id == 0)
        //{
        //    Batch();
        //}
        //// ������ ��ũ��Ʈ�� �����ͼ� ������ ������ ¥���� ���� �θ�.GetComponentsInChildren�� foreach�� �̿�
        //// ������ ��ũ��Ʈ���� �޼��常 �����Ű�� �� ���� BroadcastMessage�� �̿�
        //player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
    }

    public void ApplyColor(ColorData colorData)
    {
        if (!colorState.ContainsKey(colorData.colorType))
        {
            colorState.Add(colorData.colorType, 1);
        }
        else
        {
            colorState[colorData.colorType]++;
        }

    }
}
