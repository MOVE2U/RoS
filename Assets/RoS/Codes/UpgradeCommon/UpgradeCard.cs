using UnityEngine;
using UnityEngine.UI;

public class UpgradeCard : MonoBehaviour
{
    public UpgradeData upgradeData;
    public BasicAttackController basicAttackController;

    Image imageIcon;
    Text textLevel;
    Text textName;
    Text textDesc;

    private void Awake()
    {
        imageIcon = GetComponentsInChildren<Image>()[1];

        Text[] texts = GetComponentsInChildren<Text>();
        textLevel = texts[0];
        textName = texts[1];
        textDesc = texts[2];
    }

    public void SetUp(UpgradeData data)
    {
        upgradeData = data;

        int curLevel = data.GetLevel(basicAttackController);

        imageIcon.sprite = data.icon;
        textLevel.text = "Lv." + (curLevel + 1);
        textName.text = data.title;
        textDesc.text = data.GetDesc(curLevel);
    }

    public void OnClick()
    {
        upgradeData.Apply(basicAttackController);
    }
}
