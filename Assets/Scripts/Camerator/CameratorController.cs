using System.Linq;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class CameratorController : MonoBehaviour
{
    [SerializeField] bool lateUpdate;
    [SerializeField] bool fixedUpdate;
    [SerializeField] float blendTime;
    [SerializeField] AnimationCurve blendCurve;

    Camera cam;
    CameratorSettings from;
    float changeTime;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void OnEnable()
    {
        from = new CameratorSettings(cam);
        changeTime = Time.time;

        CameratorDriver.NewCurrentDriverEvent += OnNewDriver;
    }

    private void OnDisable()
    {
        CameratorDriver.NewCurrentDriverEvent -= OnNewDriver;
    }

    private void OnNewDriver(CameratorDriver newDriver)
    {
        from = new CameratorSettings(cam);
        changeTime = Time.time;
    }

    private void LateUpdate()
    {
        if (!lateUpdate) return;

        UpdateCamera();
    }

    private void FixedUpdate()
    {
        if (!fixedUpdate) return;

        UpdateCamera();
    }

    private void UpdateCamera()
    {
        if (!CameratorDriver.CurrentDriver) return;

        float percent = blendCurve.Evaluate(Mathf.Clamp01((Time.time - changeTime) / blendTime));

        CameratorSettings.Lerp(from, CameratorDriver.CurrentDriver.Settings, percent).Apply(cam);
    }
}
