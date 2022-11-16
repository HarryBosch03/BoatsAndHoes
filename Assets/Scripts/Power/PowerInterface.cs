using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[SelectionBase]
[DisallowMultipleComponent]
public sealed class PowerInterface : MonoBehaviour
{
    [SerializeField] float push;
    [SerializeField] float capacity;
    [SerializeField] float volume;
    [SerializeField] float supply;

    public float Push => push;
    public float Volume { get => volume; set => volume = value; }
    public float Capacity => capacity;
    public float Supply { get => supply; set => supply = value; }
    public float FrameSupply { get; private set; }

    private void FixedUpdate()
    {
        float frameSupply = Supply * Time.deltaTime;

        if (Volume + frameSupply > Capacity) frameSupply = Capacity - Volume;
        if (Volume + frameSupply < 0.0f) frameSupply = -Volume;

        Volume += frameSupply;
        FrameSupply = frameSupply;
    }
}