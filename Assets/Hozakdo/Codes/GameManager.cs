using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // ?뺤쟻 蹂?섎뒗 ?좊땲?곗뿉???몄뒪?숉꽣 李쎌뿉 ?몄텧?섏? ?딅뒗?? ?곕씪???쒕옒洹몄븻?쒕∼?쇰줈 ?좊떦?????녿떎.
    public static GameManager instance;

    [Header("Game Control")]
    public bool isLive;
    public float gameTime;
    public float maxGameTime;
    [Header("Player Info")]
    public int playerId;
    public int level;
    public int kill;
    public int coin;
    public int spawnCountTotal;
    public int exp;
    public int NPCCount;
    // ?숈쟻?쇰줈 媛앹껜??而댄룷?뚰듃瑜?珥덇린?뷀븷 ?뚮뒗 Awake??Start?먯꽌 ?섏?留? 湲곕낯 ?곗씠????낆? ?꾨뱶?먯꽌 媛?ν븯??
    public int[] nextExp;
    [Header("Game Object")]
    // ?좊땲?곗뿉????蹂?섏뿉 ?ㅻ툕?앺듃瑜??쒕옒洹몄븻?쒕∼ ?섎㈃,
    // PoolManager 而댄룷?뚰듃媛 ?덈뒗吏 ?뺤씤?섍퀬 ?덈떎硫?洹?而댄룷?뚰듃??李몄“媛 pool 蹂?섏뿉 ??λ릺??諛⑹떇??
    // ??諛⑹떇? Awake ?몄텧 ?꾩뿉 Unity?먯꽌 ?섑뻾??
    public PoolManager pool;
    public Player player;
    public UpgradePanel uiLevelUp;
    public Result uiResult;
    public GameObject enemyCleaner;
    void Awake()
    {
        instance = this;
    }
    public void GameStart(int id)
    {

        playerId = id;

        player.gameObject.SetActive(true);
        Vector2Int playerGridPos = GridManager.instance.WorldToGrid(player.transform.position);
        player.transform.position = GridManager.instance.GridToWorld(playerGridPos); // 蹂댁젙
        GridManager.instance.RegisterOccupant(playerGridPos, player.gameObject); // ?깅줉
        Resume();

        AudioManager.instance.PlayBgm(true);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);

        TurnManager.instance.ChangeCurState(TurnState.PlayerTurn);
        TurnManager.instance.StartPlayerTurn();
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

        if (exp == nextExp[Mathf.Min(level, nextExp.Length - 1)])
        {
            level++;
            exp = 0;
            uiLevelUp.Show(0);
        }
    }

    public void Stop()
    {
        // isLive -> 寃뚯엫 濡쒖쭅(?? 寃쏀뿕移? UI 媛깆떊 ????硫덉땄
        // Time.timeScale -> ?쒓컙 愿???곗궛, 臾쇰━ ?붿쭊, ?좊땲硫붿씠?섏쓣 ?뺤?

        // Time.timeScale??0?쇰줈 ?섎㈃ Time.deltaTime, Time.fixedDeltaTime ?깆씠 0???섏뼱 ?쒓컙 湲곕컲 ?곗궛??0???쒕떎.
        // 洹쇰뜲 ?닿쾶 Update瑜?留됱???紐삵븿. ?쒓컙怨?愿?⑥씠 ?녿뒗 ?ㅻⅨ ?⑥닔?ㅼ? ?ㅽ뻾?????덈떎.
        isLive = false;
        Time.timeScale = 0;
    }
    public void Resume()
    {
        isLive = true;
        Time.timeScale = 1;
    }
}
