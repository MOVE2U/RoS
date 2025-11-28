using UnityEngine;
using System.Collections;

public class SkillBasic : SkillAbstract
{
    public override void ExecuteSkill()
    {
        // 1. 공격 타일 계산
        Player player = GameManager.instance.player;
        Vector2Int attackTile = GetAttackTile(player.gridPos);

        // 2. 대미지 적용
        GameObject obj = GridManager.instance.GetOccupant(attackTile);
        if (obj != null && obj.TryGetComponent<EnemyAbstract>(out var enemy))
        {
            enemy.ProtoAttacked(damage);
        }

        // 3. 동일한 타일에 이펙트 표시
        StartCoroutine(ShowVFX(attackTile));
    }

    private Vector2Int GetAttackTile(Vector2Int baseGridPos)
    {
        // 1. 기본 공격 타일 설정
        Vector2Int baseAttackTile = new Vector2Int(1, 0);
        Vector2Int rotatedAttackTile;

        // 2. 입력 방향에 따라 공격 타일 회전
        Vector2Int lastInputDir = GameManager.instance.player.lastInputDir;
        switch (lastInputDir.x, lastInputDir.y)
        {
            case (-1, 0):
                rotatedAttackTile = new Vector2Int(-1, 0);
                break;
            case (0, -1):
                rotatedAttackTile = new Vector2Int(0, -1);
                break;
            case (0, 1):
                rotatedAttackTile = new Vector2Int(0, 1);
                break;
            default:
                rotatedAttackTile = baseAttackTile;
                break;
        }

        // 3. 최종 공격 타일 반환
        Vector2Int finalAttackTile = baseGridPos + rotatedAttackTile;

        return finalAttackTile;
    }

    IEnumerator ShowVFX(Vector2Int tile)
    {
        // 1. 이펙트 생성
        Vector3 vfxPos = GridManager.instance.GridToWorld(tile);

        Transform vfx = GameManager.instance.pool.Get(effect).transform;
        vfx.position = vfxPos;

        // 2. 0.3초 후 이펙트 삭제
        yield return new WaitForSeconds(0.3f);

        vfx.gameObject.SetActive(false);
    }
}
