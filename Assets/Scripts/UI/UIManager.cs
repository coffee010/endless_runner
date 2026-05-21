using UnityEngine;
using UnityEngine.UI;

public sealed class UIManager : MonoBehaviour
{
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private EnergyBurst energyBurst;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text speedText;
    [SerializeField] private Slider energySlider;
    [SerializeField] private GameObject gameOverPanel;

    private void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StateChanged += HandleStateChanged;
        }

        if (scoreManager != null)
        {
            scoreManager.ScoreChanged += HandleScoreChanged;
        }

        if (energyBurst != null)
        {
            energyBurst.EnergyChanged += HandleEnergyChanged;
        }
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StateChanged -= HandleStateChanged;
        }

        if (scoreManager != null)
        {
            scoreManager.ScoreChanged -= HandleScoreChanged;
        }

        if (energyBurst != null)
        {
            energyBurst.EnergyChanged -= HandleEnergyChanged;
        }
    }

    private void Update()
    {
        if (speedText != null && scoreManager != null)
        {
            speedText.text = $"Speed +{scoreManager.SpeedBonus:0.0}";
        }
    }

    private void HandleStateChanged(GameState state)
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(state == GameState.GameOver);
        }
    }

    private void HandleScoreChanged(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score {score}";
        }
    }

    private void HandleEnergyChanged(float normalizedEnergy)
    {
        if (energySlider != null)
        {
            energySlider.value = normalizedEnergy;
        }
    }
}
