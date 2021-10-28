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

    public void Setup(ScriptableRenderContext ctx, CullingResults cullingResults) {
        this.cullingResults = cullingResults;
        buffer.BeginSample(bufferName);
        SetupLights();
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

        // for(int i=visibleLights.Length; i<maxDirLightCount; i++){
        //     dirLightColors[i] = Color.clear;
        // }

        buffer.SetGlobalInt(dirLightCountId, dirLightCount);
        buffer.SetGlobalVectorArray(dirLightColorsId, dirLightColors);
        buffer.SetGlobalVectorArray(dirLightDirectionsId, dirLightDirections);
    }

    void SetupDirectionalLight(int i, ref VisibleLight l){
        dirLightColors[i] = l.finalColor;
        dirLightDirections[i] = -l.localToWorldMatrix.GetColumn(2);
    }

    // void SetupDirectionalLight() {
    //     Light l = RenderSettings.sun;
    //     buffer.SetGlobalVector(dirLightColorId, l.color.linear * l.intensity);
    //     buffer.SetGlobalVector(dirLightDirectionId, -l.transform.forward);
    // }
}
