using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager instance;

    [SerializeField] private SpawnData magpie;
    [SerializeField] private List<Vector2Int> magpiePoint1;
    [SerializeField] private List<Vector2Int> magpiePoint2;
    [SerializeField] private List<Vector2Int> magpiePoint3;

    public UpgradeNPC npc;
    public int curIndex = 0;

    public Image blackScreen;
    public Text blackScreenText;
    public GameObject nextArrowBlack;
    public GameObject ingameNext;
    public GameObject[] bubbles;
    public GameObject[] introImages;

    void Awake()
    {
        instance = this;
    }

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
                blackScreenText.text = "..�׷�, �� ���̾�.";
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
                blackScreenText.text = "���� ó�� �� �������� ��ġ�� Ȧ��, ���� �𸣰� ���� ������.";
                break;
            case 8:
                blackScreen.gameObject.SetActive(false);
                introImages[0].SetActive(false);
                introImages[1].SetActive(true);
                break;
            case 9:
                GameManager.instance.Stop();
                blackScreen.gameObject.SetActive(true);
                blackScreenText.text = "��ġ���� ���� ���� ����, �׸��� ���� �����ų �� �������.";
                break;
            case 10:
                blackScreenText.text = "�׸��� ������ ������ ��..";
                break;
            case 11:
                blackScreen.gameObject.SetActive(false);
                ingameNext.SetActive(true);
                break;
            case 12:
                bubbles[5].SetActive(false);
                bubbles[7].SetActive(true);
                bubbles[10].SetActive(true);
                bubbles[11].SetActive(true);
                break;
            case 13:
                bubbles[7].SetActive(false);
                bubbles[10].GetComponentsInChildren<Image>()[1].enabled = true;
                bubbles[8].SetActive(true);
                bubbles[11].GetComponentsInChildren<Image>()[1].enabled = false;
                break;
            case 14:
                bubbles[8].GetComponentInChildren<Text>().text = "..�� ����־�.";
                break;
            case 15:
                bubbles[8].GetComponentInChildren<Text>().text = "�ʰ� '�׸�'�̶�� �θ��� �� ���̾�.";
                break;
            case 16:
                bubbles[8].GetComponentInChildren<Text>().text = "��� �̷� ����̱� ������.";
                break;
            case 17:
                bubbles[8].SetActive(false);
                bubbles[10].SetActive(false);
                bubbles[11].SetActive(false);
                ingameNext.SetActive(false);
                bubbles[15].SetActive(true);
                GameManager.instance.Resume();
                break;
            case 18:
                bubbles[15].SetActive(false);
                bubbles[9].SetActive(true);
                break;
            case 19:
                GameManager.instance.Stop();
                bubbles[9].SetActive(false);
                ingameNext.SetActive(true);
                bubbles[6].SetActive(true);
                bubbles[6].GetComponentInChildren<Text>().text = "������ �༮��.\n�� �� �����̰� �ϳ�.";
                LayoutRebuilder.ForceRebuildLayoutImmediate(bubbles[6].GetComponent<RectTransform>());
                break;
            case 20:
                bubbles[6].GetComponentInChildren<Text>().text = "�ʰ� �������� �ʰ� ó����.";
                LayoutRebuilder.ForceRebuildLayoutImmediate(bubbles[6].GetComponent<RectTransform>());
                break;
            case 21:
                bubbles[6].GetComponentInChildren<Text>().text = "�׳�..\n�׸��� �׸���, ������ ��.";
                LayoutRebuilder.ForceRebuildLayoutImmediate(bubbles[6].GetComponent<RectTransform>());
                break;
            case 22:
                ingameNext.SetActive(false);
                GameManager.instance.Resume();
                break;
            case 23:
                GameManager.instance.Stop();
                ingameNext.SetActive(true);
                bubbles[6].GetComponentInChildren<Text>().text = "�׸��� �� ũ�� �׷���.";
                LayoutRebuilder.ForceRebuildLayoutImmediate(bubbles[6].GetComponent<RectTransform>());
                break;
            case 24:
                ingameNext.SetActive(false);
                npc.Hide();
                Spawner.instance.FixedSpawn(magpiePoint1, magpie);
                GameManager.instance.uiLevelUp.Show(0);
                break;
            case 25:
                bubbles[12].SetActive(true);
                break;
            case 26:
                bubbles[12].SetActive(false);
                npc.Hide();
                Spawner.instance.FixedSpawn(magpiePoint2, magpie);
                bubbles[13].SetActive(true);
                break;
            case 27:
                bubbles[13].SetActive(false);
                npc.Hide();
                Spawner.instance.FixedSpawn(magpiePoint3, magpie);
                bubbles[14].SetActive(true);
                break;
            case 28:
                bubbles[14].SetActive(false);
                npc.Hide();
                GameManager.instance.Stop();
                introImages[2].SetActive(true);
                break;
            default:
                break;
        }
    }
}
