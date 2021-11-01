using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Shadows {
    struct ShadowedDirectionalLight {
        public int visibleLightIndex;
    }
    const string bufferName = "Shadows";
    const int maxShadowedDirectionalLightCount = 1;
    static int dirShadowAtlasId = Shader.PropertyToID("_DirectionalShadowAtlas");
    CommandBuffer buffer = new CommandBuffer { name = bufferName };
    ScriptableRenderContext context;

    CullingResults cullingResults;

    ShadowSettings settings;
    ShadowedDirectionalLight[] ShadowedDirectionalLights =
        new ShadowedDirectionalLight[maxShadowedDirectionalLightCount];
    int shadowedDirectionalLightCount;

    public void Setup(ScriptableRenderContext context,
        CullingResults cullingResults,
        ShadowSettings settings) {
        this.context = context;
        this.cullingResults = cullingResults;
        this.settings = settings;
        shadowedDirectionalLightCount = 0;
    }

    public void ReserveDirectionalShadows(Light light, int visibleLightIndex) {
        if (shadowedDirectionalLightCount < maxShadowedDirectionalLightCount &&
            light.shadows != LightShadows.None &&
            light.shadowStrength > 0f &&
            cullingResults.GetShadowCasterBounds(visibleLightIndex, out var b)) {
            ShadowedDirectionalLights[shadowedDirectionalLightCount++] = new ShadowedDirectionalLight {
                visibleLightIndex = visibleLightIndex
            };
        }
    }

    public void Render() {
        if (shadowedDirectionalLightCount <= 0) return;
        RenderDirectionalShadows();
    }

    void RenderDirectionalShadows() {
        int atlasSize = (int)settings.directional.atlasSize;
        buffer.GetTemporaryRT(dirShadowAtlasId, atlasSize, atlasSize,
        32, FilterMode.Bilinear, RenderTextureFormat.Shadowmap);
        buffer.SetRenderTarget(dirShadowAtlasId, 
        RenderBufferLoadAction.DontCare, 
        RenderBufferStoreAction.Store);
        buffer.ClearRenderTarget(true, true, Color.clear);

		buffer.BeginSample(bufferName);
        ExecuteBuffer();

        for(int i=0; i<shadowedDirectionalLightCount; i++){
            RenderDirectionalShadows(i, atlasSize);
        }

        buffer.EndSample(bufferName);
        ExecuteBuffer();
    }

    void RenderDirectionalShadows(int i, int tileSize){
        ShadowedDirectionalLight l= ShadowedDirectionalLights[i];
        var shadowSettings = new ShadowDrawingSettings(cullingResults, l.visibleLightIndex);

        cullingResults.ComputeDirectionalShadowMatricesAndCullingPrimitives(
			l.visibleLightIndex, 0, 1, Vector3.zero, tileSize, 0f,
			out Matrix4x4 viewMatrix, out Matrix4x4 projectionMatrix,
			out ShadowSplitData splitData
		);
        shadowSettings.splitData = splitData;
        buffer.SetViewProjectionMatrices(viewMatrix, projectionMatrix);
        ExecuteBuffer();
        context.DrawShadows(ref shadowSettings);
    }

    public void Cleanup(){
        if (shadowedDirectionalLightCount <= 0) return;
        buffer.ReleaseTemporaryRT(dirShadowAtlasId);
        ExecuteBuffer();
    }

    void ExecuteBuffer() {
        context.ExecuteCommandBuffer(buffer);
        buffer.Clear();
    }
}
