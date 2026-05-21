using System;
using UnityEngine;

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
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetMode(EnergyMode.Red);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetMode(EnergyMode.Blue);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SetMode(EnergyMode.Green);
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            Cycle(-1);
        }
        else if (Input.GetKeyDown(KeyCode.E))
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
