using UnityEngine;

public sealed class ColorGate : MonoBehaviour
{
    [SerializeField] private EnergyMode requiredMode = EnergyMode.Blue;
    [SerializeField] private GameObject passVfxPrefab;

    public EnergyMode RequiredMode => requiredMode;

    public bool TryPass(EnergyMode currentMode)
    {
        bool passed = currentMode == requiredMode;

        if (passed && passVfxPrefab != null)
        {
            Instantiate(passVfxPrefab, transform.position, Quaternion.identity);
        }

        return passed;
    }
}
