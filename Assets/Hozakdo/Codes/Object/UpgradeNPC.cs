using UnityEngine;

public class UpgradeNPC : MonoBehaviour, ISpawnable, Iinteractable
{
    Vector2Int gridPos;

    public void OnSpawn(SpawnData spawnData, Vector2Int pos)
    {
        // 논리 좌표 업데이트
        gridPos = pos;
        GridManager.instance.RegisterOccupant(pos, this.gameObject);

        // 월드 좌표 업데이트
        transform.position = new Vector3(gridPos.x, gridPos.y, 0);

        // 주변을 트리거로 등록
        GridManager.instance.RegisterTrigger(new Vector2Int(pos.x + 1, pos.y), this.gameObject);
        GridManager.instance.RegisterTrigger(new Vector2Int(pos.x, pos.y + 1), this.gameObject);
        GridManager.instance.RegisterTrigger(new Vector2Int(pos.x - 1, pos.y), this.gameObject);
        GridManager.instance.RegisterTrigger(new Vector2Int(pos.x, pos.y - 1), this.gameObject);
    }

    public void Hide()
    {
        // 트리거 제거, 활성화 제거
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

        TutorialManager.instance.npc = this;

        if (GameManager.instance.NPCCount == 1)
        {
            TutorialManager.instance.NextStep();
        }
        else if(GameManager.instance.NPCCount == 4)
        {
            TutorialManager.instance.NextStep();
        }
        else
        {
            Hide();

            // 선택지 띄우기
            GameManager.instance.uiLevelUp.Show(0);
        }
    }
}
