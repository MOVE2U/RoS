using UnityEngine;

public class TigerBasic : EnemyAbstract
{
    [Header("Value")]
    public float baseDamage;
    public float skillDamageMultiplier;
    public float moveRate;
    public float skillRate;

    public override void ExecuteAI()
    {
        // 1. 플레이어가 근접해있으면 기본 공격
        if (IsPlayerAdjacent())
        {
            GameManager.instance.player.ProtoAttacked(baseDamage);
        }
        // 2. 플레이어가 근접해있지 않으면 이동 또는 스킬 공격
        else
        {
            float ran = Random.value;

            if (ran < moveRate)
            {
                EnemyMoveJudge();
            }
            else
            {
                TigerDash();
            }
        }
    }

    private bool IsPlayerAdjacent()
    {
        float distance = Vector2Int.Distance(gridPos, GameManager.instance.player.gridPos);
        return distance <= 1f;
    }

    public void TigerDash()
    {
        Debug.Log("TigerDash");
    }

}
