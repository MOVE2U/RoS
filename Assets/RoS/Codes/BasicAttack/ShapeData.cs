using UnityEngine;

// 이 데이터는 에셋 메뉴에서 직접 만들기보다, UpgradePanel에서 동적으로 생성될 것입니다.
[CreateAssetMenu(fileName = "ShapeData", menuName = "Scriptable Objects/ShapeData")]
public class ShapeData : UpgradeData
{
    // 업그레이드 시 추가될 타일의 상대 위치
    // 이 값은 UpgradePanel이 동적으로 할당합니다.
    public Vector2Int tileToAdd;

    // 카드를 클릭했을 때 호출될 메서드
    public override void Apply(BasicAttackController basicAttackController)
    {
        // BasicAttackController에 있는 새로운 메서드를 호출해 형태를 적용합니다.
        basicAttackController.ApplyShape(this);
    }

    // 형태 업그레이드는 다른 업그레이드처럼 정해진 '레벨'이 없습니다.
    // 대신, 현재 공격 범위에 포함된 타일의 총 개수를 레벨처럼 사용할 수 있습니다.
    public override int GetLevel(BasicAttackController basicAttackController)
    {
        return basicAttackController.attackShape.Count;
    }

    // 카드에 표시될 설명
    // desc 변수에는 "공격 범위가 ({0}, {1}) 위치에 확장됩니다." 와 같은 문자열을 넣어두고,
    // tileToAdd 값으로 포맷팅하여 사용합니다.
    public override string GetDesc(int level)
    {
        return string.Format(desc, tileToAdd.x, tileToAdd.y);
    }
}
