using System;
using UnityEngine;
using UnityEngine.InputSystem;

public enum EnergyMode
{
    Red,
    Blue,
    Green
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
        else if (keyboard.digit3Key.wasPressedThisFrame)
        {
            SetMode(EnergyMode.Green);
        }
        else if (keyboard.qKey.wasPressedThisFrame)
        {
            Cycle(-1);
        }
        else if (keyboard.eKey.wasPressedThisFrame)
        {
            Cycle(1);
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

    private void Cycle(int direction)
    {
        int count = Enum.GetValues(typeof(EnergyMode)).Length;
        int next = ((int)CurrentMode + direction + count) % count;
        SetMode((EnergyMode)next);
    }
}
