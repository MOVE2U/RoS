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
        playerTurnCount++;

        Vector3 playerPos = GameManager.instance.player.transform.position;
        Vector2Int playerGridPos = GridManager.instance.WorldToGrid(playerPos);
        int count = spawner.spawnCount[Mathf.Min(enemyTurnCount, spawner.spawnCount.Length - 1)];
        spawner.RandomSpawn(playerGridPos, count);
    }
    public void EnemyTurn()
    {
        isPlayerTurn = false;
        isEnemyTurn = true;
        enemyMoveCount = 10;
        enemyTurnCount++;
    }
}
