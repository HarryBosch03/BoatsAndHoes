using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
[DisallowMultipleComponent]
public sealed class Switch : InteractableBase
{
    [SerializeField] Pose offPose;
    [SerializeField] Pose onPose;
    [SerializeField] Transform switchHandle;

    bool state;

    public override string InteractionDisplayName => "Toggle Switch";

    private void FixedUpdate()
    {
        if (state)
        {
            PowerInterface.DistributePower(GetComponentsInChildren<PowerInterface>());
        }
    }

    public override void Interact(GameObject interactor)
    {
        state = !state;

        switchHandle.localPosition = state ? onPose.position : offPose.position;
        switchHandle.localRotation = state ? onPose.rotation : offPose.rotation;
    }
}
