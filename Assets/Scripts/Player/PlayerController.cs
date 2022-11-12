using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[SelectionBase]
[DisallowMultipleComponent]
public sealed class PlayerController : MonoBehaviour, IController
{
    [SerializeField] GameObject avatarPrefab;

    PlayerInput input;

    public GameObject Avatar { get; private set; }
    public static HashSet<object> InputBlockers { get; } = new HashSet<object>();

    public Vector2 MoveDirection { get; private set; }
    public bool Jump { get; private set; } 
    public bool Shoot { get; private set; } 

    public bool Aim { get; private set; }
    public bool Reload { get; private set; }

    public bool Interact { get; private set; }

    public void Spawn(Vector3 point, bool force = false)
    {
        if (Avatar)
        {
            if (force)
            {
                Destroy(Avatar);
            }
            else return;
        }

        Avatar = Instantiate(avatarPrefab, point, Quaternion.identity, transform);
    }

    public void OnMove(InputValue value) => MoveDirection = value.Get<Vector2>();
    public void OnJump(InputValue value) => Jump = value.Get<float>() > 0.5f;
    public void OnShoot(InputValue value) => Shoot= value.Get<float>() > 0.5f;
    public void OnReload(InputValue value) => Reload= value.Get<float>() > 0.5f;
    public void OnAim(InputValue value) => Aim= value.Get<float>() > 0.5f;
    public void OnInteract(InputValue value) => Interact = value.Get<float>() > 0.5f;

    private void Awake()
    {
        input = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        if (input)
        {
            input.enabled = InputBlockers.Count == 0;
        }

        if (Avatar)
        {
            if (Avatar.TryGetComponent(out FPCameraController cameraController))
            {
                cameraController.enabled = InputBlockers.Count == 0;
            }
        }

        if (Keyboard.current.numpad7Key.wasPressedThisFrame)
        {
            Spawn(Vector3.zero);
        }
    }
}
