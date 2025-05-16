using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // 정적 변수는 유니티에서 인스펙터 창에 노출되지 않는다. 따라서 드래그앤드롭으로 할당할 수 없다.
    public static GameManager instance;

    [Header("Game Control")]
    public bool isLive;
    public float gameTime;
    public float maxGameTime;
    [Header("Player Info")]
    public int playerId;
    public float health;
    public float maxHealth;
    public int level;
    public int kill;
    public int spawnCountTotal;
    public int exp;
    // 동적으로 객체나 컴포넌트를 초기화할 때는 Awake나 Start에서 하지만, 기본 데이터 타입은 필드에서 가능하다.
    public int[] nextExp = { 3, 5, 10, 100, 150, 210, 280, 360, 450, 600 };
    [Header("Game Object")]
    // 유니티에서 이 변수에 오브젝트를 드래그앤드롭 하면,
    // PoolManager 컴포넌트가 있는지 확인하고 있다면 그 컴포넌트의 참조가 pool 변수에 저장되는 방식임.
    // 이 방식은 Awake 호출 전에 Unity에서 수행함.
    public PoolManager pool;
    public Player player;
    public LevelUp uiLevelUp;
    public Result uiResult;
    public GameObject enemyCleaner;
    void Awake()
    {
        instance = this;
    }
    public void GameStart(int id)
    {

        playerId = id;
        health = maxHealth;

        player.gameObject.SetActive(true);
        Vector2Int playerGridPos = GridManager.instance.WorldToGrid(player.transform.position);
        player.transform.position = GridManager.instance.GridToWorld(playerGridPos); // 보정
        GridManager.instance.Register(playerGridPos, player.gameObject); // 등록
        // uiLevelUp.Select(playerId);
        Resume();

        AudioManager.instance.PlayBgm(true);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);

        TurnManager.instance.isEnemyTurn = false;
        StartCoroutine(TurnManager.instance.PlayerTurn(0f));
    }
    public void GameOver()
    {
        StartCoroutine(GameOverRoutine());
    }
    IEnumerator GameOverRoutine()
    {
        isLive = false;
        yield return new WaitForSeconds(0.5f);
        uiResult.gameObject.SetActive(true);
        uiResult.Lose();
        Stop();

        AudioManager.instance.PlayBgm(false);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Lose);
    }
    public void GameVictory()
    {
        StartCoroutine(GameVictoryRoutine());
    }
    IEnumerator GameVictoryRoutine()
    {
        isLive = false;
        enemyCleaner.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        uiResult.gameObject.SetActive(true);
        uiResult.Win();
        Stop();

        AudioManager.instance.PlayBgm(false);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Win);
    }
    public void GameRetry()
    {
        SceneManager.LoadScene(0);
    }
    void Update()
    {
        if (!isLive)
            return;

        gameTime += Time.deltaTime;
    }

    public void GetExp()
    {
        if (!isLive)
            return;

        exp++;

        if(exp== nextExp[Mathf.Min(level,nextExp.Length-1)])
        {
            level++;
            exp = 0;
            // UI 띄우는거 잠깐 막기
            // uiLevelUp.Show();
        }
    }
    public void Stop()
    {
        // isLive -> 게임 로직(예: 경험치, UI 갱신 등)을 멈춤
        // Time.timeScale -> 시간 관련 연산, 물리 엔진, 애니메이션을 정지

        // Time.timeScale을 0으로 하면 Time.deltaTime, Time.fixedDeltaTime 등이 0이 되어 시간 기반 연산이 0이 된다.
        // 근데 이게 Update를 막지는 못함. 시간과 관련이 없는 다른 함수들은 실행될 수 있다.
        isLive = false;
        Time.timeScale = 0;
    }
    public void Resume()
    {
        isLive = true;
        Time.timeScale = 1;
    }
}
