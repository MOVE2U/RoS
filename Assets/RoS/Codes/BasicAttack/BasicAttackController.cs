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

    [Header("Effect Color")]
    public Color redColor;
    public Color blueColor;
    public Color orangeColor;

    [Header("Upgrade State")]
    // 개발 및 확인 끝나고 나면 private로 바꿀 것
    public Dictionary<ColorData.ColorType, int> colorState = new Dictionary<ColorData.ColorType, int>();
    public TextureData.TextureType? curTexture = null;
    public int textureLevel = 0;
    public float textureValue;
    public List<Vector2Int> attackShape = new List<Vector2Int> { new Vector2Int(1, 0) };

    [Header("Ref")]
    public GameObject vfxPrefab;

    private bool isCrit;
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
                enemy.Attacked(this, damage);
            }
        }

        // 4. 동일한 타일에 이펙트 표시
        StartCoroutine(ShowVFX(attackTiles));
    }

    List<Vector2Int> GetAttackTiles()
    {
        List<Vector2Int> tiles = new List<Vector2Int>();
        Vector2Int playerGridPos = GridManager.instance.WorldToGrid(player.transform.position);
        Vector2Int dir = player.LastInputDir;

        foreach (var shapeTile in attackShape)
        {
            Vector2Int rotatedTile;

            // player.LastInputDir 값에 따라 shapeTile을 회전시킵니다.
            if (dir.x == 1) // Right
                rotatedTile = shapeTile;
            else if (dir.x == -1) // Left
                rotatedTile = new Vector2Int(-shapeTile.x, -shapeTile.y);
            else if (dir.y == 1) // Up
                rotatedTile = new Vector2Int(-shapeTile.y, shapeTile.x);
            else // Down
                rotatedTile = new Vector2Int(shapeTile.y, -shapeTile.x);

            tiles.Add(playerGridPos + rotatedTile);
        }

        return tiles;
    }

    public float Damage()
    {
        isCrit = false;
        float randomValue = Random.Range(0f, 1f);

        if(randomValue <= finalCritRate)
        {
            isCrit = true;

            if(curTexture == TextureData.TextureType.Sketch)
            {
                return finalAttackPower * finalCritMultiplier * textureValue;
            }

            return finalAttackPower * finalCritMultiplier;
        }
        else
        {
            return finalAttackPower;
        }
    }

    IEnumerator ShowVFX(List<Vector2Int> tiles)
    {
        Color vfxColor = Color.white;
        if (isCrit && colorState.ContainsKey(ColorData.ColorType.Orange))
        {
            vfxColor = orangeColor;
        }
        else if(colorState.ContainsKey(ColorData.ColorType.Red))
        {
            vfxColor = redColor;
        }

        List<GameObject> vfxs = new List<GameObject>();

        // 1. 전달받은 타일 리스트에 이펙트 생성
        foreach (var tile in tiles)
        {
            Vector3 vfxPos = GridManager.instance.GridToWorld(tile);

            Transform vfx = GameManager.instance.pool.Get(vfxPrefab).transform;
            vfx.position = vfxPos;

            SpriteRenderer sr = vfx.GetComponent<SpriteRenderer>();
            sr.color = vfxColor;

            vfxs.Add(vfx.gameObject);
        }

        // 2. 0.3초 후 모든 이펙트 비활성화
        yield return new WaitForSeconds(0.3f);

        foreach (var vfx in vfxs)
            vfx.SetActive(false);
    }

    public void ApplyColor(ColorData data)
    {
        if (!colorState.ContainsKey(data.colorType))
        {
            colorState.Add(data.colorType, 1);
        }
        else
        {
            colorState[data.colorType]++;
        }

        int curLevel = colorState[data.colorType];

        switch (data.colorType)
        {
            case ColorData.ColorType.Red:
                finalAttackPower += baseAttackPower * data.value[colorState[data.colorType]-1];
                redColor = data.gradient.Evaluate(0.5f + 0.5f * (float)curLevel / data.value.Length);
                break;
            case ColorData.ColorType.Blue:
                finalAttackSpeed -= baseAttackSpeed * data.value[colorState[data.colorType] - 1];
                blueColor = data.gradient.Evaluate(0.5f + 0.5f * (float)curLevel / data.value.Length);
                break;
            case ColorData.ColorType.Orange:
                finalCritRate += data.value[colorState[data.colorType] - 1];
                orangeColor = data.gradient.Evaluate(0.5f + 0.5f * (float)curLevel / data.value.Length);
                break;
        }
    }

    public void ApplyTexture(TextureData data)
    {
        curTexture = data.textureType;

        textureLevel++;
        textureValue = data.value[textureLevel - 1];
    }

    // 클래스 맨 아래에 새로운 메서드 2개 추가
    public void ApplyShape(ShapeData data)
    {
        if (!attackShape.Contains(data.tileToAdd))
        {
            attackShape.Add(data.tileToAdd);
        }
    }

    // 업그레이드 후보 타일 목록을 반환하는 메서드
    public List<Vector2Int> GetShapeUpgradeCandidates()
    {
        List<Vector2Int> candidates = new List<Vector2Int>();
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        foreach (var tile in attackShape)
        {
            foreach (var dir in directions)
            {
                Vector2Int newCandidate = tile + dir;
                // 자기 자신, (0,0)은 후보에서 제외, 이미 공격범위거나 후보인 타일도 제외
                if (newCandidate == Vector2Int.zero || attackShape.Contains(newCandidate) ||
    candidates.Contains(newCandidate))
                    continue;

                candidates.Add(newCandidate);
            }
        }
        return candidates;
    }
}
