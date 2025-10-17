﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAttackController : MonoBehaviour
{
    [Header("Base Stat")]
    public float baseAttackPower = 10.0f;
    public float baseAttackSpeed = 1.0f;
    public float baseCritRate = 0.1f;
    public float baseCritMultiplier = 1.5f;
    public float vfxDurationRate = 0.5f;

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
    public Dictionary<ColorData.ColorType, int> colorLevels = new Dictionary<ColorData.ColorType, int>();
    public TextureData.TextureType? curTexture = null;
    public int textureLevel = 0;
    public float textureValue;
    public List<Vector2Int> shapeTiles = new List<Vector2Int>();

    [Header("Ref")]
    public GameObject prefabRanged;
    public Sprite spriteRangedBasic;
    public Sprite spriteRangedWhite;
    public GameObject prefabMelee;

    public bool isAttacking;
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

        shapeTiles.Clear(); // 기존 내용 비우기
        
        // 공격 타입에 따라 초기 공격 타일 설정
        switch (player.attackType)
        {
            case Player.AttackType.Melee:
                shapeTiles.Add(new Vector2Int(1, 0));
                break;
            case Player.AttackType.Ranged:
                shapeTiles.Add(new Vector2Int(1, 0));
                break;
            case Player.AttackType.Mouse:
                shapeTiles.Add(new Vector2Int(0, 0));
                break;
        }
    }

    // private void Update()
    // {
    //     if (TurnManager.instance.CurState == TurnState.EnemyTurn
    //         && GameManager.instance.isLive)
    //     {

    //         timer += Time.deltaTime;

    //         if (timer > finalAttackSpeed)
    //         {
    //             timer = 0;
    //             AttackRanged();
    //             // AttackMelee();
    //         }
    //     }
    // }

    public void AttackMelee()
    {
        // 1. 공격 타일 계산
        List<Vector2Int> attackTiles = GetAttackTiles(player.gridPos);
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

        // 5. 공격 로직이 완료된 시점에 턴 추가. 프로토타입에선 임시로 연출 이후에 턴 추가하자.
        // TurnManager.instance.MoveCountAdd(1);
    }

    private void AttackRanged()
    {
        // 1. 공격 타일 계산
        List<Vector2Int> attackTiles = GetAttackTiles(player.gridPos);

        // 2. 대미지 계산
        float damage = Damage();
        Debug.Log("대미지: " + damage);

        // 3. 프로젝타일 색상 결정
        Color projectileColor = Color.white;
        if (isCrit && colorLevels.ContainsKey(ColorData.ColorType.Orange))
        {
            projectileColor = orangeColor;
        }
        else if (colorLevels.ContainsKey(ColorData.ColorType.Red))
        {
            projectileColor = redColor;
        }

        foreach (var attackTile in attackTiles)
        {
            // 4. 프로젝타일 생성
            GameObject projectile = GameManager.instance.pool.Get(prefabRanged);

            // 5. 프로젝타일 위치 변경
            projectile.transform.position = new Vector3(attackTile.x, attackTile.y, 0);

            // 6. 프로젝타일 색상 변경
            SpriteRenderer sprite = projectile.GetComponent<SpriteRenderer>();
            sprite.sprite = spriteRangedBasic;
            if (projectileColor == orangeColor || projectileColor == redColor)
            {
                sprite.sprite = spriteRangedWhite;
            }

            sprite.color = projectileColor;

            // 7. 프로젝타일 초기화
            projectile.GetComponent<Projectile>().Init(player.lastInputDir, this, damage);
        }
    }

    public void AttackMouse(Vector2Int mouseGridPos)
    {
        // 1. 공격 타일 계산
        List<Vector2Int> attackTiles = GetAttackTiles(mouseGridPos);
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

        // 5. 공격 로직이 완료된 시점에 턴 추가. 프로토타입에선 임시로 연출 이후에 턴 추가하자.
        // TurnManager.instance.MoveCountAdd(1);
    }

    List<Vector2Int> GetAttackTiles(Vector2Int baseGridPos)
    {
        List<Vector2Int> attackTiles = new List<Vector2Int>();

        switch (player.attackType)
        {
            case Player.AttackType.Melee:
            case Player.AttackType.Ranged:
                Vector2Int dir = player.lastInputDir;
                foreach (var shapeTile in shapeTiles)
                {
                    Vector2Int rotatedTile;

                    // player.LastInputDir 값에 따라 shapeTile을 회전
                    if (dir.x == 1) // Right
                        rotatedTile = shapeTile;
                    else if (dir.x == -1) // Left
                        rotatedTile = new Vector2Int(-shapeTile.x, -shapeTile.y);
                    else if (dir.y == 1) // Up
                        rotatedTile = new Vector2Int(-shapeTile.y, shapeTile.x);
                    else // Down
                        rotatedTile = new Vector2Int(shapeTile.y, -shapeTile.x);

                    attackTiles.Add(baseGridPos + rotatedTile);
                }
                break;
            case Player.AttackType.Mouse:
                foreach (var shapeTile in shapeTiles)
                {
                    attackTiles.Add(baseGridPos + shapeTile);
                }
                break;
        }

        return attackTiles;
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
        isAttacking = true;

        Color vfxColor = Color.white;
        if (isCrit && colorLevels.ContainsKey(ColorData.ColorType.Orange))
        {
            vfxColor = orangeColor;
        }
        else if(colorLevels.ContainsKey(ColorData.ColorType.Red))
        {
            vfxColor = redColor;
        }

        List<GameObject> vfxs = new List<GameObject>();

        // 1. 전달받은 타일 리스트에 이펙트 생성
        foreach (var tile in tiles)
        {
            Vector3 vfxPos = GridManager.instance.GridToWorld(tile);

            Transform vfx = GameManager.instance.pool.Get(prefabMelee).transform;
            vfx.position = vfxPos;

            SpriteRenderer sr = vfx.GetComponent<SpriteRenderer>();
            sr.color = vfxColor;

            vfxs.Add(vfx.gameObject);
        }

        // 2. 0.3초 후 모든 이펙트 비활성화
        yield return new WaitForSeconds(finalAttackSpeed * vfxDurationRate);

        foreach (var vfx in vfxs)
            vfx.SetActive(false);

        // 3. 공격 로직이 완료된 시점에 턴 추가. 프로토타입에선 임시로 연출 이후에 턴 추가하자.
        TurnManager.instance.MoveCountAdd(1);

        isAttacking = false;
    }

    public void ApplyColor(ColorData data)
    {
        if (!colorLevels.ContainsKey(data.colorType))
        {
            colorLevels.Add(data.colorType, 1);
        }
        else
        {
            colorLevels[data.colorType]++;
        }

        int curLevel = colorLevels[data.colorType];

        switch (data.colorType)
        {
            case ColorData.ColorType.Red:
                finalAttackPower += baseAttackPower * data.value[colorLevels[data.colorType]-1];
                redColor = data.gradient.Evaluate(0.5f + 0.5f * (float)curLevel / data.value.Length);
                break;
            case ColorData.ColorType.Blue:
                finalAttackSpeed -= baseAttackSpeed * data.value[colorLevels[data.colorType] - 1];
                blueColor = data.gradient.Evaluate(0.5f + 0.5f * (float)curLevel / data.value.Length);
                break;
            case ColorData.ColorType.Orange:
                finalCritRate += data.value[colorLevels[data.colorType] - 1];
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

    public void ApplyShape(ShapeData data)
    {
        if (!shapeTiles.Contains(data.tileToAdd))
        {
            shapeTiles.Add(data.tileToAdd);
        }
    }

    public List<Vector2Int> GetShapeUpgradeCandidates()
    {
        HashSet<Vector2Int> candidates = new HashSet<Vector2Int>();
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        foreach (var shapeTile in shapeTiles)
        {
            foreach (var dir in directions)
            {
                candidates.Add(shapeTile + dir);
            }
        }

        candidates.ExceptWith(shapeTiles);
        candidates.Remove(Vector2Int.zero);

        return new List<Vector2Int>(candidates);
    }
}
