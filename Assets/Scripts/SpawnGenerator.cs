using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnGenerator : MonoBehaviour {
    public int n = 10;
    
    public float gridSize = 4;
    public GameObject[] prefabs;
    public GameObject prefab => prefabs[Random.Range(0, prefabs.Length)];
    //public Material[] materials;
    [ContextMenu("Clear")]
    public void Clear() {
        while (transform.childCount > 0) {
            DestroyImmediate(transform.GetChild(0).gameObject);
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

    public void BaseGenerator(System.Action<int, GameObject> fn, int n) {
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
        int sqr = (int)Mathf.Sqrt(n);
        BaseGenerator((i, go) => {
            float x = i % sqr;
            float y = i / sqr;
            x /= (sqr - 1);
            y /= (sqr - 1);
            y = 1 - y;
            var p = new Vector3(Mathf.Lerp(-gridSize, gridSize, x), 0, Mathf.Lerp(-gridSize, gridSize, y));
            var offset = Random.insideUnitCircle;
            var dir_offset = new Vector3(offset.x, 0, offset.y);
            p += dir_offset * Random.Range(.25f, .75f);
            p.y += Random.value;
            go.transform.localPosition = p;
            go.name = "obj" + i;
            var pomp = go.GetComponent<PerObjectMaterialProperties>();
            pomp.baseColor = Random.ColorHSV();
            pomp.smoothness = Random.value;
            pomp.metallic = Random.Range(0, .8f);
            pomp.OnValidate();
        });
    }

    [ContextMenu("Plane Generator")]
    public void PlaneGenerator() {
        BaseGenerator((i, go) => {
            float x = Random.Range(-3f, 3f);
            float y = Random.Range(-3f, 3f);
            var p = new Vector3(x, 0, y);
            go.transform.localPosition = p;
            go.name = "obj" + i;
            var pomp = go.GetComponent<PerObjectMaterialProperties>();
            pomp.baseColor = Random.ColorHSV();
            pomp.smoothness = Random.value;
            pomp.metallic = Random.Range(0, .8f);
            pomp.OnValidate();
        });
    }
}
