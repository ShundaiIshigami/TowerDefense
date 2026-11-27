using UnityEngine;

public class Wall : MonoBehaviour
{
    [Header("Wall Stats")]
    public int maxHealth = 5;
    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;

        // 敵が壁を認識できるようにタグを設定
        if (!gameObject.CompareTag("Wall"))
        {
            gameObject.tag = "Wall";
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"壁がダメージを受けました！ 残りHP: {currentHealth}");

        if (currentHealth <= 0)
        {
            DestroyWall();
        }
    }

    private void DestroyWall()
    {
        // 破壊エフェクトや効果音を入れる場合はここに記述
        Destroy(gameObject);
    }
}