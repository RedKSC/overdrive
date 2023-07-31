using System.Collections;
using System.Collections.Generic;
//using Unity.VisualScripting;
using UnityEngine;

//Basic holder class for all game math -- modified for a 2D engine
//[IncludeInSettings(true)]
public static class GameMath {
    public static int ConvertToInt(this bool check) => check ? 1 : 0;

    public static Vector2 ConvertTo2D(this Vector3 pos) => new Vector2(pos.x, pos.y);

    public static Vector3 ConvertTo3D(this Vector2 pos, float zPos = 0f) => new Vector3(pos.x, pos.y, zPos);

    public static float RoundToDirection(this float Input, int angleCount) => Input.RoundToAngle(360f / angleCount);

    public static float RoundToAngle(this float Input, float angle) {
        Input /= angle;
        Input = Mathf.Round(Input);
        Input *= angle;

        return Input;
    }

    public static float ConvertToDirection(this Vector2 ang) => Vector2.SignedAngle(Vector2.up, ang);

    public static float Wrap(this float Input, float minWrap, float maxWrap) {
        float size = maxWrap - minWrap;
        while (Input < minWrap) {
            Input += size;
        }
        while (Input > maxWrap) {
            Input -= size;
        }
        return Input;
    }

    public static int Wrap(this int Input, int minWrap, int maxWrap) {
        int size = maxWrap - minWrap;
        while (Input < minWrap) {
            Input += size;
        }
        while (Input > maxWrap) {
            Input -= size;
        }
        return Input;
    }

    public static Vector2 Wrap(this Vector2 input, Vector2 XMinmax, Vector2 YMinMax){
        input.x = input.x.Wrap(XMinmax.x, XMinmax.y);
        input.y = input.y.Wrap(YMinMax.x, YMinMax.y);
        return input;
    }

    public static Vector3 Wrap(this Vector3 input, Vector3 XMinmax, Vector3 YMinMax, Vector3 ZMinMax){
        input.x = input.x.Wrap(XMinmax.x, XMinmax.y);
        input.y = input.y.Wrap(YMinMax.x, YMinMax.y);
        input.z = input.z.Wrap(ZMinMax.x, ZMinMax.y);
        return input;
    }

    public static float Dot01(Vector2 input, Vector2 check) {
        float dot = Vector2.Dot(input, check);
        dot++;
        dot /= 2f;
        return dot;
    }

    public static float Dot01(Vector3 input, Vector3 check) {
        float dot = Vector3.Dot(input, check);
        dot++;
        dot /= 2f;
        return dot;
    }

    public static Vector2 ConvertToVector(this float input, float reference = 360f) => GetLookupDirection(input / reference);

    public static float Deg2Rad(this float input) => input * Mathf.Deg2Rad;

    public static float Rad2Deg(this float input) => input * Mathf.Rad2Deg;

    public static Vector3 Round(this Vector3 input) => new Vector3(Mathf.Round(input.x), Mathf.Round(input.y), Mathf.Round(input.z));
    public static Vector3 Round(this Vector2 input) => new Vector2(Mathf.Round(input.x), Mathf.Round(input.y));

    public static Vector3 Invert(this Vector3 input, bool x = false, bool y = false, bool z = false) => new Vector3(input.x * (x ? -1f : 1f), input.y * (y ? -1f : 1f), input.z * (z ? -1f : 1f));
    public static Vector2 Invert(this Vector2 input, bool x = false, bool y = false) => new Vector2(input.x * (x ? -1f : 1f), input.y * (y ? -1f : 1f));
    public static Vector2 Flip(this Vector2 input) => new Vector2(input.y, input.x);

    public static float Clamp(this float input, float Min, float Max) => Mathf.Clamp(input, Min, Max);
    public static int Clamp(this int input, int Min, int Max) => Mathf.Clamp(input, Min, Max);
    public static float GetLookupSin(float input) => SinCosinLookup.SinLookup[Mathf.RoundToInt(input.Wrap(0f, 1f) * (SinCosinLookup.SinLookup.Length - 1))];
    public static float GetLookupCos(float input) => SinCosinLookup.CosinLookup[Mathf.RoundToInt(input.Wrap(0f, 1f) * (SinCosinLookup.CosinLookup.Length - 1))];
    public static Vector2 GetLookupDirection(float input) => SinCosinLookup.VectorLookup[Mathf.RoundToInt(input.Wrap(0f, 1f) * (SinCosinLookup.VectorLookup.Length - 1))];
}