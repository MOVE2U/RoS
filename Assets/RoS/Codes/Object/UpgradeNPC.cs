using UnityEngine;

public class UpgradeNPC : MonoBehaviour, ISpawnable, Iinteractable
{
    Vector2Int gridPos;

    public void OnSpawn(SpawnData spawnData, Vector2Int pos)
    {
        // �� ��ǥ ������Ʈ
        gridPos = pos;
        GridManager.instance.RegisterOccupant(pos, this.gameObject);

        // ���� ��ǥ ������Ʈ
        transform.position = new Vector3(gridPos.x, gridPos.y, 0);

        // �ֺ��� Ʈ���ŷ� ���
        GridManager.instance.RegisterTrigger(new Vector2Int(pos.x + 1, pos.y), this.gameObject);
        GridManager.instance.RegisterTrigger(new Vector2Int(pos.x, pos.y + 1), this.gameObject);
        GridManager.instance.RegisterTrigger(new Vector2Int(pos.x - 1, pos.y), this.gameObject);
        GridManager.instance.RegisterTrigger(new Vector2Int(pos.x, pos.y - 1), this.gameObject);
    }

    public void Hide()
    {
        // Ʈ���� ����, Ȱ��ȭ ����
        GridManager.instance.UnRegisterOccupant(gridPos);
        GridManager.instance.UnRegisterTrigger(new Vector2Int(gridPos.x + 1, gridPos.y), this.gameObject);
        GridManager.instance.UnRegisterTrigger(new Vector2Int(gridPos.x, gridPos.y + 1), this.gameObject);
        GridManager.instance.UnRegisterTrigger(new Vector2Int(gridPos.x - 1, gridPos.y), this.gameObject);
        GridManager.instance.UnRegisterTrigger(new Vector2Int(gridPos.x, gridPos.y - 1), this.gameObject);
        this.gameObject.SetActive(false);
    }

    public void OnInteract()
    {
        GameManager.instance.NPCCount++;

        if (GameManager.instance.NPCCount == 1)
        {
            TutorialManager.instance.npc = this;
            TutorialManager.instance.NextStep();
        }
        else
        {
            Hide();

            // ������ ����
            GameManager.instance.uiLevelUp.Show(0);
        }
    }
}
