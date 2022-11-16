using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
[DisallowMultipleComponent]
public sealed class PowerConnection : MonoBehaviour
{
    [SerializeField] PowerInterface a;
    [SerializeField] PowerInterface b;

    public PowerInterface A => a;
    public PowerInterface B => b;

    private void FixedUpdate()
    {
        float diff = a.Volume - b.Volume;
        float push = Mathf.Min(a.Push, b.Push);
        float change = diff * push * Time.deltaTime;

        if (a.Volume - change > a.Capacity) change = a.Volume - a.Capacity;
        if (b.Volume + change > b.Capacity) change = b.Capacity - b.Volume;

        if (a.Volume - change < 0.0f) change = a.Volume;
        if (b.Volume + change < 0.0f) change = -b.Volume;

        a.Volume -= change;
        b.Volume += change;
    }

    private void Reset()
    {
        a = GetComponent<PowerInterface>();
    }
}
