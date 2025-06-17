using System.Xml;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public enum InfoType { EXP, Level, Turn, Kill, Enemy, Health, TurnChanging, TurnCount }
    public InfoType type;

    Text myText;
    Slider mySlider;

    Text[] myTexts;
    Image[] myImages;

    private void Awake()
    {
        myText = GetComponent<Text>();
        mySlider = GetComponent<Slider>();

        myTexts = GetComponentsInChildren<Text>();
        myImages = GetComponentsInChildren<Image>();
    }
    private void LateUpdate()
    {
        switch(type)
        {
            case InfoType.EXP:
                float curExp = GameManager.instance.exp;
                float maxExp = GameManager.instance.nextExp[Mathf.Min(GameManager.instance.level, GameManager.instance.nextExp.Length - 1)];
                mySlider.value = curExp / maxExp;
                break;
            case InfoType.Level:
                myText.text = string.Format("Lv.{0:F0}", GameManager.instance.level);
                break;
            case InfoType.Turn:
                if (TurnManager.instance.CurState == TurnState.PlayerTurn)
                {
                    myText.enabled = true;
                    myText.text = string.Format("플레이어 {0:F0}턴!", TurnManager.instance.TurnCount);
                }
                else if (TurnManager.instance.CurState == TurnState.EnemyTurn)
                {
                    myText.enabled = true;
                    myText.text = string.Format("몬스터 {0:F0}턴!", TurnManager.instance.TurnCount);
                }
                else
                {
                    myText.enabled = false;
                }
                break;
            case InfoType.Kill:
                myText.text = string.Format("Kill: {0:F0}", GameManager.instance.kill);
                break;
            case InfoType.Enemy:
                myText.text = string.Format("Enemy: {0:F0}", GameManager.instance.spawnCountTotal - GameManager.instance.kill);
                break;
            case InfoType.Health:
                float curHealth = GameManager.instance.health;
                float maxHealth = GameManager.instance.maxHealth;
                mySlider.value = curHealth / maxHealth;
                break;
            case InfoType.TurnChanging:
                if (TurnManager.instance.CurState == TurnState.Transition)
                {
                    myText.enabled = true;
                }
                else
                {
                    myText.enabled = false;
                }
                break;
            case InfoType.TurnCount:

                break;
        }
    }
    public void UsePlayerTurn(int index)
    {
        if (ColorUtility.TryParseHtmlString("#C1C1C1", out Color textColor))
        {
            myTexts[index - 1].color = textColor;
        }
        if (ColorUtility.TryParseHtmlString("#989898", out Color imageColor))
        {
            myImages[index - 1].color = imageColor;
        }
    }
    public void UseEnemyTurn(int index)
    {
        if (ColorUtility.TryParseHtmlString("#E05AD8", out Color textColor))
        {
            myTexts[index-1].color = textColor;
        }
        if (ColorUtility.TryParseHtmlString("#EEACD6", out Color imageColor))
        {
            myImages[index-1].color = imageColor;
        }
    }
}
