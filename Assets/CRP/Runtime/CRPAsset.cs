using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "Rendering/Custom Render Pipeline")]
public class CRPAsset : RenderPipelineAsset {
    protected override UnityEngine.Rendering.RenderPipeline CreatePipeline() {
        return new CRP();
    }
}
