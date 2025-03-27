using System;
using System.Collections;
using UnityEngine;

public class AchieveManager : MonoBehaviour
{
    public GameObject[] lockCharacter;
    public GameObject[] unlockCharacter;
    public GameObject uiNotice;

    // enum�� ����� �̸��� ���� ������, �������� ����
    // enum Ÿ���� �����Ѱ���, enum ��ü�� �� �� ����. �� ���� �ϳ��� ����. Achieve P = Achieve.unlockPotato; ó��.
    // �׷��� enum ���� ��� ���� ���� '���'�� �迭�� ������ �Ѵ�.
    enum Achieve { unlockPotato, unlockBean }
    // �迭�� ������ ����� �ݺ� ó���� �����ϹǷ� �̰��� �ʿ��ϴٸ� enum�� �迭�� ��´�
    Achieve[] achieves;
    WaitForSecondsRealtime wait;
    private void Awake()
    {
        // enum�� �Ϻθ� �迭�� ���� ���� �ִ�. �׷��� �ʱ�ȭ�� ���� ��ü�� ��´ٴ� ��ð� �ʿ��� ��.
        // GetValues�� array�� ��ȯ�Ѵ�. array�� �迭�� �θ� Ŭ���� ���� ��.�� ��Ȯ�� Ÿ���� �����ֱ� ���� (Achieve[])��
        // GetValues�� ���ڴ� type�̴�. �׷��� typeof�� �ʿ�
        achieves = (Achieve[])Enum.GetValues(typeof(Achieve));
        wait = new WaitForSecondsRealtime(5);
        if(!PlayerPrefs.HasKey("MyData"))
        {
            Init();
        }
    }
    void Init()
    {
        PlayerPrefs.SetInt("MyData", 1);

        foreach(Achieve achieve in achieves)
        {
            PlayerPrefs.SetInt(achieve.ToString(), 0);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // �ѹ��� ���� �޼���� �޼���� ���� ȣ���ϴ°� �������� ����.
        UnlockCharacter();
    }
    void UnlockCharacter()
    {
        for(int index = 0; index < lockCharacter.Length; index++)
        {
            string achieveName = achieves[index].ToString();
            bool isUnlock = PlayerPrefs.GetInt(achieveName) == 1;
            lockCharacter[index].SetActive(!isUnlock);
            unlockCharacter[index].SetActive(isUnlock);
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        foreach(Achieve achieve in achieves)
        {
            CheckAchieve(achieve);
        }
    }
    void CheckAchieve(Achieve achieve)
    {
        bool isAchieve = false;

        switch(achieve)
        {
            case Achieve.unlockPotato:
                isAchieve = GameManager.instance.kill >= 10;
                break;
            case Achieve.unlockBean:
                isAchieve = GameManager.instance.gameTime == GameManager.instance.maxGameTime;
                break;
        }

        if(isAchieve && PlayerPrefs.GetInt(achieve.ToString()) == 0)
        {
            PlayerPrefs.SetInt(achieve.ToString(), 1);

            for(int index = 0; index < uiNotice.transform.childCount; index++)
            {
                bool isActive = index == (int)achieve;
                uiNotice.transform.GetChild(index).gameObject.SetActive(isActive);
            }

            StartCoroutine(NoticeRoutine());
        }
    }
    IEnumerator NoticeRoutine()
    {
        uiNotice.SetActive(true);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.LevelUp);

        yield return wait;

        uiNotice.SetActive(false);
    }
}
