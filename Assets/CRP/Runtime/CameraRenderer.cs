using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public partial class CameraRenderer {
    const string bufferName = "Render Camera";
    
    static ShaderTagId unlitShaderTagId = new ShaderTagId("SRPDefaultUnlit");
    CommandBuffer buffer = new CommandBuffer {
        name = bufferName
    };
    ScriptableRenderContext ctx;
    CullingResults cullingResults;
    Camera camera;

    public void Render(ScriptableRenderContext ctx, Camera camera) {
        this.ctx = ctx;
        this.camera = camera;

        PrepareBuffer();
        PrepareForSceneWindow();

        if (!Cull()) return;

        Setup();
        DrawVisibleGeometry();
        DrawUnsupportedShaders();
        DrawGizmos();
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
        );
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

    bool Cull() {
        if (camera.TryGetCullingParameters(out var p)) {
            cullingResults = ctx.Cull(ref p);
            return true;
        }
        return false;
    }
}
