using UnityEngine;
using UnityEngine.InputSystem;

[SelectionBase]
[DisallowMultipleComponent]
public sealed class FPCameraController : MonoBehaviour
{
    [SerializeField] float mouseSensitivity;
    [SerializeField] Transform cameraRotor;

    [Space]
    [SerializeField] CameratorDriver cdriver;

    public Vector2 ScreenSpaceRotation { get; private set; }
    public float FOVOverride { get; set; }

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    private void LateUpdate()
    {
        Vector2 ssr = ScreenSpaceRotation;

        float zoomSens = Mathf.Tan(cdriver.Settings.fov * Mathf.Deg2Rad * 0.5f);
        if (Mouse.current != null) ssr += Mouse.current.delta.ReadValue() * mouseSensitivity * zoomSens;

        ssr.y = Mathf.Clamp(ssr.y, -90.0f, 90.0f);

        transform.rotation = Quaternion.Euler(0.0f, ssr.x, 0.0f);
        cameraRotor.rotation = Quaternion.Euler(-ssr.y, ssr.x, 0.0f);

        ScreenSpaceRotation = ssr;

        var settings = cdriver.Settings;
        settings.fov = FOVOverride;
        cdriver.Settings = settings;
        FOVOverride = 90.0f;
    }
}
