using UnityEngine;

public class Hand : MonoBehaviour
{
    public bool isLeft;
    public SpriteRenderer spriter;

    SpriteRenderer player;

    Vector3 rightPos = new Vector3(0.35f, -0.15f, 0);
    Vector3 rightPosReverse = new Vector3(-0.35f, -0.15f, 0);
    Quaternion leftRot = Quaternion.Euler(0, 0, -35);
    Quaternion leftRotReverse = Quaternion.Euler(0, 0, -135);
    private void Awake()
    {
        // 나[0]부터 찾고 부모[1]를 찾는다.
        // GetComponentInParent를 쓰지 않는 이유는 '나'에게 SpriteRenderer가 있기 때문
        player = GetComponentsInParent<SpriteRenderer>()[1];
    }
    private void LateUpdate()
    {
        bool isReverse = player.flipX;

        if(isLeft)
        {
            // 값을 하나만 변경하는 if문의 경우 삼항연산자를 쓴다.
            transform.localRotation = isReverse ? leftRotReverse : leftRot;
            spriter.flipY = isReverse;
            spriter.sortingOrder = isReverse ? 4 : 6;
        }
        else
        {
            transform.localPosition = isReverse ? rightPosReverse : rightPos;
            spriter.flipX = isReverse;
            spriter.sortingOrder = isReverse ? 6 : 4;
        }

    }
}
