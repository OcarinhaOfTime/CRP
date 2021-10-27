using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshBall : MonoBehaviour {
    static int baseColorId = Shader.PropertyToID("_BaseColor");
    static int cutoffId = Shader.PropertyToID("_Cutoff");

	[SerializeField]
	Mesh mesh = default;

	[SerializeField]
	Material material = default;
    Matrix4x4[] matrices = new Matrix4x4[1023];
    Vector4[] baseColors = new Vector4[1023];
    float[] cutoffs = new float[1023];

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
        }
    }

    void Update(){
        if(block == null){
            block = new MaterialPropertyBlock();
            block.SetVectorArray(baseColorId, baseColors);
            block.SetFloatArray(cutoffId, cutoffs);
        }

        Graphics.DrawMeshInstanced(mesh, 0, material, matrices, 1023, block);
    }
}
