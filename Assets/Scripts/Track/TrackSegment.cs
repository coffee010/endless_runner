using UnityEngine;

public sealed class TrackSegment : MonoBehaviour, IPoolable
{
    /// <summary>产生此实例的预制体引用，供对象池回收时查找对应池。</summary>
    public TrackSegment SourcePrefab { get; set; }
    [SerializeField] private float length = 30f;
    [SerializeField] private Transform endAnchor;
    [SerializeField] private bool configureColorGates = true;
    [SerializeField] private ColorGate[] themedColorGates;
    [SerializeField] private Renderer[] themedRenderers;
    [SerializeField] private float themeEmissionIntensity = 1.8f;
    [SerializeField] private bool tintThemedBaseColor = false;
    [SerializeField] private bool applyCyberTrackMaterial = false;
    [SerializeField] private float cyberGridDensity = 18f;
    [SerializeField] private float cyberFlowSpeed = 2.2f;
    [SerializeField] private float edgePulseDensity = 10f;
    [SerializeField] private float edgePulseLength = 0.16f;
    [SerializeField] private float edgeIntensity = 1.45f;
    [SerializeField] private bool buildProceduralScenery = true;
    [SerializeField] private string environmentLayerName = "CyberEnvironment";

    private MaterialPropertyBlock themePropertyBlock;
    private Material cyberTrackMaterial;
    private Material sceneryMaterial;
    private Transform sceneryRoot;
    private int environmentLayer = -2;

    public float Length => length;

    public Vector3 EndPosition
    {
        get
        {
            if (endAnchor != null)
            {
                return endAnchor.position;
            }

            return transform.position + Vector3.forward * length;
        }
    }

    public void ApplyColorTheme(EnergyMode mode)
    {
        ApplyEnvironmentLayer(transform);

        if (!configureColorGates)
        {
            return;
        }

        ColorGate[] gates = themedColorGates != null && themedColorGates.Length > 0
            ? themedColorGates
            : GetComponentsInChildren<ColorGate>(true);
        foreach (ColorGate gate in gates)
        {
            if (gate == null)
            {
                continue;
            }

            bool isActiveThemeGate = gate.RequiredMode == mode;
            gate.gameObject.SetActive(isActiveThemeGate);
            gate.SetRequiredMode(mode);
        }

        ApplyTrackTint(mode);
        BuildScenery(mode);
    }

    private void ApplyTrackTint(EnergyMode mode)
    {
        Renderer[] targets = themedRenderers != null && themedRenderers.Length > 0
            ? themedRenderers
            : GetComponentsInChildren<Renderer>(true);

        Color themeColor = ColorGate.GetModeColor(mode);
        Color baseColor = Color.Lerp(Color.gray, themeColor, 0.35f);
        Color emissionColor = themeColor * themeEmissionIntensity;
        themePropertyBlock ??= new MaterialPropertyBlock();

        foreach (Renderer targetRenderer in targets)
        {
            if (targetRenderer == null || targetRenderer.GetComponentInParent<ColorGate>() != null)
            {
                continue;
            }

            if (applyCyberTrackMaterial)
            {
                ApplyCyberTrackMaterial(targetRenderer);
            }

            targetRenderer.GetPropertyBlock(themePropertyBlock);
            if (tintThemedBaseColor)
            {
                themePropertyBlock.SetColor("_BaseColor", Color.Lerp(Color.black, baseColor, 0.55f));
                themePropertyBlock.SetColor("_Color", baseColor);
            }

            themePropertyBlock.SetColor("_EmissionColor", emissionColor);
            themePropertyBlock.SetFloat("_GridDensity", cyberGridDensity);
            themePropertyBlock.SetFloat("_FlowSpeed", cyberFlowSpeed);
            themePropertyBlock.SetFloat("_EmissionPower", themeEmissionIntensity);
            themePropertyBlock.SetFloat("_EdgeIntensity", edgeIntensity);
            themePropertyBlock.SetFloat("_EdgePulseDensity", edgePulseDensity);
            themePropertyBlock.SetFloat("_EdgePulseLength", edgePulseLength);
            themePropertyBlock.SetFloat("_PulseDensity", edgePulseDensity);
            themePropertyBlock.SetFloat("_PulseLength", edgePulseLength);
            themePropertyBlock.SetFloat("_RimPower", edgeIntensity * 0.45f);
            targetRenderer.SetPropertyBlock(themePropertyBlock);
        }
    }

    private void ApplyCyberTrackMaterial(Renderer targetRenderer)
    {
        if (targetRenderer.GetComponentInParent<Obstacle>() != null || targetRenderer.GetComponentInParent<Collectible>() != null)
        {
            return;
        }

        if (cyberTrackMaterial == null)
        {
            Shader shader = Shader.Find("DELTation/Toon Shader");
            if (shader != null)
            {
                cyberTrackMaterial = new Material(shader);
            }
        }

        if (cyberTrackMaterial != null)
        {
            targetRenderer.sharedMaterial = cyberTrackMaterial;
        }
    }

