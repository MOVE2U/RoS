using UnityEngine;

public class UpgradePanel : MonoBehaviour
{
    RectTransform rect;
    UpgradeCard[] items;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        items = GetComponentsInChildren<UpgradeCard>(true);
    }

    public void Show()
    {
        Next();
        rect.localScale = Vector3.one;
        GameManager.instance.Stop();
        AudioManager.instance.PlaySfx(AudioManager.Sfx.LevelUp);
        AudioManager.instance.EffectBgm(true);
    }

    public void Hide()
    {
        rect.localScale = Vector3.zero;
        GameManager.instance.Resume();
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
        AudioManager.instance.EffectBgm(false);
    }

    public void Select(int index)
    {
        items[index].OnClick();
    }

    void Next()
    {
        // 1. 모든 아이템 비활성화
        foreach (UpgradeCard item in items)
        {
            item.gameObject.SetActive(false);
        }

        UpgradeCard ranItem = items[0];
        ranItem.gameObject.SetActive(true);

        //// 2. 그 중에서 랜덤 3개 아이템 활성화
        //int[] ran = new int[3];
        //while (true)
        //{
        //    ran[0] = Random.Range(0, items.Length);
        //    ran[1] = Random.Range(0, items.Length);
        //    ran[2] = Random.Range(0, items.Length);

        //    if (ran[0] != ran[1] && ran[0] != ran[2] && ran[1] != ran[2])
        //        break;
        //}
        //for (int index = 0; index < ran.Length; index++)
        //{
        //    Item ranItem = items[ran[index]];

        //    // 3. 만렙 아이템은 소비 아이템으로 대체
        //    if (ranItem.level == ranItem.data.damages.Length)
        //    {
        //        items[4].gameObject.SetActive(true);
        //    }
        //    else
        //    {
        //        ranItem.gameObject.SetActive(true);
        //    }
        //}
    }
}
