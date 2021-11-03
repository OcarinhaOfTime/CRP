using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Shadows {
    struct ShadowedDirectionalLight {
        public int visibleLightIndex;
    }
    const string bufferName = "Shadows";
    const int maxShadowedDirectionalLightCount = 4;
    static int
        dirShadowAtlasId = Shader.PropertyToID("_DirectionalShadowAtlas"),
        dirShadowMatricesId = Shader.PropertyToID("_DirectionalShadowMatrices");

    static Matrix4x4[] dirShadowMatrices = new Matrix4x4[maxShadowedDirectionalLightCount];
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

    public Vector2 ReserveDirectionalShadows(Light light, int visibleLightIndex) {
        if (shadowedDirectionalLightCount < maxShadowedDirectionalLightCount &&
            light.shadows != LightShadows.None &&
            light.shadowStrength > 0f &&
            cullingResults.GetShadowCasterBounds(visibleLightIndex, out var b)) {
            ShadowedDirectionalLights[shadowedDirectionalLightCount] = new ShadowedDirectionalLight {
                visibleLightIndex = visibleLightIndex
            };

            return new Vector2(light.shadowStrength, shadowedDirectionalLightCount++);
        }

        return Vector2.zero;
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

        int split = shadowedDirectionalLightCount <= 1 ? 1 : 2;
        int tileSize = atlasSize / split;

        for (int i = 0; i < shadowedDirectionalLightCount; i++) {
            RenderDirectionalShadows(i, split, tileSize);
        }

        buffer.SetGlobalMatrixArray(dirShadowMatricesId, dirShadowMatrices);
        buffer.EndSample(bufferName);
        ExecuteBuffer();
    }

    void RenderDirectionalShadows(int i, int split, int tileSize) {
        ShadowedDirectionalLight l = ShadowedDirectionalLights[i];
        var shadowSettings = new ShadowDrawingSettings(cullingResults, l.visibleLightIndex);

        cullingResults.ComputeDirectionalShadowMatricesAndCullingPrimitives(
            l.visibleLightIndex, 0, 1, Vector3.zero, tileSize, 0f,
            out Matrix4x4 viewMatrix, out Matrix4x4 projectionMatrix,
            out ShadowSplitData splitData
        );
        shadowSettings.splitData = splitData;

        dirShadowMatrices[i] = ConvertToAtlasMatrix(
            projectionMatrix * viewMatrix,
            SetTileViewport(i, split, tileSize),
            split
        );
        buffer.SetViewProjectionMatrices(viewMatrix, projectionMatrix);
        ExecuteBuffer();
        context.DrawShadows(ref shadowSettings);
    }

    Vector2 SetTileViewport(int index, int split, float tileSize) {
        Vector2 offset = new Vector2(index % split, index / split);
        buffer.SetViewport(new Rect(offset.x * tileSize, offset.y * tileSize, tileSize, tileSize));
        return offset;
    }

    Matrix4x4 ConvertToAtlasMatrix(Matrix4x4 m, Vector2 offset, int split) {
        if (SystemInfo.usesReversedZBuffer) {
            m.m20 = -m.m20;
            m.m21 = -m.m21;
            m.m22 = -m.m22;
            m.m23 = -m.m23;
        }
        float scale = 1f / split;
		m.m00 = (0.5f * (m.m00 + m.m30) + offset.x * m.m30) * scale;
		m.m01 = (0.5f * (m.m01 + m.m31) + offset.x * m.m31) * scale;
		m.m02 = (0.5f * (m.m02 + m.m32) + offset.x * m.m32) * scale;
		m.m03 = (0.5f * (m.m03 + m.m33) + offset.x * m.m33) * scale;
		m.m10 = (0.5f * (m.m10 + m.m30) + offset.y * m.m30) * scale;
		m.m11 = (0.5f * (m.m11 + m.m31) + offset.y * m.m31) * scale;
		m.m12 = (0.5f * (m.m12 + m.m32) + offset.y * m.m32) * scale;
		m.m13 = (0.5f * (m.m13 + m.m33) + offset.y * m.m33) * scale;
        m.m20 = 0.5f * (m.m20 + m.m30);
        m.m21 = 0.5f * (m.m21 + m.m31);
        m.m22 = 0.5f * (m.m22 + m.m32);
        m.m23 = 0.5f * (m.m23 + m.m33);

        return m;
    }

    public void Cleanup() {
        if (shadowedDirectionalLightCount <= 0) return;
        buffer.ReleaseTemporaryRT(dirShadowAtlasId);
        ExecuteBuffer();
    }

    void ExecuteBuffer() {
        context.ExecuteCommandBuffer(buffer);
        buffer.Clear();
    }
}
