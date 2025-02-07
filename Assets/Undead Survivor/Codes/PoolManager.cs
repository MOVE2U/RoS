using UnityEngine;
using System.Collections.Generic;
// 오브젝트 풀링은 필요할 때 점진적으로 오브젝트를 생성하고, 이미 생성된 오브젝트는 재사용하는 방식. 최소한의 리소스를 사용하기 위함이다.
public class PoolManager : MonoBehaviour
{
    // 원본을 담고있는 배열. 원본은 직접 사용하지 않고 복사해서 사용한다.
    public GameObject[] prefabs;

    // 복사한 프리팹을 담아두는 공간. 크기가 유동적이어야 해서 배열이 아닌 List로 선언한다.
    // 배열에 리스트를 저장
    List<GameObject>[] pools;

    void Awake()
    {
        // 배열의 초기화
        // prefabs의 길이만큼 배열 생성
        pools = new List<GameObject>[prefabs.Length];

        // 리스트의 초기화
        // 배열만 초기화하면 배열 안의 각 요소는 null로 되기 때문에 각 요소도 초기화해준다.
        // pools 자체는 배열임. pools의 각 요소 pools[0], pools[1], pools[2]...가 리스트임.
        // 그래서 pools의 길이는 고정되어 있지만, 리스트인 pools[0]은 [총알1, 총알2, 총알3...] 이런식으로 길이가 유동적으로 변하는 것임
        // 오브젝트 풀링은 이 쌓아둔 총알 중 비활성화 상태인게 있으면 활성화해서 사용하고, 더이상 비활성화된게 없으면 새로 생성해서 리소스를 최소화하는 방식임
        for (int index = 0; index < pools.Length; index++)
        {
            pools[index] = new List<GameObject>();
        }
    }
    // void가 아닌 메서드는 항상 해당 변수형으로 값을 반환해줘야 한다.
    // 메서드는 수식이 있기 때문에 매번 다른 값을 동적으로 반환해줄 수 있다.
    public GameObject Get(int index)
    {
        GameObject select = null;

        // foreach문은 컬렉션이 null일 경우엔 오류가 발생하지만, 비어있는 경우에는 실행을 하지 않기 때문에 오류가 발생하지 않는다.
        // foreach문은 리스트의 0번부터 마지막까지 순차적으로 조건을 체크한다. 아래의 경우 조건이 맞으면 break.
        foreach (GameObject item in pools[index])
        {
            if (!item.activeSelf)
            {
                select = item;
                select.SetActive(true);
                break;
            }
        }

        if (!select)
        {
            // Instantiate의 두번째 인자는 생성될 위치를 결정함.
            // new Vector3일 경우 월드 좌표로 생성됨
            // transform일 경우 해당 오브젝트를 부모로 하기 때문에 부모를 기준으로 위치가 결정됨. 다른 오브젝트를 Transform형 변수로 선언하고 넣으면 됨.
            select = Instantiate(prefabs[index], transform);
            pools[index].Add(select);
        }

        return select;
    }
}
