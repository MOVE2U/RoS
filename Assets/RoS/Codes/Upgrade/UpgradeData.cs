using UnityEngine;

public abstract class UpgradeData : ScriptableObject
{
    public string upgradeName;
    [TextArea]
    public string upgradeDesc;
    public Sprite upgradeIcon;

    public abstract void Apply(BasicAttackController basicAttackController);
}
