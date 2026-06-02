using UnityEngine;

[RequireComponent(typeof(RunnerController))]
public sealed class PlayerCollision : MonoBehaviour
{
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private EnergyBurst energyBurst;

    private RunnerController runner;
    private EnergyModeController energyModeController;

    private void Awake()
    {
        runner = GetComponent<RunnerController>();
        energyModeController = GetComponent<EnergyModeController>();

        if (energyBurst == null)
        {
            energyBurst = GetComponent<EnergyBurst>();
        }
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
        if (other.TryGetComponent(out Collectible collectible))
        {
            collectible.Collect(scoreManager);
            return;
        }

        ColorGate gate = other.GetComponentInParent<ColorGate>();
        if (gate != null)
        {
            HandleColorGate(gate);
            return;
        }

        Obstacle obstacle = other.GetComponentInParent<Obstacle>();
        if (obstacle != null)
        {
            HandleObstacle(obstacle);
        }
    }

    private void HandleColorGate(ColorGate gate)
    {
        EnergyMode currentMode = energyModeController != null
            ? energyModeController.CurrentMode
            : EnergyMode.Blue;

        bool passed = gate.TryPass(currentMode);
        if (energyBurst == null)
        {
            return;
        }

        energyBurst.AddEnergy(passed ? gate.CorrectEnergyReward : -gate.WrongEnergyPenalty);
    }

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
