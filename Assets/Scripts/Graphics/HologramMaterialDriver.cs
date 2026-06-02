using UnityEngine;

[DisallowMultipleComponent]
public sealed class HologramMaterialDriver : MonoBehaviour
{
    [SerializeField] private Renderer[] renderers;
    [SerializeField] private Color baseColor = new Color(0.08f, 0.75f, 1f, 0.58f);
    [SerializeField] private Color emissionColor = new Color(0.16f, 1.4f, 2.8f, 1f);
    [SerializeField, Range(0f, 1f)] private float alpha = 0.42f;
    [SerializeField, Range(4f, 140f)] private float lineDensity = 52f;
    [SerializeField, Range(0f, 2f)] private float lineStrength = 0.78f;
    [SerializeField, Range(-10f, 10f)] private float scanSpeed = 1.7f;
    [SerializeField, Range(0.4f, 8f)] private float fresnelPower = 2.1f;
    [SerializeField, Range(0f, 5f)] private float fresnelStrength = 1.85f;
    [SerializeField, Range(0f, 1f)] private float glitchStrength = 0.22f;
    [SerializeField, Range(0f, 1f)] private float projectionFade = 0.2f;
    [SerializeField] private bool includeInactiveChildren;
    [SerializeField] private bool applyOnEnable = true;
    [SerializeField] private bool restoreOnDisable = true;

    private Material[][] originalMaterials;
    private Material[][] hologramMaterials;
    private Shader hologramShader;

    private void OnEnable()
    {
        if (applyOnEnable)
        {
            Apply();
        }
    }

    private void OnDisable()
    {
        if (restoreOnDisable)
        {
            Restore();
        }

        DestroyHologramMaterials();
    }

    private void Update()
    {
        if (hologramMaterials == null)
        {
            return;
        }

        float pulse = 0.72f + Mathf.Sin(Time.time * 4.6f) * 0.28f;
        PushMaterialValues(pulse);
    }

    [ContextMenu("Apply Hologram")]
    public void Apply()
    {
        EnsureRenderers();

        if (renderers == null || renderers.Length == 0)
        {
            return;
        }

        hologramShader = Shader.Find("NeonRush/HologramURP");
        if (hologramShader == null)
        {
            Debug.LogWarning("NeonRush/HologramURP shader was not found.", this);
            return;
        }

        if (originalMaterials != null)
        {
            Restore();
            DestroyHologramMaterials();
        }

        originalMaterials = new Material[renderers.Length][];
        hologramMaterials = new Material[renderers.Length][];

        for (int rendererIndex = 0; rendererIndex < renderers.Length; rendererIndex++)
        {
            Renderer targetRenderer = renderers[rendererIndex];
            if (targetRenderer == null)
            {
                continue;
            }

            Material[] sourceMaterials = targetRenderer.sharedMaterials;
            originalMaterials[rendererIndex] = sourceMaterials;
            Material[] replacements = new Material[sourceMaterials.Length];

            for (int materialIndex = 0; materialIndex < sourceMaterials.Length; materialIndex++)
            {
                Material source = sourceMaterials[materialIndex];
                Material replacement = new Material(hologramShader)
                {
                    name = source != null ? source.name + " Hologram" : "Runtime Hologram"
                };

                CopyBaseTexture(source, replacement);
                replacements[materialIndex] = replacement;
            }

            hologramMaterials[rendererIndex] = replacements;
            targetRenderer.sharedMaterials = replacements;
        }

        PushMaterialValues(1f);
    }

    [ContextMenu("Restore Original Materials")]
    public void Restore()
    {
        if (renderers == null || originalMaterials == null)
        {
            return;
        }

        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null && i < originalMaterials.Length && originalMaterials[i] != null)
            {
                renderers[i].sharedMaterials = originalMaterials[i];
            }
        }
    }

    private void DestroyHologramMaterials()
    {
        if (hologramMaterials == null)
        {
            return;
        }

        foreach (Material[] materials in hologramMaterials)
        {
            if (materials == null)
            {
                continue;
            }

            foreach (Material material in materials)
            {
                if (material == null)
                {
                    continue;
                }

                if (Application.isPlaying)
                {
                    Destroy(material);
                }
                else
                {
                    DestroyImmediate(material);
                }
            }
        }

        hologramMaterials = null;
        originalMaterials = null;
    }

    public void SetTint(Color color)
    {
        baseColor = color;
        emissionColor = color * 2.8f;
        emissionColor.a = 1f;
        PushMaterialValues(1f);
    }

    private void EnsureRenderers()
    {
        if (renderers == null || renderers.Length == 0)
        {
            renderers = GetComponentsInChildren<Renderer>(includeInactiveChildren);
        }
    }

    private void PushMaterialValues(float pulse)
    {
        for (int rendererIndex = 0; rendererIndex < hologramMaterials.Length; rendererIndex++)
        {
            Material[] materials = hologramMaterials[rendererIndex];
            if (materials == null)
            {
                continue;
            }

            foreach (Material material in materials)
            {
                if (material == null)
                {
                    continue;
                }

                material.SetColor("_BaseColor", baseColor);
                material.SetColor("_EmissionColor", emissionColor);
                material.SetFloat("_Alpha", alpha);
                material.SetFloat("_LineDensity", lineDensity);
                material.SetFloat("_LineStrength", lineStrength);
                material.SetFloat("_ScanSpeed", scanSpeed);
                material.SetFloat("_FresnelPower", fresnelPower);
                material.SetFloat("_FresnelStrength", fresnelStrength);
                material.SetFloat("_GlitchStrength", glitchStrength);
                material.SetFloat("_ProjectionFade", projectionFade);
                material.SetFloat("_Pulse", pulse);
            }
        }
    }

    private static void CopyBaseTexture(Material source, Material target)
    {
        if (source == null || target == null)
        {
            return;
        }

        if (source.HasProperty("_BaseMap"))
        {
            target.SetTexture("_BaseMap", source.GetTexture("_BaseMap"));
        }
        else if (source.HasProperty("_MainTex"))
        {
            target.SetTexture("_BaseMap", source.GetTexture("_MainTex"));
        }
    }
}
