using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager instance;

    public Spawner spawner;
    public HUD hud;

    public bool isPlayerTurn;
    public bool isEnemyTurn;
    public bool isTurnChanging;
    public bool isEnemyMoving;
    public int playerTurnCount;
    public int enemyTurnCount;
    public int playerMoveCount = 0;
    public int enemyMoveCount = 0;

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
    public IEnumerator PlayerTurn(float time)
    {
        // 추후 턴전환 연출에 사용
        yield return new WaitForSeconds(time);

        isPlayerTurn = true;
        isTurnChanging = false;

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
        isTurnChanging = false;

        enemyTurnCount++;

        for (int i = 1; i <= 10; i++)
        {
            enemyMoveCount++;
            hud.UseEnemyTurn(enemyMoveCount);

            foreach (Enemy e in enemies)
            {
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
