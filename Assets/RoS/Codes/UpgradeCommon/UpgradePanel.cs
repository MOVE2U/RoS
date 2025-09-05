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
        // ��ü ���׷��̵� ��Ͽ��� �Ϻθ� ��������, ���� ���׷��̵嵵 �߰��մϴ�.
        List<UpgradeData> finalOptions = new List<UpgradeData>(upgradeDatas);

        // ���� ���׷��̵� �ĺ����� �����ɴϴ�.
        var shapeCandidates = upgradeCards[0].basicAttackController.GetShapeUpgradeCandidates();
        foreach (var candidate in shapeCandidates)
        {
            // ShapeData �ν��Ͻ��� �������� �����մϴ�.
            ShapeData shapeData = ScriptableObject.CreateInstance<ShapeData>();
            shapeData.tileToAdd = candidate;
            shapeData.title = "���� Ȯ��";
            shapeData.desc = "���� ������ ({0}, {1}) ��ġ�� Ȯ��˴ϴ�.";
            // �������� ��� ���� ���׷��̵尡 ������ �������� ������ ������ �� �ֽ��ϴ�.
            // shapeData.icon = ...

            finalOptions.Add(shapeData);
        }

        // ���� ����� ��� 3���� ī�忡 �Ҵ��մϴ�.
        for (int i = 0; i < upgradeCards.Length; i++)
        {
            // �̹� ���õ� ī��� �����ϰ� �ٽ� �̱� ���� ���� (���û���)
            int ran = Random.Range(0, finalOptions.Count);
            upgradeCards[i].SetUp(finalOptions[ran]);
            finalOptions.RemoveAt(ran); // �ߺ� ����
        }
    }
}
