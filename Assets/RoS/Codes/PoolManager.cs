using UnityEngine;
using System.Collections.Generic;

public class PoolManager : MonoBehaviour
{
    public Dictionary<GameObject, List<GameObject>> pools = new Dictionary<GameObject, List<GameObject>>();

    public GameObject Get(GameObject prefab)
    {
        if(!pools.ContainsKey(prefab))
        {
            pools[prefab] = new List<GameObject>();
        }

        GameObject select = null;

        foreach (GameObject item in pools[prefab])
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
            select = Instantiate(prefab, transform);
            pools[prefab].Add(select);
        }

        return select;
    }
}
