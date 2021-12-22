using System.Net.Http.Headers;
using System.Timers;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;
using System;
using UnityEngine;

public class GPUGraph: MonoBehaviour {
    static readonly int
        positionsId = Shader.PropertyToID("_Positions"),
        resolutionId = Shader.PropertyToID("_Resolution"),
        stepId = Shader.PropertyToID("_Step"),
        timeId = Shader.PropertyToID("_Time");

    [SerializeField]
    ComputeShader computeShader;
    
    [SerializeField]
    Material material;

    [SerializeField]
    Mesh mesh;

    [SerializeField, Range(10, 1000)]
    int resolution = 10;

    [SerializeField]
    FunctionLibrary.FunctionName function;

    [SerializeField]
    TransitionMode transitionMode;

    [SerializeField, Min(0f)]
    float functionDuration = 1f, transitionDuration = 1f;

    public enum TransitionMode { Cycle, Random };

    float duration;

    float range = 2f;

    bool transitioning;
    FunctionLibrary.FunctionName transitionFunction;


    ComputeBuffer positionsBuffer;

    void Awake () {
    }

    void OnEnable () {
        positionsBuffer = new ComputeBuffer(resolution * resolution, 3 * 4);
    }

    void OnDisable () {
        positionsBuffer.Release();
        positionsBuffer = null;
    }

    void Update() {
        UpdateFunctionOnGPU();
    }

    void UpdateFunctionOnGPU () {
        float step = range/resolution;
        computeShader.SetInt(resolutionId, resolution);
        computeShader.SetFloat(stepId, step);
        computeShader.SetFloat(timeId, Time.time);
        computeShader.SetBuffer(0, positionsId, positionsBuffer);

        int groups = Mathf.CeilToInt(resolution / 8f);
        computeShader.Dispatch(0, groups, groups, 1);

        material.SetBuffer(positionsId, positionsBuffer);
        material.SetFloat(stepId, step);

        var bounds = new Bounds(Vector3.zero, Vector3.one * (2f + step));
        Graphics.DrawMeshInstancedProcedural(mesh, 0 , material, bounds, positionsBuffer.count);
    }

    void PickNextFunction () {
        function = transitionMode == TransitionMode.Cycle ?
            FunctionLibrary.GetNextFunctionName(function) :
            FunctionLibrary.GetRandomFunctionNameOtherThan(function);
    }
}