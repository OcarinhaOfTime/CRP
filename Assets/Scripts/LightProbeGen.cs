using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightProbeGen : MonoBehaviour {
    public float radius = 5;
    [ContextMenu("Generate")]
    public void Generate() {
        var probe = GetComponent<LightProbeGroup>();
        int n = 8;
        var poss = new Vector3[n];
        poss[0] = new Vector3(-1, -1, -1) * radius;
        poss[1] = new Vector3(-1, -1, 1) * radius;
        poss[2] = new Vector3(1, -1, -1) * radius;
        poss[3] = new Vector3(1, -1, 1) * radius;
        poss[4] = new Vector3(-1, 1, -1) * radius;
        poss[5] = new Vector3(-1, 1, 1) * radius;
        poss[6] = new Vector3(1, 1, -1) * radius;
        poss[7] = new Vector3(1, 1, 1) * radius;

        probe.probePositions = poss;
        for (int i = 0; i < n; i++) {
            transform.GetChild(i).localPosition = poss[i];
        }
    }

    [ContextMenu("DebugProbe")]
    public void DebugProbe() {
        var probe = GetComponent<LightProbeGroup>();
        for (int i = 0; i < probe.probePositions.Length; i++) {
            print(probe.probePositions[i]);
        }
    }
}
