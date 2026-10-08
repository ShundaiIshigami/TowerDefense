using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Orbit Settings")]
    public Transform centerPoint;      // 回転の中心
    public float orbitDistance = 2.5f;  // 回転半径
    public float orbitSpeed = 360.0f;   // 回転速度（度/秒）

    [Header("Attack Settings")]
    public int damage = 1;              // 敵に与えるダメージ

    private float accumulatedAngle = 0f; // 回転した合計角度

    void Start()
    {
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
        accumulatedAngle += deltaAngle;

        // 円運動の位置計算（プレイヤーの前方からスタート）
        float rad = accumulatedAngle * Mathf.Deg2Rad;
        float x = centerPoint.position.x + Mathf.Cos(rad) * orbitDistance;
        float z = centerPoint.position.z + Mathf.Sin(rad) * orbitDistance;
        float y = centerPoint.position.y + 0.5f; // 高さのオフセット

        transform.position = new Vector3(x, y, z);

        // 1周（360度）回転したら消滅
        if (accumulatedAngle >= 360f)
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