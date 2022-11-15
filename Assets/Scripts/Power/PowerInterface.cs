using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[SelectionBase]
[DisallowMultipleComponent]
public sealed class PowerInterface : MonoBehaviour
{
    [SerializeField] float capacity;
    [SerializeField] float flow;
    [SerializeField] float supply;
    [SerializeField] string channel;

    [Space]
    [SerializeField] Image indicator;
    [SerializeField] Gradient gradient;
    [SerializeField] bool useIndicatorFill;
    [SerializeField] bool useIndicatorColor;

    public float Fill { get; set; }
    public float Flow => flow;
    public float Supply { get => supply; set => supply = value; }
    public float DrainThisFrame { get; private set; }
    public string Channel => channel.ToLower().Trim();

    private void FixedUpdate()
    {
        var delta = Mathf.Max(Mathf.Min(capacity - Fill, supply * Time.deltaTime), -Fill);
        Fill += delta;
        DrainThisFrame = delta;

        if (indicator)
        {
            if (useIndicatorColor)
            {
                indicator.color = gradient.Evaluate(Fill / capacity);
            }
            if (useIndicatorFill)
            {
                indicator.fillAmount = Fill / capacity;
            }
        }
    }

    public bool TryFill(float fill, out float overflow) => TryDraw(-fill, out overflow);
    public bool TryDraw(float draw, out float overflow)
    {
        Fill -= draw;

        if (Fill > capacity)
        {
            overflow = Fill - capacity;
            Fill = capacity;
            return false;
        }

        if (Fill < 0.0f)
        {
            overflow = Fill;
            Fill = 0.0f;
            return false;
        }

        overflow = 0.0f;
        return true;
    }

    public static void DistributePower(IList<PowerInterface> connections)
    {
        for (int i = 0; i < connections.Count; i++)
        {
            for (int j = i + 1; j < connections.Count; j++)
            {
                var connA = connections[i];
                var connB = connections[j];

                float diff = connA.Fill - connB.Fill;
                float delta = diff * (connA.flow + connB.flow) * 0.5f * Time.deltaTime;

                if (connA.Fill - delta < 0.0f) delta = connA.Fill;
                if (connB.Fill + delta < 0.0f) delta = -connB.Fill;

                if (connA.Fill - delta > connA.capacity) delta = connA.Fill - connA.capacity;
                if (connB.Fill + delta > connB.capacity) delta = connB.capacity - connB.Fill;

                connA.Fill -= delta;
                connB.Fill += delta;
            }
        }
    }
}