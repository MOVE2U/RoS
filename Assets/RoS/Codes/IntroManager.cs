using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class IntroManager : MonoBehaviour
{
    public int curIndex = 0;

    public Image blackScreen;
    public Text blackScreenText;
    public GameObject nextArrowBlack;
    public GameObject[] bubbles;
    public GameObject[] introImages;

    private void Start()
    {
        blackScreen.gameObject.SetActive(true);
        introImages[0].SetActive(true);
    }

    public void NextStep()
    {
        curIndex++;
        switch (curIndex)
        {
            case 1:
                blackScreenText.text = "..그래, 너 말이야.";
                break;
            case 2:
                nextArrowBlack.SetActive(true);
                bubbles[0].SetActive(true);
                blackScreen.gameObject.SetActive(false);
                break;
            case 3:
                bubbles[0].SetActive(false);
                bubbles[1].SetActive(true);
                break;
            case 4:
                bubbles[1].SetActive(false);
                bubbles[2].SetActive(true);
                break;
            case 5:
                bubbles[2].SetActive(false);
                bubbles[3].SetActive(true);
                break;
            case 6:
                bubbles[3].SetActive(false);
                bubbles[4].SetActive(true);
                break;
            case 7:
                bubbles[4].SetActive(false);
                nextArrowBlack.SetActive(false);
                blackScreen.gameObject.SetActive(true);
                blackScreenText.text = "(생전 처음 본 강압적인 까치에 홀려, 나도 모르게 솓을 뻗었다.)";
                break;
            case 8:
                blackScreen.gameObject.SetActive(false);
                introImages[0].SetActive(false);
                introImages[1].SetActive(true);
                break;
            default:
                break;
        }
    }
}
