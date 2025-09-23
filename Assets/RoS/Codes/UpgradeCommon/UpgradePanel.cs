using System.Collections.Generic;
using UnityEngine;

public class UpgradePanel : MonoBehaviour
{
    [Header("Upgrade Data")]
    public List<UpgradeSet> upgradeSets;

    [Header("External Ref")]
    public BasicAttackController basicAttackController;

    RectTransform rect;
    UpgradeCard[] upgradeCards;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        upgradeCards = GetComponentsInChildren<UpgradeCard>(true);

        foreach(var upgradeCard in upgradeCards)
        {
            upgradeCard.basicAttackController = basicAttackController;
        }
    }

    public void Show(int index)
    {
        Next(index);
        rect.localScale = Vector3.one;
        GameManager.instance.Stop();
        AudioManager.instance.PlaySfx(AudioManager.Sfx.LevelUp);
        AudioManager.instance.EffectBgm(true);
    }

    public void Hide()
    {
        if (TutorialManager.instance.curIndex <= 28)
        {
            TutorialManager.instance.NextStep();
        }

        rect.localScale = Vector3.zero;
        GameManager.instance.Resume();
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
        AudioManager.instance.EffectBgm(false);
    }

    void Next(int index)
    {
        List<UpgradeData> finalUpgradeDatas = new List<UpgradeData>(upgradeSets[index].upgradeDatas);

        var shapeCandidates = basicAttackController.GetShapeUpgradeCandidates();
        foreach (var candidate in shapeCandidates)
        {
            ShapeData shapeData = ScriptableObject.CreateInstance<ShapeData>();
            shapeData.tileToAdd = candidate;
            shapeData.title = "����";
            shapeData.desc = "���� ������ ({0}, {1}) ��ġ�� Ȯ��˴ϴ�.";
            // �������� ��� ���� ���׷��̵尡 ������ �������� ������ ������ �� �ֽ��ϴ�.
            // shapeData.icon = ...

            finalUpgradeDatas.Add(shapeData);
        }

        // ���� ����� ��� 3���� ī�忡 �Ҵ��մϴ�.
        for (int i = 0; i < upgradeCards.Length; i++)
        {
            // �̹� ���õ� ī��� �����ϰ� �ٽ� �̱� ���� ���� (���û���)
            int ran = Random.Range(0, finalUpgradeDatas.Count);
            upgradeCards[i].SetUp(finalUpgradeDatas[ran]);
            finalUpgradeDatas.RemoveAt(ran); // �ߺ� ����
        }
    }
}
