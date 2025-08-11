using UnityEngine;

[CreateAssetMenu(fileName = "ColorData", menuName = "Scriptable Objects/ColorData")]
public class ColorData : UpgradeData
{
    public enum ColorType
    {
        Red,
        Green,
        Blue
    }

    public float[] attackPowerMultipliers;
    public float[] criticalRateMultipliers;
    public float[] attackSpeedMultipliers;

    public override void Apply(BasicAttackController basicAttackController)
    {
        basicAttackController.ColorLevelUp(this);
    }
}
