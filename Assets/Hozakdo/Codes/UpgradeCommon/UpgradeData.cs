using UnityEngine;

public abstract class UpgradeData : ScriptableObject
{
    public string title;
    [TextArea]
    public string desc;
    public Sprite icon;

    public float[] value;

    public abstract void Apply(BasicAttackController basicAttackController);

    public abstract int GetLevel(BasicAttackController basicAttackController);

    public abstract string GetDesc(int level);
}
