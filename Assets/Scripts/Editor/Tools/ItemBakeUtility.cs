using System.IO;
using UnityEditor;
using UnityEngine;

public sealed class ItemBakeUtility : EditorWindow
{
    [MenuItem("Tools/Item Bake Utility")]
    static void Open ()
    {
        CreateWindow<ItemBakeUtility>("Item Bake Utility");
    }

    ItemType type;
    Camera cam;
    GameObject prefabInstance;
    Transform workingGroup;
    RenderTexture output;
    BakeConfig config;

    private void OnEnable()
    {
        workingGroup = new GameObject("[ITEM BAKE UTILITY]").transform;
        workingGroup.SetAsLastSibling();

        var configPath = Application.dataPath + "/Rendering/Item Baking/";
        var fileName = "ItemBakeConfig.json";

        if (File.Exists(configPath + fileName))
        {
            config = JsonUtility.FromJson<BakeConfig>(File.ReadAllText(configPath + fileName));
        }
    }

    private void OnDisable()
    {
        DestroyImmediate(workingGroup.gameObject);
        if (output) output.Release();

        var configPath = Application.dataPath + "/Rendering/Item Baking/";
        var fileName = "ItemBakeConfig.json";

        if (!Directory.Exists(configPath))
        {
            Directory.CreateDirectory(configPath);
        }

        string json = JsonUtility.ToJson(config);
        File.WriteAllText(configPath + fileName, json);
    }

    private void OnGUI()
    {
        if (Application.isPlaying)
        {
            EditorGUILayout.HelpBox("Cannot Bake in Playmode.", MessageType.Error);
            return;
        }
            
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.BeginVertical("box", GUILayout.MaxWidth(200), GUILayout.ExpandHeight(true));
        type = EditorGUILayout.ObjectField(type, typeof(ItemType), false) as ItemType;

        if (type)
        {
            if (!prefabInstance)
            {
                prefabInstance = PrefabUtility.InstantiatePrefab(type.worldItemPrefab, workingGroup) as GameObject;
                prefabInstance.name = $"[ITEM BAKE UTILITY] {prefabInstance.name}";
            }
            prefabInstance.gameObject.layer = 31;
            prefabInstance.transform.position = Vector3.zero;
            prefabInstance.transform.rotation = Quaternion.Euler(config.prefabRotation);

            EditorGUILayout.Space();

            config.cellSize = Mathf.Max(EditorGUILayout.IntField("Cell Size", config.cellSize), 1);
            config.cellGap = Mathf.Max(EditorGUILayout.IntField("Cell Gap", config.cellGap), 0);
            config.upscale = EditorGUILayout.FloatField("Upscale", config.upscale);

            EditorGUILayout.Space();

            config.camPos = EditorGUILayout.Vector3Field("Camera Position", config.camPos);
            config.orthoSize = EditorGUILayout.FloatField("Ortho Size", config.orthoSize);
            config.prefabRotation = EditorGUILayout.Vector3Field("Prefab Rotation", config.prefabRotation);

            EditorGUILayout.Space();

            int tWidth = type.size.x * config.cellSize + (type.size.x - 1) * config.cellGap;
            int tHeight = type.size.y * config.cellSize + (type.size.y - 1) * config.cellGap;

            tWidth = (int)(tWidth * config.upscale);
            tHeight = (int)(tHeight * config.upscale);

            if (tWidth > 0 && tHeight > 0)
            {
                if (!output)
                {
                    output = new RenderTexture(tWidth, tHeight, 0);
                }
                else if (output.width != tWidth || output.width != tHeight)
                {
                    output.Release();
                    output = new RenderTexture(tWidth, tHeight, 0);
                }

                if (!cam)
                {
                    cam = new GameObject("[ITEM BAKE UTILITY] Bake Camera").AddComponent<Camera>();
                }
                cam.transform.parent = workingGroup;
                cam.cullingMask = 1 << 31;
                cam.transform.position = config.camPos;
                cam.transform.rotation = Quaternion.LookRotation(-cam.transform.position);
                cam.targetTexture = output;
                cam.backgroundColor = Color.clear;
                cam.clearFlags = CameraClearFlags.Color;
                cam.orthographic = true;
                cam.orthographicSize = config.orthoSize;
                cam.Render();
            }
        }

        if (!type && prefabInstance)
        {
            DestroyImmediate(prefabInstance);
        }

        if (GUILayout.Button("Bake"))
        {
            Bake();
        }

        EditorGUILayout.EndVertical();
        
        EditorGUILayout.BeginVertical("box");

        if (output)
        {
            Rect controlRect = EditorGUILayout.GetControlRect();
            float aspect = output.height / (float)output.width;
            controlRect.height = controlRect.width * aspect;
            GUI.DrawTexture(controlRect, output);
        }

        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();
    }

    private void Bake()
    {
        var oldOut = RenderTexture.active;

        RenderTexture.active = output;
        Texture2D assetOutput = new Texture2D(output.width, output.height);
        assetOutput.ReadPixels(new Rect(0, 0, output.width, output.height), 0, 0);

        RenderTexture.active = oldOut;

        byte[] rawData = assetOutput.EncodeToPNG();

        var partialPath = "/Textures/Items/Icons/";
        var fullPath = Application.dataPath + partialPath;
        var fileName = type.name + " Icon.png";

        if (!Directory.Exists(fullPath))
        {
            Directory.CreateDirectory(fullPath);
        }

        File.WriteAllBytes(fullPath + fileName, rawData);
        Debug.Log($"Texture successfully baked to {fullPath}{fileName}");

        AssetDatabase.Refresh();

        var importer = AssetImporter.GetAtPath("Assets" + partialPath + fileName) as TextureImporter;
        importer.textureType = TextureImporterType.Sprite;
        EditorUtility.SetDirty(importer);
        importer.SaveAndReimport();

        AssetDatabase.Refresh();

        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets" + partialPath + fileName);
        type.icon = sprite;
    }
}

[System.Serializable]
public struct BakeConfig
{
    public int cellSize;
    public int cellGap;
    public float upscale;
    public float orthoSize;
    public Vector3 camPos;
    public Vector3 prefabRotation;
}