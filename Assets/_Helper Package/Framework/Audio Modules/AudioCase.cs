using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[System.Serializable]
public class AudioCase
{
    public AudioSource source;

    public AudioType type;

    public delegate void AudioCallback();
    public AudioCallback onAudioEnded;

    private Coroutine endCoroutine;

    public AudioCase(AudioClip clip, AudioSource source, AudioType type, AudioCallback callback = null)
    {
        this.source = source;
        this.type = type;
        this.source.clip = clip;
    }

    private IEnumerator OnAudioEndCoroutine(float clipDuration)
    {
        yield return new WaitForSeconds(clipDuration);

        onAudioEnded.Invoke();
    }

    public AudioCase OnComplete(AudioCallback callback)
    {
        onAudioEnded = callback;

        endCoroutine = AudioController.InvokeAudioCoroutine(OnAudioEndCoroutine(source.clip.length));

        return this;
    }

    public virtual void Play()
    {
        source.Play();
    }

    public void Stop()
    {
        source.Stop();

        if (endCoroutine != null) {
            AudioController.StopAudioCoroutine(endCoroutine);
        }
    }

}

public enum AudioType
{
    Music = 0,
    Sound = 1
}
