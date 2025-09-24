using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradePanel : MonoBehaviour
{
    [Header("Upgrade Data")]
    public List<UpgradeSet> upgradeSets;

    [Header("External Ref")]
    public BasicAttackController basicAttackController;
    public Sprite[] panelImages;
    public Sprite shapeIcon;

    public bool isMegpie;

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
        Image panelImage = GetComponentsInChildren<Image>()[1];
        Text panelTitle = GetComponentsInChildren<Text>()[0];

        if(index == 0)
        {
            isMegpie = true;
            panelImage.sprite = panelImages[0];
            panelTitle.text = "�׸���";
        }
        else
        {
            isMegpie = false;
            panelImage.sprite = panelImages[1];
            panelTitle.text = "ĥ�ϱ�";
        }

        Next(index);
        rect.localScale = Vector3.one;
        GameManager.instance.Stop();
        AudioManager.instance.PlaySfx(AudioManager.Sfx.LevelUp);
        AudioManager.instance.EffectBgm(true);
    }

    public void Hide()
    {
        if (isMegpie)
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

        // ���� ���׷��̵��� ��쿡��
        if(index == 0)
        {
            var shapeCandidates = basicAttackController.GetShapeUpgradeCandidates();
            foreach (var candidate in shapeCandidates)
            {
                ShapeData shapeData = ScriptableObject.CreateInstance<ShapeData>();
                shapeData.tileToAdd = candidate;
                shapeData.title = "����";
                shapeData.desc = "���� ������ ({0}, {1}) ��ġ�� Ȯ��˴ϴ�.";
                // �������� ��� ���� ���׷��̵尡 ������ �������� ������ ������ �� ����
                shapeData.icon = shapeIcon;

                finalUpgradeDatas.Add(shapeData);
            }
        }

        // ���� ����� ��� 3���� ī�忡 �Ҵ�
        for (int i = 0; i < upgradeCards.Length; i++)
        {
            // �̹� ���õ� ī��� �����ϰ� �ٽ� �̱� ���� ����
            int ran = Random.Range(0, finalUpgradeDatas.Count);
            upgradeCards[i].SetUp(finalUpgradeDatas[ran]);
            finalUpgradeDatas.RemoveAt(ran); // �ߺ� ����
        }
    }


}
