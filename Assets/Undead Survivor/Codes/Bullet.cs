using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage;
    public int per;

    Rigidbody rigid;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    public void Init(float damage, int per, Vector3 dir)
    {
        this.damage = damage;
        this.per = per;

        if(per > -1)
        {
            rigid.linearVelocity = dir;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy") || per == -1)
            return;

        per--;
        // 최초 per == -1인건 return을 했지만, 코드는 순차적으로 실행되기 때문에 per --를 한 이후에 다시 확인한 if문에서는 per == -1이 있을 수 있다.
        if (per==-1)
        {
            gameObject.SetActive(false);
        }
    }
}