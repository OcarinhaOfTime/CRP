using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnGenerator : MonoBehaviour {
    public int n = 10;
    public GameObject prefab;
    //public Material[] materials;
    [ContextMenu("Clear")]
    public void Clear() {
        while (transform.childCount > 1) {
            DestroyImmediate(transform.GetChild(1).gameObject);
        }
    }

    public void BaseGenerator(System.Action<int, GameObject> fn) {
        Clear();
        for (int i = 0; i < n; i++) {
            var inst = Instantiate(prefab);
            inst.transform.SetParent(transform);
            inst.SetActive(true);
            fn(i, inst);
        }
    }

    [ContextMenu("Rand Generator")]
    public void RandGenerator() {
        BaseGenerator((i, go) => {
            go.transform.localPosition = Random.insideUnitCircle * 4;
        });
    }

    [ContextMenu("Rand Generator Color")]
    public void RandGeneratorColor() {
        BaseGenerator((i, go) => {
            go.transform.localPosition = Random.insideUnitCircle * 4;
            var pomp = go.GetComponent<PerObjectMaterialProperties>();
            //pomp.color(Random.ColorHSV());
        });
    }

    [ContextMenu("Grid Generator")]
    public void GridGenerator() {
        BaseGenerator((i, go) => {
            float x = i % 4;
            float y = i / 4;
            x /= 3;
            y /= 3;
            y = 1-y;
            var p = new Vector3(Mathf.Lerp(-2, 2, x), 0, Mathf.Lerp(-2, 2, y));
            go.transform.localPosition = p;
            go.name = "obj" + i;
            var pomp = go.GetComponent<PerObjectMaterialProperties>();
            pomp.smoothness = 1-y;
            pomp.metallic = x;
            pomp.OnValidate();
        });
    }
}
