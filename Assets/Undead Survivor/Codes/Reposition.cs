using UnityEngine;

public class Reposition : MonoBehaviour
{
    // Ʈ������ ��ü�� �� ��ũ��Ʈ�� ���� ��ü
    // �� ��ü�� Ʈ���ŷ� ������ �ݶ��̴����� ��
    // Ʈ���� �ȿ� �����ų� �����鼭 �̺�Ʈ�� �߻���Ű�� ��ü�� collision�� ���� ���޵�
    // collision�� ���� �̺�Ʈ�� �߻���Ų ��ü�� ������ �� �� ����. 
    // collision.gameObject: Ʈ���ſ� ��ȣ�ۿ��� ���� ������Ʈ
    // collision.transform.position: �ش� ��ü�� ��ġ
    // collision.CompareTag: �ش� ��ü�� �±׸� Ȯ��
    Collider2D coll;

    void Awake()
    {
     coll = GetComponent<Collider2D>();
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Area"))
            return;
        
        Vector3 playerPos = GameManager.instance.player.transform.position;
        Vector3 myPos = transform.position;
        float diffX = Mathf.Abs(playerPos.x - myPos.x);
        float diffY = Mathf.Abs(playerPos.y - myPos.y);

        Vector3 playerDir = GameManager.instance.player.inputVec;
        float dirX = playerDir.x < 0 ? -1 : 1;
        float dirY = playerDir.y < 0 ? -1 : 1;

        switch(transform.tag)
        {
            case "Ground":
                if(diffX>diffY)
                {
                    transform.Translate(Vector3.right * dirX * 40);
                }
                else if (diffX < diffY)
                {
                    transform.Translate(Vector3.up * dirY * 40);
                }
                break;
            case "Enemy":

                break;
        }
    }
}
