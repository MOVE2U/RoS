using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public enum InfoType { EXP, Level, Turn, Kill, Time, Health }
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
                if (TurnManager.instance.isPlayerTurn)
                {
                    myText.text = string.Format("Player Turn: {0:F0}", TurnManager.instance.playerTurnCount);
                }
                else if (TurnManager.instance.isEnemyTurn)
                {
                    myText.text = string.Format("Enemy Turn: {0:F0}", TurnManager.instance.enemyTurnCount);
                }
                break;
            case InfoType.Kill:
                myText.text = string.Format("{0:F0}", GameManager.instance.kill);
                break;
            case InfoType.Time:
                float gameTime = GameManager.instance.gameTime;
                int min = Mathf.FloorToInt(gameTime / 60);
                int sec = Mathf.FloorToInt(gameTime % 60);
                myText.text = string.Format("{0:D2}:{1:D2}", min, sec);
                break;
            case InfoType.Health:
                float curHealth = GameManager.instance.health;
                float maxHealth = GameManager.instance.maxHealth;
                mySlider.value = curHealth / maxHealth;
                break;
        }
    }
}
