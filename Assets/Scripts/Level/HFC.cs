using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
[DisallowMultipleComponent]
public sealed class HFC : MonoBehaviour
{
    [SerializeField] float expectedDrainRate;
    [SerializeField] float durationMinutes;
    [SerializeField] [Range(0.0f, 1.0f)]float fillPercent;

    [Space]
    [SerializeField] float peakBrightness;

    Material mat;

    public float Capacity => expectedDrainRate * 60.0f * durationMinutes;
    public float Fill
    {
        get => fillPercent * Capacity;
        set => fillPercent = value / Capacity;
    }

    private void Awake()
    {
        mat = GetComponent<Renderer>().material;
    }

    private void Update()
    {
        mat.SetFloat("_Emmision_Brightness", fillPercent * peakBrightness);
    }
}
