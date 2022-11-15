using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[SelectionBase]
[DisallowMultipleComponent]
public sealed class PowerInterface : MonoBehaviour
{
    [SerializeField] float supply;
    [SerializeField] string channelEditor;

    public float Supply { get => supply; set => supply = value; }
    public float Energy { get; set; }
    public string Channel { get; set; }

    public static Dictionary<string, HashSet<PowerInterface>> All { get; } = new Dictionary<string, HashSet<PowerInterface>>();


    private void OnEnable()
    {
        var _ = PowerManager.Instance;

        Channel = channelEditor;
        if (!All.ContainsKey(Channel)) All.Add(Channel, new HashSet<PowerInterface>());
        All[Channel].Add(this);
    }

    private void OnDisable()
    {
        All[Channel].Remove(this);
        if (All[Channel].Count == 0) All.Remove(Channel);
    }

    private void Update()
    {
        channelEditor = Channel;
    }
}

public class PowerBridge
{
    public string channelA;
    public string channelB;
    public bool active;
    public bool dead;

    public static HashSet<PowerBridge> All { get; } = new HashSet<PowerBridge>();

    public PowerBridge ()
    {
        All.Add(this);
    }

    public PowerBridge(string channelA, string channelB) : this()
    {
        this.channelA = channelA;
        this.channelB = channelB;
    }

    public void Delete ()
    {
        dead = true;
        All.Remove(this);
    }
}