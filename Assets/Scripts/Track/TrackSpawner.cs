using System.Collections.Generic;
using UnityEngine;

public sealed class TrackSpawner : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private List<TrackSegment> segmentPrefabs = new List<TrackSegment>();
    [SerializeField] private int initialSegments = 5;
    [SerializeField] private int keepSegmentsBehind = 2;
    [SerializeField] private float defaultSegmentLength = 30f;
    [SerializeField] private int redSegmentEvery = 3;
    [SerializeField] private int redSegmentOffset = 2;

    [Header("Pool")]
    [SerializeField] private int poolCapacityPerPrefab = 4;

    [Header("Obstacle Spawning")]
    [SerializeField] private bool spawnObstacles = true;
    [SerializeField] private int safeSegments = 2;
    [SerializeField] private int obstaclesPerSegment = 2;
    [SerializeField] private float obstacleSpacingMin = 6f;
    [SerializeField] private float obstacleSpacingMax = 12f;
    [SerializeField] private float sameLaneObstacleSpacingMin = 7f;
    [SerializeField] private bool useAllObstacleTypes = true;
    [SerializeField] private ObstacleType[] allowedObstacleTypes =
    {
        ObstacleType.LaneBlock,
        ObstacleType.Low,
        ObstacleType.High,
    };
    [SerializeField] private float obstacleYOffset = 0.05f;

    [Header("Color Gate Spawning")]
    [SerializeField] private bool spawnColorGates = true;
    [SerializeField] private float colorGateChance = 0.4f;
    [SerializeField] private float colorGateZOffsetMin = 8f;
    [SerializeField] private float colorGateZOffsetMax = 20f;

    [Header("VFX")]
    [SerializeField] private GameObject obstacleCollisionVfxPrefab;
    [SerializeField] private GameObject gatePassVfxPrefab;
    [SerializeField] private GameObject gateFailVfxPrefab;
    [SerializeField] private float obstacleCollisionVfxScale = 1.9f;
    [SerializeField] private float gateVfxScale = 2.1f;

    [Header("Collectible Spawning")]
    [SerializeField] private bool spawnCollectibles = true;
    [SerializeField] private int collectibleGroupSize = 6;
    [SerializeField] private float collectibleSpacing = 2.5f;
    [SerializeField, Range(0f, 1f)] private float collectibleChance = 0.5f;
    [SerializeField] private float collectibleGroupMinZ = 8f;
    [SerializeField] private float collectibleGroupMaxZ = 20f;
    [SerializeField] private float collectibleYOffset = 1.5f;
    [SerializeField] private float collectibleObstacleMinDistance = 3.5f;
    [SerializeField] private GameObject collectVfxPrefab;
    [SerializeField] private float collectVfxScale = 1.8f;

    private readonly Queue<TrackSegment> activeSegments = new Queue<TrackSegment>();
    private readonly Dictionary<TrackSegment, SimpleObjectPool<TrackSegment>> pools = new Dictionary<TrackSegment, SimpleObjectPool<TrackSegment>>();
    private float nextSpawnZ;
    private int spawnedSegmentCount;

    private void Start()
    {
        InitializePools();
        for (int i = 0; i < initialSegments; i++)
        {
            SpawnNext();
        }
    }

    private void Update()
    {
        if (player == null)
        {
            return;
        }

        while (player.position.z + defaultSegmentLength * initialSegments > nextSpawnZ)
        {
            SpawnNext();
        }

        RecycleBehindPlayer();
    }

    // ───────────────────── 对象池 ─────────────────────

    private void InitializePools()
    {
        foreach (TrackSegment prefab in segmentPrefabs)
        {
            if (prefab == null || pools.ContainsKey(prefab))
            {
                continue;
            }

            SimpleObjectPool<TrackSegment> pool = new SimpleObjectPool<TrackSegment>(
                prefab,
                poolCapacityPerPrefab,
                transform
            );
            pools.Add(prefab, pool);
        }
    }

    private SimpleObjectPool<TrackSegment> GetOrCreatePool(TrackSegment prefab)
    {
        if (!pools.TryGetValue(prefab, out SimpleObjectPool<TrackSegment> pool))
        {
            pool = new SimpleObjectPool<TrackSegment>(prefab, poolCapacityPerPrefab, transform);
            pools.Add(prefab, pool);
        }

        return pool;
    }

    // ───────────────────── 生成 / 回收 ─────────────────────

    private void SpawnNext()
    {
        if (segmentPrefabs.Count == 0)
        {
            return;
        }

        TrackSegment prefab = segmentPrefabs[Random.Range(0, segmentPrefabs.Count)];
        SimpleObjectPool<TrackSegment> pool = GetOrCreatePool(prefab);

        TrackSegment segment = pool.Get(new Vector3(0f, 0f, nextSpawnZ), Quaternion.identity);
        segment.SourcePrefab = prefab;
        EnergyMode theme = GetSegmentTheme(spawnedSegmentCount);
        segment.ApplyColorTheme(theme);

        // 前 safeSegments 段不生成障碍物，给玩家一个干净的起跑区
        if (spawnObstacles && spawnedSegmentCount >= safeSegments)
        {
            SpawnObstaclesOnSegment(segment, theme);
        }

        // 颜色门
        if (spawnColorGates && spawnedSegmentCount >= safeSegments)
        {
            TrySpawnColorGate(segment, theme);
        }

        // 收集物（在障碍物和颜色门之后生成，利用已生成的子对象做防重叠检测）
        if (spawnCollectibles && spawnedSegmentCount >= safeSegments)
        {
            TrySpawnCollectibleGroup(segment);
        }

        activeSegments.Enqueue(segment);
        spawnedSegmentCount++;
        nextSpawnZ += segment.Length > 0f ? segment.Length : defaultSegmentLength;
    }

    private EnergyMode GetSegmentTheme(int segmentIndex)
    {
        int interval = Mathf.Max(1, redSegmentEvery);
        int offset = Mathf.Clamp(redSegmentOffset, 0, interval - 1);
        return segmentIndex % interval == offset ? EnergyMode.Red : EnergyMode.Blue;
    }

    private void RecycleBehindPlayer()
    {
        while (activeSegments.Count > keepSegmentsBehind + initialSegments)
        {
            TrackSegment oldest = activeSegments.Peek();
            if (player.position.z - oldest.EndPosition.z < defaultSegmentLength)
            {
                break;
            }

            TrackSegment segment = activeSegments.Dequeue();

            // 清理路段上的障碍物再放回池
            DestroySegmentObstacles(segment);

            TrackSegment sourcePrefab = segment.SourcePrefab ?? segment;
            if (pools.TryGetValue(sourcePrefab, out SimpleObjectPool<TrackSegment> pool))
            {
                pool.Release(segment);
            }
            else
            {
                Destroy(segment.gameObject);
            }
        }
    }

    // ───────────────────── 障碍物生成 ─────────────────────

    private void SpawnObstaclesOnSegment(TrackSegment segment, EnergyMode theme)
    {
        float segmentLength = segment.Length > 0f ? segment.Length : defaultSegmentLength;
        float z = obstacleSpacingMin;
        float[] lastLaneZ = { float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity };
        int spawned = 0;
        int attempts = 0;

        while (z < segmentLength - 3f && spawned < obstaclesPerSegment && attempts < 30)
        {
            attempts++;

            // 随机间距
            z += Random.Range(obstacleSpacingMin, obstacleSpacingMax);

            if (z >= segmentLength - 3f)
            {
                break;
            }

            ObstacleType type = PickObstacleType();
            int lane = PickObstacleLane(lastLaneZ, z);
            if (lane < 0)
            {
                continue;
            }

            CreateObstacle(segment.transform, type, lane, z);
            lastLaneZ[lane] = z;
            spawned++;
        }
    }

    private int PickObstacleLane(float[] lastLaneZ, float z)
    {
        int startLane = Random.Range(0, 3);
        for (int i = 0; i < 3; i++)
        {
            int lane = (startLane + i) % 3;
            if (z - lastLaneZ[lane] >= sameLaneObstacleSpacingMin)
            {
                return lane;
            }
        }

        return -1;
    }

    private ObstacleType PickObstacleType()
    {
        if (useAllObstacleTypes)
        {
            return (ObstacleType)Random.Range(0, System.Enum.GetValues(typeof(ObstacleType)).Length);
        }

        if (allowedObstacleTypes == null || allowedObstacleTypes.Length == 0)
        {
            return ObstacleType.LaneBlock;
        }

        return allowedObstacleTypes[Random.Range(0, allowedObstacleTypes.Length)];
    }

    /// <summary>
    /// 从零创建障碍物 GameObject（无需预制体）。
    /// ObstacleVisual 会自动生成对应的霓虹造型。
    /// </summary>
    private void CreateObstacle(Transform parent, ObstacleType type, int lane, float localZ)
    {
        GameObject obj = new GameObject($"Obstacle_{type}");
        obj.transform.SetParent(parent, false);

        // 三跑道：lane 0 = 左 (-2.5), lane 1 = 中 (0), lane 2 = 右 (+2.5)
        float x = (lane - 1) * 2.5f;
        obj.transform.localPosition = new Vector3(x, obstacleYOffset, localZ);

        // 1. 先加 Obstacle
        Obstacle obstacle = obj.AddComponent<Obstacle>();
        if (obstacle == null)
        {
            Debug.LogError($"[TrackSpawner] AddComponent<Obstacle>() 返回 null！type={type}");
            Destroy(obj);
            return;
        }

        // 运行时反射写入字段（必须在 ObstacleVisual.Awake 之前）
        SetPrivateField(obstacle, "obstacleType", (int)type);
        SetPrivateField(obstacle, "response", (int)GetResponseForType(type));
        SetPrivateField(obstacle, "animateMovement", type is ObstacleType.Moving or ObstacleType.Rotating);
        SetPrivateField(obstacle, "collisionVfxPrefab", obstacleCollisionVfxPrefab);
        SetPrivateField(obstacle, "collisionVfxScale", obstacleCollisionVfxScale);

        // 2. 再加 ObstacleVisual
        ObstacleVisual visual = obj.AddComponent<ObstacleVisual>();
        if (visual == null)
        {
            Debug.LogError($"[TrackSpawner] AddComponent<ObstacleVisual>() 返回 null！type={type}");
        }

        // 3. 碰撞体
        BoxCollider collider = obj.AddComponent<BoxCollider>();
        if (collider != null)
        {
            ConfigureCollider(collider, type);
        }

        Debug.Log($"[TrackSpawner] 创建障碍物: {type} at lane {lane}, z={localZ:F1}");
    }

    private static ObstacleResponse GetResponseForType(ObstacleType type)
    {
        return type switch
        {
            ObstacleType.Slow => ObstacleResponse.Slow,
            ObstacleType.Knockback => ObstacleResponse.Knockback,
            _ => ObstacleResponse.Kill,
        };
    }

    private static void SetPrivateField<T>(object target, string fieldName, T value)
    {
        var field = target.GetType().GetField(fieldName,
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        if (field != null)
        {
            field.SetValue(target, value);
        }
    }

    private static void ConfigureCollider(BoxCollider collider, ObstacleType type)
    {
        collider.isTrigger = type switch
        {
            ObstacleType.Slow => true,
            ObstacleType.Knockback => true,
            _ => false,
        };

        collider.center = type switch
        {
            ObstacleType.High => new Vector3(0f, 2.55f, 0f),
            ObstacleType.Low => new Vector3(0f, 0.7f, 0f),
            _ => new Vector3(0f, 1.2f, 0f),
        };

        collider.size = type switch
        {
            ObstacleType.LaneBlock => new Vector3(0.5f, 2.5f, 0.3f),
            ObstacleType.Low => new Vector3(1.9f, 0.75f, 0.15f),
            ObstacleType.High => new Vector3(1.9f, 0.2f, 0.2f),
            ObstacleType.Moving => new Vector3(0.75f, 0.6f, 0.6f),
            ObstacleType.Rotating => new Vector3(2f, 0.3f, 2f),
            ObstacleType.Slow => new Vector3(2f, 0.1f, 2.5f),
            ObstacleType.Knockback => new Vector3(1.5f, 0.2f, 1f),
            _ => new Vector3(1f, 1f, 1f),
        };
    }

    private static void DestroySegmentObstacles(TrackSegment segment)
    {
        var toDestroy = new List<GameObject>();
        foreach (Transform child in segment.transform)
        {
            if (child.GetComponent<Obstacle>() != null
                || child.GetComponent<ColorGate>() != null
                || child.GetComponent<Collectible>() != null)
            {
                toDestroy.Add(child.gameObject);
            }
        }

        foreach (GameObject obj in toDestroy)
        {
            Destroy(obj);
        }
    }

    // ───────────────────── 颜色门生成 ─────────────────────

    private void TrySpawnColorGate(TrackSegment segment, EnergyMode theme)
    {
        if (Random.value > colorGateChance)
        {
            return;
        }

        float segmentLength = segment.Length > 0f ? segment.Length : defaultSegmentLength;
        float z = Random.Range(colorGateZOffsetMin, Mathf.Min(colorGateZOffsetMax, segmentLength - 3f));

        // 门颜色与路段主题相反才有趣（蓝段出红门，红段出蓝门）
        // 这样玩家必须切换颜色才能通过
        EnergyMode gateColor = theme == EnergyMode.Blue ? EnergyMode.Red : EnergyMode.Blue;

        // 偶尔也出同色门（30% 概率），让玩家轻松一下
        if (Random.value < 0.3f)
        {
            gateColor = theme;
        }

        CreateColorGate(segment.transform, gateColor, z);
    }

    private void CreateColorGate(Transform parent, EnergyMode gateColor, float localZ)
    {
        int lane = Random.Range(0, 3);
        float x = (lane - 1) * 2.5f;

        GameObject obj = new GameObject($"ColorGate_{gateColor}_L{lane}");
        obj.transform.SetParent(parent, false);
        obj.transform.localPosition = new Vector3(x, 0f, localZ);

        // 1. ColorGate 组件
        ColorGate gate = obj.AddComponent<ColorGate>();
        SetPrivateField(gate, "requiredMode", (int)gateColor);
        SetPrivateField(gate, "passVfxPrefab", gatePassVfxPrefab);
        SetPrivateField(gate, "failVfxPrefab", gateFailVfxPrefab);
        SetPrivateField(gate, "vfxScale", gateVfxScale);

        // 2. 视觉（ColorGateVisual 由 Awake 自动创建门框模型）
        // 注：ColorGateVisual 有 [RequireComponent(typeof(ColorGate))]，
        // AddComponent<ColorGateVisual> 应该没问题
        obj.AddComponent<ColorGateVisual>();

        // 3. 触发器碰撞体已在 ColorGate.Awake 中自动创建

        Debug.Log($"[TrackSpawner] 创建颜色门: {gateColor} at z={localZ:F1}");
    }

    // ───────────────────── 收集物生成 ─────────────────────

    /// <summary>
    /// 尝试在当前段上生成一组收集物（默认 6 个连在一起）。
    /// 会避开同跑道已生成的障碍物和颜色门位置。
    /// </summary>
    private void TrySpawnCollectibleGroup(TrackSegment segment)
    {
        // 概率检查
        if (Random.value > collectibleChance)
        {
            return;
        }

        float segmentLength = segment.Length > 0f ? segment.Length : defaultSegmentLength;
        int groupSize = Mathf.Max(1, collectibleGroupSize);
        float spacing = Mathf.Max(0.5f, collectibleSpacing);
        float groupTotalLength = (groupSize - 1) * spacing;

        // 尝试找到合适的起始 Z 位置
        float minStartZ = Mathf.Max(3f, collectibleGroupMinZ);
        float maxStartZ = Mathf.Min(segmentLength - 3f - groupTotalLength, collectibleGroupMaxZ);

        if (maxStartZ <= minStartZ)
        {
            Debug.Log($"[TrackSpawner] 段太短，放不下收集物组 (需要 {groupTotalLength:F1}m, 可用 Z=[{minStartZ:F1}, {maxStartZ:F1}])");
            return;
        }

        // 尝试不同跑道和位置
        int maxAttempts = 20;
        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            int lane = Random.Range(0, 3);
            float startZ = Random.Range(minStartZ, maxStartZ);

            // 只收集同跑道的障碍物/颜色门 Z 坐标
            List<float> sameLaneOccupiedZ = GatherOccupiedZonesInLane(segment, lane);

            if (IsGroupPositionClear(startZ, groupSize, spacing, sameLaneOccupiedZ))
            {
                for (int i = 0; i < groupSize; i++)
                {
                    float z = startZ + i * spacing;
                    CreateCollectible(segment.transform, lane, z);
                }

                Debug.Log($"[TrackSpawner] 创建收集物组: {groupSize} 个, lane {lane}, z={startZ:F1}~{startZ + groupTotalLength:F1}");
                return;
            }
        }

        Debug.Log($"[TrackSpawner] 收集物组 {maxAttempts} 次尝试均未找到不重叠位置，跳过此段");
    }

    /// <summary>
    /// 收集当前段上与目标跑道重叠的障碍物和颜色门 Z 坐标。
    /// 只检查 X 轴距离 ≤ 1.5m 的对象（同跑道或相邻边界）。
    /// </summary>
    private static List<float> GatherOccupiedZonesInLane(TrackSegment segment, int targetLane)
    {
        List<float> occupied = new List<float>();
        float targetX = (targetLane - 1) * 2.5f;
        float laneThreshold = 1.5f; // X 轴距离阈值：只考虑同跑道对象

        foreach (Transform child in segment.transform)
        {
            if (child.GetComponent<Obstacle>() != null || child.GetComponent<ColorGate>() != null)
            {
                // 只收集与目标跑道 X 轴接近的对象
                if (Mathf.Abs(child.localPosition.x - targetX) <= laneThreshold)
                {
                    occupied.Add(child.localPosition.z);
                }
            }
        }

        return occupied;
    }

    /// <summary>
    /// 检查组内所有收集物的 Z 位置是否与已占用区域保持足够距离。
    /// </summary>
    private bool IsGroupPositionClear(float startZ, int groupSize, float spacing, List<float> occupiedZ)
    {
        // 如果没有同跑道障碍物，直接通过
        if (occupiedZ.Count == 0)
        {
            return true;
        }

        float minDist = collectibleObstacleMinDistance;

        for (int i = 0; i < groupSize; i++)
        {
            float z = startZ + i * spacing;

            foreach (float occupied in occupiedZ)
            {
                if (Mathf.Abs(z - occupied) < minDist)
                {
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// 从零创建单个收集物 GameObject（无需预制体）。
    /// CollectibleVisual 会自动生成银色旋转柱体。
    /// </summary>
    private void CreateCollectible(Transform parent, int lane, float localZ)
    {
        GameObject obj = new GameObject($"Collectible_Energy_L{lane}");
        obj.transform.SetParent(parent, false);

        // 三跑道：lane 0 = 左 (-2.5), lane 1 = 中 (0), lane 2 = 右 (+2.5)
        float x = (lane - 1) * 2.5f;
        obj.transform.localPosition = new Vector3(x, collectibleYOffset, localZ);

        // 1. Collectible 组件（CollectibleVisual 由 [RequireComponent] 自动添加）
        Collectible collectible = obj.AddComponent<Collectible>();
        if (collectible == null)
        {
            Debug.LogError($"[TrackSpawner] AddComponent<Collectible>() 返回 null！");
            Destroy(obj);
            return;
        }

        // 运行时反射写入字段
        SetPrivateField(collectible, "type", (int)CollectibleType.Energy);
        SetPrivateField(collectible, "energyValue", 5f);
        SetPrivateField(collectible, "collectVfxPrefab", collectVfxPrefab);
        SetPrivateField(collectible, "collectVfxScale", collectVfxScale);

        // 2. 视觉（CollectibleVisual 在 Awake 中自动构建银色柱体）
        obj.AddComponent<CollectibleVisual>();

        Debug.Log($"[TrackSpawner] 创建收集物: Energy at lane {lane}, z={localZ:F1}");
    }
}
