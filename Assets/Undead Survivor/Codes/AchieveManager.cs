using System;
using System.Collections;
using UnityEngine;

public class AchieveManager : MonoBehaviour
{
    public GameObject[] lockCharacter;
    public GameObject[] unlockCharacter;
    public GameObject uiNotice;

    // enum은 상수에 이름을 붙인 것으로, 가독성이 목적
    // enum 타입을 정의한거지, enum 자체를 쓸 순 없다. 쓸 때는 하나씩 쓴다. Achieve P = Achieve.unlockPotato; 처럼.
    // 그래서 enum 안의 모든 값을 가진 '어떤것'은 배열로 만들어야 한다.
    enum Achieve { unlockPotato, unlockBean }
    // 배열은 데이터 저장과 반복 처리에 유리하므로 이것이 필요하다면 enum을 배열에 담는다
    Achieve[] achieves;
    WaitForSecondsRealtime wait;
    private void Awake()
    {
        // enum의 일부만 배열에 담을 수도 있다. 그래서 초기화를 통해 전체를 담는다는 명시가 필요한 것.
        // GetValues는 array를 반환한다. array는 배열의 부모 클래스 같은 것.더 정확한 타입을 말해주기 위해 (Achieve[])씀
        // GetValues의 인자는 type이다. 그래서 typeof가 필요
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
        // 한번만 쓰는 메서드라도 메서드로 만들어서 호출하는게 가독성에 좋다.
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
