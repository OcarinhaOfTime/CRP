using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public partial class CameraRenderer {
    const string bufferName = "Render Camera";
    bool enableDynamicBatching, enableInstancing;
    
    static ShaderTagId 
    unlitShaderTagId = new ShaderTagId("SRPDefaultUnlit"),
    litShaderTagId = new ShaderTagId("CustomLit");
    CommandBuffer buffer = new CommandBuffer {
        name = bufferName
    };
    ScriptableRenderContext ctx;
    CullingResults cullingResults;
    Camera camera;
    Lighting lighting = new Lighting();

    public CameraRenderer(bool enableDynamicBatching, bool enableInstancing){
        this.enableDynamicBatching = enableDynamicBatching;
        this.enableInstancing = enableInstancing;
    }

    public void Render(ScriptableRenderContext ctx, Camera camera, ShadowSettings shadowSettings) {
        this.ctx = ctx;
        this.camera = camera;

        PrepareBuffer();
        PrepareForSceneWindow();

        if (!Cull(shadowSettings.maxDistance)) return;
        
        buffer.BeginSample(SampleName);
		ExecuteBuffer();
        lighting.Setup(ctx, cullingResults, shadowSettings);
        buffer.EndSample(SampleName);
        Setup();
        DrawVisibleGeometry();
        DrawUnsupportedShaders();
        DrawGizmos();
        lighting.Cleanup();
        Submit();
    }

    void Setup() {
        ctx.SetupCameraProperties(camera);
        CameraClearFlags flags = camera.clearFlags;
        buffer.ClearRenderTarget(
            flags <= CameraClearFlags.Depth, 
            flags == CameraClearFlags.Color, 
            flags == CameraClearFlags.Color ?
				camera.backgroundColor.linear : Color.clear);
        buffer.BeginSample(SampleName);
        ExecuteBuffer();
    }

    void DrawVisibleGeometry() {
        var sortingSettings = new SortingSettings(camera) {
            criteria = SortingCriteria.CommonOpaque
        };
        var drawingSettings = new DrawingSettings(
            unlitShaderTagId, sortingSettings
        ){
            enableDynamicBatching = enableDynamicBatching,
			enableInstancing = enableInstancing
        };
        drawingSettings.SetShaderPassName(1, litShaderTagId);
        var filteringSettings = new FilteringSettings(RenderQueueRange.opaque);

        ctx.DrawRenderers(
            cullingResults, ref drawingSettings, ref filteringSettings
        );

        ctx.DrawSkybox(camera);

        sortingSettings.criteria = SortingCriteria.CommonTransparent;
        drawingSettings.sortingSettings = sortingSettings;
        filteringSettings.renderQueueRange = RenderQueueRange.transparent;

        ctx.DrawRenderers(
            cullingResults, ref drawingSettings, ref filteringSettings
        );

    }    

    void Submit() {
        buffer.EndSample(SampleName);
        ExecuteBuffer();
        ctx.Submit();
    }

    void ExecuteBuffer() {
        ctx.ExecuteCommandBuffer(buffer);
        buffer.Clear();
    }

    bool Cull(float maxShadowDistance) {
        if (camera.TryGetCullingParameters(out var p)) {
            p.shadowDistance = Mathf.Min(maxShadowDistance, camera.farClipPlane);
            cullingResults = ctx.Cull(ref p);
            return true;
        }
        return false;
    }
}
