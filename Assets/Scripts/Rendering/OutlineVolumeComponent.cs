using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[System.Serializable, VolumeComponentMenuForRenderPipeline("Custom/Outline", typeof(UniversalRenderPipeline))]
public class OutlineVolumeComponent : VolumeComponent, IPostProcessComponent
{
    public bool IsActive() => true;
    public bool IsTileCompatible() => true;

    public Texture2DParameter OverlayTexture = new Texture2DParameter(null);
    public ColorParameter OverlayTint = new ColorParameter(Color.white);
    public FloatParameter OverlayTextureSize = new FloatParameter(1.0f);
}
