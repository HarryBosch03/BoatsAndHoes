using UnityEngine;

[SelectionBase]
[DisallowMultipleComponent]
public sealed class PlayerInventoryDrawer : MonoBehaviour
{
    [SerializeField] CanvasGroup inventoryGroup;
    [SerializeField] float fadeSpeed;

    IController controller;
    bool state;

    private void Awake()
    {
        inventoryGroup.alpha = 0.0f;
    }

    private void OnDisable()
    {
        if (controller != null) controller.ToggleInventoryEvent -= ToggleState;
    }

    private void FixedUpdate()
    {
        inventoryGroup.alpha += ((state ? 1.0f : 0.0f) - inventoryGroup.alpha) * fadeSpeed * Time.deltaTime;
        inventoryGroup.blocksRaycasts = state;
        inventoryGroup.interactable = state;
    }

    private void Update()
    {
        IController newController = GetComponentInParent<IController>();
        if (newController != controller)
        {
            if (controller != null) newController.ToggleInventoryEvent -= ToggleState;
            if (newController != null) newController.ToggleInventoryEvent += ToggleState;
        }

        controller = newController;
    }

    public void ToggleState ()
    {
        SwitchState(!state);
    }

    private void SwitchState(bool newState)
    {
        if (newState == state) return;

        if (newState)
        {
            GetComponent<SingleInventoryDrawer>().Rebuild();
            PlayerController.InputBlockers.Add(this);
        }
        else PlayerController.InputBlockers.Remove(this);

        state = newState;
    }
}
