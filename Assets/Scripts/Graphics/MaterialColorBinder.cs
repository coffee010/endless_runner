using UnityEngine;

public sealed class MaterialColorBinder : MonoBehaviour
{
    [SerializeField] private EnergyModeController modeController;
    [SerializeField] private Renderer[] renderers;
    [SerializeField] private Color red = new Color(1f, 0.15f, 0.1f);
    [SerializeField] private Color blue = new Color(0.1f, 0.45f, 1f);
    [SerializeField] private Color green = new Color(0.1f, 1f, 0.35f);
    [SerializeField] private string colorProperty = "_EmissionColor";

    private void OnEnable()
    {
        if (modeController != null)
        {
            modeController.ModeChanged += ApplyMode;
            ApplyMode(modeController.CurrentMode);
        }
    }

    private void OnDisable()
    {
        if (modeController != null)
        {
            modeController.ModeChanged -= ApplyMode;
        }
    }

    private void ApplyMode(EnergyMode mode)
    {
        Color color = mode == EnergyMode.Red ? red : mode == EnergyMode.Green ? green : blue;

        foreach (Renderer targetRenderer in renderers)
        {
            if (targetRenderer == null)
            {
                continue;
            }

            foreach (Material material in targetRenderer.materials)
            {
                if (material.HasProperty(colorProperty))
                {
                    material.SetColor(colorProperty, color);
                }

                if (material.HasProperty("_BaseColor"))
                {
                    material.SetColor("_BaseColor", color);
                }
            }
        }
    }
}
