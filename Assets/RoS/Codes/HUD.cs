using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public enum InfoType { MoveCount, Level, Turn, Kill, Coin, Exp }
    public InfoType type;

    Text myText;
    TextMeshProUGUI myTextMeshPro;
    Slider mySlider;

    Text[] myTexts;
    TextMeshProUGUI[] myTextsMeshPro;
    Image[] myImages;

    private void Awake()
    {
        myText = GetComponent<Text>();
        myTextMeshPro = GetComponent<TextMeshProUGUI>();
        mySlider = GetComponent<Slider>();

        myTexts = GetComponentsInChildren<Text>();
        myTextsMeshPro = GetComponentsInChildren<TextMeshProUGUI>();
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
                if (TurnManager.instance.CurState == TurnState.MoveTurn)
                {
                    float curCount = TurnManager.instance.MoveCount;
                    float maxCount = TurnManager.instance.MaxMoveCount;
                    mySlider.value = curCount / maxCount;
                }
                if (TurnManager.instance.CurState == TurnState.AttackTurn)
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
                if (TurnManager.instance.CurState == TurnState.MoveTurn)
                {
                    myText.enabled = true;
                    myText.text = string.Format("..그들이 깬다");
                }
                else if (TurnManager.instance.CurState == TurnState.AttackTurn)
                {
                    myText.enabled = true;
                    myText.text = string.Format("..그들이 다시 잠든다");
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
            case InfoType.Exp:
                int curExp = GameManager.instance.exp;
                int nextExp = GameManager.instance.nextExp[Mathf.Min(GameManager.instance.level, GameManager.instance.nextExp.Length - 1)];
                mySlider.value = (float)curExp / nextExp;
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
