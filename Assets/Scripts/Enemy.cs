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

    [Header("Wall Attack Settings")]
    public float wallAttackRange = 1.5f;    // 壁を攻撃できる距離
    public float wallAttackInterval = 1.0f; // 壁を攻撃する間隔（秒）
    private float wallAttackTimer = 0f;

    [Header("Hit Effects")]
    public float invincibilityTime = 0.2f;
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

        if (!gameObject.CompareTag("Enemy"))
        {
            gameObject.tag = "Enemy";
        }

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;

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

        // 周囲に壁があるかチェックして攻撃
        CheckAndAttackWall();

        // タワー到達判定
        float distanceToTarget = Vector3.Distance(transform.position, targetTower.position);
        if (distanceToTarget <= agent.stoppingDistance + 1.0f)
        {
            ReachBase();
        }
    }

    // 近くの壁を検知して攻撃する処理
    private void CheckAndAttackWall()
    {
        // 敵の周囲（wallAttackRange以内）にあるコライダーを取得
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, wallAttackRange);
        Wall nearestWall = null;
        float minDistance = Mathf.Infinity;

        foreach (var hit in hitColliders)
        {
            if (hit.CompareTag("Wall"))
            {
                float dist = Vector3.Distance(transform.position, hit.transform.position);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    nearestWall = hit.GetComponent<Wall>() ?? hit.GetComponentInParent<Wall>();
                }
            }
        }

        if (nearestWall != null)
        {
            wallAttackTimer += Time.deltaTime;
            if (wallAttackTimer >= wallAttackInterval)
            {
                nearestWall.TakeDamage(attackDamage);
                wallAttackTimer = 0f;
            }
        }
        else
        {
            wallAttackTimer = 0f;
        }
    }

    public void TakeDamage(int damage)
    {
        if (Time.time < lastDamageTime + invincibilityTime) return;
        lastDamageTime = Time.time;

        health -= damage;
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