using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager instance;

    public Spawner spawner;

    public bool isPlayerTurn;
    public bool isEnemyTurn;
    public int playerTurnCount;
    public int enemyTurnCount;
    public int playerMoveCount;
    public float enemyMoveCount;
    private void Awake()
    {
        instance = this;
    }
    private void Update()
    {
        if (isPlayerTurn && playerMoveCount <= 0)
        {
            EnemyTurn();
        }
        if (isEnemyTurn)
        {
            enemyMoveCount -= Time.deltaTime;
            if (enemyMoveCount <= 0)
            {
                PlayerTurn();
            }
        }
    }
    public void PlayerTurn()
    {
        isPlayerTurn = true;
        isEnemyTurn = false;
        playerMoveCount = 10;
        spawner.RandomSpawn(GameManager.instance.player.transform.position, 10);
        playerTurnCount++;
    }
    public void EnemyTurn()
    {
        isPlayerTurn = false;
        isEnemyTurn = true;
        enemyMoveCount = 10;
        enemyTurnCount++;
    }
}
