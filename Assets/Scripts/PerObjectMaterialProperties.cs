using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[DisallowMultipleComponent]
public class PerObjectMaterialProperties : MonoBehaviour {
    
	static int baseColorId = Shader.PropertyToID("_BaseColor");
	static int cutoffId = Shader.PropertyToID("_Cutoff");
    static MaterialPropertyBlock block;
	
	[SerializeField]
	Color baseColor = Color.white;
	[SerializeField, Range(0, 1)]
	float cutoff = .5f;

    public void SetColor(Color c){
        baseColor = c;
        OnValidate();
    }

    void Awake () {
		OnValidate();
	}

    void OnValidate () {
		if (block == null) {
			block = new MaterialPropertyBlock();
		}
		block.SetColor(baseColorId, baseColor);
		block.SetFloat(cutoffId, cutoff);
		GetComponent<Renderer>().SetPropertyBlock(block);
	}
}
