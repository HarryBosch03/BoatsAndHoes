using UnityEngine.Rendering.Universal;

[System.Serializable]
public class OutlinePostProcessRenderer : ScriptableRendererFeature
{
    OutlinePostProcessPass pass;

    public override void Create()
    {
        pass = new OutlinePostProcessPass();
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(pass);
    }
}
