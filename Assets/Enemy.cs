using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Enemy : MonoBehaviour
{
    [Header("Enemy Stats")]
    public int health = 3;
    public int goldValue = 20;
    public int attackDamage = 1;

    [Header("Hit Effects")]
    public float invincibilityTime = 0.2f; // ダメージ直後の無敵時間（多重ヒット防止）
    private float lastDamageTime = -999f;

    private NavMeshAgent agent;
    private GameManager gameManager;
    private WaveManager waveManager;
    private Transform targetTower;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        gameManager = FindFirstObjectByType<GameManager>();
        waveManager = FindFirstObjectByType<WaveManager>();

        // Tagの設定確認推奨（Inspectorで "Enemy" タグを設定してください）
        if (!gameObject.CompareTag("Enemy"))
        {
            gameObject.tag = "Enemy";
        }

        // Rigidbody の設定（Trigger判定に必要なため）
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; // ナビメッシュ移動の妨げにならないようKinematicにする

        // ターゲット設定
        Player player = FindFirstObjectByType<Player>();
        if (player != null && player.targetTower != null)
        {
            targetTower = player.targetTower;
            agent.SetDestination(targetTower.position);
        }
    }

    void Update()
    {
        if (targetTower == null) return;

        // タワー到達判定
        float distanceToTarget = Vector3.Distance(transform.position, targetTower.position);
        if (distanceToTarget <= agent.stoppingDistance + 1.0f)
        {
            ReachBase();
        }
    }

    // 弾から呼び出されるダメージ処理
    public void TakeDamage(int damage)
    {
        // 連続ヒット防止の無敵時間チェック
        if (Time.time < lastDamageTime + invincibilityTime) return;
        lastDamageTime = Time.time;

        health -= damage;
        Debug.Log($"敵がダメージを受けました！ 残りHP: {health}");

        if (health <= 0)
        {
            Die();
        }
    }

    private void ReachBase()
    {
        if (gameManager != null)
        {
            gameManager.baseHealth -= attackDamage;
            if (gameManager.baseHealth <= 0) gameManager.baseHealth = 0;
        }
        OnDestroyEnemy();
    }

    private void Die()
    {
        if (gameManager != null)
        {
            gameManager.playerGold += goldValue;
        }
        OnDestroyEnemy();
    }

    private void OnDestroyEnemy()
    {
        if (waveManager != null)
        {
            waveManager.OnEnemyDefeated();
        }
        Destroy(gameObject);
    }
}