    private void BuildScenery(EnergyMode mode)
    {
        if (!buildProceduralScenery)
        {
            return;
        }

        if (sceneryRoot != null)
        {
            // 场景已存在（对象池复用），只更新颜色，不重建几何体
            UpdateSceneryColors(mode);
            return;
        }

        GameObject root = new GameObject("CyberScenery");
        root.transform.SetParent(transform, false);
        sceneryRoot = root.transform;
        ApplyEnvironmentLayer(root.transform);

        Color themeColor = ColorGate.GetModeColor(mode);
        Color railColor = themeColor * 0.8f;

        BuildSceneryGeometry(railColor, themeColor);
    }

    private void BuildSceneryGeometry(Color railColor, Color themeColor)
    {
        Color wallColor = new Color(0.018f, 0.02f, 0.03f);

        CreateSceneryPart("LeftWall", new Vector3(-4.25f, 1.4f, length * 0.5f), new Vector3(0.18f, 2.8f, length), wallColor);
        CreateSceneryPart("RightWall", new Vector3(4.25f, 1.4f, length * 0.5f), new Vector3(0.18f, 2.8f, length), wallColor);
        CreateSceneryPart("LeftFloorRail", new Vector3(-3.25f, 0.05f, length * 0.5f), new Vector3(0.08f, 0.08f, length), railColor);
        CreateSceneryPart("RightFloorRail", new Vector3(3.25f, 0.05f, length * 0.5f), new Vector3(0.08f, 0.08f, length), railColor);

        for (int i = 0; i < 4; i++)
        {
            float z = 4f + i * 7f;
            CreateSceneryPart("LeftLightPillar", new Vector3(-4.05f, 1.45f, z), new Vector3(0.12f, 2.2f, 0.08f), railColor);
            CreateSceneryPart("RightLightPillar", new Vector3(4.05f, 1.45f, z), new Vector3(0.12f, 2.2f, 0.08f), railColor);
        }
    }

    private void UpdateSceneryColors(EnergyMode mode)
    {
        Color themeColor = ColorGate.GetModeColor(mode);
        Color railColor = themeColor * 0.8f;

        foreach (Transform child in sceneryRoot)
        {
            Renderer renderer = child.GetComponent<Renderer>();
            if (renderer == null || renderer.material == null)
            {
                continue;
            }

            renderer.material.SetColor("_BaseColor", railColor);
        }
    }

    private void CreateSceneryPart(string partName, Vector3 localPosition, Vector3 localScale, Color color)
    {
        GameObject part = GameObject.CreatePrimitive(PrimitiveType.Cube);
        part.name = partName;
        part.transform.SetParent(sceneryRoot, false);
        part.transform.localPosition = localPosition;
        part.transform.localScale = localScale;
        ApplyEnvironmentLayer(part.transform);

        Collider partCollider = part.GetComponent<Collider>();
        if (partCollider != null)
        {
            Destroy(partCollider);
        }

        Renderer renderer = part.GetComponent<Renderer>();
        if (renderer == null)
        {
            return;
        }

        if (sceneryMaterial == null)
        {
            // 优先 URP/Unlit，保证颜色正确
            Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
            if (shader == null)
            {
                shader = Shader.Find("Unlit/Color");
            }
            if (shader == null)
            {
                shader = Shader.Find("DELTation/Toon Shader");
            }

            if (shader != null)
            {
                sceneryMaterial = new Material(shader);
            }
        }

        // 独立材质实例，保证每部件的颜色正确显示
        if (sceneryMaterial != null)
        {
            renderer.material = new Material(sceneryMaterial);
            renderer.material.SetColor("_BaseColor", color);
        }
    }

    private void ApplyEnvironmentLayer(Transform root)
    {
        if (root == null)
        {
            return;
        }

        int layer = GetEnvironmentLayer();
        if (layer < 0)
        {
            return;
        }

        root.gameObject.layer = layer;
        foreach (Transform child in root)
        {
            ApplyEnvironmentLayer(child);
        }
    }

    private int GetEnvironmentLayer()
    {
        if (environmentLayer == -2)
        {
            environmentLayer = LayerMask.NameToLayer(environmentLayerName);
        }

        return environmentLayer;
    }

    // ───────────────────── IPoolable ─────────────────────

    public void OnSpawn()
    {
        // 对象池取出时重置：由于 TrackSpawner 随后会调用 ApplyColorTheme，
        // 这里不需要做额外操作。
    }

    public void OnDespawn()
    {
        // 放回池时清理：重置场景根节点引用，但保留几何体（下次取出时只更新颜色）
        // 注：几何体保留不销毁——这是对象池性能优势的关键
        themePropertyBlock?.Clear();
    }
}
