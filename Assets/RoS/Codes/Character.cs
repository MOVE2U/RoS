using UnityEngine;

public class Character : MonoBehaviour
{
    // ������Ƽ�� �������� ���� �������� ����
    // ������ �����ϰ�, Update���� get ���ϸ� ó���ϴ� �Ͱ� ����ϴ�.
    // ���� ������ �ִ°Ÿ� Update, �ƴϸ� ������Ƽ�� ����ϴ°� ����ȭ�� ����.
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
