using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[System.Serializable]
public class OutlinePostProcessPass : ScriptableRenderPass
{
    RenderTargetIdentifier source;
    RenderTargetIdentifier destinationA;
    RenderTargetIdentifier destinationB;
    RenderTargetIdentifier latestDest;
    RenderTargetIdentifier outlineDest;

    readonly int temporaryRTIdOutlineMask = Shader.PropertyToID("_OutlineMask");
    readonly int temporaryRTIdA = Shader.PropertyToID("_TempRT");
    readonly int temporaryRTIdB = Shader.PropertyToID("_TempRTB");

    Material outlineMat;
    Material material;

    public OutlinePostProcessPass()
    {
        // Set the render pass event
        renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }

    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        // Grab the camera target descriptor. We will use this when creating a temporary render texture.
        RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;
        descriptor.depthBufferBits = 0;

        var renderer = renderingData.cameraData.renderer;
        source = renderer.cameraColorTarget;

        // Create a temporary render texture using the descriptor from above.
        cmd.GetTemporaryRT(temporaryRTIdA, descriptor, FilterMode.Bilinear);
        destinationA = new RenderTargetIdentifier(temporaryRTIdA);
        cmd.GetTemporaryRT(temporaryRTIdB, descriptor, FilterMode.Bilinear);
        destinationB = new RenderTargetIdentifier(temporaryRTIdB);

        cmd.GetTemporaryRT(temporaryRTIdOutlineMask, renderingData.cameraData.cameraTargetDescriptor);
        outlineDest = new RenderTargetIdentifier(temporaryRTIdOutlineMask);
    }

    // The actual execution of the pass. This is where custom rendering occurs.
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        CommandBuffer cmd = CommandBufferPool.Get("Custom Post Processing");
        cmd.Clear();

        // This holds all the current Volumes information
        // which we will need later
        var stack = VolumeManager.instance.stack;

        #region Local Methods

        // Swaps render destinations back and forth, so that
        // we can have multiple passes and similar with only a few textures
        void BlitTo(Material mat, int pass = 0)
        {
            var first = latestDest;
            var last = first == destinationA ? destinationB : destinationA;
            Blit(cmd, first, last, mat, pass);

            latestDest = last;
        }

        #endregion

        // Starts with the camera source
        latestDest = source;

        cmd.SetRenderTarget(outlineDest);
        cmd.ClearRenderTarget(true, true, Color.clear);
        if (PlayerInteractor.Highlighted)
        {
            if (!outlineMat)
            {
                outlineMat = new Material(Shader.Find("Shader Graphs/New Shader Graph"));
            }

            cmd.DrawRenderer(PlayerInteractor.Highlighted.GetComponentInChildren<Renderer>(), outlineMat);
        }

        //---Custom effect here---
        var customEffect = stack.GetComponent<OutlineVolumeComponent>();
        // Only process if the effect is active
        if (customEffect.IsActive())
        {
            if (!material) material = new Material(Shader.Find("Hidden/Custom/Outline"));

            // P.s. optimize by caching the property ID somewhere else
            material.SetTexture(Shader.PropertyToID("_Overlay"), customEffect.OverlayTexture.value);
            material.SetFloat(Shader.PropertyToID("_OverlayScale"), customEffect.OverlayTextureSize.value);
            material.SetColor(Shader.PropertyToID("_OverlayTint"), customEffect.OverlayTint.value);

            BlitTo(material);
        }

        // Add any other custom effect/component you want, in your preferred order
        // Custom effect 2, 3 , ...


        // DONE! Now that we have processed all our custom effects, applies the final result to camera
        Blit(cmd, latestDest, source);

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    //Cleans the temporary RTs when we don't need them anymore
    public override void OnCameraCleanup(CommandBuffer cmd)
    {
        cmd.ReleaseTemporaryRT(temporaryRTIdA);
        cmd.ReleaseTemporaryRT(temporaryRTIdB);
    }
}
