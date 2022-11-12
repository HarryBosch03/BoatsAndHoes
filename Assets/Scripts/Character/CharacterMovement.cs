using Palmmedia.ReportGenerator.Core.Reporting.Builders;
using UnityEngine;

[SelectionBase]
[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
public sealed class CharacterMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 10.0f;
    [SerializeField] float groundAcceleration = 8.0f;

    [Space]
    [SerializeField] float airMoveForce = 5.0f;

    [Space]
    [SerializeField] float jumpHeight = 3.5f;
    [SerializeField] float upGravity = 2.0f;
    [SerializeField] float downGravity = 3.0f;
    [SerializeField] float jumpSpringPauseTime = 0.1f;

    [Space]
    [SerializeField] float springDistance = 1.2f;
    [SerializeField] float springForce = 250.0f;
    [SerializeField] float springDamper = 15.0f;
    [SerializeField] float groundCheckRadius = 0.2f;
    [SerializeField] LayerMask groundCheckMask = 1;

    bool previousJumpState;
    float lastJumpTime;

    public float MoveSpeed => moveSpeed;
    public Rigidbody DrivingRigidbody { get; private set; }

    public float DistanceToGround { get; private set; }
    public bool IsGrounded => DistanceToGround < springDistance;
    public Rigidbody Ground { get; private set; }

    private void Awake()
    {
        DrivingRigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        IController controller = GetComponentInParent<IController>();

        DistanceToGround = GetDistanceToGround();

        MoveCharacter(controller);

        if (controller.Jump && !previousJumpState)
        {
            Jump(controller);
        }
        previousJumpState = controller.Jump;

        ApplySpring();
        ApplyGravity(controller);
    }

    private void ApplySpring()
    {
        if (IsGrounded && Time.time > lastJumpTime + jumpSpringPauseTime)
        {
            float contraction = 1.0f - (DistanceToGround / springDistance);
            Vector3 force = Vector3.up * contraction * springForce * Time.deltaTime;
            force -= Vector3.up * DrivingRigidbody.velocity.y * springDamper * Time.deltaTime;

            if (Ground) Ground.velocity -= force * DrivingRigidbody.mass / Ground.mass;
            DrivingRigidbody.velocity += force;
        }
    }

    private void ApplyGravity(IController controller)
    {
        DrivingRigidbody.useGravity = false;
        DrivingRigidbody.velocity += GetGravity(controller) * Time.deltaTime;
    }

    private void MoveCharacter(IController controller)
    {
        Vector2 input = controller.MoveDirection;
        Vector3 direction = transform.TransformDirection(input.x, 0.0f, input.y);

        if (IsGrounded)
        {
            Vector3 target = direction * moveSpeed;
            Vector3 current = DrivingRigidbody.velocity;

            Vector3 delta = Vector3.ClampMagnitude(target - current, moveSpeed);
            delta.y = 0.0f;

            Vector3 force = delta / moveSpeed * groundAcceleration;

            DrivingRigidbody.velocity += force * Time.deltaTime;
        }
        else
        {
            DrivingRigidbody.velocity += direction * airMoveForce * Time.deltaTime;
        }
    }

    private void Jump(IController controller)
    {
        if (IsGrounded)
        {
            float gravity = Vector3.Dot(Vector3.down, GetGravity(controller));
            float jumpForce = Mathf.Sqrt(2.0f * gravity * jumpHeight);
            DrivingRigidbody.velocity = new Vector3(DrivingRigidbody.velocity.x, jumpForce, DrivingRigidbody.velocity.z);

            lastJumpTime = Time.time;
        }
    }

    private Vector3 GetGravity(IController controller)
    {
        float scale = upGravity;
        if (!controller.Jump)
        {
            scale = downGravity;
        }
        else if (DrivingRigidbody.velocity.y < 0.0f)
        {
            scale = downGravity;
        }

        return Physics.gravity * scale;
    }

    public float GetDistanceToGround()
    {
        Ground = null;
        if (Physics.SphereCast(DrivingRigidbody.position + Vector3.up * groundCheckRadius, groundCheckRadius, Vector3.down, out var hit, 1000.0f, groundCheckMask))
        {
            if (hit.rigidbody)
            {
                Ground = hit.rigidbody;
            }
            return hit.distance;
        }
        else return float.PositiveInfinity;
    }

    private void OnDrawGizmosSelected()
    {
        if (!DrivingRigidbody) DrivingRigidbody = GetComponent<Rigidbody>();
        float dist = GetDistanceToGround();
        Gizmos.color = dist < springDistance ? Color.green : Color.red;

        Gizmos.DrawRay(transform.position, Vector3.down * dist);

        Gizmos.color = Color.white;
    }
}
