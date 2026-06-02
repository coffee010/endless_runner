using UnityEngine;

public sealed class ColorGate : MonoBehaviour, IPoolable
{
    [SerializeField] private EnergyMode requiredMode = EnergyMode.Blue;
    [SerializeField] private GameObject passVfxPrefab;
    [SerializeField] private GameObject failVfxPrefab;
    [SerializeField] private bool deactivateOnPass = true;
    [SerializeField] private bool deactivateOnFail = true;
    [SerializeField] private float correctEnergyReward = 100f;
    [SerializeField] private float wrongEnergyPenalty = 10f;
    [SerializeField] private float vfxScale = 2.1f;

    public EnergyMode RequiredMode => requiredMode;
    public Color RequiredColor => GetModeColor(requiredMode);
    public float CorrectEnergyReward => correctEnergyReward;
    public float WrongEnergyPenalty => wrongEnergyPenalty;

    private void Awake()
    {
        // 确保有触发碰撞体，玩家穿过门时触发 OnTriggerEnter
        BoxCollider col = GetComponent<BoxCollider>();
        if (col == null)
        {
            col = gameObject.AddComponent<BoxCollider>();
        }

        col.isTrigger = true;
        col.center = new Vector3(0f, 1.35f, 0f);
        col.size = new Vector3(1.8f, 2.7f, 0.3f);
    }

    public bool TryPass(EnergyMode currentMode)
    {
        bool passed = currentMode == requiredMode;

        if (passed)
        {
            SpawnVfx(passVfxPrefab);

            if (deactivateOnPass)
            {
                gameObject.SetActive(false);
            }
        }
        else
        {
            SpawnVfx(failVfxPrefab);

            if (deactivateOnFail)
            {
                gameObject.SetActive(false);
            }
        }

        return passed;
    }

    public void SetRequiredMode(EnergyMode mode)
    {
        requiredMode = mode;
    }

    public static Color GetModeColor(EnergyMode mode)
    {
        if (mode == EnergyMode.Red)
        {
            return new Color(1f, 0.12f, 0.08f);
        }

        return new Color(0.1f, 0.45f, 1f);
    }

    private void SpawnVfx(GameObject prefab)
    {
        if (prefab != null)
        {
            VfxUtility.SpawnTinted(prefab, transform.position, Quaternion.identity, vfxScale, RequiredColor);
        }
    }

    // ───────────────────── IPoolable ─────────────────────

    public void OnSpawn()
    {
        // 对象池取出时无需额外重置。
        // requiredMode 由 TrackSegment.ApplyColorTheme 在取出后设置，
        // 可见性也由 TrackSegment 控制。
    }

    public void OnDespawn()
    {
        // 放回池时无需额外清理。
    }
}
