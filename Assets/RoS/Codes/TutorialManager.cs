using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public int curIndex = 0;

    public Image blackScreen;
    public Text blackScreenText;
    public GameObject nextArrowBlack;
    public GameObject ingameNext;
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
                blackScreenText.text = "생전 처음 본 강압적인 까치에 홀려, 나도 모르게 손을 뻗었다.";
                break;
            case 8:
                blackScreen.gameObject.SetActive(false);
                introImages[0].SetActive(false);
                introImages[1].SetActive(true);
                break;
            case 9:
                GameManager.instance.Stop();
                blackScreen.gameObject.SetActive(true);
                blackScreenText.text = "까치에게 손이 닿은 순간, 그림이 나를 집어삼킬 듯 끌어당겼다.";
                break;
            case 10:
                blackScreenText.text = "그리고 정신을 차렸을 땐..";
                break;
            case 11:
                blackScreen.gameObject.SetActive(false);
                ingameNext.SetActive(true);
                break;
            case 12:
                bubbles[5].GetComponentInChildren<Text>().text = "..뭐야 여긴! 뭐야 내 몸!";
                LayoutRebuilder.ForceRebuildLayoutImmediate(bubbles[5].GetComponent<RectTransform>());
                break;
            case 13:
                bubbles[5].SetActive(false);
                bubbles[6].SetActive(true);
                break;
            case 14:
                bubbles[6].GetComponentInChildren<Text>().text = "..우린 살아있어.";
                LayoutRebuilder.ForceRebuildLayoutImmediate(bubbles[6].GetComponent<RectTransform>());
                break;
            case 15:
                bubbles[6].GetComponentInChildren<Text>().text = "너가 '그림'이라고 부르는 것들 말이야.";
                LayoutRebuilder.ForceRebuildLayoutImmediate(bubbles[6].GetComponent<RectTransform>());
                break;
            case 16:
                bubbles[6].GetComponentInChildren<Text>().text = "비록 이런 모습이긴 하지만.";
                LayoutRebuilder.ForceRebuildLayoutImmediate(bubbles[6].GetComponent<RectTransform>());
                break;
            case 17:
                ingameNext.SetActive(false);
                GameManager.instance.Resume();
                break;
            default:
                break;
        }
    }
}
