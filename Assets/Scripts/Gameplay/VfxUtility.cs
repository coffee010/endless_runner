using UnityEngine;

public static class VfxUtility
{
    public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, float scaleMultiplier = 1.6f)
    {
        return Spawn(prefab, position, rotation, scaleMultiplier, false, Color.white);
    }

    public static GameObject SpawnTinted(GameObject prefab, Vector3 position, Quaternion rotation, float scaleMultiplier, Color tintColor)
    {
        return Spawn(prefab, position, rotation, scaleMultiplier, true, tintColor);
    }

    private static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, float scaleMultiplier, bool tint, Color tintColor)
    {
        if (prefab == null)
        {
            return null;
        }

        GameObject instance = Object.Instantiate(prefab, position, rotation);
        instance.transform.localScale *= Mathf.Max(0.01f, scaleMultiplier);

        ParticleSystem[] particles = instance.GetComponentsInChildren<ParticleSystem>(true);
        foreach (ParticleSystem particle in particles)
        {
            ParticleSystem.MainModule main = particle.main;
            main.useUnscaledTime = true;

            if (tint)
            {
                ApplyParticleTint(particle, tintColor);
            }

            if (!particle.isPlaying)
            {
                particle.Play();
            }
        }

        if (tint)
        {
            ApplyRendererTint(instance, tintColor);
            ApplyLightTint(instance, tintColor);
        }

        return instance;
    }

    private static void ApplyParticleTint(ParticleSystem particle, Color tintColor)
    {
        ParticleSystem.MainModule main = particle.main;
        Color brightTint = tintColor * 1.35f;
        brightTint.a = 1f;
        main.startColor = brightTint;

        ParticleSystem.ColorOverLifetimeModule colorOverLifetime = particle.colorOverLifetime;
        if (!colorOverLifetime.enabled)
        {
            return;
        }

        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new[]
            {
                new GradientColorKey(Color.white, 0f),
                new GradientColorKey(brightTint, 0.25f),
                new GradientColorKey(tintColor, 1f)
            },
            new[]
            {
                new GradientAlphaKey(0f, 0f),
                new GradientAlphaKey(1f, 0.12f),
                new GradientAlphaKey(0f, 1f)
            }
        );
        colorOverLifetime.color = gradient;
    }

    private static void ApplyRendererTint(GameObject instance, Color tintColor)
    {
        Renderer[] renderers = instance.GetComponentsInChildren<Renderer>(true);
        foreach (Renderer renderer in renderers)
        {
            Material material = renderer.material;
            if (material == null)
            {
                continue;
            }

            Color emission = tintColor * 2f;
            emission.a = 1f;

            if (material.HasProperty("_BaseColor"))
            {
                material.SetColor("_BaseColor", tintColor);
            }

            if (material.HasProperty("_Color"))
            {
                material.SetColor("_Color", tintColor);
            }

            if (material.HasProperty("_EmissionColor"))
            {
                material.SetColor("_EmissionColor", emission);
            }
        }
    }

    private static void ApplyLightTint(GameObject instance, Color tintColor)
    {
        Light[] lights = instance.GetComponentsInChildren<Light>(true);
        foreach (Light light in lights)
        {
            light.color = tintColor;
        }
    }
}
