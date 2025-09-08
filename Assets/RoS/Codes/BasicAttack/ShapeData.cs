using UnityEngine;

// UpgradePanel���� �������� ����
[CreateAssetMenu(fileName = "ShapeData", menuName = "Scriptable Objects/ShapeData")]
public class ShapeData : UpgradeData
{
    // ���׷��̵� �� �߰��� Ÿ���� ��� ��ġ. UpgradePanel�� �������� �Ҵ�
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
