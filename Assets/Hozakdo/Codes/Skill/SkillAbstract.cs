using UnityEngine;

public abstract class SkillAbstract : MonoBehaviour
{
    public int cost;
    public float damage;
    public GameObject effect;

    public void UseSkill()
    {
        if (TurnManager.instance.CurState == TurnState.PlayerTurn
        && TurnManager.instance.MaxPlayerMoveCount - TurnManager.instance.MoveCount - cost >= 0
        && StageManager.instance.isStageInProgress)
        {
            TurnManager.instance.MoveCountAdd(cost);

            ExecuteSkill();
        }
    }

    public abstract void ExecuteSkill();
}
