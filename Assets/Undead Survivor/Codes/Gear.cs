using UnityEngine;

public class Gear : MonoBehaviour
{
    public ItemData.ItemType type;
    public float rate;

    public void Init(ItemData data)
    {
        // 기본 설정
        name = "Gear " + data.itemId;
        transform.parent = GameManager.instance.player.transform;
        transform.localPosition = Vector3.zero;
        // 데이터 설정
        type = data.itemType;
        rate = data.damages[0];
    }
    public void LevelUp(float rate)
    {
        this.rate = rate;
    }
}
