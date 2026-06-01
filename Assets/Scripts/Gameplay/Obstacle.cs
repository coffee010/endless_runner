using UnityEngine;

public enum ObstacleType
{
    LaneBlock,   // 静态阻挡 — 占一条跑道，玩家需换道
    Low,         // 低位障碍 — 需跳跃越过
    High,        // 高位障碍 — 需下滑通过
    Moving,      // 动态移动 — 在跑道间水平摆动
    Rotating,    // 旋转刀片 — 绕轴旋转的危险物
    Slow,        // 减速力场 — 踩中降低速度
    Knockback,   // 击退板 — 将玩家向后弹开
}

public enum ObstacleResponse
{
    Kill,        // 直接死亡
    Slow,        // 减速（临时降低速度）
    Knockback,   // 击退 + 扣分
}

public sealed class Obstacle : MonoBehaviour, IPoolable
{
    // ──────────────────── 类型配置 ────────────────────

    [Header("Type")]
    [SerializeField] private ObstacleType obstacleType = ObstacleType.LaneBlock;
    [SerializeField] private ObstacleResponse response = ObstacleResponse.Kill;

    // ──────────────────── 行为参数 ────────────────────

    [Header("Response")]
    [SerializeField] private float slowMultiplier = 0.35f;
    [SerializeField] private float slowDuration = 1.5f;
    [SerializeField] private float knockbackForce = 14f;
    [SerializeField] private float knockbackUpward = 3f;
    [SerializeField] private int penaltyScore = 50;

    // ──────────────────── 动态障碍物配置 ────────────────────

    [Header("Movement (Moving / Rotating)")]
    [SerializeField] private bool animateMovement = true;
    [SerializeField] private float moveRange = 2.5f;
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private float movePhaseOffset;
    [SerializeField] private Vector3 rotationAxis = Vector3.up;
    [SerializeField] private float rotationSpeed = 180f;

    // ──────────────────── VFX ────────────────────

    [Header("VFX")]
    [SerializeField] private GameObject collisionVfxPrefab;
    [SerializeField] private GameObject idleVfxPrefab;
    [SerializeField] private float collisionVfxScale = 1.9f;

    private Vector3 startPosition;
    private float phase;
    private bool hasIdleVfx;

    // ──────────────────── 属性 ────────────────────

    public ObstacleType Type => obstacleType;
    public ObstacleResponse Response => response;
    public float SlowMultiplier => slowMultiplier;
    public float SlowDuration => slowDuration;
    public float KnockbackForce => knockbackForce;
    public float KnockbackUpward => knockbackUpward;
    public int PenaltyScore => penaltyScore;

    /// <summary>是否已被触发（防止同一障碍重复生效）</summary>
    public bool IsTriggered { get; private set; }

    /// <summary>
    /// 运行时配置障碍物（无需依赖 Editor 命名空间）。
    /// 在 AddComponent 之后、OnEnable 之前调用。
    /// </summary>
    public void RuntimeConfigure(ObstacleType type, ObstacleResponse resp)
    {
        obstacleType = type;
        response = resp;
        animateMovement = type is ObstacleType.Moving or ObstacleType.Rotating;
    }

    private void Awake()
    {
        startPosition = transform.localPosition;
        phase = movePhaseOffset;

        if (idleVfxPrefab != null)
        {
            Instantiate(idleVfxPrefab, transform);
            hasIdleVfx = true;
        }
    }

    private void Update()
    {
        if (!animateMovement)
        {
            return;
        }

        ApplyMovement();
    }

    // ──────────────────── 动态移动 ────────────────────

    private void ApplyMovement()
    {
        switch (obstacleType)
        {
            case ObstacleType.Moving:
            {
                phase += Time.deltaTime * moveSpeed;
                float xOffset = Mathf.Sin(phase) * moveRange;
                transform.localPosition = new Vector3(startPosition.x + xOffset, startPosition.y, startPosition.z);
                break;
            }
            case ObstacleType.Rotating:
            {
                transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime, Space.Self);
                break;
            }
        }
    }

    // ──────────────────── 碰撞判断 ────────────────────

    /// <summary>
    /// 判断玩家当前状态是否可以安全通过此障碍。
    /// LaneBlock 需换道绕过，Low 需跳跃，High 需下滑。
    /// Moving/Rotating/Slow/Knockback 类型不可通过操作规避。
    /// </summary>
    public bool CanBeIgnoredBy(RunnerController runner)
    {
        if (runner == null)
        {
            return false;
        }

        switch (obstacleType)
        {
            case ObstacleType.Low:
                // 玩家在空中 → 跳过低位障碍
                return !runner.IsGrounded;

            case ObstacleType.High:
                // 玩家在滑行 → 通过高位障碍
                return runner.IsSliding;

            case ObstacleType.LaneBlock:
                // 站桩阻挡，必须换道
                return false;

            default:
                // Moving / Rotating / Slow / Knockback 不可忽略
                return false;
        }
    }

    // ──────────────────── 碰撞反馈 ────────────────────

    /// <summary>
    /// 播放碰撞 VFX 并返回碰撞响应类型，供 PlayerCollision 处理。
    /// </summary>
    public ObstacleResponse OnCollide()
    {
        if (IsTriggered)
        {
            return response;
        }

        IsTriggered = true;

        if (collisionVfxPrefab != null)
        {
            VfxUtility.Spawn(collisionVfxPrefab, transform.position, Quaternion.identity, collisionVfxScale);
        }

        return response;
    }

    // ──────────────────── IPoolable ────────────────────

    public void OnSpawn()
    {
        // 重置触发标记和动态状态
        IsTriggered = false;
        startPosition = transform.localPosition;
        phase = movePhaseOffset;
        animateMovement = obstacleType is ObstacleType.Moving or ObstacleType.Rotating;

        // 重新生成待机 VFX
        if (idleVfxPrefab != null && !hasIdleVfx)
        {
            Instantiate(idleVfxPrefab, transform);
            hasIdleVfx = true;
        }
    }

    public void OnDespawn()
    {
        // 清理 VFX 子对象
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        hasIdleVfx = false;
    }
}
