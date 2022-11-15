using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[SelectionBase]
[DisallowMultipleComponent]
public sealed class PlayerController : MonoBehaviour, IController
{
    [SerializeField] GameObject avatarPrefab;

    ControlMap controlMap;

    public GameObject Avatar { get; private set; }
    public static HashSet<object> InputBlockers { get; } = new HashSet<object>();

    public Vector2 MoveDirection => GetBlockedInput<Vector2>(m => m.Move);
    public bool Jump => GetBlockedInputFlag(m => m.Jump);
    public bool Shoot => GetBlockedInputFlag(m => m.Shoot);

    public bool Aim => GetBlockedInputFlag(m => m.Aim);
    public bool Reload => GetBlockedInputFlag(m => m.Reload);

    public bool Interact => GetBlockedInputFlag(m => m.Interact);

    public bool InputBlocked => InputBlockers.Count > 0;

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

    public T GetBlockedInput<T> (System.Func<ControlMap.PlayerActions, InputAction> action) where T : struct
    {
        if (InputBlocked) return default;
        return action(controlMap.Player).ReadValue<T>();
    }

    public bool GetBlockedInputFlag(System.Func<ControlMap.PlayerActions, InputAction> action)
    {
        if (InputBlocked) return false;
        return action(controlMap.Player).ReadValue<float>() > 0.5f;
    }

    private void Awake()
    {
        controlMap = new ControlMap();
    }

    private void OnEnable()
    {
        controlMap.Enable();
    }

    private void OnDisable()
    {
        controlMap.Disable();
    }

    private void Update()
    {
        if (Avatar)
        {
            if (Avatar.TryGetComponent(out FPCameraController cameraController))
            {
                cameraController.enabled = !InputBlocked;
            }
        }

        if (Keyboard.current.numpad7Key.wasPressedThisFrame)
        {
            Spawn(Vector3.zero);
        }
    }
}
