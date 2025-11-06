using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public enum InfoType { MoveCount, Level, Turn, Kill, Coin, Exp, ProtoTurn, AttackRange }
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
        switch (type)
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
                    myText.text = string.Format("..그들이 깬다");
                }
                else if (TurnManager.instance.CurState == TurnState.EnemyTurn)
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
            case InfoType.ProtoTurn:
                if (TurnManager.instance.CurState == TurnState.PlayerTurn)
                {
                    myTexts[0].text = "내 턴";
                    myTexts[1].text = string.Format("{0:F0} / {1:F0}", TurnManager.instance.MoveCount, TurnManager.instance.MaxMoveCount);
                }
                else if (TurnManager.instance.CurState == TurnState.EnemyTurn)
                {
                    myTexts[0].text = "적 턴";
                    myTexts[1].text = string.Format("{0:F0} / {1:F0}", TurnManager.instance.MoveCount, TurnManager.instance.MaxMoveCount);
                }
                break;
            case InfoType.AttackRange:
                if (GameManager.instance.player.basicAttackController != null)
                {
                    foreach (var shapeTile in GameManager.instance.player.basicAttackController.shapeTiles)
                    {
                        GridVisual(shapeTile, Color.black);
                    }    
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

    void GridVisual(Vector2Int gridPos, Color color)
    {
        if (gridPos == new Vector2Int(-2, 2))
        {
            myImages[1].color = color;
        }
        else if (gridPos == new Vector2Int(-1, 2))
        {
            myImages[2].color = color;
        }
        else if (gridPos == new Vector2Int(0, 2))
        {
            myImages[3].color = color;
        }
        else if (gridPos == new Vector2Int(1, 2))
        {
            myImages[4].color = color;
        }
        else if (gridPos == new Vector2Int(2, 2))
        {
            myImages[5].color = color;
        }
        else if (gridPos == new Vector2Int(-2, 1))
        {
            myImages[6].color = color;
        }
        else if (gridPos == new Vector2Int(-1, 1))
        {
            myImages[7].color = color;
        }
        else if (gridPos == new Vector2Int(0, 1))
        {
            myImages[8].color = color;
        }
        else if (gridPos == new Vector2Int(1, 1))
        {
            myImages[9].color = color;
        }
        else if (gridPos == new Vector2Int(2, 1))
        {
            myImages[10].color = color;
        }
        else if (gridPos == new Vector2Int(-2, 0))
        {
            myImages[11].color = color;
        }
        else if (gridPos == new Vector2Int(-1, 0))
        {
            myImages[12].color = color;
        }
        else if (gridPos == new Vector2Int(0, 0))
        {
            myImages[13].color = color;
        }
        else if (gridPos == new Vector2Int(1, 0))
        {
            myImages[14].color = color;
        }
        else if (gridPos == new Vector2Int(2, 0))
        {
            myImages[15].color = color;
        }
        else if (gridPos == new Vector2Int(-2, -1))
        {
            myImages[16].color = color;
        }
        else if (gridPos == new Vector2Int(-1, -1))
        {
            myImages[17].color = color;
        }
        else if (gridPos == new Vector2Int(0, -1))
        {
            myImages[18].color = color;
        }
        else if (gridPos == new Vector2Int(1, -1))
        {
            myImages[19].color = color;
        }
        else if (gridPos == new Vector2Int(2, -1))
        {
            myImages[20].color = color;
        }
        else if (gridPos == new Vector2Int(-2, -2))
        {
            myImages[21].color = color;
        }
        else if (gridPos == new Vector2Int(-1, -2))
        {
            myImages[22].color = color;
        }
        else if (gridPos == new Vector2Int(0, -2))
        {
            myImages[23].color = color;
        }
        else if (gridPos == new Vector2Int(1, -2))
        {
            myImages[24].color = color;
        }
        else if (gridPos == new Vector2Int(2, -2))
        {
            myImages[25].color = color;
        }
    }
}
