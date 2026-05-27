using UnityEngine;

[RequireComponent(typeof(ColorGate))]
public sealed class ColorGateVisual : MonoBehaviour
{
    [SerializeField] private Renderer[] renderers;
    [SerializeField] private Transform pulseRoot;
    [SerializeField] private float emissionIntensity = 1.8f;
    [SerializeField] private float pulseSpeed = 5.5f;
    [SerializeField] private float pulseScale = 0.08f;
    [SerializeField] private float alpha = 0.26f;

    private MaterialPropertyBlock propertyBlock;
    private ColorGate gate;
    private Vector3 baseScale = Vector3.one;
    private Material energyMaterial;

    private void Awake()
    {
        gate = GetComponent<ColorGate>();
        energyMaterial = CreateEnergyMaterial();

        if (renderers == null || renderers.Length == 0)
        {
            renderers = GetComponentsInChildren<Renderer>();
        }

        if (renderers == null || renderers.Length == 0)
        {
            renderers = CreateDefaultFrame();
        }

        if (pulseRoot == null)
        {
            pulseRoot = transform;
        }

        baseScale = pulseRoot.localScale;
        AssignMaterial();
        ApplyColor(1f);
    }

    private void Update()
    {
        float pulse = 0.75f + Mathf.Sin(Time.time * pulseSpeed) * 0.25f;
        ApplyColor(pulse);

        if (pulseRoot != null)
        {
            float scale = 1f + pulse * pulseScale;
            pulseRoot.localScale = baseScale * scale;
        }
    }

    private void ApplyColor(float pulse)
    {
        if (gate == null || renderers == null)
        {
            return;
        }

        Color baseColor = gate.RequiredColor;
        Color emissionColor = baseColor * (emissionIntensity + pulse);
        propertyBlock ??= new MaterialPropertyBlock();

        foreach (Renderer targetRenderer in renderers)
        {
            if (targetRenderer == null)
            {
                continue;
            }

            targetRenderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor("_BaseColor", baseColor);
            propertyBlock.SetColor("_Color", baseColor);
            propertyBlock.SetColor("_EmissionColor", emissionColor);
            propertyBlock.SetFloat("_Alpha", alpha);
            propertyBlock.SetFloat("_Pulse", pulse);
            targetRenderer.SetPropertyBlock(propertyBlock);
        }
    }

    private Renderer[] CreateDefaultFrame()
    {
        Renderer energyPanel = CreateQuadPart("EnergyPanel", new Vector3(0f, 1.35f, 0.02f), new Vector3(1.65f, 2.55f, 1f));
        Renderer backPanel = CreateQuadPart("BackEnergyPanel", new Vector3(0f, 1.35f, -0.08f), new Vector3(1.35f, 2.15f, 1f));
        Renderer left = CreateFramePart("LeftRail", new Vector3(-0.9f, 1.35f, 0f), new Vector3(0.14f, 2.85f, 0.16f));
        Renderer right = CreateFramePart("RightRail", new Vector3(0.9f, 1.35f, 0f), new Vector3(0.14f, 2.85f, 0.16f));
        Renderer top = CreateFramePart("TopRail", new Vector3(0f, 2.72f, 0f), new Vector3(1.95f, 0.14f, 0.16f));
        Renderer bottom = CreateFramePart("BottomRail", new Vector3(0f, 0.06f, 0f), new Vector3(1.95f, 0.1f, 0.14f));
        Renderer scanBar = CreateFramePart("ScanBar", new Vector3(0f, 1.35f, -0.03f), new Vector3(1.7f, 0.035f, 0.05f));
        Renderer crossA = CreateFramePart("DiagonalA", new Vector3(0f, 1.35f, -0.05f), new Vector3(0.045f, 3.05f, 0.035f));
        Renderer crossB = CreateFramePart("DiagonalB", new Vector3(0f, 1.35f, -0.05f), new Vector3(0.045f, 3.05f, 0.035f));

        crossA.transform.localEulerAngles = new Vector3(0f, 0f, 31f);
        crossB.transform.localEulerAngles = new Vector3(0f, 0f, -31f);

        return new[] { energyPanel, backPanel, left, right, top, bottom, scanBar, crossA, crossB };
    }

    private Renderer CreateFramePart(string partName, Vector3 localPosition, Vector3 localScale)
    {
        GameObject part = GameObject.CreatePrimitive(PrimitiveType.Cube);
        part.name = partName;
        part.transform.SetParent(transform, false);
        part.transform.localPosition = localPosition;
        part.transform.localScale = localScale;

        Collider partCollider = part.GetComponent<Collider>();
        if (partCollider != null)
        {
            Destroy(partCollider);
        }

        return part.GetComponent<Renderer>();
    }

    private Renderer CreateQuadPart(string partName, Vector3 localPosition, Vector3 localScale)
    {
        GameObject part = GameObject.CreatePrimitive(PrimitiveType.Quad);
        part.name = partName;
        part.transform.SetParent(transform, false);
        part.transform.localPosition = localPosition;
        part.transform.localScale = localScale;

        Collider partCollider = part.GetComponent<Collider>();
        if (partCollider != null)
        {
            Destroy(partCollider);
        }

        return part.GetComponent<Renderer>();
    }

    private Material CreateEnergyMaterial()
    {
        Shader shader = Shader.Find("NeonRush/EnergyGate");
        if (shader == null)
        {
            shader = Shader.Find("Universal Render Pipeline/Unlit");
        }

        if (shader == null)
        {
            shader = Shader.Find("Unlit/Color");
        }

        return shader != null ? new Material(shader) : null;
    }

    private void AssignMaterial()
    {
        if (energyMaterial == null || renderers == null)
        {
            return;
        }

        foreach (Renderer targetRenderer in renderers)
        {
            if (targetRenderer != null)
            {
                targetRenderer.sharedMaterial = energyMaterial;
            }
        }
    }
}
