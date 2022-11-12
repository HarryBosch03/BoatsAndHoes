using UnityEngine;
using UnityEngine.Rendering.UI;

[SelectionBase]
[DisallowMultipleComponent]
public class WeaponAnimator : MonoBehaviour
{
    public Transform root;

    [Space]
    public float weaponSway;
    public float weaponSwaySmoothing;
    public AnimationCurve weaponSwayRemap;

    [Space]
    public Animator animator;

    [Space]
    public string fireAnimationName;
    public string randFireBlend;

    [Space]
    public string reloadAnimationName;
    public float reloadAnimationBlend;

    FPCameraController cameraController;
    CharacterMovement movement;

    protected WeaponEffect effect;
    protected WeaponAmmo ammo;

    Vector2 lastCamRotation;
    Vector2 swayPosition;
    Vector2 swayVelocity;

    float grounded;

    protected virtual void Awake()
    {
        cameraController = GetComponentInParent<FPCameraController>();
        movement = GetComponentInParent<CharacterMovement>();

        effect = GetComponent<WeaponEffect>();
        ammo = GetComponent<WeaponAmmo>();
    }

    protected virtual void OnEnable()
    {
        ammo.ReloadEvent += OnReload;
        effect.PostExecuteEvent += OnUseEvent;
    }

    protected virtual void OnDisable()
    {
        ammo.ReloadEvent -= OnReload;
        effect.PostExecuteEvent -= OnUseEvent;
    }

    protected virtual void OnReload()
    {
        animator.CrossFade(reloadAnimationName, reloadAnimationBlend);
    }

    private void OnUseEvent()
    {
        animator.SetFloat(randFireBlend, Random.value);
        animator.Play(fireAnimationName, 0, 0.0f);
    }

    protected virtual void LateUpdate()
    {
        ApplyMovementAnimation();

        Vector3 velocity = movement.DrivingRigidbody.velocity;
        float speed = new Vector2(velocity.x, velocity.z).magnitude;
        if (!movement.IsGrounded) speed = 0.0f;

        if (animator ? animator.isActiveAndEnabled : false)
        {
            animator.SetFloat("speed", speed);
        }
    }

    private void ApplyMovementAnimation()
    {
        if (!cameraController) return;

        Vector2 camDelta = cameraController.ScreenSpaceRotation - lastCamRotation;
        swayPosition = Vector2.SmoothDamp(swayPosition, camDelta * weaponSway, ref swayVelocity, weaponSwaySmoothing);

        float remapedSwayMagnitude = weaponSwayRemap.Evaluate(swayPosition.magnitude);
        Vector2 remapedSway = swayPosition.normalized * remapedSwayMagnitude;

        root.rotation *= Quaternion.Euler(remapedSway.y, 0.0f, remapedSway.x);
        lastCamRotation = cameraController.ScreenSpaceRotation;
    }
}