using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Unity.Collections;

public class Lighting {
    const string bufferName = "Lighting";
    const int maxDirLightCount = 4;
    static int
        dirLightCountId = Shader.PropertyToID("_DirectionalLightCount"),
		dirLightColorsId = Shader.PropertyToID("_DirectionalLightColors"),
		dirLightDirectionsId = Shader.PropertyToID("_DirectionalLightDirections");

    static Vector4[]
		dirLightColors = new Vector4[maxDirLightCount],
		dirLightDirections = new Vector4[maxDirLightCount];
    CommandBuffer buffer = new CommandBuffer {
        name = bufferName
    };
    CullingResults cullingResults;
    Shadows shadows = new Shadows();

    public void Setup(ScriptableRenderContext ctx, 
        CullingResults cullingResults,
        ShadowSettings shadowSettings) {
        this.cullingResults = cullingResults;
        buffer.BeginSample(bufferName);
        shadows.Setup(ctx, cullingResults, shadowSettings);
        SetupLights();
        shadows.Render();
        buffer.EndSample(bufferName);
        ctx.ExecuteCommandBuffer(buffer);
        buffer.Clear();

    }

    void SetupLights() {
        var visibleLights = cullingResults.visibleLights;
        var dirLightCount = 0;
        for(int i=0; i<visibleLights.Length; i++){
            var l = visibleLights[i];
            if (l.lightType != LightType.Directional) continue;
            SetupDirectionalLight(dirLightCount++, ref l);
            if(dirLightCount >= maxDirLightCount) break;
        }

        buffer.SetGlobalInt(dirLightCountId, dirLightCount);
        buffer.SetGlobalVectorArray(dirLightColorsId, dirLightColors);
        buffer.SetGlobalVectorArray(dirLightDirectionsId, dirLightDirections);
    }

    void SetupDirectionalLight(int i, ref VisibleLight l){
        dirLightColors[i] = l.finalColor;
        dirLightDirections[i] = -l.localToWorldMatrix.GetColumn(2);
        shadows.ReserveDirectionalShadows(l.light, i);
    }

    public void Cleanup(){
        shadows.Cleanup();
    }
}
