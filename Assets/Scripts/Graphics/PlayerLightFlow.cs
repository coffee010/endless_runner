using UnityEngine;

public sealed class PlayerLightFlow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;
    [SerializeField] private RunnerController runner;
    [SerializeField] private EnergyModeController modeController;

    [Header("Follow")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 2.2f, -1.4f);
    [SerializeField] private float positionSmoothTime = 0.08f;
    [SerializeField] private bool followLaneX = true;
    [SerializeField] private bool lookAtTarget = true;
    [SerializeField] private Vector3 lookOffset = new Vector3(0f, 1.1f, 2.5f);

    [Header("Light")]
    [SerializeField] private Light followLight;
    [SerializeField] private float baseIntensity = 2.2f;
    [SerializeField] private float speedIntensity = 0.08f;
    [SerializeField] private float pulseIntensity = 0.45f;
    [SerializeField] private float pulseSpeed = 5f;
    [SerializeField] private float lightRange = 7f;
    [SerializeField] private Color blue = new Color(0.08f, 0.45f, 1f);
    [SerializeField] private Color red = new Color(1f, 0.12f, 0.08f);

    [Header("Trail")]
    [SerializeField] private TrailRenderer trail;
    [SerializeField] private Material trailMaterial;
    [SerializeField] private float trailTime = 0.35f;
    [SerializeField] private float trailWidth = 0.38f;
    [SerializeField] private float emissionMultiplier = 3f;

    private Vector3 positionVelocity;
    private MaterialPropertyBlock propertyBlock;
    private Material runtimeTrailMaterial;

    private void Awake()
    {
        ResolveReferences();
        ConfigureLight();
        ConfigureTrail();
    }

    private void OnEnable()
    {
        if (modeController != null)
        {
            modeController.ModeChanged += ApplyMode;
            ApplyMode(modeController.CurrentMode);
        }
        else
        {
            ApplyColor(blue);
        }
    }

    private void OnDisable()
    {
        if (modeController != null)
        {
            modeController.ModeChanged -= ApplyMode;
        }
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            ResolveReferences();
        }

        if (target == null)
        {
            return;
        }

        Vector3 basePosition = target.position;
        if (!followLaneX)
        {
            basePosition.x = 0f;
        }

        Vector3 desiredPosition = basePosition + offset;
        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref positionVelocity,
            positionSmoothTime);

        if (lookAtTarget)
        {
            Vector3 lookPoint = target.position + lookOffset;
            if (!followLaneX)
            {
                lookPoint.x = 0f;
            }

            Vector3 lookDirection = lookPoint - transform.position;
            if (lookDirection.sqrMagnitude > 0.0001f)
            {
                transform.rotation = Quaternion.LookRotation(lookDirection, Vector3.up);
            }
        }

        UpdateLightIntensity();
    }

    private void ResolveReferences()
    {
        if (runner == null)
        {
            runner = FindAnyObjectByType<RunnerController>();
        }

        if (target == null && runner != null)
        {
            target = runner.transform;
        }

        if (modeController == null && target != null)
        {
            modeController = target.GetComponentInParent<EnergyModeController>();
        }

        if (followLight == null)
        {
            followLight = GetComponentInChildren<Light>();
        }

        if (trail == null)
        {
            trail = GetComponentInChildren<TrailRenderer>();
        }
    }

    private void ConfigureLight()
    {
        if (followLight == null)
        {
            return;
        }

        followLight.type = LightType.Point;
        followLight.range = lightRange;
        followLight.intensity = baseIntensity;
    }

    private void ConfigureTrail()
    {
        if (trail == null)
        {
            return;
        }

        trail.time = trailTime;
        trail.startWidth = trailWidth;
        trail.endWidth = 0f;
        trail.alignment = LineAlignment.View;
        trail.textureMode = LineTextureMode.Stretch;
        trail.emitting = true;

        if (trailMaterial != null)
        {
            trail.sharedMaterial = trailMaterial;
        }
        else if (trail.sharedMaterial == null)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
            if (shader == null)
            {
                shader = Shader.Find("Sprites/Default");
            }

            if (shader != null)
            {
                runtimeTrailMaterial = new Material(shader)
                {
                    name = "Runtime_PlayerLightFlow_Trail"
                };
                trail.sharedMaterial = runtimeTrailMaterial;
            }
        }
    }

    private void UpdateLightIntensity()
    {
        if (followLight == null)
        {
            return;
        }

        float runnerSpeed = runner != null ? runner.CurrentSpeed : 0f;
        float pulse = (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f;
        followLight.intensity = baseIntensity + runnerSpeed * speedIntensity + pulse * pulseIntensity;
        followLight.range = lightRange + pulse * 0.8f;
    }

    private void ApplyMode(EnergyMode mode)
    {
        ApplyColor(mode == EnergyMode.Red ? red : blue);
    }

    private void ApplyColor(Color color)
    {
        Color emissionColor = color * emissionMultiplier;

        if (followLight != null)
        {
            followLight.color = color;
        }

        if (trail == null)
        {
            return;
        }

        trail.startColor = new Color(color.r, color.g, color.b, 0.85f);
        trail.endColor = new Color(color.r, color.g, color.b, 0f);

        Renderer trailRenderer = trail.GetComponent<Renderer>();
        if (trailRenderer == null)
        {
            return;
        }

        propertyBlock ??= new MaterialPropertyBlock();
        trailRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetColor("_BaseColor", color);
        propertyBlock.SetColor("_EmissionColor", emissionColor);
        trailRenderer.SetPropertyBlock(propertyBlock);
    }

    private void OnDrawGizmosSelected()
    {
        Transform currentTarget = target;
        if (currentTarget == null)
        {
            return;
        }

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(currentTarget.position + offset, 0.35f);
        Gizmos.DrawLine(currentTarget.position, currentTarget.position + offset);
    }
}
