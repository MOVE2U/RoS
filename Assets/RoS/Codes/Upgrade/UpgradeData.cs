using UnityEngine;

public abstract class UpgradeData : ScriptableObject
{
    private string upgradeName;
    [TextArea]
    private string upgradeDesc;
    private Sprite upgradeIcon;

    public abstract void Apply(BasicAttackController basicAttackController);
}
