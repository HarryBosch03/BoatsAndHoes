using UnityEngine;

[System.Serializable]
public class CameratorSettings
{
    public Vector3 position;
    public Quaternion rotation;
    public float fov;
    public float nearPlane;
    public float farPlane;

    public CameratorSettings() { }

    public CameratorSettings(Vector3 position, Quaternion rotation, float fov, float nearPlane, float farPlane)
    {
        this.position = position;
        this.rotation = rotation;
        this.fov = fov;
        this.nearPlane = nearPlane;
        this.farPlane = farPlane;
    }

    public CameratorSettings(Camera cam)
    {
        position = cam.transform.position;
        rotation = cam.transform.rotation;
        fov = cam.fieldOfView;
        nearPlane = cam.nearClipPlane;
        farPlane = cam.farClipPlane;
    }

    public static CameratorSettings Lerp (CameratorSettings a, CameratorSettings b, float t)
    {
        CameratorSettings r = new CameratorSettings();

        r.position = Vector3.Lerp(a.position, b.position, t);
        r.rotation = Quaternion.Slerp(a.rotation, b.rotation, t);
        r.fov = Mathf.Lerp(a.fov, b.fov, t);
        r.nearPlane = Mathf.Lerp(a.nearPlane, b.nearPlane, t);
        r.farPlane = Mathf.Lerp(a.farPlane, b.farPlane, t);

        return r;
    }

    public void Apply (Camera cam)
    {
        cam.transform.position = position;
        cam.transform.rotation = rotation;
        cam.fieldOfView = fov;
        cam.nearClipPlane = nearPlane;
        cam.farClipPlane = farPlane;
    }
}
