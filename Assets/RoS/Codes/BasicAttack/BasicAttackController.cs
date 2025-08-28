using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAttackController : MonoBehaviour
{
    [Header("Base Stat")]
    public float baseAttackPower = 10.0f;
    public float baseAttackSpeed = 0.7f;
    public float baseCritRate = 0.1f;
    public float baseCritMultiplier = 1.5f;
    public int baseRange = 1;

    [Header("Final Stat")]
    public float finalAttackPower;
    public float finalAttackSpeed;
    public float finalCritRate;
    public float finalCritMultiplier;

    [Header("Upgrade State")]
    // 개발 및 확인 끝나고 나면 private로 바꿀 것
    public Dictionary<ColorData.ColorType, int> colorState = new Dictionary<ColorData.ColorType, int>();

    [Header("Ref")]
    public GameObject effectPrefab;

    private float timer;
    private Player player;

    private void Awake()
    {
        finalAttackPower = baseAttackPower;
        finalAttackSpeed = baseAttackSpeed;
        finalCritRate = baseCritRate;
        finalCritMultiplier = baseCritMultiplier;
    }

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

            if (timer > finalAttackSpeed)
            {
                timer = 0;
                Attack();
            }
        }
    }

    void Attack()
    {
        // 1. 공격 타일 계산
        List<Vector2Int> attackTiles = GetAttackTiles();
        // Debug.Log("공격 타일 목록: " + string.Join(", ", tiles));

        // 2. 대미지 계산
        float damage = Damage();
        Debug.Log("대미지: " + damage);

        // 3. 공격 타일에 있는 적에게 대미지 적용
        foreach (var attackTile in attackTiles)
        {
            GameObject obj = GridManager.instance.GetOccupant(attackTile);
            if (obj != null && obj.TryGetComponent<Enemy>(out var enemy))
            {
                enemy.Attacked(damage);
            }
        }

        // 4. 동일한 타일에 이펙트 표시
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

    public float Damage()
    {
        float randomValue = Random.Range(0f, 1f);

        if(randomValue <= finalCritRate)
        {
            return finalAttackPower * finalCritMultiplier;
        }
        else
        {
            return finalAttackPower;
        }
    }

    IEnumerator ShowEffect(List<Vector2Int> tiles)
    {
        List<GameObject> effects = new List<GameObject>();

        // 1. 전달받은 타일 리스트에 이펙트 생성
        foreach (var tile in tiles)
        {
            Vector3 effectPos = GridManager.instance.GridToWorld(tile);

            Transform effect = GameManager.instance.pool.Get(effectPrefab).transform;
            effect.position = effectPos;
            effects.Add(effect.gameObject);
        }

        // 2. 0.3초 후 모든 이펙트 비활성화
        yield return new WaitForSeconds(0.3f);

        foreach (var effect in effects)
            effect.SetActive(false);
    }

    public void LevelUp(float damage, int count)
    {
        this.baseAttackPower = damage * Character.Damage;
        this.baseRange += count;

        //if (id == 0)
        //{
        //    Batch();
        //}
        //// 형제의 스크립트를 가져와서 복잡한 로직을 짜야할 때는 부모.GetComponentsInChildren와 foreach를 이용
        //// 형제의 스크립트에서 메서드만 실행시키면 될 때는 BroadcastMessage를 이용
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

        switch(colorData.colorType)
        {
            case ColorData.ColorType.Red:
                finalAttackPower += baseAttackPower * colorData.value[colorState[colorData.colorType]-1];
                break;
            case ColorData.ColorType.Blue:
                finalAttackSpeed -= baseAttackSpeed * colorData.value[colorState[colorData.colorType] - 1];
                break;
            case ColorData.ColorType.Green:
                finalCritRate += baseCritRate * colorData.value[colorState[colorData.colorType] - 1];
                break;
        }
    }
}
