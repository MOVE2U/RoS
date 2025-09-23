using UnityEngine;
using System;
using System.Collections;

public class Enemy : Unit, ISpawnable
{
    [Header("value")]
    [SerializeField] private float health;
    [SerializeField] private float maxHealth;

    [Header("for check")]
    [SerializeField] private Vector2Int targetGridPos;
    [SerializeField] private bool isArrived;

    [Header("Status Condition")]
    public int pointillismStacks = 0;
    public bool canMove = true;

    private Player player;
    private Spawner spawner;

    private new void Awake()
    {
        base.Awake();

        // Unit ���� ���� �ʱ�ȭ
        moveTime = 0.3f;
    }

    // �ʱ�ȭ - Ȱ��ȭ �� �� ����
    private void OnEnable()
    {
        // ������Ʈ Ǯ���� OnEnable���� �ʱ�ȭ�ϱ⵵ ��. �̱��� ������ ����� ����
        player = GameManager.instance.player;
        spawner = Spawner.instance;

        if (spawner == null)
            spawner = Spawner.instance;
        spawner.AddEnemy(this);
        isMoving = false;
        health = maxHealth;
    }

    // �ʱ�ȭ - �ܺο��� ���޹��� �����ͷ� �����ϴ� �׸�
    public void OnSpawn(SpawnData spawnData, Vector2Int pos)
    {
        health = spawnData.health;
        maxHealth = spawnData.health;

        // �� ��ǥ ������Ʈ
        gridPos = pos;
        GridManager.instance.RegisterOccupant(pos, this.gameObject);

        // ���� ��ǥ ������Ʈ
        transform.position = new Vector3(gridPos.x, gridPos.y, 0);
    }

    // ���� - ��Ȱ��ȭ �� �� ����
    private void OnDisable()
    {
        GridManager.instance.UnRegisterOccupant(gridPos);
        spawner.RemoveEnemy(this);
    }

    // ���� - �׾��� ���� ȣ��Ǵ� �׸�
    void Dead()
    {
        GridManager.instance.UnRegisterOccupant(gridPos);
        gameObject.SetActive(false);
        GameManager.instance.kill++;
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Dead);
    }

    private void EnemyMoveJudge()
    {
        GetTargetGridPos();

        if (!isArrived)
        {
            GetInputDir();

            if (!TryMove(inputDir))
            {
                Vector2Int inputDirCW = new Vector2Int(moveDir.y, -moveDir.x);
                Vector2Int inputDirCCW = new Vector2Int(-moveDir.y, moveDir.x);

                if (UnityEngine.Random.value < 0.5f)
                {
                    TryMove(inputDirCW);
                }
                else
                {
                    TryMove(inputDirCCW);
                }
            }
        }
        else
        {
            Vector2Int dir = player.gridPos - this.gridPos;
            SetSprite(dir);
        }
    }

    private void GetTargetGridPos()
    {
        targetGridPos = Vector2Int.zero;
        isArrived = false;

        Vector2Int[] candidates = new Vector2Int[]
        {
            player.gridPos + Vector2Int.right,
            player.gridPos + Vector2Int.left,
            player.gridPos + Vector2Int.down,
            player.gridPos + Vector2Int.up,
        };

        float minDistance = float.MaxValue;

        foreach (Vector2Int candidate in candidates)
        {
            if (gridPos == candidate)
            {
                isArrived = true;
                break;
            }

            if (GridManager.instance.IsOccupant(candidate))
            {
                continue;
            }

            float distance = Vector2Int.Distance(candidate, gridPos);
            if (distance < minDistance)
            {
                minDistance = distance;
                targetGridPos = candidate;
            }
        }
    }

    private void GetInputDir()
    {
        Vector2Int dir = targetGridPos - gridPos;

        if (UnityEngine.Random.value < 0.5f)
        {
            inputDir = new Vector2Int(Math.Sign(dir.x), 0);
        }
        else
        {
            inputDir = new Vector2Int(0, Math.Sign(dir.y));
        }

    }

    // ���߿� ��ų ����� BasicAttackController�� ���� Ŭ������ ����, ���� Ŭ������ �Ű������� ���� �ʿ�
    public void Attacked(BasicAttackController attack, float damage)
    {
        health -= damage;

        if (health > 0)
        {
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Hit);

            if (attack.curTexture == TextureData.TextureType.Pointillism)
            {
                pointillismStacks++;
                if (pointillismStacks >= attack.textureValue)
                {
                    Dead();
                }
            }
            else if (attack.curTexture == TextureData.TextureType.OilPainting)
            {
                StartCoroutine(StopMove(attack, attack.textureValue));
            }
        }
        else
        {
            Dead();
        }
    }

    public void AutoMove(float randomWait)
    {
        if(canMove)
        {
            wait = randomWait;
            EnemyMoveJudge();
        }
    }

    // �� ��ũ��Ʈ �ȿ� �ִ� IEnumerator�� StartCoroutine�� ���� ȣ���� �� �ִ�. �Ʒ��� Player���� Enemy�� StartCoroutine�� ȣ���� �ʿ䰡 �־ ������ �޼��带 ���� ��.
    public void PushedMove(Vector2Int dir)
    {
        StartCoroutine(ExecuteMove(dir));
    }

    public IEnumerator StopMove(BasicAttackController attack, float time)
    {
        canMove = false;

        yield return new WaitForSeconds(time);

        canMove = true;

    }
}
