using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillAoe : SkillAbstract
{
    public override void ExecuteSkill()
    {
        // 1. 공격 타일 계산
        Player player = GameManager.instance.player;
        List<Vector2Int> attackTiles = GetAttackTiles(player.gridPos);

        // 2. 대미지 적용
        foreach (var attackTile in attackTiles)
        {
            GameObject obj = GridManager.instance.GetOccupant(attackTile);
            if (obj != null && obj.TryGetComponent<EnemyAbstract>(out var enemy))
            {
                enemy.ProtoAttacked(damage);
            }
        }

        // 3. 동일한 타일에 이펙트 표시
        StartCoroutine(ShowVFX(attackTiles));
    }

    private List<Vector2Int> GetAttackTiles(Vector2Int baseGridPos)
    {
        List<Vector2Int> attackTiles = new List<Vector2Int>();

        // 범위 크기 (나중에 스킬 레벨변수로 대체 가능)
        int range = 1; 

        // x는 -1 ~ 1, y는 -1 ~ 1 까지 반복
        for (int x = -range; x <= range; x++)
        {
            for (int y = -range; y <= range; y++)
            {
                // (0,0)은 중심(플레이어 위치)이므로 공격에서 제외하려면 이 조건 추가
                if (x == 0 && y == 0) continue;

                attackTiles.Add(baseGridPos + new Vector2Int(x, y));
            }
        }

        return attackTiles;
    }

    IEnumerator ShowVFX(List<Vector2Int> tiles)
    {
        List<GameObject> vfxs = new List<GameObject>();

        // 1. 이펙트 생성
        foreach (var tile in tiles)
        {
            Vector3 vfxPos = GridManager.instance.GridToWorld(tile);

            Transform vfx = GameManager.instance.pool.Get(effect).transform;
            vfx.position = vfxPos;

            vfxs.Add(vfx.gameObject);
        }


        // 2. 0.3초 후 이펙트 삭제
        yield return new WaitForSeconds(0.3f);

        foreach (var vfx in vfxs)
        {
            vfx.SetActive(false);
        }
    }
}
