using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Ŭ���� �ܺο� ����� enum�� ���������� ��� ����
public enum TurnState
{
    None,
    PlayerTurn,
    EnemyTurn,
}

public class TurnManager : MonoBehaviour
{
    public static TurnManager instance;

    [Header("value")]
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
    [SerializeField] private SpawnData choiceNPCFirst;
    [SerializeField] private List<Vector2Int> choiceNPCPoints;

    public TurnState CurState => curState;
    public int TurnCount => turnCount;
    public int MoveCount => moveCount;
    public int MaxMoveCount => maxMoveCount;
    
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
                    StartPlayerTurn();
                }
                break;
        }
    }

    public void StartPlayerTurn()
    {
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
        }
    }

    private IEnumerator StartEnemyTurn()
    {
        curState = TurnState.EnemyTurn;
        moveCount = 0;

        for (int i = 1; i <= maxMoveCount; i++)
        {
            MoveCountChange(1);

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

    public void MoveCountChange(int i)
    {
        moveCount += i;
    }
}
