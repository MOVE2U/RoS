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
        // ���� per == -1�ΰ� return�� ������, �ڵ�� ���������� ����Ǳ� ������ per --�� �� ���Ŀ� �ٽ� Ȯ���� if�������� per == -1�� ���� �� �ִ�.
        if (per==-1)
        {
            gameObject.SetActive(false);
        }
    }
}