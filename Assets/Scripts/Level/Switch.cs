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
    [SerializeField] float animationSpeed;
    
    [Space]    
    [SerializeField] string channelA;
    [SerializeField] string channelB;

    [Space]
    [SerializeField] bool state;

    PowerBridge bridge;

    public override string InteractionDisplayName => "Toggle Switch";

    private void Start()
    {
        SetState(state);
    }

    private void OnEnable()
    {
        bridge = new PowerBridge(channelA, channelB);
    }

    private void OnDisable()
    {
        bridge.Delete();
    }

    private void Update()
    {
        channelA = bridge.channelA;
        channelB = bridge.channelB;
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
        bridge.active = state;
    }
}
