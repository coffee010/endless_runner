using UnityEngine;

[RequireComponent(typeof(RunnerController))]
public sealed class PlayerCollision : MonoBehaviour
{
    [SerializeField] private ScoreManager scoreManager;

    private RunnerController runner;

    private void Awake()
    {
        runner = GetComponent<RunnerController>();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Obstacle obstacle = hit.collider.GetComponentInParent<Obstacle>();
        if (obstacle != null)
        {
            HandleObstacle(obstacle);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Collectible collectible))
        {
            collectible.Collect(scoreManager);
        }
    }

    private void HandleObstacle(Obstacle obstacle)
    {
        if (obstacle.CanBeIgnoredBy(runner))
        {
            return;
        }

        FailIfVulnerable();
    }

    private void FailIfVulnerable()
    {
        if (runner != null && runner.IsInvulnerable)
        {
            return;
        }

        GameManager.Instance?.GameOver();
    }
}
