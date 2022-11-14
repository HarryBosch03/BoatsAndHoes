using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

[SelectionBase]
[DisallowMultipleComponent]
public sealed class PowerInterface : MonoBehaviour
{
    [SerializeField] float capacity;
    [SerializeField] float flow;
    [SerializeField] float supply;
    [SerializeField] string channel;

    public float Fill { get; set; }
    public float Flow => flow;
    public string Channel => channel.ToLower().Trim();

    private void FixedUpdate()
    {
        Fill = Mathf.Clamp(Fill + supply * Time.deltaTime, 0.0f, capacity);
    }

    public bool TryFill(float fill) => TryDraw(-fill);
    public bool TryDraw (float draw)
    {
        Fill = Mathf.Clamp(Fill - draw, 0.0f, capacity);
        return Fill > 0;
    }

    public static void DistributePower (IEnumerable<PowerInterface> connections)
    {
        float avg = 0.0f;
        float max = 0.0f;
        float min = 0.0f;
        float flow = 0.0f;
        foreach (var pInterface in connections)
        {
            avg += pInterface.Fill;
            max = Mathf.Max(pInterface.Fill, max);
            min = Mathf.Min(pInterface.Fill, min);
            flow += pInterface.Flow;
        }
        avg /= connections.Count();
        flow /= connections.Count();
        float rng = max - min;

        foreach (var pInterface in connections)
        {
            pInterface.TryFill((avg - pInterface.Fill) * flow * rng * Time.deltaTime);
        }
    }
}