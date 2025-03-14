using System.Linq;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using static AnimationManager;

public class PlayerMovement : MonoBehaviour
{
    public float movementSpeed = 5f;
    [SerializeField] float baseMovementSpeed = 5;
    public float sprintAdittion = 5f;
    public float jumpForce = 18f;
    public float jumpTime = 0.85f;
    public float gravity = 9.8f;
    Statistics statistics;
    bool isJumping = false;
    bool isSprinting = false;
    Rigidbody rb;
    [SerializeField] float sphereRadius;
    [SerializeField] float checkDistance;
    float inputHorizontal;
    float inputVertical;
    private bool hasReachedPeak;
    AnimationManager animationManager;
    Health health;
    public float rotationSpeed = 5f;
    [SerializeField]
    [Range(0.5f, 100f)]
    float mouseSense = 20;

    private void OnEnable()
    {
        animationManager = GetComponentInChildren<AnimationManager>();
        rb = GetComponent<Rigidbody>();
        if (InputManager.Instance != null)
        {
            Debug.Log("Input setup");
            InputManager.Instance.OnMove += MoveInput;
            InputManager.Instance.OnToggleSprint += ToggleSprint;
            InputManager.Instance.OnJump += Jump;
        }
        statistics = GetComponent<Statistics>();
        health = GetComponent<Health>();
        if (health != null)
        {
            health.OnDeath += DisableWhenDead;
        }
        movementSpeed = baseMovementSpeed * (1 + statistics.MovementSpeed / 100f);
    }

    private void OnDisable()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnMove -= MoveInput;
            InputManager.Instance.OnToggleSprint -= ToggleSprint;
            InputManager.Instance.OnJump -= Jump;
        }
        if (health != null)
        {
            health.OnDeath -= DisableWhenDead;
        }
    }
    private void DisableWhenDead()
    {
        InputManager.Instance.OnMove -= MoveInput;
        this.enabled = false;
    }
    private void ToggleSprint(bool state)
    {
        isSprinting = state;

        animationManager.ToggleLayer(AnimationLayer.Shield, !state, 0.1f);
        animationManager.ToggleLayer(AnimationLayer.Attack, !state, 0.1f);

    }

    public void MoveInput(Vector2 input)
    {
        inputHorizontal = input.x;
        inputVertical = input.y;
    }
    public void Jump()
    {
        if (IsGrounded() && !isJumping)
        {
            isJumping = true;
            hasReachedPeak = false;
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            animationManager.ToggleLayer(AnimationLayer.Attack, false, 0f);
            animationManager.PlaySingleAnimation(SingleAnimations.jump);
        }
    }
    private void Update()
    {
        IsGrounded();
        if (isJumping && rb.velocity.y < 0.1) { hasReachedPeak = true; }
    }
    private void FixedUpdate()
    {
        float velocityAddition = 0;
        if (isSprinting)
        {
            velocityAddition = sprintAdittion;
        }
        float directionX = inputHorizontal * (movementSpeed + velocityAddition);
        float directionZ = inputVertical * (movementSpeed + velocityAddition);
        float directionY = rb.velocity.y;
        directionY = directionY - gravity * Time.deltaTime;
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();
        Vector3 horizontalVelocity = (forward * directionZ) + (right * directionX);

        if (!isJumping)
        {
            rb.velocity = new Vector3(horizontalVelocity.x, directionY, horizontalVelocity.z);
            animationManager.AnimateMovement(rb.velocity.x, rb.velocity.z);
        }

    }
    private void LateUpdate()
    {
        RotateBasedOnMouseInput();
    }

    private void RotateBasedOnMouseInput()
    {
        float mouseDeltaX = Input.GetAxis("Mouse X") * mouseSense * Time.deltaTime;
        transform.Rotate(0, mouseDeltaX, 0);
    }
    private bool IsGrounded()
    {
        RaycastHit hit;
        Vector3 origin = transform.position + Vector3.up * sphereRadius;
        int groundLayer = LayerMask.GetMask("Ground");
        if (Physics.SphereCast(origin, sphereRadius, Vector3.down, out hit, checkDistance, groundLayer))
        {
            if (hasReachedPeak)
            {
                animationManager.ToggleLayer(AnimationLayer.Attack, true, 0f);
                isJumping = false;
                hasReachedPeak = false;
            }
            return true;
        }
        return false;
    }


}
