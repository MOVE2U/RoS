using UnityEngine;

public class Projectile : MonoBehaviour
{
    public BasicAttackController basicAttackController;
    public Vector3 dir;
    public Vector2Int lastGridPos;
    public float damage;
    public float speed = 20f;

    public void Init(Vector2Int dir, BasicAttackController basicAttackController, float damage)
    {
        this.dir = new Vector3(dir.x, dir.y, 0);
        this.basicAttackController = basicAttackController;
        this.damage = damage;

        // ���� ���� �� �浹 üũ. OnEnable���� �ϸ� ���� ��ġ���� �浹üũ�ϹǷ� Init���� �ؾ���
        lastGridPos = GridManager.instance.WorldToGrid(transform.position);
        CheckHit(lastGridPos);
    }

    private void Update()
    {
        transform.Translate(dir * speed * Time.deltaTime);

        // 2. gridPos�� ������� �� �浹 üũ
        Vector2Int curGridPos = GridManager.instance.WorldToGrid(transform.position);
        if(curGridPos != lastGridPos)
        {
            lastGridPos = curGridPos;
            CheckHit(curGridPos);
        }
    }

    private void CheckHit(Vector2Int gridPos)
    {
        GameObject obj = GridManager.instance.GetOccupant(gridPos);

        // 1. �浹�� ������Ʈ�� ������ return
        if (obj == null)
        {
            return;
        }

        // 2. UpgradeNPC�� return
        if (obj.TryGetComponent<UpgradeNPC>(out _))
        {
            return;
        }

        // 3. �� �� ������Ʈ �浹 �� ������Ÿ�� �����
        this.gameObject.SetActive(false);

        // 4. Enemy�� ����� ����
        if (obj.TryGetComponent<Enemy>(out var enemy))
        {
            enemy.Attacked(basicAttackController, damage);
        }
    }
}