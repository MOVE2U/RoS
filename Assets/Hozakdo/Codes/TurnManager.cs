using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 클래스 외부에 선언된 enum은 전역적으로 사용 가능
public enum TurnState
{
    None,
    MoveTurn,
    AttackTurn,
    MoveToAttackTurn,
    AttackToMoveTurn,
}

public class TurnManager : MonoBehaviour
{
    public static TurnManager instance;

    [Header("value")]
    [SerializeField] private int maxMoveCount = 10;

    [Header("for check")]
    [SerializeField] private TurnState curState = TurnState.None;
    [SerializeField] private int turnCount = 0;
    [SerializeField] private float moveCount = 0;

    [Header("external ref")]
    [SerializeField] private Spawner spawner;
    [SerializeField] private HUD hud;
    [SerializeField] private TutorialManager tutorialManager;
    [SerializeField] private SpawnData monsterFirst;
    [SerializeField] private SpawnData monsterGeneral;
    [SerializeField] private SpawnData coinFirst;
    [SerializeField] private List<Vector2Int> coinPoints;
    [SerializeField] private SpawnData choiceNPCFirst;
    [SerializeField] private List<Vector2Int> choiceNPCPoints;
    [SerializeField] private GameObject freezeNotice;
    [SerializeField] private GameObject moveAgainNotice;

    public TurnState CurState => curState;
    public int TurnCount => turnCount;
    public float MoveCount => moveCount;
    public int MaxMoveCount => maxMoveCount;
    
    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        switch (curState)
        {
            case TurnState.MoveTurn:
                if (moveCount >= maxMoveCount)
                {
                    StartCoroutine(StartAttackTurn());
                }
                break;
            case TurnState.AttackTurn:
                if (moveCount >= maxMoveCount)
                {
                    StartCoroutine(StartMoveTurn());
                }
                break;
        }
    }

    public IEnumerator StartMoveTurn()
    {
        if(turnCount > 0)
        {
            curState = TurnState.AttackToMoveTurn;
            moveAgainNotice.SetActive(true);
            yield return new WaitForSeconds(2f);
            moveAgainNotice.SetActive(false);
        }

        curState = TurnState.MoveTurn;
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

    private IEnumerator StartAttackTurn()
    {
        curState = TurnState.MoveToAttackTurn;
        if(turnCount == 1)
        {
            tutorialManager.NextStep();
            yield return new WaitForSeconds(2f);
            tutorialManager.NextStep();
        }
        else
        {
            freezeNotice.SetActive(true);
            yield return new WaitForSeconds(2f);
            freezeNotice.SetActive(false);
        }

        curState = TurnState.AttackTurn;
        moveCount = 0;

        for (int i = 1; i <= maxMoveCount; i++)
        {
            MoveCountChange(1);

            float[] ws = { 0.5f, 0.7f };
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

            GameManager.instance.player.CheckSurrounded();

            if (GameManager.instance.isLive == false)
            {
                yield break;
            }
        }
    }

    public void MoveCountChange(float f)
    {
        moveCount += f;
    }
}
