using UnityEngine;
using UnityEngine.UI;
using TMPro;

public sealed class UIManager : MonoBehaviour
{
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private EnergyBurst energyBurst;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private TextMeshProUGUI gameOverStatsText;
    [SerializeField] private Slider energySlider;
    [SerializeField] private GameObject gameOverPanel;

    private bool subscribedToGameManager;

    private void Awake()
    {
        EnsureGameOverStatsText();
    }

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

        if (state == GameState.GameOver)
        {
            UpdateGameOverStats();
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

    private void UpdateGameOverStats()
    {
        if (gameOverStatsText == null || scoreManager == null)
        {
            return;
        }

        gameOverStatsText.text = $"Score {scoreManager.Score}\nDistance {scoreManager.Distance:0} m";
    }

    private void EnsureGameOverStatsText()
    {
        if (gameOverStatsText != null || gameOverPanel == null)
        {
            return;
        }

        Transform existing = gameOverPanel.transform.Find("GameOverStatsText");
        if (existing != null && existing.TryGetComponent(out gameOverStatsText))
        {
            return;
        }

        GameObject textObject = new GameObject("GameOverStatsText", typeof(RectTransform));
        textObject.transform.SetParent(gameOverPanel.transform, false);

        RectTransform rectTransform = textObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = new Vector2(0f, 120f);
        rectTransform.sizeDelta = new Vector2(520f, 90f);

        gameOverStatsText = textObject.AddComponent<TextMeshProUGUI>();
        gameOverStatsText.alignment = TextAlignmentOptions.Center;
        gameOverStatsText.fontSize = 28f;
        gameOverStatsText.color = Color.white;
        gameOverStatsText.raycastTarget = false;
        gameOverStatsText.text = "Score 0\nDistance 0 m";
    }
}
