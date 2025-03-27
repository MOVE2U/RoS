using UnityEngine;

public class Reposition : MonoBehaviour
{
    // 트리거의 주체는 이 스크립트를 가진 객체
    // 이 객체는 트리거로 설정된 콜라이더여야 함
    // 트리거 안에 들어오거나 나가면서 이벤트를 발생시키는 객체가 collision을 통해 전달됨
    // collision을 통해 이벤트를 발생시킨 객체의 정보를 알 수 있음. 
    // collision.gameObject: 트리거와 상호작용한 게임 오브젝트
    // collision.transform.position: 해당 객체의 위치
    // collision.CompareTag: 해당 객체의 태그를 확인
    Collider2D coll;

    void Awake()
    {
        coll = GetComponent<Collider2D>();
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        // 충돌한 객체의 태그가 Area일 때만 이하가 실행됨
        if (!collision.CompareTag("Area"))
            return;
        
        Vector3 playerPos = GameManager.instance.player.transform.position;
        Vector3 myPos = transform.position;

        switch(transform.tag)
        {
            case "Ground":
                float diffX = playerPos.x - myPos.x;
                float diffY = playerPos.y - myPos.y;
                float dirX = diffX < 0 ? -1 : 1;
                float dirY = diffY < 0 ? -1 : 1;
                diffX = Mathf.Abs(diffX);
                diffY = Mathf.Abs(diffY);

                if (diffX>diffY)
                {
                    transform.Translate(Vector3.right * dirX * 40);
                }
                else if (diffX < diffY)
                {
                    transform.Translate(Vector3.up * dirY * 40);
                }
                break;
            case "Enemy":
                if(coll.enabled)
                {
                    Vector3 dist = playerPos - myPos;
                    Vector3 ran = new Vector3(Random.Range(-3, 3), Random.Range(-3, 3), 0);
                    transform.Translate(ran + dist * 2);
                }
                break;
        }
    }
}
