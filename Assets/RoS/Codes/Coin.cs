using UnityEngine;

public class Coin : MonoBehaviour, ISpawnable, Iinteractable
{
    Vector2Int gridPos;

    public void OnSpawn(SpawnData spawnData, Vector2Int pos)
    {
        // �� ��ǥ ������Ʈ
        gridPos = pos;
        GridManager.instance.RegisterTrigger(pos, this.gameObject);

        // ���� ��ǥ ������Ʈ
        transform.position = new Vector3(gridPos.x, gridPos.y, 0);
    }

    public void OnInteract()
    {
        // ���� ȹ�� ����
        GameManager.instance.coin++;

        // ���� ����
        GridManager.instance.UnRegisterTrigger(gridPos, this.gameObject);
        this.gameObject.SetActive(false);
    }
}
