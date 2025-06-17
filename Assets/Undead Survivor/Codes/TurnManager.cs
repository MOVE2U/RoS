using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [Header("Turn-related value")]
    [SerializeField] private float transitionTime = 1.5f;
    [SerializeField] private int maxMoveCount = 10;
    [SerializeField] private TurnState curState = TurnState.None;
    [SerializeField] private int turnCount = 0;
    [SerializeField] private int moveCount = 0;

    private List<Enemy> enemies = new List<Enemy>();

    public TurnState CurState => curState;
    public int TurnCount => turnCount;
    public int MoveCount => moveCount;

    public Spawner spawner;
    public HUD hud;
    
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

    public void MoveCountInc()
    {
        moveCount++;

        switch(curState)
        {
            case TurnState.PlayerTurn:
                hud.UsePlayerTurn(moveCount);
                break;
            case TurnState.EnemyTurn:
                hud.UseEnemyTurn(moveCount);
                break;
        }
    }

    public IEnumerator StartPlayerTurn()
    {
        // 추후 턴전환 연출에 사용
        curState = TurnState.Transition;
        Debug.Log(curState);
        yield return new WaitForSeconds(transitionTime);

        curState = TurnState.PlayerTurn;
        Debug.Log(curState);
        moveCount = 0;
        turnCount++;

        spawner.RandomSpawn(turnCount);
    }

    public IEnumerator StartEnemyTurn()
    {
        // 추후 턴전환 연출에 사용
        curState = TurnState.Transition;
        Debug.Log(curState);
        yield return new WaitForSeconds(transitionTime);

        curState = TurnState.EnemyTurn;
        Debug.Log(curState);
        moveCount = 0;

        for (int i = 1; i <= 10; i++)
        {
            MoveCountInc();

            float[] ws = { 0.1f, 1f };
            float w = ws[UnityEngine.Random.Range(0, ws.Length)];

            foreach (Enemy e in enemies)
            {
                e.wait = w;
                e.Move();
            }
            yield return new WaitUntil(() => enemies.TrueForAll(x => !x.isMoving));
        }
    }

    public void AddEnemy(Enemy e)
    {
        if(!enemies.Contains(e))
        {
            enemies.Add(e);
        }
    }

    public void RemoveEnemy(Enemy e)
    {
        enemies.Remove(e);
    }
}
