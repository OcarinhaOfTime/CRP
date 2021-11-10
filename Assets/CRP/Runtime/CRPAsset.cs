using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "Rendering/Custom Render Pipeline")]
public class CRPAsset : RenderPipelineAsset {
    [SerializeField]
	bool enableDynamicBatching = true, enableInstancing = true, useSRPBatcher = true;
    [SerializeField]
	ShadowSettings shadowSettings = default;
    protected override UnityEngine.Rendering.RenderPipeline CreatePipeline() {
        return new CRP(
            enableDynamicBatching, enableInstancing, useSRPBatcher, shadowSettings
        );
    }

    [ContextMenu("Print this crap")]
    public void PrintFilterSettingValue(){
        Debug.Log((int)shadowSettings.directional.filter);
    }
}
