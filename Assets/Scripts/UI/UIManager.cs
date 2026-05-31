using UnityEngine;
using UnityEngine.UI;
using TMPro;

public sealed class UIManager : MonoBehaviour
{
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private EnergyBurst energyBurst;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private Slider energySlider;
    [SerializeField] private GameObject gameOverPanel;

    private bool subscribedToGameManager;

    private void OnEnable()
    {
        TrySubscribeGameManager();

        if (scoreManager != null)
        {
            scoreManager.ScoreChanged += HandleScoreChanged;
        }

        if (energyBurst != null)
        {
            energyBurst.EnergyChanged += HandleEnergyChanged;
        }
    }

    private void Start()
    {
        TrySubscribeGameManager();
    }

    private void OnDisable()
    {
        if (subscribedToGameManager && GameManager.Instance != null)
        {
            GameManager.Instance.StateChanged -= HandleStateChanged;
        }
        subscribedToGameManager = false;

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
        if (!subscribedToGameManager)
        {
            TrySubscribeGameManager();
        }

        if (speedText != null && scoreManager != null)
        {
            speedText.text = $"Speed {scoreManager.CurrentSpeed:0.0}";
        }
    }

    private void TrySubscribeGameManager()
    {
        if (subscribedToGameManager || GameManager.Instance == null)
        {
            return;
        }

        GameManager.Instance.StateChanged += HandleStateChanged;
        subscribedToGameManager = true;
        HandleStateChanged(GameManager.Instance.State);
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
