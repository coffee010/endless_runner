using UnityEngine;

public enum ObstacleType
{
    LaneBlock,
    Low,
    High
}

public sealed class Obstacle : MonoBehaviour
{
    [SerializeField] private ObstacleType obstacleType = ObstacleType.LaneBlock;

    public bool CanBeIgnoredBy(RunnerController runner)
    {
        if (runner == null)
        {
            return false;
        }

        if (obstacleType == ObstacleType.Low)
        {
            return !runner.IsGrounded;
        }

        if (obstacleType == ObstacleType.High)
        {
            return runner.IsSliding;
        }

        return false;
    }
}
