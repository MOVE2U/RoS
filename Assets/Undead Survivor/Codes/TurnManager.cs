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

    private TurnState curState = TurnState.None;

    public Spawner spawner;
    public HUD hud;

    public int turnCount = 0;

    private List<Enemy> enemies = new List<Enemy>();
    private void Awake()
    {
        instance = this;
    }
    private void Update()
    {
        if (isPlayerTurn)
        {
            if (playerMoveCount >= 10)
            {
                isPlayerTurn = false;
                isTurnChanging = true;
                enemyMoveCount = 0;
                StartCoroutine(EnemyTurn(1.5f));
            }
        }
        else if (isEnemyTurn)
        {
            if (enemyMoveCount >= 10)
            {
                isEnemyTurn = false;
                isTurnChanging = true;
                playerMoveCount = 0;
                StartCoroutine(PlayerTurn(1.5f));
            }
        }
    }
    private IEnumerator StartPlayerTurn()
    {
        // 추후 턴전환 연출에 사용
        curState = TurnState.Transition;        
        yield return new WaitForSeconds(transitionTime);

        curState = TurnState.PlayerTurn;
        turnCount++;

        spawner.RandomSpawn(turnCount);
    }
    public IEnumerator EnemyTurn(float time)
    {
        // 추후 턴전환 연출에 사용
        yield return new WaitForSeconds(time);

        isEnemyTurn = true;
        isTurnChanging = false;

        enemyTurnCount++;

        for (int i = 1; i <= 10; i++)
        {
            enemyMoveCount++;
            hud.UseEnemyTurn(enemyMoveCount);

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
