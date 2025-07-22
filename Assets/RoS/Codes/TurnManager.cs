using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 클래스 외부에 선언된 enum은 전역적으로 사용 가능
public enum TurnState
{
    None,
    PlayerTurn,
    EnemyTurn,
    Transition
}

public class TurnManager : MonoBehaviour
{
    public static TurnManager instance;

    [Header("value")]
    [SerializeField] private float transitionTime = 2.0f;
    [SerializeField] private int maxMoveCount = 10;

    [Header("for check")]
    [SerializeField] private TurnState curState = TurnState.None;
    [SerializeField] private int turnCount = 0;
    [SerializeField] private int moveCount = 0;

    [Header("external ref")]
    [SerializeField] private Spawner spawner;
    [SerializeField] private HUD hud;
    [SerializeField] private SpawnData monsterFirst;
    [SerializeField] private SpawnData monsterGeneral;
    [SerializeField] private SpawnData coinFirst;
    [SerializeField] private List<Vector2Int> coinPoints;
    [SerializeField] private SpawnData coinGeneral;
    [SerializeField] private SpawnData choiceNPCFirst;
    [SerializeField] private List<Vector2Int> choiceNPCPoints;
    [SerializeField] private SpawnData choiceNPCGeneral;

    public TurnState CurState => curState;
    public int TurnCount => turnCount;
    public int MoveCount => moveCount;
    
    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        switch (curState)
        {
            case TurnState.PlayerTurn:
                if (moveCount >= maxMoveCount)
                {
                    StartCoroutine(StartEnemyTurn());
                }
                break;
            case TurnState.EnemyTurn:
                if (moveCount >= maxMoveCount)
                {
                    StartCoroutine(StartPlayerTurn());
                }
                break;
        }
    }

    public IEnumerator StartPlayerTurn()
    {
        // 추후 턴전환 연출에 사용
        curState = TurnState.Transition;
        yield return new WaitForSeconds(transitionTime);

        curState = TurnState.PlayerTurn;
        moveCount = 0;
        turnCount++;

        if(turnCount == 1)
        {
            spawner.RandomSpawn(turnCount, monsterFirst);
            spawner.FixedSpawn(coinPoints, coinFirst);
            spawner.FixedSpawn(choiceNPCPoints, choiceNPCFirst);
        }
        else
        {
            spawner.RandomSpawn(turnCount, monsterGeneral);
            spawner.RandomSpawn(turnCount, coinGeneral);
            spawner.RandomSpawn(turnCount, choiceNPCGeneral);
        }
    }

    private IEnumerator StartEnemyTurn()
    {
        // 추후 턴전환 연출에 사용
        curState = TurnState.Transition;
        yield return new WaitForSeconds(transitionTime);

        curState = TurnState.EnemyTurn;
        moveCount = 0;

        for (int i = 1; i <= maxMoveCount; i++)
        {
            MoveCountInc();

            float[] ws = { 0.1f, 1f };
            float w = ws[UnityEngine.Random.Range(0, ws.Length)];

            foreach (Enemy e in spawner.activeEnemies)
            {
                if (e == null)
                {
                    continue;
                }

                e.AutoMove(w);
            }

            yield return new WaitUntil(() => spawner.activeEnemies.TrueForAll(x => !x.IsMoving));
        }
    }

    public void MoveCountInc()
    {
        moveCount++;
        hud.UpdateMoveCountUI(curState, moveCount);
    }
}
