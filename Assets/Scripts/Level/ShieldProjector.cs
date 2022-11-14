using UnityEngine;

[SelectionBase]
[DisallowMultipleComponent]
[RequireComponent(typeof(PowerInterface))]
public sealed class ShieldProjector : MonoBehaviour
{
    [SerializeField] Vector2 size;
    [SerializeField] float smoothTime;
    [SerializeField] Transform handle;
    [SerializeField] bool state;
    [SerializeField] float chargeThreshold;

    PowerInterface pInterface;
    float velocity;

    private void Awake()
    {
        pInterface = GetComponent<PowerInterface>();
    }

    private void FixedUpdate()
    {
        float width = handle.localScale.x;
        width = Mathf.SmoothDamp(width, state ? size.x : 0.0f, ref velocity, smoothTime);
        handle.localScale = new Vector3(width, 1.0f, width > 0.1f ? size.y : 0.0f);

        state = pInterface.Fill > chargeThreshold;
    }

    private void Update()
    {
        if (UnityEngine.InputSystem.Keyboard.current.enterKey.wasPressedThisFrame) state = !state;
    }

    private void OnValidate()
    {
        if (handle)
        {
            handle.transform.localScale = new Vector3(size.x, 1.0f, size.y);
        }
    }
}
