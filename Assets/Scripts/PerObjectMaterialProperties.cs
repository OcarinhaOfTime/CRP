using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[DisallowMultipleComponent]
public class PerObjectMaterialProperties : MonoBehaviour {  
	static int 
		baseColorId = Shader.PropertyToID("_BaseColor"),
		cutoffId = Shader.PropertyToID("_Cutoff"),
		metallicId = Shader.PropertyToID("_Metallic"),
		smoothnessId = Shader.PropertyToID("_Smoothness"),
		emissionColorId = Shader.PropertyToID("_EmissionColor");
    static MaterialPropertyBlock block;
	
	[SerializeField]public Color baseColor = Color.white;
	[SerializeField, ColorUsage(false, true)]
	Color emissionColor = Color.black;
	
	[SerializeField, Range(0, 1)] public float 
		cutoff = .5f,
		metallic = 0f, 
		smoothness = 0.5f;

    void Awake () {
		OnValidate();
	}

    public void OnValidate () {
		if (block == null) {
			block = new MaterialPropertyBlock();
		}
		block.SetColor(baseColorId, baseColor);
		block.SetFloat(cutoffId, cutoff);
		block.SetFloat(metallicId, metallic);
		block.SetFloat(smoothnessId, smoothness);
		block.SetColor(emissionColorId, emissionColor);
		
		GetComponent<Renderer>().SetPropertyBlock(block);
	}
}
