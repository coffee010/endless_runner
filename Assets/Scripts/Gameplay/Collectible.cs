using UnityEngine;

public enum CollectibleType
{
    Score,
    Energy
}

public sealed class Collectible : MonoBehaviour
{
    [SerializeField] private CollectibleType type = CollectibleType.Energy;
    [SerializeField] private int scoreValue = 50;
    [SerializeField] private float energyValue = 10f;
    [SerializeField] private GameObject collectVfxPrefab;

    private bool collected;

    public void Collect(ScoreManager scoreManager)
    {
        if (collected)
        {
            return;
        }

        collected = true;

        if (type == CollectibleType.Score && scoreManager != null)
        {
            scoreManager.AddBonus(scoreValue);
        }

        EnergyBurst burst = FindObjectOfType<EnergyBurst>();
        if (type == CollectibleType.Energy && burst != null)
        {
            burst.AddEnergy(energyValue);
        }

        if (collectVfxPrefab != null)
        {
            Instantiate(collectVfxPrefab, transform.position, Quaternion.identity);
        }

        gameObject.SetActive(false);
    }
}
