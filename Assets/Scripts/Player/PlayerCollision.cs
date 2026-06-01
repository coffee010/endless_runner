using UnityEngine;

[RequireComponent(typeof(RunnerController))]
public sealed class PlayerCollision : MonoBehaviour
{
    [SerializeField] private ScoreManager scoreManager;

    private RunnerController runner;
    private EnergyModeController energyModeController;

    private void Awake()
    {
        runner = GetComponent<RunnerController>();
        energyModeController = GetComponent<EnergyModeController>();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Obstacle obstacle = hit.collider.GetComponentInParent<Obstacle>();
        if (obstacle == null)
        {
            return;
        }

        HandleObstacle(obstacle);
    }

    private void OnTriggerEnter(Collider other)
    {
        // 收集物
        if (other.TryGetComponent(out Collectible collectible))
        {
            collectible.Collect(scoreManager);
            return;
        }

        // 颜色门
        ColorGate gate = other.GetComponentInParent<ColorGate>();
        if (gate != null)
        {
            HandleColorGate(gate);
            return;
        }

        // 触发器类型的障碍物（Slow 力场、Knockback 弹射板）
        Obstacle obstacle = other.GetComponentInParent<Obstacle>();
        if (obstacle != null)
        {
            HandleObstacle(obstacle);
        }
    }

    // ──────────── 颜色门 ────────────

    private void HandleColorGate(ColorGate gate)
    {
        EnergyMode currentMode = energyModeController != null
            ? energyModeController.CurrentMode
            : EnergyMode.Blue;

        bool passed = gate.TryPass(currentMode);

        if (passed)
        {
            // 通过：加分
            if (scoreManager != null)
            {
                scoreManager.AddBonus(50);
            }
        }
        else
        {
            // 失败：扣分
            if (scoreManager != null)
            {
                scoreManager.AddPenalty(gate.WrongColorPenalty);
            }
        }
    }

    // ──────────── 障碍物 ────────────

    private void HandleObstacle(Obstacle obstacle)
    {
        if (obstacle.IsTriggered)
        {
            return;
        }

        if (obstacle.CanBeIgnoredBy(runner))
        {
            return;
        }

        ObstacleResponse response = obstacle.OnCollide();

        switch (response)
        {
            case ObstacleResponse.Kill:
                HandleKill(obstacle);
                break;
            case ObstacleResponse.Slow:
                HandleSlow(obstacle);
                break;
            case ObstacleResponse.Knockback:
                HandleKnockback(obstacle);
                break;
        }
    }

    private void HandleKill(Obstacle obstacle)
    {
        if (runner != null && runner.IsInvulnerable)
        {
            return;
        }

        GameManager.Instance?.GameOver();
    }

    private void HandleSlow(Obstacle obstacle)
    {
        if (runner != null)
        {
            runner.ApplySlow(obstacle.SlowMultiplier, obstacle.SlowDuration);
        }
    }

    private void HandleKnockback(Obstacle obstacle)
    {
        if (runner != null)
        {
            Vector3 knockback = -Vector3.forward * obstacle.KnockbackForce
                                + Vector3.up * obstacle.KnockbackUpward;
            runner.ApplyKnockback(knockback);
        }

        if (scoreManager != null)
        {
            scoreManager.AddPenalty(obstacle.PenaltyScore);
        }
    }
}
