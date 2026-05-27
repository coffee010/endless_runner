using UnityEngine;

public sealed class ColorGate : MonoBehaviour
{
    [SerializeField] private EnergyMode requiredMode = EnergyMode.Blue;
    [SerializeField] private GameObject passVfxPrefab;
    [SerializeField] private GameObject failVfxPrefab;
    [SerializeField] private bool deactivateOnPass = true;
    [SerializeField] private bool deactivateOnFail = true;
    [SerializeField] private int wrongColorPenalty = 100;

    public EnergyMode RequiredMode => requiredMode;
    public Color RequiredColor => GetModeColor(requiredMode);
    public int WrongColorPenalty => wrongColorPenalty;

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
            Instantiate(prefab, transform.position, Quaternion.identity);
        }
    }
}
