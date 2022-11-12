using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Spline))]
public class SplineEditor : Editor
{
    private void OnSceneGUI()
    {
        Spline spline = target as Spline;

        for (int i = 0; i < spline.transform.childCount; i++)
        {
            Transform handle = spline.transform.GetChild(i);

            Vector3 p = handle.position; Quaternion r = handle.rotation; Vector3 s = handle.localScale;

            switch (Tools.current)
            {
                case Tool.Rotate:
                    r = Handles.RotationHandle(r, p);
                    break;
                case Tool.Scale:
                    s = Handles.ScaleHandle(s, p, r);
                    break;
                case Tool.Transform:
                    Handles.TransformHandle(ref p, ref r, ref s);
                    break;
                case Tool.Move:
                default:
                    p = Handles.PositionHandle(p, r);
                    break;
            }

            handle.position = p; handle.rotation = r; handle.localScale = s;
        }
    }
}
