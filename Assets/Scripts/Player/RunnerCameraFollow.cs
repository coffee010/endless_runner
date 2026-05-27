using UnityEngine;

public sealed class RunnerCameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0f, 5f, -8f);
    [SerializeField] private Vector3 lookAtOffset = new Vector3(0f, 1.5f, 5f);
    [SerializeField] private float positionSmoothTime = 0.08f;
    [SerializeField] private float rotationSmoothSpeed = 12f;
    [SerializeField] private bool followLaneX = false;

    private Vector3 positionVelocity;

    private void Awake()
    {
        if (target == null)
        {
            RunnerController runner = FindAnyObjectByType<RunnerController>();
            if (runner != null)
            {
                target = runner.transform;
            }
        }
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        Vector3 basePosition = target.position;
        if (!followLaneX)
        {
            basePosition.x = 0f;
        }

        Vector3 desiredPosition = basePosition + offset;
        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref positionVelocity,
            positionSmoothTime);

        Vector3 lookPoint = target.position + lookAtOffset;
        if (!followLaneX)
        {
            lookPoint.x = 0f;
        }

        Vector3 lookDirection = lookPoint - transform.position;
        if (lookDirection.sqrMagnitude <= 0.0001f)
        {
            return;
        }

        Quaternion desiredRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            desiredRotation,
            rotationSmoothSpeed * Time.deltaTime);
    }
}
