using UnityEngine;

[CreateAssetMenu(fileName = "ColorData", menuName = "Scriptable Objects/ColorData")]
public class ColorData : UpgradeData
{
    public enum ColorType
    {
        Red,
        Blue,
        Orange
    }
    public ColorType colorType;

    public Gradient gradient;

    public override void Apply(BasicAttackController basicAttackController)
    {
        basicAttackController.ApplyColor(this);
    }

    public override int GetLevel(BasicAttackController basicAttackController)
    {
        return basicAttackController.colorLevels.ContainsKey(colorType) ? basicAttackController.colorLevels[colorType] : 0;
    }

    public override string GetDesc(int level)
    {
        return string.Format(desc, value[level] * 100);
    }
}
