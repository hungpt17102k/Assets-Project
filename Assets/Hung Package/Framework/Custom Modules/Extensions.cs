using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class Extensions
{
    public static float ScaleValue(this float OldValue, float OldMin, float OldMax, float NewMin, float NewMax)
    {
        float OldRange = (OldMax - OldMin);
        float NewRange = (NewMax - NewMin);
        float NewValue = (((OldValue - OldMin) * NewRange) / OldRange) + NewMin;

        return (NewValue);
    }

    public static List<GameObject> FindObjectsWithTag(this Transform parent, string tag)
    {
        List<GameObject> taggedGameObjects = new List<GameObject>();

        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            if (child.tag == tag)
            {
                taggedGameObjects.Add(child.gameObject);
            }
            if (child.childCount > 0)
            {
                taggedGameObjects.AddRange(FindObjectsWithTag(child, tag));
            }
        }
        return taggedGameObjects;
    }

    public static float GetDifficultyPercent(float timer, float secondsToMaxDifficulty)
    {
        return Mathf.Clamp01(timer / secondsToMaxDifficulty);
    }

    public static float ValueIncreaseOverTime(float min, float max, float timer, float secondsToMaxDifficulty)
    {
        return Mathf.Lerp(min, max, GetDifficultyPercent(timer, secondsToMaxDifficulty));
    }

    public static int ConvertStringToInt(this string text)
    {
        return Int32.Parse(text);
    }

    public static IEnumerator DelayCallBackByTime(Action callback, float time = 0f)
    {
        yield return new WaitForSeconds(time);
        callback.Invoke();
    }

    public static IEnumerator DelayCallBackByFrame(Action callback)
    {
        yield return new WaitForEndOfFrame();
        callback.Invoke();
    }

    public static void ChangePhysic2DVelocityInteration(int step)
    {
        Physics2D.velocityIterations = step;
    }

    public static T RandomEnumValue<T>()
    {
        System.Random _R = new System.Random();
        var v = Enum.GetValues(typeof(T));
        return (T)v.GetValue(_R.Next(v.Length));
    }

}
