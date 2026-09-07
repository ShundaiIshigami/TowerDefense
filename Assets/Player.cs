using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Target & Orbit Settings")]
    public Transform targetTower;      // 中心となるタワー
    public float orbitDistance = 3.0f;  // タワーからの距離（半径）
    public float rotateSpeed = 50.0f;   // 回転速度（度/s）

    [Header("Height Settings")]
    public float playerHeight = -1.5f;   // 地面からの高さ（Y軸のオフセット）

    private float currentAngle = 0f;    // 現在の角度

    void Start()
    {
        if (targetTower == null)
        {
            Debug.LogWarning("Target Tower が設定されていません。");
            return;
        }

        // 初期位置から現在の角度を計算
        Vector3 offset = transform.position - targetTower.position;
        currentAngle = Mathf.Atan2(offset.z, offset.x);

        UpdatePositionAndRotation();
    }

    void Update()
    {
        if (targetTower == null) return;

        // A/D キー または 矢印キー左右で周回移動
        float input = Input.GetAxis("Horizontal");

        if (Mathf.Abs(input) > 0.01f)
        {
            // 入力に応じて角度を更新
            currentAngle -= input * rotateSpeed * Mathf.Deg2Rad * Time.deltaTime;

            // 位置と向き（外側固定）を更新
            UpdatePositionAndRotation();
        }
    }

    // 位置と向き（常に外側）の計算
    private void UpdatePositionAndRotation()
    {
        // 位置の更新（円運動）
        float x = targetTower.position.x + Mathf.Cos(currentAngle) * orbitDistance;
        float z = targetTower.position.z + Mathf.Sin(currentAngle) * orbitDistance;
        float y = targetTower.position.y + playerHeight;

        transform.position = new Vector3(x, y, z);

        // 向きの更新（タワーと逆の外側を向く）
        Vector3 lookAwayPoint = transform.position + (transform.position - targetTower.position);
        lookAwayPoint.y = transform.position.y; // 高さを固定して水平に外を向く

        transform.LookAt(lookAwayPoint);
    }

    private void OnDrawGizmosSelected()
    {
        if (targetTower != null)
        {
            Gizmos.color = Color.green;
            Vector3 center = new Vector3(targetTower.position.x, targetTower.position.y + playerHeight, targetTower.position.z);
            Gizmos.DrawWireSphere(center, orbitDistance);
        }
    }
}