using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    public float spawnInterval = 1.5f;

    private int activeEnemyCount = 0;

    void Update()
    {
        // GameManagerがWavePhaseになったら自動生成を開始するトリガー例
        if (gameManager.currentGameState == GameManager.GameState.BuildPhase)
        {
            // ビルドフェーズ中は待機
        }
    }

    // GameManagerの StartWave から呼ぶ
    public void StartNextWave()
    {
        int enemyCountToSpawn = gameManager.currentWave * 3 + 2; // ウェーブ数に応じた敵の数
        activeEnemyCount = enemyCountToSpawn;

        StartCoroutine(SpawnRoutine(enemyCountToSpawn));
    }

    private IEnumerator SpawnRoutine(int totalEnemies)
    {
        for (int i = 0; i < totalEnemies; i++)
        {
            if (spawnPoints.Length > 0 && enemyPrefab != null)
            {
                Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    public void OnEnemyDefeated()
    {
        activeEnemyCount--;
        if (activeEnemyCount <= 0)
        {
            gameManager.EndWave();
        }
    }
}