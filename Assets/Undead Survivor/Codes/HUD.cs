using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public enum InfoType { EXP, Level, Turn, Kill, Enemy, Count, Health }
    public InfoType type;

    Text myText;
    Slider mySlider;

    private void Awake()
    {
        myText = GetComponent<Text>();
        mySlider = GetComponent<Slider>();
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
                if (TurnManager.instance.isPlayerTurn && !TurnManager.instance.isEnemyTurn)
                {
                    myText.text = string.Format("�÷��̾� {0:F0}��!", TurnManager.instance.playerTurnCount);
                }
                else if (TurnManager.instance.isEnemyTurn && !TurnManager.instance.isPlayerTurn)
                {
                    myText.text = string.Format("���� {0:F0}��!", TurnManager.instance.enemyTurnCount);
                }
                else
                {
                    myText.text = string.Format("�� ��ȯ��..");
                }
                break;
            case InfoType.Kill:
                myText.text = string.Format("Kill: {0:F0}", GameManager.instance.kill);
                break;
            case InfoType.Enemy:
                myText.text = string.Format("Enemy: {0:F0}", GameManager.instance.spawnCountTotal - GameManager.instance.kill);
                break;
            case InfoType.Count:
                if (TurnManager.instance.isPlayerTurn)
                {
                    //myText.text = string.Format("���� �̵�: {0:F0}", TurnManager.instance.playerMoveCount);
                    float gameTime = TurnManager.instance.playerMoveCount;
                    int min = Mathf.FloorToInt(gameTime / 60);
                    float sec = gameTime % 60;
                    myText.text = string.Format("���� �ð�: {0:00.00}", sec);
                }
                else if (TurnManager.instance.isEnemyTurn)
                {
                    float gameTime = TurnManager.instance.enemyMoveCount;
                    int min = Mathf.FloorToInt(gameTime / 60);
                    float sec = gameTime % 60;
                    myText.text = string.Format("���� �ð�: {0:00.00}", sec);
                }
                break;
            case InfoType.Health:
                float curHealth = GameManager.instance.health;
                float maxHealth = GameManager.instance.maxHealth;
                mySlider.value = curHealth / maxHealth;
                break;
        }
    }
}
