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

    public ColorType colorType;
    public float[] value;

    public override void Apply(BasicAttackController basicAttackController)
    {
        basicAttackController.ApplyColor(this);
    }
}
