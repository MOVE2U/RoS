using UnityEngine;

public class TigerBasic : EnemyAbstract
{
    [Header("Value")]
    public float baseDamage;
    public float skillDamageMultiplier;
    public float moveRate;
    public float skillRate;

    public override void IntentCalculate()
    {
        // 1. 플레이어가 근접해있으면 기본 공격
        if (IsPlayerAdjacent())
        {
            intentState = EnemyIntentState.Attack;
        }
        // 2. 플레이어가 근접해있지 않은데 x 또는 y 좌표가 같으면 스킬 공격
        else if (gridPos.x == GameManager.instance.player.gridPos.x || gridPos.y == GameManager.instance.player.gridPos.y)
        {
            intentState = EnemyIntentState.Skill;
        }
        // 3. 그 외엔 이동
        else
        {
            intentState = EnemyIntentState.Move;
        }
    }

    public override void IntentExecute()
    {
        switch (intentState)
        {
            case EnemyIntentState.Attack:
                Debug.Log("TigerAttack");
                if (IsPlayerAdjacent())
                {
                    GameManager.instance.player.ProtoAttacked(baseDamage);
                }
                break;
            case EnemyIntentState.Skill:
                TigerDash();
                break;
            case EnemyIntentState.Move:
                Debug.Log("TigerMove");
                EnemyMoveJudge();
                break;
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
