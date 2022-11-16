using System.Text;
using UnityEditor;
using UnityEditor.MemoryProfiler;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(PowerInterface))]
public sealed class PowerInterfaceEditor : Editor
{
    private void OnSceneGUI()
    {
        var allInterfaces = FindObjectsOfType<PowerInterface>();
        foreach (var pInterface in allInterfaces)
        {
            StringBuilder b = new StringBuilder();
            b.Append($"{pInterface.name}\n");
            b.Append($"Push: {pInterface.Push}\n");
            b.Append($"Volume: {pInterface.Volume}\n");
            b.Append($"Capacity: {pInterface.Capacity}\n");
            b.Append($"Supply: {pInterface.Supply}\n");
            b.Append($"Frame Loss: {pInterface.FrameSupply}\n");

            Handles.Label(pInterface.transform.position + Vector3.up * 0.5f, b.ToString());
        }

        var allConnections = FindObjectsOfType<PowerConnection>();
        foreach (var conn in allConnections)
        {
            if (!conn.A || !conn.B) continue;

            Handles.DrawLine(conn.A.transform.position, conn.B.transform.position, 5.0f);
        }
    }
}
