﻿using System.Collections.Generic;
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
            panelTitle.text = "그리기";
        }
        else
        {
            isMegpie = false;
            panelImage.sprite = panelImages[1];
            panelTitle.text = "칠하기";
        }

        Next(index);
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

    void Next(int index)
    {
        List<UpgradeData> finalUpgradeDatas = new List<UpgradeData>(upgradeSets[index].upgradeDatas);

        // 형태 업그레이드일 경우에만
        if(index == 0)
        {
            var shapeCandidates = basicAttackController.GetShapeUpgradeCandidates();
            foreach (var candidate in shapeCandidates)
            {
                ShapeData shapeData = ScriptableObject.CreateInstance<ShapeData>();
                shapeData.tileToAdd = candidate;
                shapeData.title = "형태";
                shapeData.desc = "공격 범위가 ({0}, {1}) 위치에 확장됩니다.";
                // 아이콘은 모든 형태 업그레이드가 동일한 아이콘을 쓰도록 설정할 수 있음
                shapeData.icon = shapeIcon;

                finalUpgradeDatas.Add(shapeData);
            }
        }

        // 최종 목록을 섞어서 3개를 카드에 할당
        for (int i = 0; i < upgradeCards.Length; i++)
        {
            // 이미 선택된 카드는 제외하고 다시 뽑기 위한 로직
            int ran = Random.Range(0, finalUpgradeDatas.Count);
            upgradeCards[i].SetUp(finalUpgradeDatas[ran]);
            finalUpgradeDatas.RemoveAt(ran); // 중복 방지
        }
    }


}
