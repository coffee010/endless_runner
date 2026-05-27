using System;
using UnityEngine;
using UnityEngine.InputSystem;

public enum EnergyMode
{
    Red,
    Blue
}

public sealed class EnergyModeController : MonoBehaviour
{
    [SerializeField] private EnergyMode startingMode = EnergyMode.Blue;

    public EnergyMode CurrentMode { get; private set; }
    public event Action<EnergyMode> ModeChanged;

    private void Awake()
    {
        CurrentMode = startingMode;
    }

    private void Start()
    {
        ModeChanged?.Invoke(CurrentMode);
    }

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
        {
            return;
        }

        if (keyboard.digit1Key.wasPressedThisFrame)
        {
            SetMode(EnergyMode.Red);
        }
        else if (keyboard.digit2Key.wasPressedThisFrame)
        {
            SetMode(EnergyMode.Blue);
        }
        else if (keyboard.qKey.wasPressedThisFrame)
        {
            ToggleMode();
        }
        else if (keyboard.eKey.wasPressedThisFrame)
        {
            ToggleMode();
        }
    }

    public void SetMode(EnergyMode mode)
    {
        if (CurrentMode == mode)
        {
            return;
        }

        CurrentMode = mode;
        ModeChanged?.Invoke(CurrentMode);
    }

    private void ToggleMode()
    {
        SetMode(CurrentMode == EnergyMode.Red ? EnergyMode.Blue : EnergyMode.Red);
    }
}
