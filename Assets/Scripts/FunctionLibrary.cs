using UnityEngine;

using static UnityEngine.Mathf;

public static class FunctionLibrary {

    public delegate Vector3 Function (float u, float v, float t);

    public enum FunctionName { Wave, MultiWave, Ripple, Sphere, Torus };

    static Function[] functions = { Wave, MultiWave, Ripple, Sphere, Torus };

    public static Function GetFunction(FunctionName name) {
        return functions[(int)name];
    }

    public static FunctionName GetNextFunctionName (FunctionName name) {
        return (int) name < functions.Length - 1 ? name + 1 : 0;
    }

    public static FunctionName GetRandomFunctionName() {
        var choice = (FunctionName)Random.Range(0, functions.Length);
        return choice;
    }

    public static FunctionName GetRandomFunctionNameOtherThan (FunctionName name) {
        var choice = (FunctionName)Random.Range(1, functions.Length);
        return choice == name ? 0 : choice;
    }

    public static Vector3 Wave(float u, float v, float t) {
        // return Sin(PI * (x + z + t));
        Vector3 p;
        p.x = u;
        p.y = Sin(PI * (u + v + t));
        p.z = v;
        return p;
    }

    public static Vector3 MultiWave(float u, float v, float t) {
        // float y = Sin(PI * (x + t));
        // y += Sin(2f * PI * (x + t)) * 0.5f;
        // y *= (2f / 3f);
        // return y;

        // float y = Sin(PI * (x + 0.5f * t));
        // y += 0.5f * Sin(2f * PI * (x + t));
        // return y;

        // float y = Sin(PI * (x + t));
        // y += 0.5f * Sin(2f * PI * (z + t));
        // y += Sin(PI * (x + z + 0.25f * t));
        // y *= (1f / 2.5f);
        // return y;

        Vector3 p;
        p.x = u;
        p.y = Sin(PI * (u + t));
        p.y += 0.5f * Sin(2f * PI * (v + t));
        p.y += Sin(PI * (u + v + 0.25f * t));
        p.y *= (1f / 2.5f);
        p.z = v;
        return p;

        // Vector3 p;
        // p.x = u;
        // p.y = Sin(PI * (u + t)) + Cos(PI * (v + t));
        // p.y *= 0.5f;
        // p.z = v;
        // return p;
    }

    public static Vector3 Ripple (float u, float v, float t) {
        float d = Sqrt(u*u + v*v);
        // float y = Sin(PI * (4f * d - t));
        // return y / (1f + 10f * d);

        Vector3 p;
        p.x = u;
        p.y = Sin(PI * (4f * d - t));
        p.y /= (1f + 10f * d);
        p.z = v;
        return p;
    }   

    public static Vector3 Sphere (float u, float v, float t) {
        float r = 0.9f + 0.1f * Sin(PI * (6f *  u + 12f * v + t));
        float s = r * Cos(0.5f * PI * v);
        Vector3 p;
        p.x = s * Sin(PI * u);
        p.y = r * Sin(0.5f * PI * v);
        p.z = s * Cos(PI * u);
        return p;

        // float r = 0.5f + 0.5f * Sin(PI * t);
        // float s = r * Cos(0.5f * PI * v);
        // Vector3 p;
        // p.x = s * Sin(PI * u);
        // p.y = r * Sin(0.5f * PI * v);
        // p.z = s * Cos(PI * u);
        // return p;

        // float r = 0.9f + 0.1f * Sin(8f * PI * u);
        // float r = 0.9f + 0.1f * Sin(8f * PI * v);
        // float r = 0.9f + 0.1f * Sin(PI * (6f * u + 4f * v + t));
        // float s = r * Cos(0.5f * PI * v);
        // Vector3 p;
        // p.x = s * Sin(PI * u);
        // p.y = r * Sin(0.5f * PI * v);
        // p.z = s * Cos(PI * u);
        // return p;
    }

    public static Vector3 Torus (float u, float v, float t) {
        float tt = 1f; //Abs(Sin(PI * 0.05f * t));
        // float r1 = 3f;
        // float r2 = 1f;
        float r1 = 0.7f + 0.1f * Sin(PI * (6f * u + 0.5f * t));
        float r2 = 0.15f + 0.05f * Sin(PI * (8f * u + 2f * v + t));

        float s = r1 + r2 * Cos(tt * PI * v);
        Vector3 p;
        p.x = s * Sin(PI * u);
        p.y = r2 * Sin(tt * PI * v);
        p.z = s * Cos(PI * u);
        return p;
    }

    public static Vector3 Morph (float u, float v, float t, Function from, Function to, float progress) {
        return Vector3.LerpUnclamped( from(u, v, t), to(u, v, t), SmoothStep(0f, 1f, progress) );
    }
}
