using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions.Must;

[SelectionBase]
[DisallowMultipleComponent]
public sealed class PowerManager : MonoBehaviour
{
    static PowerManager _instance;
    public static PowerManager Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = new GameObject("Power Manager").AddComponent<PowerManager>();
            }
            return _instance;
        }
    }

    private void OnEnable()
    {
        if (_instance) Destroy(_instance.gameObject);
        _instance = this;
    }

    private void Update()
    {
        HashSet<string> exclude = new HashSet<string>();

        foreach (var channel in PowerInterface.All)
        {
            if (exclude.Contains(channel.Key)) continue;

            var interfaces = new List<PowerInterface>(channel.Value);
            foreach (var bridge in PowerBridge.All)
            {
                if (!bridge.active) continue;
                if (bridge.channelA == bridge.channelB) continue;
                if (bridge.channelA == channel.Key)
                {
                    interfaces.AddRange(PowerInterface.All[bridge.channelB]);
                    exclude.Add(bridge.channelB);
                }
                if (bridge.channelB == channel.Key)
                {
                    interfaces.AddRange(PowerInterface.All[bridge.channelA]);
                    exclude.Add(bridge.channelA);
                }
            }

            float netDraw = 0.0f;
            float netSupply = 0.0f;
            foreach (var pInterface in interfaces)
            {
                if (pInterface.Supply > 0.0f) netSupply += pInterface.Supply;
                else netDraw -= pInterface.Supply;
            }

            float delta = Mathf.Min(netDraw, netSupply);
            foreach (var pInterface in channel.Value)
            {
                if (pInterface.Supply > 0.0f)
                {
                    pInterface.Energy = -delta * (pInterface.Supply / netSupply);
                }
                else
                {
                    pInterface.Energy = delta * (-pInterface.Supply / netDraw);
                }
            }
        }
    }
}
