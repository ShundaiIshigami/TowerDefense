using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Unity.Cinemachine;

public class GameManager : MonoBehaviour
{
    // ゲームの状態定義
    public enum GameState
    {
        BuildPhase,  // 塔（タワー）の配置フェーズ（上から視点推奨）
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
    public GameObject buildUIGroup;   // 建築用UI（ショップ、設置ボタンなど）
    public GameObject actionUIGroup;  // 主人公用UI（クロスヘア、弾薬など）
    public GameObject resultUIGroup;  // リザルト画面UI

    [Header("UI Displays")]
    public Text waveText;
    public Text goldText;
    public Text baseHealthText;
    
    [Header("Game Data")]
    public int currentWave = 1;
    public int playerGold = 100;
    public int baseHealth = 10;

    void Start()
    {
        InitializeGame();
    }

    void Update()
    {
        //Tabキーによる視点切り替え処理
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleViewMode();
        }

        //フェーズごとのループ処理
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

        //UI表示の更新
        UpdateStatsUI();
    }

    //視点（カメラと対応UI）の切り替え
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
            // 上から視点に切替
            if (overviewCamera != null) overviewCamera.Priority = 10;
            if (playerFollowCamera != null) playerFollowCamera.Priority = 0;

            // カーソルを表示して操作可能にする（UIクリック用）
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // UI切り替え
            if (buildUIGroup) buildUIGroup.SetActive(currentGameState == GameState.BuildPhase);
            if (actionUIGroup) actionUIGroup.SetActive(false);
        }
        else if (currentViewMode == ViewMode.PlayerView)
        {
            // 主人公視点に切替
            if (overviewCamera != null) overviewCamera.Priority = 0;
            if (playerFollowCamera != null) playerFollowCamera.Priority = 10;

            // カーソルを画面中央に固定（TPS/FPS操作用）
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // UI切り替え
            if (buildUIGroup) buildUIGroup.SetActive(false);
            if (actionUIGroup) actionUIGroup.SetActive(true);
        }
    }

    //建築フェーズの処理
    private void UpdateBuildPhase()
    {
        // スペースキーでウェーブ開始
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartWave();
        }
    }

    //ウェーブ中の処理
    private void UpdateWavePhase()
    {
        /*
        if (baseHealth <= 0)
        {
            GameOver();
        }*/
    }

    public void StartWave()
    {
        currentGameState = GameState.WavePhase;

        // ウェーブ開始時は自動的に主人公視点に移行させる場合
        SetViewMode(ViewMode.PlayerView);
    }

    public void EndWave()
    {
        currentWave++;
        currentGameState = GameState.BuildPhase;

        // ウェーブ終了時は建築のために上から視点へ戻す
        SetViewMode(ViewMode.Overview);
    }

    private void GameOver()
    {
        currentGameState = GameState.ResultPhase;
        HideAllUI();

        if (resultUIGroup) resultUIGroup.SetActive(true);

        // カーソルロック解除
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void UpdateStatsUI()
    {
        if (waveText) waveText.text = $"WAVE: {currentWave}";
        if (goldText) goldText.text = $"GOLD: {playerGold}";
        if (baseHealthText) baseHealthText.text = $"BASE HP: {baseHealth}";
    }

    private void HideAllUI()
    {
        if (buildUIGroup) buildUIGroup.SetActive(false);
        if (actionUIGroup) actionUIGroup.SetActive(false);
        if (resultUIGroup) resultUIGroup.SetActive(false);
    }

    private void InitializeGame()
    {
        currentWave = 1;
        playerGold = 100;
        baseHealth = 10;

        currentGameState = GameState.BuildPhase;
        SetViewMode(ViewMode.Overview); // 初期状態は建築用の見下ろし視点
    }

    public void ResetGame()
    {
        HideAllUI();
        InitializeGame();
    }
}