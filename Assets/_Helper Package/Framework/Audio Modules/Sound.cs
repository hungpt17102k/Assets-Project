using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    [Tooltip("Name with no space")]
    public string soundName;
    public AudioClip clip;
}
