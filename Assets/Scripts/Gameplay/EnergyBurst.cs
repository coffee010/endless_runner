using System;
using UnityEngine;

public sealed class EnergyBurst : MonoBehaviour
{
    [SerializeField] private RunnerController runner;
    [SerializeField] private float maxEnergy = 100f;
    [SerializeField] private float invulnerableSeconds = 2f;
    [SerializeField] private float clearDistance = 25f;
    [SerializeField] private GameObject burstVfxPrefab;

    public float Energy { get; private set; }
    public float NormalizedEnergy => maxEnergy <= 0f ? 0f : Energy / maxEnergy;

    public event Action<float> EnergyChanged;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && Energy >= maxEnergy)
        {
            TriggerBurst();
        }
    }

    public void AddEnergy(float amount)
    {
        Energy = Mathf.Clamp(Energy + amount, 0f, maxEnergy);
        EnergyChanged?.Invoke(NormalizedEnergy);
    }

    public void TriggerBurst()
    {
        Energy = 0f;
        EnergyChanged?.Invoke(NormalizedEnergy);

        if (runner != null)
        {
            runner.SetInvulnerable(invulnerableSeconds);
        }

        if (burstVfxPrefab != null)
        {
            Instantiate(burstVfxPrefab, transform.position, Quaternion.identity);
        }

        ClearForwardObstacles();
    }

    private void ClearForwardObstacles()
    {
        Obstacle[] obstacles = FindObjectsOfType<Obstacle>();
        foreach (Obstacle obstacle in obstacles)
        {
            float zDelta = obstacle.transform.position.z - transform.position.z;
            if (zDelta > 0f && zDelta <= clearDistance)
            {
                obstacle.gameObject.SetActive(false);
            }
        }
    }
}
