using UnityEngine;

public enum CollectibleType
{
    Score,
    Energy
}

public sealed class Collectible : MonoBehaviour, IPoolable
{
    [SerializeField] private CollectibleType type = CollectibleType.Energy;
    [SerializeField] private int scoreValue = 50;
    [SerializeField] private float energyValue = 5f;
    [SerializeField] private GameObject collectVfxPrefab;
    [SerializeField] private float collectVfxScale = 1.8f;

    private bool collected;
    private EnergyBurst cachedBurst;

    /// <summary>收集物的能量值（只读，供 TrackSpawner 反射写入）</summary>
    public float EnergyValue => energyValue;

    private void Awake()
    {
        // 缓存引用，避免每次收集时 FindObjectOfType
        cachedBurst = FindAnyObjectByType<EnergyBurst>();

        // 确保有触发器碰撞体，供 PlayerCollision.OnTriggerEnter 检测
        BoxCollider col = GetComponent<BoxCollider>();
        if (col == null)
        {
            col = gameObject.AddComponent<BoxCollider>();
        }

        col.isTrigger = true;
        col.center = new Vector3(0f, 0.7f, 0f);
        col.size = new Vector3(0.7f, 1.5f, 0.7f);
    }

    public void Collect(ScoreManager scoreManager)
    {
        if (collected)
        {
            return;
        }

        collected = true;

        if (type == CollectibleType.Score && scoreManager != null)
        {
            scoreManager.AddBonus(scoreValue);
        }

        if (type == CollectibleType.Energy && cachedBurst != null)
        {
            cachedBurst.AddEnergy(energyValue);
        }

        if (collectVfxPrefab != null)
        {
            VfxUtility.Spawn(collectVfxPrefab, transform.position, Quaternion.identity, collectVfxScale);
        }

        // 隐藏 → 对象池回收（在外部调用 pool.Release）
        gameObject.SetActive(false);
    }

    // ───────────────────── IPoolable ─────────────────────

    public void OnSpawn()
    {
        collected = false;

        // 再次尝试缓存（防止首次 Awake 时 EnergyBurst 尚未初始化）
        if (cachedBurst == null)
        {
            cachedBurst = FindAnyObjectByType<EnergyBurst>();
        }
    }

    public void OnDespawn()
    {
        // 无需额外清理
    }
}
