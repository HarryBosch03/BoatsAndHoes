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
    [SerializeField] float speed;

    [SerializeField] bool state;

    public override string InteractionDisplayName => "Toggle Switch";

    private void Start()
    {
        SetState(state);
    }

    private void FixedUpdate()
    {
        if (state)
        {
            PowerInterface.DistributePower(GetComponentsInChildren<PowerInterface>());
        }

        Vector3 tPos = state ? onPose.position : offPose.position;
        Quaternion tRot = state ? onPose.rotation : offPose.rotation;

        switchHandle.localPosition = Vector3.Lerp(switchHandle.localPosition, tPos, speed * Time.deltaTime);
        switchHandle.localRotation = Quaternion.Slerp(switchHandle.localRotation, tRot, speed * Time.deltaTime);
    }

    public override void Interact(GameObject interactor)
    {
        SetState(!state);
    }

    public void SetState (bool state)
    {
        this.state = state;
    }
}
