using UnityEngine;

public class StructurePlacementManager : MonoBehaviour
{
    [Header("References")]
    public GameManager gameManager;
    public LayerMask groundLayer; // 地面（Ground）のレイヤー

    [Header("Prefabs & Costs")]
    public GameObject wallPrefab;
    public int wallCost = 30;

    public GameObject spikePrefab;
    public int spikeCost = 20;

    // 現在選択中の設置タイプ
    private enum PlacementType { None, Wall, Spike }
    private PlacementType currentType = PlacementType.None;

    private GameObject previewObject; // 設置プレビュー表示用
    private float currentYRotation = 0f; // 💡 追加: 現在のY軸回転角度（0度/90度など）

    void Update()
    {
        // 1キーで壁、2キーでスパイクの設置モード切り替え
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SelectStructure(PlacementType.Wall);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SelectStructure(PlacementType.Spike);
        }

        // 右クリックまたはESCでキャンセル
        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
        {
            CancelPlacement();
        }

        // 設置モード中の処理
        if (currentType != PlacementType.None)
        {
            // 💡 追加: Rキーで90度回転（縦横切替）
            if (Input.GetKeyDown(KeyCode.R))
            {
                currentYRotation += 90f;
                if (currentYRotation >= 360f) currentYRotation = 0f;
            }

            // 💡 追加: マウスホイール上下でも回転可能
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll > 0f) currentYRotation += 90f;
            else if (scroll < 0f) currentYRotation -= 90f;

            // プレビューの位置と回転を更新
            UpdatePlacementPreview();

            // 左クリックで設置
            if (Input.GetMouseButtonDown(0))
            {
                TryPlaceStructure();
            }
        }
    }

    private void SelectStructure(PlacementType type)
    {
        currentType = type;
        currentYRotation = 0f; // 選択時に角度をリセット
        if (previewObject != null) Destroy(previewObject);

        GameObject prefabToSpawn = (type == PlacementType.Wall) ? wallPrefab : spikePrefab;
        if (prefabToSpawn != null)
        {
            previewObject = Instantiate(prefabToSpawn);

            // プレビュー用にColliderを一時的に無効化（レイキャストの邪魔にならないようにする）
            Collider[] colliders = previewObject.GetComponentsInChildren<Collider>();
            foreach (var col in colliders)
            {
                col.enabled = false;
            }
        }
    }

    private void UpdatePlacementPreview()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundLayer))
        {
            if (previewObject != null)
            {
                previewObject.transform.position = hit.point;
                // 💡 回転角度（currentYRotation）をプレビューに反映
                previewObject.transform.rotation = Quaternion.Euler(0f, currentYRotation, 0f);
            }
        }
    }

    private void TryPlaceStructure()
    {
        int cost = (currentType == PlacementType.Wall) ? wallCost : spikeCost;

        if (gameManager.playerGold < cost)
        {
            Debug.Log("ゴールドが足りません！");
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundLayer))
        {
            // ゴールド消費
            gameManager.playerGold -= cost;

            // 実体を回転値を引き継いで生成
            GameObject prefabToSpawn = (currentType == PlacementType.Wall) ? wallPrefab : spikePrefab;
            Quaternion spawnRotation = Quaternion.Euler(0f, currentYRotation, 0f); // 💡 回転を適用

            Instantiate(prefabToSpawn, hit.point, spawnRotation);
        }
    }

    private void CancelPlacement()
    {
        currentType = PlacementType.None;
        currentYRotation = 0f;
        if (previewObject != null)
        {
            Destroy(previewObject);
        }
    }
}