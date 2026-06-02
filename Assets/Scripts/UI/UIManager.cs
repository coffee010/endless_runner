using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

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
    private GameObject startMenuPanel;
    private CanvasGroup startMenuGroup;
    private TextMeshProUGUI titleText;
    private TextMeshProUGUI promptText;
    private float titlePulseOffset;

    private void Awake()
    {
        EnsureGameOverStatsText();
        EnsureStartMenu();
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

        AnimateStartMenu();
        ReadStartMenuShortcut();
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

        if (startMenuPanel != null)
        {
            bool showStartMenu = state == GameState.Ready;
            startMenuPanel.SetActive(showStartMenu);
            if (startMenuGroup != null)
            {
                startMenuGroup.alpha = showStartMenu ? 1f : 0f;
                startMenuGroup.blocksRaycasts = showStartMenu;
                startMenuGroup.interactable = showStartMenu;
            }
        }

        SetHudVisible(state != GameState.Ready);

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

    private void EnsureStartMenu()
    {
        if (startMenuPanel != null)
        {
            return;
        }

        Canvas canvas = FindAnyObjectByType<Canvas>();
        if (canvas == null)
        {
            return;
        }

        startMenuPanel = CreateUiObject("StartMenu", canvas.transform);
        RectTransform panelRect = startMenuPanel.GetComponent<RectTransform>();
        Stretch(panelRect);
        startMenuPanel.transform.SetAsLastSibling();

        startMenuGroup = startMenuPanel.AddComponent<CanvasGroup>();

        Image dim = startMenuPanel.AddComponent<Image>();
        dim.color = new Color(0.015f, 0.018f, 0.035f, 0.72f);
        dim.raycastTarget = true;

        titleText = CreateText(startMenuPanel.transform, "Title", "NEON RUSH", 96f, FontStyles.Bold, new Color(0.7f, 0.96f, 1f, 1f));
        RectTransform titleRect = titleText.rectTransform;
        titleRect.anchorMin = new Vector2(0.5f, 0.5f);
        titleRect.anchorMax = new Vector2(0.5f, 0.5f);
        titleRect.anchoredPosition = new Vector2(0f, 135f);
        titleRect.sizeDelta = new Vector2(860f, 130f);
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.characterSpacing = 8f;

        TextMeshProUGUI subtitle = CreateText(startMenuPanel.transform, "Subtitle", "ENDLESS RUNNER", 22f, FontStyles.UpperCase, new Color(1f, 0.28f, 0.75f, 0.92f));
        RectTransform subtitleRect = subtitle.rectTransform;
        subtitleRect.anchorMin = new Vector2(0.5f, 0.5f);
        subtitleRect.anchorMax = new Vector2(0.5f, 0.5f);
        subtitleRect.anchoredPosition = new Vector2(0f, 78f);
        subtitleRect.sizeDelta = new Vector2(460f, 40f);
        subtitle.alignment = TextAlignmentOptions.Center;
        subtitle.characterSpacing = 4f;

        Button startButton = CreateButton(startMenuPanel.transform, "StartButton", "START", new Vector2(0f, -10f), new Color(0.02f, 0.55f, 0.9f, 0.48f), new Color(0.1f, 0.95f, 1f, 1f));
        startButton.onClick.AddListener(BeginRun);

        Button quitButton = CreateButton(startMenuPanel.transform, "QuitButton", "QUIT", new Vector2(0f, -120f), new Color(0.7f, 0.04f, 0.36f, 0.42f), new Color(1f, 0.22f, 0.74f, 1f));
        quitButton.onClick.AddListener(QuitGame);

        promptText = CreateText(startMenuPanel.transform, "Controls", "A/D Move   Space Jump   S Slide   1&2/E Switch Energy", 20f, FontStyles.Normal, new Color(0.82f, 0.92f, 1f, 0.82f));
        RectTransform promptRect = promptText.rectTransform;
        promptRect.anchorMin = new Vector2(0.5f, 0f);
        promptRect.anchorMax = new Vector2(0.5f, 0f);
        promptRect.anchoredPosition = new Vector2(0f, 34f);
        promptRect.sizeDelta = new Vector2(800f, 34f);
        promptText.alignment = TextAlignmentOptions.Center;
    }

    private static GameObject CreateUiObject(string objectName, Transform parent)
    {
        GameObject obj = new GameObject(objectName, typeof(RectTransform));
        obj.transform.SetParent(parent, false);
        return obj;
    }

    private static void Stretch(RectTransform rectTransform)
    {
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
    }

    private static TextMeshProUGUI CreateText(Transform parent, string objectName, string text, float fontSize, FontStyles style, Color color)
    {
        GameObject obj = CreateUiObject(objectName, parent);
        TextMeshProUGUI label = obj.AddComponent<TextMeshProUGUI>();
        label.text = text;
        label.fontSize = fontSize;
        label.fontStyle = style;
        label.color = color;
        label.raycastTarget = false;
        label.enableAutoSizing = true;
        label.fontSizeMin = Mathf.Min(16f, fontSize);
        label.fontSizeMax = fontSize;
        return label;
    }

    private static void CreateAccentBar(Transform parent, string objectName, Vector2 anchorMin, Vector2 anchorMax, Vector2 position, Vector2 size, Color color)
    {
        GameObject bar = CreateUiObject(objectName, parent);
        RectTransform rect = bar.GetComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        Image image = bar.AddComponent<Image>();
        image.color = color;
        image.raycastTarget = false;
    }

    private static Button CreateButton(Transform parent, string objectName, string text, Vector2 position, Color baseColor, Color textColor)
    {
        GameObject obj = CreateUiObject(objectName, parent);
        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(320f, 68f);

        Image image = obj.AddComponent<Image>();
        image.color = baseColor;

        Button button = obj.AddComponent<Button>();
        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1.18f, 1.18f, 1.18f, 1f);
        colors.pressedColor = new Color(0.78f, 0.9f, 1f, 1f);
        colors.selectedColor = Color.white;
        colors.fadeDuration = 0.08f;
        button.colors = colors;
        button.targetGraphic = image;

        CreateAccentBar(obj.transform, "GlowEdge", new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, -2f), new Vector2(0f, 3f), textColor);

        TextMeshProUGUI label = CreateText(obj.transform, "Label", text, 34f, FontStyles.Bold, textColor);
        Stretch(label.rectTransform);
        label.alignment = TextAlignmentOptions.Center;
        label.characterSpacing = 5f;

        return button;
    }

    private void BeginRun()
    {
        GameManager.Instance?.BeginRun();
    }

    private static void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void ReadStartMenuShortcut()
    {
        if (GameManager.Instance == null || GameManager.Instance.State != GameState.Ready)
        {
            return;
        }

        Keyboard keyboard = Keyboard.current;
        if (keyboard != null && (keyboard.enterKey.wasPressedThisFrame || keyboard.spaceKey.wasPressedThisFrame))
        {
            BeginRun();
        }
    }

    private void AnimateStartMenu()
    {
        if (startMenuPanel == null || !startMenuPanel.activeSelf)
        {
            return;
        }

        titlePulseOffset += Time.unscaledDeltaTime;
        float pulse = 0.78f + Mathf.Sin(titlePulseOffset * 2.8f) * 0.22f;

        if (titleText != null)
        {
            titleText.color = Color.Lerp(new Color(0.45f, 0.88f, 1f, 0.9f), new Color(1f, 0.24f, 0.78f, 1f), pulse);
        }

        if (promptText != null)
        {
            float promptAlpha = 0.62f + Mathf.Sin(titlePulseOffset * 3.4f) * 0.2f;
            promptText.color = new Color(0.82f, 0.92f, 1f, promptAlpha);
        }
    }

    private void SetHudVisible(bool visible)
    {
        SetObjectVisible(scoreText, visible);
        SetObjectVisible(speedText, visible);

        if (energySlider != null)
        {
            energySlider.gameObject.SetActive(visible);
        }
    }

    private static void SetObjectVisible(Component component, bool visible)
    {
        if (component != null)
        {
            component.gameObject.SetActive(visible);
        }
    }
}
