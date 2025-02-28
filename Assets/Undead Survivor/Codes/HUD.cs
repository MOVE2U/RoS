using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public enum InfoType { EXP, Level, Kill, Time, Health }
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
                float maxExp = GameManager.instance.nextExp[GameManager.instance.level];
                mySlider.value = curExp / maxExp;
                break;
            case InfoType.Level:
                myText.text = string.Format("Lv.{0:f0}", GameManager.instance.level);
                break;
            case InfoType.Kill:
                myText.text = string.Format("{0:f0}", GameManager.instance.kill);
                break;
            case InfoType.Time:
                break;
            case InfoType.Health:
                break;
        }
    }
}
