using UnityEngine;

public sealed class TrackSegment : MonoBehaviour
{
    [SerializeField] private float length = 30f;
    [SerializeField] private Transform endAnchor;
    [SerializeField] private bool configureColorGates = true;
    [SerializeField] private ColorGate[] themedColorGates;
    [SerializeField] private Renderer[] themedRenderers;
    [SerializeField] private float themeEmissionIntensity = 1.8f;
    [SerializeField] private bool applyCyberTrackMaterial = true;
    [SerializeField] private float cyberGridDensity = 18f;
    [SerializeField] private float cyberFlowSpeed = 2.2f;
    [SerializeField] private bool buildProceduralScenery = true;

    private MaterialPropertyBlock themePropertyBlock;
    private Material cyberTrackMaterial;
    private Material sceneryMaterial;
    private Transform sceneryRoot;

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
            themePropertyBlock.SetColor("_BaseColor", Color.Lerp(Color.black, baseColor, 0.55f));
            themePropertyBlock.SetColor("_Color", baseColor);
            themePropertyBlock.SetColor("_EmissionColor", emissionColor);
            themePropertyBlock.SetFloat("_GridDensity", cyberGridDensity);
            themePropertyBlock.SetFloat("_FlowSpeed", cyberFlowSpeed);
            themePropertyBlock.SetFloat("_EmissionPower", themeEmissionIntensity);
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
            Shader shader = Shader.Find("NeonRush/CyberTrack");
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
        if (!buildProceduralScenery || sceneryRoot != null)
        {
            return;
        }

        GameObject root = new GameObject("CyberScenery");
        root.transform.SetParent(transform, false);
        sceneryRoot = root.transform;

        Color themeColor = ColorGate.GetModeColor(mode);
        Color wallColor = new Color(0.018f, 0.02f, 0.03f);
        Color railColor = themeColor * 0.8f;

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

    private void CreateSceneryPart(string partName, Vector3 localPosition, Vector3 localScale, Color color)
    {
        GameObject part = GameObject.CreatePrimitive(PrimitiveType.Cube);
        part.name = partName;
        part.transform.SetParent(sceneryRoot, false);
        part.transform.localPosition = localPosition;
        part.transform.localScale = localScale;

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
            Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
            if (shader == null)
            {
                shader = Shader.Find("Unlit/Color");
            }

            if (shader != null)
            {
                sceneryMaterial = new Material(shader);
            }
        }

        if (sceneryMaterial != null)
        {
            renderer.sharedMaterial = sceneryMaterial;
        }

        themePropertyBlock ??= new MaterialPropertyBlock();
        themePropertyBlock.Clear();
        renderer.GetPropertyBlock(themePropertyBlock);
        themePropertyBlock.SetColor("_BaseColor", color);
        themePropertyBlock.SetColor("_Color", color);
        themePropertyBlock.SetColor("_EmissionColor", color);
        renderer.SetPropertyBlock(themePropertyBlock);
    }
}
