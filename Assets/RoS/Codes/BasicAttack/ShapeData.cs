using UnityEngine;

// �� �����ʹ� ���� �޴����� ���� ����⺸��, UpgradePanel���� �������� ������ ���Դϴ�.
[CreateAssetMenu(fileName = "ShapeData", menuName = "Scriptable Objects/ShapeData")]
public class ShapeData : UpgradeData
{
    // ���׷��̵� �� �߰��� Ÿ���� ��� ��ġ
    // �� ���� UpgradePanel�� �������� �Ҵ��մϴ�.
    public Vector2Int tileToAdd;

    // ī�带 Ŭ������ �� ȣ��� �޼���
    public override void Apply(BasicAttackController basicAttackController)
    {
        // BasicAttackController�� �ִ� ���ο� �޼��带 ȣ���� ���¸� �����մϴ�.
        basicAttackController.ApplyShape(this);
    }

    // ���� ���׷��̵�� �ٸ� ���׷��̵�ó�� ������ '����'�� �����ϴ�.
    // ���, ���� ���� ������ ���Ե� Ÿ���� �� ������ ����ó�� ����� �� �ֽ��ϴ�.
    public override int GetLevel(BasicAttackController basicAttackController)
    {
        return basicAttackController.attackShape.Count;
    }

    // ī�忡 ǥ�õ� ����
    // desc �������� "���� ������ ({0}, {1}) ��ġ�� Ȯ��˴ϴ�." �� ���� ���ڿ��� �־�ΰ�,
    // tileToAdd ������ �������Ͽ� ����մϴ�.
    public override string GetDesc(int level)
    {
        return string.Format(desc, tileToAdd.x, tileToAdd.y);
    }
}
