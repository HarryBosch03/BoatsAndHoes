using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
[ExecuteAlways]
public class CameratorDriver : MonoBehaviour
{
    [SerializeField] CameratorSettings settings;
    [SerializeField] int priority;

    public int Priority => priority;

    public CameratorSettings Settings
    {
        get
        {
            settings.position = transform.position;
            settings.rotation = transform.rotation;
            return settings;
        }
        set => settings = value;
    }
    public static HashSet<CameratorDriver> Drivers = new HashSet<CameratorDriver>();

    public static event System.Action<CameratorDriver> NewCurrentDriverEvent;
    public static event System.Action<CameratorDriver> ActiveCameraDisabledEvent;
    public static CameratorDriver CurrentDriver;

    static CameratorDriver ()
    {
        CurrentDriver = null;
    }

    private void OnEnable()
    {
        TrySwitchDriver();

        ActiveCameraDisabledEvent += OnActiveCameraDisabled;
    }

    private void OnActiveCameraDisabled(CameratorDriver oldCamera)
    {
        TrySwitchDriver();
    }

    private void TrySwitchDriver()
    {
        if (!CurrentDriver)
        {
            CurrentDriver = this;
            NewCurrentDriverEvent?.Invoke(this);
        }

        if (priority > CurrentDriver.priority)
        {
            CurrentDriver = this;
            NewCurrentDriverEvent?.Invoke(this);
        }
    }

    private void OnDisable()
    {
        ActiveCameraDisabledEvent -= OnActiveCameraDisabled;
        CurrentDriver = null;

        if (CurrentDriver == this) ActiveCameraDisabledEvent?.Invoke(this);
    }
}
