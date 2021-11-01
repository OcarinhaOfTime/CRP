using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CRP : RenderPipeline {    
    CameraRenderer renderer;
    ShadowSettings shadowSettings;
    public CRP(
        bool enableDynamicBatching, bool enableInstancing, 
        bool useSRPBatcher, ShadowSettings shadowSettings){
        renderer = new CameraRenderer(enableDynamicBatching, enableInstancing);
        GraphicsSettings.useScriptableRenderPipelineBatching = useSRPBatcher;
        GraphicsSettings.lightsUseLinearIntensity = true;
        this.shadowSettings = shadowSettings;
    }
    protected override void Render(ScriptableRenderContext ctx, Camera[] cameras) {
        foreach(var cam in cameras){
            renderer.Render(ctx, cam, shadowSettings);
        }
    }
}
