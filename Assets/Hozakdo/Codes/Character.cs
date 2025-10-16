using UnityEngine;

public class Character : MonoBehaviour
{
    // 프로퍼티는 동적으로 값을 가져오는 변수
    // 변수를 설정하고, Update에서 get 이하를 처리하는 것과 비슷하다.
    // 잦은 변경이 있는거면 Update, 아니면 프로퍼티를 사용하는게 최적화에 좋다.
    public static float Speed
    {
        get
        {
            if(GameManager.instance == null)
                return 1f;

            return GameManager.instance.playerId == 0 ? 1.1f : 1f;
        }
    }
    public static float WeaponSpeed
    {
        get
        {
            if (GameManager.instance == null)
                return 1f;

            return GameManager.instance.playerId == 1 ? 1.1f : 1f;
        }
    }
    public static float WeaponRate
    {
        get
        {
            if (GameManager.instance == null)
                return 1f;

            return GameManager.instance.playerId == 1 ? 0.9f : 1f;
        }
    }
    public static float Damage
    {
        get
        {
            if (GameManager.instance == null)
                return 1f;

            return GameManager.instance.playerId == 2 ? 1.2f : 1f;
        }
    }
    public static int Count
    {
        get
        {
            if (GameManager.instance == null)
                return 1;

            return GameManager.instance.playerId == 3 ? 1 : 0;
        }
    }
}
