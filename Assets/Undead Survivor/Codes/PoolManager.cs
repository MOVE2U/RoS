using UnityEngine;
using System.Collections.Generic;

public class PoolManager : MonoBehaviour
{
    // 원본을 담고있는 배열. 원본은 직접 사용하지 않고 복사해서 사용한다.
    public GameObject[] prefabs;

    // 복사한 프리팹을 담아두는 공간. 크기가 유동적이어야 해서 배열이 아닌 List로 선언한다.
    List<GameObject>[] pools;

    void Awake()
    {
        pools = new List<GameObject>[prefabs.Length];
    // 배열만 초기화하면 배열 안의 각 요소는 null로 되기 때문에 각 요소도 초기화해준다.
        for (int index = 0; index < pools.Length; index++)
        {
            pools[index] = new List<GameObject>();
        }
    }
    public GameObject Get(int index)
    {
        GameObject select = null;

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
            select = Instantiate(prefabs[index], transform);
            pools[index].Add(select);
        }

        return select;
    }
}
