using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MeshBallBaked : MonoBehaviour {
    static int 
        baseColorId = Shader.PropertyToID("_BaseColor"),
        cutoffId = Shader.PropertyToID("_Cutoff"),
        smoothnessId = Shader.PropertyToID("_Smoothness"),
        metallicId = Shader.PropertyToID("_Metallic");

	[SerializeField]
	Mesh mesh = default;

	[SerializeField]
	Material material = default;
    [SerializeField]
	LightProbeProxyVolume lightProbeVolume = null;
    Matrix4x4[] matrices = new Matrix4x4[1023];
    Vector4[] baseColors = new Vector4[1023];
    float[] 
        cutoffs = new float[1023],
        smoothnesses = new float[1023],
        metallics = new float[1023];

    MaterialPropertyBlock block;

    void Awake() {
        for (int i = 0; i < matrices.Length; i++) {
            matrices[i] = Matrix4x4.TRS(
                Random.insideUnitSphere * 10f, 
                Random.rotation, 
                Vector3.one * (.5f + Random.value)
            );
            baseColors[i] =
                new Vector4(Random.value, Random.value, Random.value, 1f);

            cutoffs[i] = Random.Range(.2f, .8f);
            smoothnesses[i] = Random.value;
            metallics[i] = Random.value;
        }
    }

    void Update(){
        if(block == null){
            block = new MaterialPropertyBlock();
            block.SetVectorArray(baseColorId, baseColors);
            block.SetFloatArray(cutoffId, cutoffs);
            block.SetFloatArray(smoothnessId, smoothnesses);
            block.SetFloatArray(metallicId, metallics);
            
            if(!lightProbeVolume){
                var positions = new Vector3[1023];
                for(int i=0; i<1023; i++){
                    positions[i] = matrices[i].GetColumn(3);
                }

                var lprobes = new SphericalHarmonicsL2[1023];
                LightProbes.CalculateInterpolatedLightAndOcclusionProbes(positions, lprobes, null);
                block.CopySHCoefficientArraysFrom(lprobes);
            }
            
        }

        Graphics.DrawMeshInstanced(mesh, 0, material, matrices, 1023, block,
        ShadowCastingMode.On, true, 0, null, 
        lightProbeVolume ? LightProbeUsage.UseProxyVolume : LightProbeUsage.CustomProvided, lightProbeVolume);
    }
}
