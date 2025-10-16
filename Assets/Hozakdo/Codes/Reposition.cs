using UnityEngine;

public class Reposition : MonoBehaviour
{
    // ?몃━嫄곗쓽 二쇱껜?????ㅽ겕由쏀듃瑜?媛吏?媛앹껜
    // ??媛앹껜???몃━嫄곕줈 ?ㅼ젙??肄쒕씪?대뜑?ъ빞 ??
    // ?몃━嫄??덉뿉 ?ㅼ뼱?ㅺ굅???섍?硫댁꽌 ?대깽?몃? 諛쒖깮?쒗궎??媛앹껜媛 collision???듯빐 ?꾨떖??
    // collision???듯빐 ?대깽?몃? 諛쒖깮?쒗궓 媛앹껜???뺣낫瑜??????덉쓬. 
    // collision.gameObject: ?몃━嫄곗? ?곹샇?묒슜??寃뚯엫 ?ㅻ툕?앺듃
    // collision.transform.position: ?대떦 媛앹껜???꾩튂
    // collision.CompareTag: ?대떦 媛앹껜???쒓렇瑜??뺤씤
    Collider2D coll;

    void Awake()
    {
        coll = GetComponent<Collider2D>();
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        // 異⑸룎??媛앹껜???쒓렇媛 Area???뚮쭔 ?댄븯媛 ?ㅽ뻾??
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
