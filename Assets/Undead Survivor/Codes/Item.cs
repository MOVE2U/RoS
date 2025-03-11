using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public ItemData data;
    public int level;
    public Weapon weapon;

    Image icon;
    Text textLevel;

    private void Awake()
    {
        icon = GetComponentsInChildren<Image>()[1];
        icon.sprite = data.itemIcon;

        Text[] texts = GetComponentsInChildren<Text>();
        textLevel = texts[0];
    }
    private void LateUpdate()
    {
        textLevel.text = "Lv." + (level + 1);
    }

    public void OnClick()
    {
        switch(data.itemType)
        {
            case ItemData.ItemType.Melee:
            case ItemData.ItemType.Range:
                // Item은 무기를 생성할지 말지를 결정
                // Weapon은 무기가 생성된 후 어떻게 동작하는지를 관리
                if(level==0)
                {
                    GameObject newWeapon = new GameObject();
                    newWeapon.AddComponent<Weapon>();
                    weapon = newWeapon.GetComponent<Weapon>();
                    weapon.Init(data);
                }
                else
                {
                    float nextDamage = data.baseDamage;
                    int nextCount = 0;

                    nextDamage += data.baseDamage * data.damages[level];
                    nextCount += data.counts[level];

                    weapon.LevelUp(nextDamage, nextCount);
                }
                break;
            case ItemData.ItemType.Glove:
            case ItemData.ItemType.Shoe:
                break;
            case ItemData.ItemType.Heal:
                break;
        }
        level++;

        if(level==data.damages.Length)
        {
            GetComponent<Button>().interactable = false;
        }
    }
}
