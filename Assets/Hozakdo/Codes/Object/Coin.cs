using UnityEngine;

public class Coin : MonoBehaviour, ISpawnable, Iinteractable
{
    Vector2Int gridPos;

    public void OnSpawn(SpawnData spawnData, Vector2Int pos)
    {
        // 논리 좌표 업데이트
        gridPos = pos;
        GridManager.instance.RegisterTrigger(pos, this.gameObject);

        // 월드 좌표 업데이트
        transform.position = new Vector3(gridPos.x, gridPos.y, 0);
    }

    public void OnInteract()
    {
        // 코인 획득 로직
        GameManager.instance.coin++;

        // 코인 제거
        GridManager.instance.UnRegisterTrigger(gridPos, this.gameObject);
        this.gameObject.SetActive(false);
    }
}
