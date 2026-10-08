using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Unity.Cinemachine;

public class GameManager : MonoBehaviour
{
    // ゲームの状態定義
    public enum GameState
    {
        BuildPhase,  // 塔（タワー）の配置フェーズ
        WavePhase,   // 敵の侵攻フェーズ（操作・攻撃フェーズ）
        ResultPhase  // クリア・ゲームオーバー
    }

    // 視点の状態定義
    public enum ViewMode
    {
        Overview,   // 俯瞰（上から視点）
        PlayerView  // 主人公（三人称/一人称視点）
    }

    [Header("State")]
    public GameState currentGameState = GameState.BuildPhase;
    public ViewMode currentViewMode = ViewMode.Overview;

    [Header("Cinemachine Cameras")]
    public CinemachineCamera overviewCamera;   // 上から見下ろすカメラ
    public CinemachineCamera playerFollowCamera; // 主人公に追従するカメラ

    [Header("UI Groups")]
    public GameObject buildUIGroup;   // 建築用UI
    public GameObject actionUIGroup;  // 主人公用UI
    public GameObject resultUIGroup;  // リザルト画面UI

    [Header("UI Displays")]
    public Text waveText;
    public Text goldText;
    public Text baseHealthText;
    public Text startWaveGuideText;

    [Header("Result UI Displays")]
    public Text resultTitleText;    // 「GAME OVER」または「GAME CLEAR」
    public Text resultWaveText;     // 「Reached Wave: X」など

    [Header("Game Data")]
    public int currentWave = 1;
    public int maxWave = 5;          // 全何ウェーブでクリアにするか
    public int playerGold = 100;
    public int baseHealth = 10;

    [Header("Passive Income Settings")]
    public int goldPerInterval = 2;     // 1回にもらえるゴールドの量
    public float incomeInterval = 1.0f;  // ゴールドが入る間隔（秒）
    private float incomeTimer = 0f;      // タイマー用変数

    void Start()
    {
        InitializeGame();
    }

    void Update()
    {
        // Tabキーによる視点切り替え処理
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleViewMode();
        }

        // フェーズごとのループ処理
        switch (currentGameState)
        {
            case GameState.BuildPhase:
                UpdateBuildPhase();
                break;

            case GameState.WavePhase:
                UpdateWavePhase();
                break;

            case GameState.ResultPhase:
                if (Input.GetKeyDown(KeyCode.R))
                {
                    ResetGame();
                }
                break;
        }

        // UI表示の更新
        UpdateStatsUI();
    }

    // 視点（カメラと対応UI）の切り替え
    public void ToggleViewMode()
    {
        if (currentViewMode == ViewMode.Overview)
        {
            SetViewMode(ViewMode.PlayerView);
        }
        else
        {
            SetViewMode(ViewMode.Overview);
        }
    }

    private void SetViewMode(ViewMode newMode)
    {
        currentViewMode = newMode;

        if (currentViewMode == ViewMode.Overview)
        {
            if (overviewCamera != null) overviewCamera.Priority = 10;
            if (playerFollowCamera != null) playerFollowCamera.Priority = 0;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // 💡 リザルトフェーズでない時のみ建築UIを表示
            if (buildUIGroup) buildUIGroup.SetActive(currentGameState == GameState.BuildPhase);
            if (actionUIGroup) actionUIGroup.SetActive(false);
        }
        else if (currentViewMode == ViewMode.PlayerView)
        {
            if (overviewCamera != null) overviewCamera.Priority = 0;
            if (playerFollowCamera != null) playerFollowCamera.Priority = 10;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            if (buildUIGroup) buildUIGroup.SetActive(false);
            if (actionUIGroup) actionUIGroup.SetActive(true);
        }
    }

    private void UpdateBuildPhase()
    {
        // スペースキーでウェーブ開始
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartWave();
        }
    }

    private void UpdateWavePhase()
    {
        // タイマーを進める
        incomeTimer += Time.deltaTime;

        // 設定した間隔が経過したらゴールドを加算
        if (incomeTimer >= incomeInterval)
        {
            playerGold += goldPerInterval;
            incomeTimer = 0f;
        }

        // 拠点HPが0以下でゲームオーバー
        if (baseHealth <= 0)
        {
            GameOver();
        }
    }

    public void StartWave()
    {
        currentGameState = GameState.WavePhase;
        incomeTimer = 0f;

        SetViewMode(ViewMode.PlayerView);

        // WaveManagerへ開始通知
        WaveManager waveManager = FindFirstObjectByType<WaveManager>();
        if (waveManager != null)
        {
            waveManager.StartNextWave();
        }
    }

    public void EndWave()
    {
        // 最終ウェーブをクリアしたかの判定
        if (currentWave >= maxWave)
        {
            GameClear();
            return;
        }

        currentWave++;
        currentGameState = GameState.BuildPhase;

        SetViewMode(ViewMode.Overview);
    }

    private void GameOver()
    {
        currentGameState = GameState.ResultPhase;
        HideAllUI();

        if (resultTitleText)
        {
            resultTitleText.text = "GAME OVER";
            resultTitleText.color = Color.red;
        }
        if (resultWaveText)
        {
            resultWaveText.text = $"Reached Wave: {currentWave}";
        }

        if (resultUIGroup) resultUIGroup.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void GameClear()
    {
        currentGameState = GameState.ResultPhase;
        HideAllUI();

        if (resultTitleText)
        {
            resultTitleText.text = "ALL CLEAR!";
            resultTitleText.color = Color.yellow;
        }
        if (resultWaveText)
        {
            resultWaveText.text = $"Cleared All {maxWave} Waves!";
        }

        if (resultUIGroup) resultUIGroup.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void UpdateStatsUI()
    {
        if (waveText) waveText.text = $"WAVE: {currentWave} / {maxWave}";
        if (goldText) goldText.text = $"GOLD: {playerGold}";
        if (baseHealthText) baseHealthText.text = $"BASE HP: {baseHealth}";

        if (startWaveGuideText)
        {
            bool isBuildPhase = (currentGameState == GameState.BuildPhase);
            startWaveGuideText.gameObject.SetActive(isBuildPhase);

            if (isBuildPhase)
            {
                startWaveGuideText.text = "[ SPACE ] キーでウェーブ開始";
            }
        }
    }

    private void HideAllUI()
    {
        if (buildUIGroup) buildUIGroup.SetActive(false);
        if (actionUIGroup) actionUIGroup.SetActive(false);
        if (resultUIGroup) resultUIGroup.SetActive(false);
    }

    // 💡 修正: ゲーム開始時に全てのUIを一回非表示にしてから初期化
    private void InitializeGame()
    {
        currentWave = 1;
        playerGold = 100;
        baseHealth = 10;
        incomeTimer = 0f;

        currentGameState = GameState.BuildPhase;

        // 全UIを一度隠す（これでResultUIGroupも消えます）
        HideAllUI();

        // 俯瞰視点にして建築用UIを有効化
        SetViewMode(ViewMode.Overview);
    }

    public void ResetGame()
    {
        InitializeGame();
    }
}