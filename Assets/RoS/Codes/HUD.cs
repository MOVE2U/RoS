using System;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public enum InfoType { EXP, Level, Turn, Kill, Coin, Health, TurnChanging, MoveCount }
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
                if (TurnManager.instance.CurState == TurnState.Transition)
                {
                    myText.enabled = true;
                }
                else
                {
                    myText.enabled = false;
                }
                break;
        }
    }

    public void UpdateMoveCountUI(TurnState turnState, int moveCount)
    {
        if (type != InfoType.MoveCount)
            return;

        switch (turnState)
        {
            case TurnState.PlayerTurn:
                SetTurnColors(moveCount, "#E05AD8", "#EEACD6");
                break;
            case TurnState.EnemyTurn:
                int reverseMoveCount = myTexts.Length - moveCount + 1;
                SetTurnColors(reverseMoveCount, "#C1C1C1", "#989898");
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
