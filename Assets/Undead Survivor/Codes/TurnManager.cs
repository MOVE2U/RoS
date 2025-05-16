using System;
using System.Collections;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager instance;

    public Spawner spawner;

    public bool isPlayerTurn;
    public bool isEnemyTurn;
    public int playerTurnCount;
    public int enemyTurnCount;
    public float playerMoveCount;
    public float enemyMoveCount;
    private void Awake()
    {
        instance = this;
    }
    private void Update()
    {
        if (isPlayerTurn)
        {
            playerMoveCount = Mathf.Max(0, playerMoveCount - Time.deltaTime);
            if (playerMoveCount <= 0)
            {
                isPlayerTurn = false;

                StartCoroutine(EnemyTurn(1f));
            }
        }
        else if (isEnemyTurn)
        {
            enemyMoveCount = Mathf.Max(0, enemyMoveCount - Time.deltaTime);
            if (enemyMoveCount <= 0)
            {
                isEnemyTurn = false;

                StartCoroutine(PlayerTurn(1f));
            }
        }
    }
    public IEnumerator PlayerTurn(float time)
    {
        // 추후 턴전환 연출에 사용
        yield return new WaitForSeconds(time);

        isPlayerTurn = true;

        playerMoveCount = 10;
        playerTurnCount++;

        Vector3 playerPos = GameManager.instance.player.transform.position;
        Vector2Int playerGridPos = GridManager.instance.WorldToGrid(playerPos);
        int count = spawner.spawnCount[Mathf.Min(enemyTurnCount, spawner.spawnCount.Length - 1)];
        spawner.RandomSpawn(playerGridPos, count);
    }
    public IEnumerator EnemyTurn(float time)
    {
        // 추후 턴전환 연출에 사용
        yield return new WaitForSeconds(time);

        isEnemyTurn = true;

        enemyMoveCount = 10;
        enemyTurnCount++;
    }
}
