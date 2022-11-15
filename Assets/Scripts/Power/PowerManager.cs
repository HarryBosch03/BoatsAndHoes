using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[SelectionBase]
[DisallowMultipleComponent]
public sealed class PowerManager : MonoBehaviour
{
    private void FixedUpdate()
    {
        var interfaces = FindObjectsOfType<PowerInterface>();

        var channels = new Dictionary<string, HashSet<PowerInterface>>();
        foreach (var pInterface in interfaces)
        {
            if (!channels.ContainsKey(pInterface.Channel))
            {
                channels.Add(pInterface.Channel, new HashSet<PowerInterface>());
            }
            channels[pInterface.Channel].Add(pInterface);
        }

        foreach (var pair in channels)
        {
            PowerInterface.DistributePower(pair.Value.ToArray());
        }
    }
}
