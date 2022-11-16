using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
[DisallowMultipleComponent]
[RequireComponent(typeof(PowerConnection))]
public sealed class Switch : InteractableBase
{
    [SerializeField] Pose offPose;
    [SerializeField] Pose onPose;
    [SerializeField] Transform switchHandle;
    [SerializeField] float animationSpeed;
    
    [Space]    
    [SerializeField] string channelA;
    [SerializeField] string channelB;

    [Space]
    [SerializeField] bool state;

    PowerConnection connection;
    
    public override string InteractionDisplayName => "Toggle Switch";

    private void Start()
    {
        connection = GetComponent<PowerConnection>();
        SetState(state);
    }

    private void FixedUpdate()
    {
        Vector3 tPos = state ? onPose.position : offPose.position;
        Quaternion tRot = state ? onPose.rotation : offPose.rotation;

        switchHandle.localPosition = Vector3.Lerp(switchHandle.localPosition, tPos, animationSpeed * Time.deltaTime);
        switchHandle.localRotation = Quaternion.Slerp(switchHandle.localRotation, tRot, animationSpeed * Time.deltaTime);
    }

    public override void Interact(GameObject interactor)
    {
        SetState(!state);
    }

    public void SetState (bool state)
    {
        this.state = state;
        connection.enabled = state;
    }
}
