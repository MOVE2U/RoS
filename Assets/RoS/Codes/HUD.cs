using System;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public enum InfoType { MoveCount, Level, Turn, Kill, Coin, Health, TurnChanging }
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

    private void Start()
    {
        if (type != InfoType.MoveCount)
            return;

        for (int i = 1; i <= myTexts.Length; i++)
        {
            SetTurnColors(i, "#C1C1C1", "#989898");
        }
    }

    private void LateUpdate()
    {
        switch(type)
        {
            case InfoType.MoveCount:
                if (TurnManager.instance.CurState == TurnState.PlayerTurn)
                {
                    float curCount = TurnManager.instance.MoveCount;
                    float maxCount = TurnManager.instance.MaxMoveCount;
                    mySlider.value = curCount / maxCount;
                }
                if (TurnManager.instance.CurState == TurnState.EnemyTurn)
                {
                    float curCount = TurnManager.instance.MaxMoveCount - TurnManager.instance.MoveCount;
                    float maxCount = TurnManager.instance.MaxMoveCount;
                    mySlider.value = curCount / maxCount;
                }
                break;
            case InfoType.Level:
                myText.text = string.Format("Lv.{0:F0}", GameManager.instance.level);
                break;
            case InfoType.Turn:
                if (TurnManager.instance.CurState == TurnState.PlayerTurn)
                {
                    myText.enabled = true;
                    myText.text = string.Format("..'그들'이 깬다");
                }
                else if (TurnManager.instance.CurState == TurnState.EnemyTurn)
                {
                    myText.enabled = true;
                    myText.text = string.Format("..'그들'이 잠든다");
                }
                else
                {
                    myText.enabled = false;
                }
                break;
            case InfoType.Kill:
                myText.text = string.Format("Soul: {0:F0}", GameManager.instance.kill);
                break;
            case InfoType.Coin:
                myText.text = string.Format("Coin: {0:F0}", GameManager.instance.coin);
                break;
            case InfoType.Health:
                float curHealth = GameManager.instance.health;
                float maxHealth = GameManager.instance.maxHealth;
                mySlider.value = curHealth / maxHealth;
                break;
            case InfoType.TurnChanging:
                if (TurnManager.instance.CurState == TurnState.ToPlayerTurn)
                {
                    myImages[0].enabled = true;
                    myTexts[0].enabled = true;
                    myTexts[1].enabled = false;
                }
                else if (TurnManager.instance.CurState == TurnState.ToEnemyTurn)
                {
                    myImages[0].enabled = true;
                    myTexts[0].enabled = false;
                    myTexts[1].enabled = true;
                }
                else
                {
                    myImages[0].enabled = false;
                    myTexts[0].enabled = false;
                    myTexts[1].enabled = false;
                }
                break;
        }
    }

    private void SetTurnColors(int index, string textColorHex, string imageColorHex)
    {
        if (type != InfoType.MoveCount)
            return;

        if (ColorUtility.TryParseHtmlString(textColorHex, out Color textColor))
        {
            myTexts[index - 1].color = textColor;
        }
        if (ColorUtility.TryParseHtmlString(imageColorHex, out Color imageColor))
        {
            myImages[index - 1].color = imageColor;
        }
    }
}
