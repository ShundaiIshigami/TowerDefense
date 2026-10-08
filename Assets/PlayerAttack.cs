using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("References")]
    public GameManager gameManager;

    [Header("Bullet Settings")]
    public GameObject bulletPrefab;     // 弾のプレハブ
    public int bulletCost = 10;         // 弾1発のゴールドコスト
    public float orbitDistance = 2.5f;  // 回転半径
    public float orbitSpeed = 360.0f;   // 回転速度（360なら1秒で1周）

    void Update()
    {
        if (gameManager == null) return;

        // WavePhase かつ PlayerView 視点の時のみ攻撃可能
        if (gameManager.currentGameState == GameManager.GameState.WavePhase &&
            gameManager.currentViewMode == GameManager.ViewMode.PlayerView)
        {
            // マウス左クリック（または E キーなど）で発動
            if (Input.GetMouseButtonDown(0))
            {
                TryShootOrbitBullet();
            }
        }
    }

    public void TryShootOrbitBullet()
    {
        // ゴールドが足りない場合は発動不可
        if (gameManager.playerGold < bulletCost)
        {
            Debug.Log("ゴールドが足りません！");
            return;
        }

        // ゴールドを消費
        gameManager.playerGold -= bulletCost;

        // 弾を生成
        if (bulletPrefab != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

            Bullet orbitComp = bullet.GetComponent<Bullet>();
            if (orbitComp == null)
            {
                orbitComp = bullet.AddComponent<Bullet>();
            }

            orbitComp.centerPoint = transform; // プレイヤーを中心に回転
            orbitComp.orbitDistance = orbitDistance;
            orbitComp.orbitSpeed = orbitSpeed;
        }
    }
}