using UnityEngine;

[CreateAssetMenu(fileName = "TextureData", menuName = "Scriptable Objects/TextureData")]
public class TextureData : UpgradeData
{
    public enum TextureType
    {
        OilPainting,
        Pointillism,
        Sketch
    }
    public TextureType textureType;

    public override void Apply(BasicAttackController basicAttackController)
    {
        basicAttackController.ApplyTexture(this);
    }

    public override int GetLevel(BasicAttackController basicAttackController)
    {
        return basicAttackController.textureLevel;
    }

    public override string GetDesc(int level)
    {
        return string.Format(desc, value[level]);
    }
}
