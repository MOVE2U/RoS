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
    [SerializeField] private float transitionTime = 1.5f;
    [SerializeField] private int maxMoveCount = 10;

    [Header("for check")]
    [SerializeField] private TurnState curState = TurnState.None;
    [SerializeField] private int turnCount = 0;
    [SerializeField] private int moveCount = 0;

    [Header("external ref")]
    [SerializeField] private Spawner spawner;
    [SerializeField] private HUD hud;

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
        if(turnCount != 0)
        {
            curState = TurnState.Transition;
            yield return new WaitForSeconds(transitionTime);
        }        

        curState = TurnState.PlayerTurn;
        moveCount = 0;
        turnCount++;

        if(turnCount == 1)
        {
            spawner.RandomSpawn(turnCount, 5, 10);
        }
        else
        {
            spawner.RandomSpawn(turnCount);
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
