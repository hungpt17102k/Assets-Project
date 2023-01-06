using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public static class TweenExtension
{
    public static Tweener DOVolume(this AudioSource tweenObject, float resultValue, float time, TweenCallback callBack)
    {
        return tweenObject.DOFade(resultValue, time).OnComplete(callBack);
    }
}
