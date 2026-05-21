using System;
using UnityEngine;

public sealed class ScoreManager : MonoBehaviour
{
    [SerializeField] private RunnerController runner;
    [SerializeField] private float distanceScoreMultiplier = 1f;
    [SerializeField] private float speedIncreasePerSecond = 0.15f;
    [SerializeField] private float maxSpeedBonus = 8f;

    private float distanceScore;
    private int bonusScore;
    private float speedBonus;

    public int Score => Mathf.FloorToInt(distanceScore) + bonusScore;
    public float Distance => distanceScore / Mathf.Max(distanceScoreMultiplier, 0.0001f);
    public float SpeedBonus => speedBonus;

    public event Action<int> ScoreChanged;

    private void Update()
    {
        if (GameManager.Instance == null || !GameManager.Instance.IsPlaying)
        {
            return;
        }

        float speed = runner != null ? runner.CurrentSpeed : 0f;
        distanceScore += speed * distanceScoreMultiplier * Time.deltaTime;
        speedBonus = Mathf.Min(maxSpeedBonus, speedBonus + speedIncreasePerSecond * Time.deltaTime);

        if (runner != null)
        {
            runner.SetSpeedBonus(speedBonus);
        }

        ScoreChanged?.Invoke(Score);
    }

    public void AddBonus(int value)
    {
        bonusScore += Mathf.Max(0, value);
        ScoreChanged?.Invoke(Score);
    }
}
