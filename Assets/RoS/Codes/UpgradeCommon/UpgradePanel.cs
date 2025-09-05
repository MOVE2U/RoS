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
        for(int i = 0; i < 3; i++)
        {
            int ran = Random.Range(0, upgradeDatas.Count);
            upgradeCards[i].SetUp(upgradeDatas[ran]);
        }
    }
}
