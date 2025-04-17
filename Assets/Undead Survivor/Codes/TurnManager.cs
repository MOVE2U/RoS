using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager instance;

    public Spawner spawner;

    public bool isPlayerTurn;
    public bool isEnemyTurn;
    public int playerTurnCount;
    public float maxTurnTime;
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
        if (isEnemyTurn)
        {
            maxTurnTime -= Time.deltaTime;
            if (maxTurnTime <= 0)
            {
                PlayerTurn();
            }
        }
    }
    public void PlayerTurn()
    {
        isPlayerTurn = true;
        isEnemyTurn = false;
        playerTurnCount = 10;
        spawner.RandomSpawn(GameManager.instance.player.transform.position, 10);
    }
    public void EnemyTurn()
    {
        isPlayerTurn = false;
        isEnemyTurn = true;
        maxTurnTime = 10;
    }
}
