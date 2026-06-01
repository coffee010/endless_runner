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
    [SerializeField] private int obstaclesPerSegment = 3;
    [SerializeField] private float obstacleSpacingMin = 4f;
    [SerializeField] private float obstacleSpacingMax = 10f;
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
        int spawned = 0;
        int attempts = 0;

        while (z < segmentLength - 2f && spawned < obstaclesPerSegment && attempts < 20)
        {
            attempts++;

            // 随机间距
            z += Random.Range(1.5f, obstacleSpacingMax - obstacleSpacingMin);

            if (z >= segmentLength - 2f)
            {
                break;
            }

            ObstacleType type = PickObstacleType();
            int lane = Random.Range(0, 3);   // 随机左/中/右跑道

            CreateObstacle(segment.transform, type, lane, z);
            spawned++;
        }
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
            if (child.GetComponent<Obstacle>() != null || child.GetComponent<ColorGate>() != null)
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
}
