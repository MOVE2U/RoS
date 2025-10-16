using UnityEngine;

// UpgradePanel에서 동적으로 생성
[CreateAssetMenu(fileName = "ShapeData", menuName = "Scriptable Objects/ShapeData")]
public class ShapeData : UpgradeData
{
    // 업그레이드 시 추가될 타일의 상대 위치. UpgradePanel이 동적으로 할당
    public Vector2Int tileToAdd;

    public override void Apply(BasicAttackController basicAttackController)
    {
        basicAttackController.ApplyShape(this);
    }

    public override int GetLevel(BasicAttackController basicAttackController)
    {
        return basicAttackController.shapeTiles.Count - 1;
    }

    public override string GetDesc(int level)
    {
        return string.Format(desc, tileToAdd.x, tileToAdd.y);
    }
}
