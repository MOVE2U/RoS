using UnityEngine;

public class Scanner : MonoBehaviour
{
    public float scanRange;
    public LayerMask targetLayer;
    // RaycastHit은 의도적으로 탐색 도구(CircleCastAll, Raycast 등)를 통해 탐색 도구와 충돌한 오브젝트의 정보를 담음
    // 유사하게 OnTriggerEnter2D는 이벤트가 발생했을 때 자동 호출되며, 충돌한 오브젝트의 정보를 담음
    // 참고로 RaycastHit은 public이어도 유니티 인스펙터 창에 노출되지 않음
    public RaycastHit2D[] targets;
    public Transform nearestTarget;

    private void FixedUpdate()
    {
        // CircleCastAll로 오브젝트를 탐색하고, 탐색한 오브젝트의 정보를 RaycastHit2D 배열에 저장
        targets = Physics2D.CircleCastAll(transform.position, scanRange, Vector2.zero, 0, targetLayer);
        nearestTarget = GetNearest();
    }

    Transform GetNearest()
    {
        Transform result = null;
        float diff = 100;

        foreach (RaycastHit2D target in targets)
        {
            Vector3 myPos = transform.position;
            Vector3 targetPos = target.transform.position;
            float curDiff = Vector3.Distance(myPos, targetPos);

            if(curDiff < diff)
            {
                diff = curDiff;
                result = target.transform;
            }
        }

        return result;
    }
}
