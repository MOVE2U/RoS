using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager instance;

    public bool isPlayerTurn;
    public bool isEnemyTurn;
    public int playerTurnCount;
    public float enemyTurnCount;
    private void Awake()
    {
        instance = this;
    }
    private void Update()
    {
        if (isPlayerTurn && playerTurnCount <= 0)
        {
            EnemyTurn();
        }
    }
    public void PlayerTurn()
    {
        isPlayerTurn = true;
        isEnemyTurn = false;
        playerTurnCount = 10;
    }
    public void EnemyTurn()
    {
        isPlayerTurn = false;
        isEnemyTurn = true;
        enemyTurnCount = 10;
    }
}
