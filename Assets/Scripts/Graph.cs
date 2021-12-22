using System.Timers;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;
using System;
using UnityEngine;

public class Graph: MonoBehaviour {

    [SerializeField]
    Transform pointPrefab;

    [SerializeField, Range(10, 100)]
    int resolution = 10;

    [SerializeField]
    FunctionLibrary.FunctionName function;

    [SerializeField, Min(0f)]
    float functionDuration = 1f, transitionDuration = 1f;

    public enum TransitionMode { Cycle, Random };

    [SerializeField]
    TransitionMode transitionMode;

    Transform[] points;

    float duration;

    float range = 2f;

    bool transitioning;
    FunctionLibrary.FunctionName transitionFunction;

    void Awake() {
        points = new Transform[resolution * resolution];

        float step = range / resolution;
        var scale = Vector3.one * step;
        for(int i = 0, x = 0, z = 0; i < points.Length; i++, x ++) {
            Transform point = Instantiate(pointPrefab);
            point.localScale = scale;
            point.SetParent(transform, false);
            points[i] = point;
        }
    }

    void Update() {
        duration += Time.deltaTime;
        if (transitioning) {
            if (duration >= transitionDuration) {
                duration -= transitionDuration;
                transitioning = false;
            }
        } else if (duration >= functionDuration) {
            duration -= functionDuration;
            transitioning = true;
            transitionFunction = function;
            PickNextFunction();
        }

        if (transitioning) {
            UpdateFunctionTransition();
        } else {
            UpdateFunction();
        }
    }

    void UpdateFunction() {
        float time = Time.time;
        FunctionLibrary.Function f = FunctionLibrary.GetFunction(function);
        float step = range / resolution;
        float v = 0.5f * step - range/2;
        for (int i = 0, x = 0, z = 0; i < points.Length; i++, x++) {
            if (x == resolution) {
                x = 0;
                z += 1;
                v = (z + 0.5f) * step - range/2;
            }
            Transform point = points[i];
            float u = (x + 0.5f) * step - range/2;
            point.localPosition = f(u, v, time);;
        }
    }

    void UpdateFunctionTransition() {
        FunctionLibrary.Function
            from = FunctionLibrary.GetFunction(transitionFunction),
            to = FunctionLibrary.GetFunction(function);

        float progress = duration / transitionDuration;
        float time = Time.time;
        float step = range / resolution;
        float v = 0.5f * step - range/2;
        for (int i = 0, x = 0, z = 0; i < points.Length; i++, x++) {
            if (x == resolution) {
                x = 0;
                z += 1;
                v = (z + 0.5f) * step - range/2;
            }
            Transform point = points[i];
            float u = (x + 0.5f) * step - range/2;
            point.localPosition = FunctionLibrary.Morph(u, v, time, from, to, progress);
        }
    }

    void PickNextFunction () {
        function = transitionMode == TransitionMode.Cycle ?
            FunctionLibrary.GetNextFunctionName(function) :
            FunctionLibrary.GetRandomFunctionNameOtherThan(function);
    }
}