using System;
using UnityEngine;
using UnityEngine.InputSystem;

public sealed class EnergyBurst : MonoBehaviour
{
    [SerializeField] private RunnerController runner;
    [SerializeField] private float maxEnergy = 100f;
    [SerializeField] private float invulnerableSeconds = 2f;
    [SerializeField] private float clearDistance = 25f;
    [SerializeField] private float clearWidth = 10f;
    [SerializeField] private float clearHeight = 10f;
    [SerializeField] private LayerMask clearObstacleLayers = ~0;
    [SerializeField, Min(1)] private int maxClearHits = 32;
    [SerializeField] private GameObject burstVfxPrefab;

    public float Energy { get; private set; }
    public float NormalizedEnergy => maxEnergy <= 0f ? 0f : Energy / maxEnergy;

    public event Action<float> EnergyChanged;

    private Collider[] clearHits;

    private void Awake()
    {
        EnsureClearHitCapacity();
    }

    private void OnValidate()
    {
        maxClearHits = Mathf.Max(1, maxClearHits);
        clearDistance = Mathf.Max(0f, clearDistance);
        clearWidth = Mathf.Max(0f, clearWidth);
        clearHeight = Mathf.Max(0f, clearHeight);
    }

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard != null && keyboard.fKey.wasPressedThisFrame && Energy >= maxEnergy)
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
        EnsureClearHitCapacity();

        Vector3 center = transform.position + Vector3.forward * (clearDistance * 0.5f);
        Vector3 halfExtents = new Vector3(clearWidth * 0.5f, clearHeight * 0.5f, clearDistance * 0.5f);
        int layerMask = clearObstacleLayers.value == 0 ? Physics.AllLayers : clearObstacleLayers.value;
        int hitCount = Physics.OverlapBoxNonAlloc(
            center,
            halfExtents,
            clearHits,
            Quaternion.identity,
            layerMask,
            QueryTriggerInteraction.Collide
        );

        for (int i = 0; i < hitCount; i++)
        {
            Collider hit = clearHits[i];
            if (hit == null)
            {
                continue;
            }

            Obstacle obstacle = hit.GetComponentInParent<Obstacle>();
            if (obstacle != null && obstacle.gameObject.activeSelf)
            {
                obstacle.gameObject.SetActive(false);
            }

            clearHits[i] = null;
        }
    }

    private void EnsureClearHitCapacity()
    {
        maxClearHits = Mathf.Max(1, maxClearHits);

        if (clearHits == null || clearHits.Length != maxClearHits)
        {
            clearHits = new Collider[maxClearHits];
        }
    }
}
