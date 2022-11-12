using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ArrayTool : EditorWindow
{
    Mesh mesh;
    Mesh endCapA;
    Mesh endCapB;
    Material material;
    CollisionType type;
    float distanceIncrement;
    float accuracy;

    Vector3 rotation;
    Vector3 endCapRotationA;
    Vector3 endCapRotationB;

    List<GameObject> lastBake = new List<GameObject>();

    [MenuItem("Tools/Array Tool")]
    public static void Open()
    {
        CreateWindow<ArrayTool>("Array Tool");
    }

    private void OnGUI()
    {
        OField("Mesh", ref mesh);

        if (!mesh)
        {
            EditorGUILayout.HelpBox("Missing Mesh", MessageType.Warning);
            return;
        }

        OField("End Cap Start", ref endCapA);
        OField("End Cap End", ref endCapB);

        OField("Material", ref material);

        EditorGUILayout.Space();

        type = (CollisionType)EditorGUILayout.EnumFlagsField("Collision Type", type);

        EditorGUILayout.Space();

        rotation = EditorGUILayout.Vector3Field("Rotation", rotation);
        if (endCapA) endCapRotationA = EditorGUILayout.Vector3Field("End Cap Start Rotation", endCapRotationA);
        if (endCapB) endCapRotationB = EditorGUILayout.Vector3Field("End Cap End Rotation", endCapRotationB);

        EditorGUILayout.Space();
        distanceIncrement = EditorGUILayout.FloatField("Distance", distanceIncrement);
        accuracy = EditorGUILayout.FloatField("Accuracy", accuracy);

        List<Spline> splines = new List<Spline>();
        foreach (var go in Selection.gameObjects)
        {
            if (go.TryGetComponent(out Spline spline))
            {
                splines.Add(spline);
            }
        }

        if (splines.Count == 0)
        {
            EditorGUILayout.HelpBox("Select Spline Object to Bake", MessageType.Warning);
        }
        else
        {
            if (GUILayout.Button("Bake"))
            {
                Bake(splines);
            }
            if (GUILayout.Button("Rebake Previous"))
            {
                foreach (var previous in lastBake)
                {
                    DestroyImmediate(previous);
                }

                Bake(splines);
            }
        }
    }

    private void Bake(List<Spline> splines)
    {
        lastBake.Clear();

        List<GameObject> result = new List<GameObject>();

        int i = 0;

        foreach (Spline spline in splines)
        {
            GameObject space = new GameObject(splines.Count == 1 ? $"{mesh.name} Spline Bake" : $"{mesh.name} Spline Bake {i + 1}");
            space.transform.position = spline.transform.position;
            space.transform.SetAsLastSibling();

            Mesh bakedMesh = new Mesh();
            List<CombineInstance> collection = new List<CombineInstance>();

            List<Vector3> points = spline.GetEvenlyDistributedPointsOverLine(distanceIncrement, accuracy);

            for (int j = 0; j < points.Count - 1; j++)
            {
                Vector3 point = points[j];
                Vector3 direction = (points[j + 1] - points[j]).normalized;

                CombineInstance combineInstance = new CombineInstance();
                combineInstance.mesh = mesh;
                combineInstance.transform = Matrix4x4.TRS(point - spline.transform.position, Quaternion.LookRotation(direction) * Quaternion.Euler(rotation), Vector3.one);

                if (j == 0 && endCapA)
                {
                    combineInstance.mesh = endCapA;
                    combineInstance.transform = Matrix4x4.TRS(point - spline.transform.position, Quaternion.LookRotation(direction) * Quaternion.Euler(endCapRotationA), Vector3.one);
                }

                if (j == points.Count - 2 && endCapB)
                {
                    combineInstance.mesh = endCapB;
                    combineInstance.transform = Matrix4x4.TRS(point - spline.transform.position, Quaternion.LookRotation(direction) * Quaternion.Euler(endCapRotationB), Vector3.one);
                }

                collection.Add(combineInstance);
            }

            bakedMesh.CombineMeshes(collection.ToArray());

            MeshFilter filter = space.AddComponent<MeshFilter>();
            filter.mesh = bakedMesh;

            MeshRenderer renderer = space.AddComponent<MeshRenderer>();
            renderer.material = material ? material : AssetDatabase.LoadAssetAtPath<Material>("Packages/com.unity.render-pipelines.universal/Runtime/Materials/Lit.mat");

            switch (type)
            {
                case CollisionType.Mesh:

                    MeshCollider collider = space.AddComponent<MeshCollider>();
                    collider.convex = true;

                    break;
                case CollisionType.None:
                case CollisionType.Box:
                default:
                    break;
            }

            result.Add(space);
            i++;
        }
    }

    private static void OField<T>(string label, ref T thing) where T : UnityEngine.Object
    {
        thing = EditorGUILayout.ObjectField(label, thing, typeof(T), false) as T;
    }

    public enum CollisionType
    {
        None,
        Box,
        Mesh,
    }
}
