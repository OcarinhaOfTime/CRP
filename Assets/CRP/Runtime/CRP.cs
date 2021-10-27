using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CRP : RenderPipeline {    
    CameraRenderer renderer;
    public CRP(bool enableDynamicBatching, bool enableInstancing, bool useSRPBatcher){
        renderer = new CameraRenderer(enableDynamicBatching, enableInstancing);
        GraphicsSettings.useScriptableRenderPipelineBatching = useSRPBatcher;
    }
    protected override void Render(ScriptableRenderContext ctx, Camera[] cameras) {
        foreach(var cam in cameras){
            renderer.Render(ctx, cam);
        }
    }
}
