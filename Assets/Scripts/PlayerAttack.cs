using System.Collections.Generic;
using UnityEngine;

public class OrbitWeaponManager : MonoBehaviour
{
    [Header("References")]
    public GameManager gameManager;

    [Header("Bullet Settings")]
    public GameObject bulletPrefab;     // 弾のプレハブ
    public int bulletCount = 2;         // 弾の数を 2 個に変更
    public int bulletCost = 10;         // 発動に必要なゴールド
    public float orbitDistance = 2.5f;  // 回転半径
    public float orbitSpeed = 360.0f;   // 回転速度

    void Update()
    {
        if (gameManager == null) return;

        // WavePhase かつ PlayerView 視点の時のみ攻撃可能
        if (gameManager.currentGameState == GameManager.GameState.WavePhase &&
            gameManager.currentViewMode == GameManager.ViewMode.PlayerView)
        {
            if (Input.GetMouseButtonDown(0))
            {
                TryShootOrbitBullet();
            }
        }
    }

    public void TryShootOrbitBullet()
    {
        if (gameManager.playerGold < bulletCost)
        {
            Debug.Log("ゴールドが足りません！");
            return;
        }

        gameManager.playerGold -= bulletCost;

        if (bulletPrefab != null)
        {
            // 均等な角度間隔を計算（2発なら 180度 間隔）
            float angleStep = 360f / bulletCount;

            for (int i = 0; i < bulletCount; i++)
            {
                float initialAngle = i * angleStep;

                GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

                Bullet orbitComp = bullet.GetComponent<Bullet>();
                if (orbitComp == null)
                {
                    orbitComp = bullet.AddComponent<Bullet>();
                }

                orbitComp.centerPoint = transform;
                orbitComp.orbitDistance = orbitDistance;
                orbitComp.orbitSpeed = orbitSpeed;
                orbitComp.initialAngle = initialAngle; // 初期角度を渡す
            }
        }
    }
}