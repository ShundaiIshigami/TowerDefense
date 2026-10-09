using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Orbit Settings")]
    public Transform centerPoint;      // 回転の中心
    public float orbitDistance = 2.5f;  // 回転半径
    public float orbitSpeed = 360.0f;   // 回転速度（度/秒）
    public float initialAngle = 0f;     // 初期角度

    [Header("Attack Settings")]
    public int damage = 1;              // 敵に与えるダメージ

    private float currentAngle = 0f;    // 現在の位置の角度
    private float rotatedTotalAngle = 0f; // 回転した「合計」角度

    void Start()
    {
        currentAngle = initialAngle;
        rotatedTotalAngle = 0f;

        if (centerPoint == null && transform.parent != null)
        {
            centerPoint = transform.parent;
        }
    }

    void Update()
    {
        if (centerPoint == null) return;

        // フレームごとの移動角度
        float deltaAngle = orbitSpeed * Time.deltaTime;

        currentAngle += deltaAngle;
        rotatedTotalAngle += deltaAngle; // 実際に回転した角度を加算

        // 円運動の位置計算
        float rad = currentAngle * Mathf.Deg2Rad;
        float x = centerPoint.position.x + Mathf.Cos(rad) * orbitDistance;
        float z = centerPoint.position.z + Mathf.Sin(rad) * orbitDistance;
        float y = centerPoint.position.y + 0.5f; // 高さのオフセット

        transform.position = new Vector3(x, y, z);

        if (rotatedTotalAngle >= 360f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 敵に当たった時のダメージ処理
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
    }
}