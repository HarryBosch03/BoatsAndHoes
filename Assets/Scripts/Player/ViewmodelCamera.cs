using UnityEngine;

[RequireComponent(typeof(Camera))]
public class ViewmodelCamera : MonoBehaviour
{
    public static float ViewmodelFOV { get; set; }

    new Camera camera;

    private void Awake()
    {
        camera = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        camera.fieldOfView = ViewmodelFOV;
    }
}
