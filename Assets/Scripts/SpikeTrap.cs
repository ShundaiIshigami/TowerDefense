using System.Collections.Generic;
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    public int damage = 1;
    public float damageInterval = 0.5f; // ダメージ間隔（秒）

    // 敵ごとのタイマー管理
    private Dictionary<Enemy, float> enemyTimers = new Dictionary<Enemy, float>();

    private void OnTriggerStay(Collider other)
    {
        // 敵タグのチェック
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();

            if (enemy == null)
            {
                enemy = other.GetComponentInParent<Enemy>();
            }

            if (enemy != null)
            {
                if (!enemyTimers.ContainsKey(enemy))
                {
                    enemyTimers[enemy] = 0f;
                }

                enemyTimers[enemy] += Time.deltaTime;

                if (enemyTimers[enemy] >= damageInterval)
                {
                    enemy.TakeDamage(damage);
                    enemyTimers[enemy] = 0f; // タイマーリセット
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>() ?? other.GetComponentInParent<Enemy>();
            if (enemy != null && enemyTimers.ContainsKey(enemy))
            {
                enemyTimers.Remove(enemy);
            }
        }
    }
}