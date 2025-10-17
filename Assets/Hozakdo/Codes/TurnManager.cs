using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 클래스 외부에 선언된 enum은 전역적으로 사용 가능
public enum TurnState
{
    None,
    PlayerTurn,
    EnemyTurn,
}

public class TurnManager : MonoBehaviour
{
    public static TurnManager instance;

    // 초기값 인스펙터에서 설정.
    [SerializeField] private float enemyWaitTime;
    [SerializeField] private int enemyMoveCount;

    // 초기값은 인스펙터에서 설정. 외부 수정은 메서드로.
    [field: SerializeField] public int MaxMoveCount { get; private set; }

    // 초기값은 Awake에서 설정. 체크용으로 인스펙터에 노출. 외부 수정은 메서드로.
    [field: SerializeField] public TurnState CurState { get; private set; }
    [field: SerializeField] public int TurnCount { get; private set; }
    [field: SerializeField] public int MoveCount { get; private set; }

    [Header("external ref")]
    [SerializeField] private Spawner spawner;
    [SerializeField] private HUD hud;
    [SerializeField] private TutorialManager tutorialManager;
    [SerializeField] private SpawnData monsterFirst;
    [SerializeField] private SpawnData monsterGeneral;
    [SerializeField] private GameObject freezeNotice;
    [SerializeField] private GameObject moveAgainNotice;
    public SpawnData coinDrop;

    private void Awake()
    {
        instance = this;

        CurState = TurnState.None;
        TurnCount = 0;
        MoveCount = 0;
    }

    public void StartPlayerTurn()
    {
        TurnCount++;
        MoveCount = 0;

        if (TurnCount == 1)
        {
            spawner.RandomSpawn(TurnCount, monsterFirst);
        }
        else
        {
            spawner.RandomSpawn(TurnCount, monsterGeneral);
        }
    }

    private IEnumerator StartEnemyTurn()
    {
        MoveCount = 0;

        for (int i = 1; i <= MaxMoveCount; i++)
        {
            // 적 이동
            float waitTime = enemyWaitTime;
            foreach (Enemy enemy in spawner.activeEnemies)
            {
                if (enemy == null)
                {
                    continue;
                }

                enemy.AutoMove(waitTime);
            }
            yield return new WaitUntil(() => spawner.activeEnemies.TrueForAll(x => !x.IsMoving));

            // 플레이어 패배 조건 확인
            GameManager.instance.player.CheckSurrounded();
            if (GameManager.instance.isLive == false)
            {
                yield break;
            }

            // 적 이동 후 이동 횟수 증가
            MoveCountAdd(enemyMoveCount);
        }
    }

    public void MoveCountAdd(int i)
    {
        MoveCount += i;

        if (MoveCount >= MaxMoveCount)
        {
            StartCoroutine(DelayedTurnChange());
        }
    }

    private IEnumerator DelayedTurnChange()
    {
        yield return new WaitForSeconds(0.1f);
        
        switch (CurState)
        {
            case TurnState.PlayerTurn:
                CurState = TurnState.EnemyTurn;
                StartCoroutine(StartEnemyTurn());
                break;
            case TurnState.EnemyTurn:
                CurState = TurnState.PlayerTurn;
                StartPlayerTurn();
                break;
        }
    }

    public void ChangeCurState(TurnState state)
    {
        CurState = state;
    }
}
