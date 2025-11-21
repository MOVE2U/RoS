using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using System.Collections;
using UnityEditor.SceneManagement;

public class StageManager : MonoBehaviour
{
    public static StageManager instance;

    [Header("References")]
    [SerializeField] private CinemachineCamera camera;
    [SerializeField] private SpawnData enemySpawn;
    public GameObject stageMoveUI;

    private Dictionary<Vector2Int, StageData> stageDict = new Dictionary<Vector2Int, StageData>();
    public StageData curStage { get; private set; }
    public bool isStageInProgress { get; private set; }

    private void Awake()
    {
        instance = this;

        StageData[] stageDatas = GetComponentsInChildren<StageData>();
        foreach (StageData stageData in stageDatas)
        {
            if (!stageDict.ContainsKey(stageData.index))
            {
                stageDict.Add(stageData.index, stageData);
            }

            stageData.SetFog(false);
        }
    }

    // 최초 스테이지 선택에만 사용
    public void SetStartStage()
    {
        // 1. 랜덤 스테이지 선택
        int ran = Random.Range(0, stageDict.Count);
        curStage = stageDict.Values.ElementAt(ran);
        curStage.SetFog(true);

        // 2. 카메라 이동. Z는 보통 -10으로 고정
        camera.transform.position = new Vector3(curStage.centerCoord.x, curStage.centerCoord.y, camera.transform.position.z);

        // 3. 캐릭터 이동
        GameManager.instance.player.gridPos = curStage.centerCoord;
        GridManager.instance.RegisterOccupant(GameManager.instance.player.gridPos, GameManager.instance.player.gameObject);
        GameManager.instance.player.transform.position = GridManager.instance.GridToWorld(GameManager.instance.player.gridPos);

        // 4. 몬스터 스폰
        Spawner.instance.RandomSpawn(enemySpawn, curStage.minCoord, curStage.maxCoord);

        // 5. 스테이지 진행 중 상태 설정
        isStageInProgress = true;

        // 6. 방문 여부 설정
        curStage.IsVisited = true;
    }

    // 스테이지 이동 시 사용
    public void MoveStage(Vector2Int dirIndex)
    {
        // 1. 이동 UI 끄기
        HideStageMoveUI();

        // 2. 다음 스테이지 데이터 설정
        Vector2Int nextStageIndex = curStage.index + dirIndex;
        StageData nextStage = stageDict[nextStageIndex];

        // 3. 기존 스테이지 fog 처리
        curStage.SetFog(false);

        // 4. 현재 스테이지 변경
        curStage = nextStage;
        curStage.SetFog(true);

        // 5. 카메라 이동. Z는 보통 -10으로 고정
        camera.transform.position = new Vector3(curStage.centerCoord.x, curStage.centerCoord.y, camera.transform.position.z);

        // 6. 캐릭터 이동
        Vector2Int playerSpawnPos = curStage.centerCoord;
        int offset = 1;

        if (dirIndex == Vector2Int.right)
        {
            playerSpawnPos.x = curStage.minCoord.x + offset;
        }
        else if (dirIndex == Vector2Int.left)
        {/*  */
            playerSpawnPos.x = curStage.maxCoord.x - offset;
        }
        else if (dirIndex == Vector2Int.down)
        {
            playerSpawnPos.y = curStage.maxCoord.y - offset;
        }
        else if (dirIndex == Vector2Int.up)
        {
            playerSpawnPos.y = curStage.minCoord.y + offset;
        }

        GameManager.instance.player.gridPos = playerSpawnPos;
        GridManager.instance.RegisterOccupant(GameManager.instance.player.gridPos, GameManager.instance.player.gameObject);
        GameManager.instance.player.transform.position = GridManager.instance.GridToWorld(GameManager.instance.player.gridPos);

        if (curStage.IsVisited == false)
        {
            // 7. 몬스터 스폰
            Spawner.instance.RandomSpawn(enemySpawn, curStage.minCoord, curStage.maxCoord);

            // 8. 스테이지 진행 중 상태 설정
            isStageInProgress = true;

            // 9. 플레이어 턴으로 변경
            TurnManager.instance.ChangeCurState(TurnState.PlayerTurn);
            TurnManager.instance.StartPlayerTurn();
        }
        else
        {
            // 방문했던 스테이지면 바로 이동 UI 표시
            ShowStageMoveUI();
        }

        // 10. 방문 여부 설정
        curStage.IsVisited = true;
    }

    // Unity Button OnClick에서 사용할 방향별 메서드들
    public void MoveEast() => MoveStage(Vector2Int.right);
    public void MoveWest() => MoveStage(Vector2Int.left);
    public void MoveSouth() => MoveStage(Vector2Int.down);
    public void MoveNorth() => MoveStage(Vector2Int.up);

    // 스테이지 종료 체크
    public void CheckStageClear(bool isLastEnemy)
    {
        // 1. 마지막 적이 죽었으면
        if (isLastEnemy)
        {
            // 2. 스테이지 종료 처리
            isStageInProgress = false;

            // 3. 업그레이드 패널 출력
            StartCoroutine(ShowUpgradePanel());
        }
    }

    // n초 뒤 업그레이드 패널 출력
    private IEnumerator ShowUpgradePanel()
    {
        yield return new WaitForSeconds(1f);
        GameManager.instance.uiLevelUp.Show(0);
    }

    // 스테이지 이동 UI 표시
    public void ShowStageMoveUI()
    {
        StartCoroutine(ShowStageMoveUICoroutine());
    }

    private IEnumerator ShowStageMoveUICoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        stageMoveUI.SetActive(true);
    }

    // 스테이지 이동 UI 숨김
    public void HideStageMoveUI()
    {
        stageMoveUI.SetActive(false);
    }

    // 유닛이 이동할 때 현재 스테이지 범위를 벗어나지 않는지 체크
    public bool IsWithinBounds(Vector2Int pos)
    {
        return pos.x >= curStage.minCoord.x
        && pos.x <= curStage.maxCoord.x
        && pos.y >= curStage.minCoord.y
        && pos.y <= curStage.maxCoord.y;
    }
}
