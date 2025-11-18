using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
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
    }

    // 스테이지 이동 시 사용
    public void MoveStage(Vector2Int dirIndex)
    {
        // 0. 이동 UI 끄기
        stageMoveUI.SetActive(false);

        // 1. 다음 스테이지 데이터 설정
        Vector2Int nextStageIndex = curStage.index + dirIndex;
        StageData nextStage = stageDict[nextStageIndex];

        // 2. 기존 스테이지 fog 처리
        curStage.SetFog(false);

        // 3. 현재 스테이지 변경
        curStage = nextStage;
        curStage.SetFog(true);

        // 4. 카메라 이동. Z는 보통 -10으로 고정
        camera.transform.position = new Vector3(curStage.centerCoord.x, curStage.centerCoord.y, camera.transform.position.z);

        // 5. 캐릭터 이동
        Vector2Int playerSpawnPos = curStage.centerCoord;
        int offset = 1;

        if (dirIndex == Vector2Int.right)
        {
            playerSpawnPos.x = curStage.minCoord.x + offset;
        }
        else if (dirIndex == Vector2Int.left)
        {
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

        // 6. 몬스터 스폰
        Spawner.instance.RandomSpawn(enemySpawn, curStage.minCoord, curStage.maxCoord);
    }

    // Unity Button OnClick에서 사용할 방향별 메서드들
    public void MoveEast() => MoveStage(Vector2Int.right);
    public void MoveWest() => MoveStage(Vector2Int.left);
    public void MoveSouth() => MoveStage(Vector2Int.down);
    public void MoveNorth() => MoveStage(Vector2Int.up);

    // 유닛이 이동할 때 현재 스테이지 범위를 벗어나지 않는지 체크
    public bool IsWithinBounds(Vector2Int pos)
    {
        return pos.x >= curStage.minCoord.x
        && pos.x <= curStage.maxCoord.x
        && pos.y >= curStage.minCoord.y
        && pos.y <= curStage.maxCoord.y;
    }
}
