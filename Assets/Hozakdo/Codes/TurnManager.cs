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
    public float enemyTurnDelay;
    [SerializeField] private int enemyMoveCount;

    // 초기값은 인스펙터에서 설정. 외부 수정은 메서드로.
    [field: SerializeField] public int MaxPlayerMoveCount { get; private set; }
    [field: SerializeField] public int MaxEnemyMoveCount { get; private set; }
    [field: SerializeField] public int MaxTurnCount { get; private set; }

    // 초기값은 Awake에서 설정. 체크용으로 인스펙터에 노출. 외부 수정은 메서드로.
    [field: SerializeField] public TurnState CurState { get; private set; }
    [field: SerializeField] public int TurnCount { get; private set; }
    [field: SerializeField] public int MoveCount { get; private set; }

    [Header("external ref")]
    [SerializeField] private Spawner spawner;
    [SerializeField] private HUD hud;
    [SerializeField] private TutorialManager tutorialManager;
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

    private void Update()
    {
        // 플레이어 턴일 때 스페이스 바를 누르면 턴 종료
        if (CurState == TurnState.PlayerTurn && Input.GetKeyDown(KeyCode.Tab))
        {
            EndPlayerTurn();
        }
    }

    public void StartPlayerTurn()
    {
        MoveCount = 0;
    }

    private IEnumerator StartEnemyTurn()
    {
        TurnCount++;
        if(TurnCount >= MaxTurnCount)
        {
            GameManager.instance.Stop();
            GameManager.instance.player.gameOver.SetActive(true);
        }

        MoveCount = 0;

        // 여기서 IntentExecute 실행하고, 플레이어 패배 조건 확인. 끝나면 다음 for문 돌도록 해야함.
        for (int i = 1; i <= MaxEnemyMoveCount; i++)
        {
            // 적 AI 실행
            foreach (EnemyAbstract enemy in spawner.activeEnemies)
            {
                if (enemy == null)
                {
                    continue;
                }

                enemy.IntentExecute();
            }

            // 플레이어 패배 조건 확인
            GameManager.instance.player.CheckSurrounded();
            if (GameManager.instance.isLive == false)
            {
                yield break;
            }

            // 적 이동 후 이동 횟수 증가
            MoveCountAdd(enemyMoveCount);
        }

        yield return new WaitForSeconds(2f);

        for (int i = 1; i <= MaxEnemyMoveCount; i++)
        {
            // 적 AI 실행
            foreach (EnemyAbstract enemy in spawner.activeEnemies)
            {
                if (enemy == null)
                {
                    continue;
                }

                enemy.IntentCalculate();
            }
            yield return new WaitUntil(() => spawner.activeEnemies.TrueForAll(x => !x.IsMoving));
        }
    }

    public void MoveCountAdd(int i)
    {
        MoveCount += i;

        switch (CurState)
        {
            case TurnState.PlayerTurn:
                // if (MoveCount >= MaxPlayerMoveCount)
                // {
                //     StartCoroutine(DelayedTurnChange());
                // }
                break;
            case TurnState.EnemyTurn:
                if (MoveCount >= MaxEnemyMoveCount)
                {
                    StartCoroutine(DelayedTurnChange());
                }
                break;
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

    public void EndPlayerTurn()
    {
        StartCoroutine(DelayedTurnChange());
    }

    public void ChangeCurState(TurnState state)
    {
        CurState = state;
    }
}
