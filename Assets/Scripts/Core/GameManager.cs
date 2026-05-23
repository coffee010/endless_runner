using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public enum GameState
{
    Ready,
    Playing,
    GameOver
}

public sealed class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private bool startOnPlay = true;

    public GameState State { get; private set; } = GameState.Ready;
    public bool IsPlaying => State == GameState.Playing;

    public event Action<GameState> StateChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        SetState(startOnPlay ? GameState.Playing : GameState.Ready);
    }

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;
        if (State == GameState.GameOver && keyboard != null && keyboard.rKey.wasPressedThisFrame)
        {
            Restart();
        }
    }

    public void BeginRun()
    {
        SetState(GameState.Playing);
    }

    public void GameOver()
    {
        if (State == GameState.GameOver)
        {
            return;
        }

        SetState(GameState.GameOver);
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void SetState(GameState newState)
    {
        State = newState;
        Time.timeScale = newState == GameState.GameOver ? 0f : 1f;
        StateChanged?.Invoke(State);
    }
}
