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

        // 최초 생성 시 충돌 체크. OnEnable에서 하면 이전 위치에서 충돌체크하므로 Init에서 해야함
        lastGridPos = GridManager.instance.WorldToGrid(transform.position);
        CheckHit(lastGridPos);
    }

    private void Update()
    {
        transform.Translate(dir * speed * Time.deltaTime);

        // 2. gridPos가 변경됐을 때 충돌 체크
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

        // 1. 충돌한 오브젝트가 없으면 return
        if (obj == null)
        {
            return;
        }

        // 2. UpgradeNPC면 return
        if (obj.TryGetComponent<UpgradeNPC>(out _))
        {
            return;
        }

        // 3. 그 외 오브젝트 충돌 시 프로젝타일 사라짐
        this.gameObject.SetActive(false);

        // 4. Enemy면 대미지 적용
        if (obj.TryGetComponent<Enemy>(out var enemy))
        {
            enemy.Attacked(basicAttackController, damage);
        }
    }
}