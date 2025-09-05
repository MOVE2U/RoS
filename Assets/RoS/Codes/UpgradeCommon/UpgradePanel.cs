using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class UpgradePanel : MonoBehaviour
{
    [Header("Upgrade Data")]
    public List<UpgradeData> upgradeDatas;

    RectTransform rect;
    UpgradeCard[] upgradeCards;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        upgradeCards = GetComponentsInChildren<UpgradeCard>(true);
    }

    public void Show()
    {
        Next();
        rect.localScale = Vector3.one;
        GameManager.instance.Stop();
        AudioManager.instance.PlaySfx(AudioManager.Sfx.LevelUp);
        AudioManager.instance.EffectBgm(true);
    }

    public void Hide()
    {
        rect.localScale = Vector3.zero;
        GameManager.instance.Resume();
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
        AudioManager.instance.EffectBgm(false);
    }

    void Next()
    {
        // 전체 업그레이드 목록에서 일부를 가져오고, 형태 업그레이드도 추가합니다.
        List<UpgradeData> finalOptions = new List<UpgradeData>(upgradeDatas);

        // 형태 업그레이드 후보들을 가져옵니다.
        var shapeCandidates = upgradeCards[0].basicAttackController.GetShapeUpgradeCandidates();
        foreach (var candidate in shapeCandidates)
        {
            // ShapeData 인스턴스를 동적으로 생성합니다.
            ShapeData shapeData = ScriptableObject.CreateInstance<ShapeData>();
            shapeData.tileToAdd = candidate;
            shapeData.title = "형태 확장";
            shapeData.desc = "공격 범위가 ({0}, {1}) 위치에 확장됩니다.";
            // 아이콘은 모든 형태 업그레이드가 동일한 아이콘을 쓰도록 설정할 수 있습니다.
            // shapeData.icon = ...

            finalOptions.Add(shapeData);
        }

        // 최종 목록을 섞어서 3개를 카드에 할당합니다.
        for (int i = 0; i < upgradeCards.Length; i++)
        {
            // 이미 선택된 카드는 제외하고 다시 뽑기 위한 로직 (선택사항)
            int ran = Random.Range(0, finalOptions.Count);
            upgradeCards[i].SetUp(finalOptions[ran]);
            finalOptions.RemoveAt(ran); // 중복 방지
        }
    }
}
