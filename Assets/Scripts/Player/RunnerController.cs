using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public sealed class RunnerController : MonoBehaviour
{
    [Header("Forward Movement")]
    [SerializeField] private float baseForwardSpeed = 9f;
    [SerializeField] private float laneDistance = 2.5f;

    [Header("Lane Movement")]
    [SerializeField] private float laneSwitchSpeed = 12f;
    [SerializeField] private int startingLane = 1;

    [Header("Jump")]
    [SerializeField] private float jumpHeight = 2.4f;
    [SerializeField] private float gravity = -28f;

    [Header("Slide")]
    [SerializeField] private float slideDuration = 0.75f;
    [SerializeField] private float slideHeight = 1f;

    [Header("Rendering")]
    [SerializeField] private string playerLayerName = "Player";

    private CharacterController controller;
    private Vector3 velocity;
    private int currentLane;
    private float speedBonus;
    private bool isSliding;
    private float originalHeight;
    private Vector3 originalCenter;

    public float CurrentSpeed => baseForwardSpeed + speedBonus;
    public int CurrentLane => currentLane;
    public bool IsGrounded => controller != null && controller.isGrounded;
    public bool IsSliding => isSliding;
    public bool IsInvulnerable { get; private set; }

    private void Awake()
    {
        ApplyPlayerLayer(transform);

        controller = GetComponent<CharacterController>();
        originalHeight = controller.height;
        originalCenter = controller.center;
        currentLane = Mathf.Clamp(startingLane, 0, 2);
    }

    private void Update()
    {
        if (GameManager.Instance != null && !GameManager.Instance.IsPlaying)
        {
            return;
        }

        ReadLaneInput();
        ReadActionInput();
        Move();
    }

    public void SetSpeedBonus(float value)
    {
        speedBonus = Mathf.Max(0f, value);
    }

    public void SetInvulnerable(float seconds)
    {
        StartCoroutine(InvulnerabilityRoutine(seconds));
    }

    private void ReadLaneInput()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
        {
            return;
        }

        if (keyboard.aKey.wasPressedThisFrame || keyboard.leftArrowKey.wasPressedThisFrame)
        {
            currentLane = Mathf.Max(0, currentLane - 1);
        }
        else if (keyboard.dKey.wasPressedThisFrame || keyboard.rightArrowKey.wasPressedThisFrame)
        {
            currentLane = Mathf.Min(2, currentLane + 1);
        }
    }

    private void ReadActionInput()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
        {
            return;
        }

        if ((keyboard.spaceKey.wasPressedThisFrame || keyboard.wKey.wasPressedThisFrame || keyboard.upArrowKey.wasPressedThisFrame) && controller.isGrounded && !isSliding)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        if ((keyboard.sKey.wasPressedThisFrame || keyboard.downArrowKey.wasPressedThisFrame) && controller.isGrounded && !isSliding)
        {
            StartCoroutine(SlideRoutine());
        }
    }

    private void Move()
    {
        if (controller.isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
        }

        float targetX = (currentLane - 1) * laneDistance;
        float xDelta = Mathf.MoveTowards(transform.position.x, targetX, laneSwitchSpeed * Time.deltaTime) - transform.position.x;

        velocity.y += gravity * Time.deltaTime;

        Vector3 motion = new Vector3(xDelta, velocity.y * Time.deltaTime, CurrentSpeed * Time.deltaTime);
        controller.Move(motion);
    }

    private IEnumerator SlideRoutine()
    {
        isSliding = true;
        controller.height = slideHeight;
        controller.center = new Vector3(originalCenter.x, slideHeight * 0.5f, originalCenter.z);

        yield return new WaitForSeconds(slideDuration);

        controller.height = originalHeight;
        controller.center = originalCenter;
        isSliding = false;
    }

    private IEnumerator InvulnerabilityRoutine(float seconds)
    {
        IsInvulnerable = true;
        yield return new WaitForSeconds(seconds);
        IsInvulnerable = false;
    }

    private void ApplyPlayerLayer(Transform root)
    {
        int playerLayer = LayerMask.NameToLayer(playerLayerName);
        if (root == null || playerLayer < 0)
        {
            return;
        }

        root.gameObject.layer = playerLayer;
        foreach (Transform child in root)
        {
            ApplyPlayerLayer(child);
        }
    }
}
