using System;
using UnityEngine;

static public class Math
{
    public static float EaseInSine(float x)
    {
        return 1 - MathF.Cos(x * MathF.PI / 2);
    }
    public static float EaseOutSine(float x)
    {
        return Mathf.Sin(x * Mathf.PI / 2);
    }
}
