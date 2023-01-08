using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AudioController : MonoBehaviour
{
    private static AudioController audioController;

    // Key for save load value
    private const string AUDIO_SETTING_SOUND = "Audio_Sound";
    private const string AUDIO_SETTING_MUSIC = "Audio_Music";

    // Sound list and list music
    [Space(10)]
    public List<Sound> Sounds;

    [Space(10)]
    public List<Music> Musics;

    private GameObject _targetGameObject;

    private List<AudioSource> audioSources = new List<AudioSource>();

    private List<AudioSource> activeSounds = new List<AudioSource>();
    private List<AudioSource> activeMusic = new List<AudioSource>();

    private bool _soundState;
    private bool _musicState;

    //---------------------------Audio Event----------------------------------


    //---------------------------Unity Functions----------------------------------
    void Awake()
    {
        audioController = this;

        CreateAudioSourceGO();

        LoadAudioSetting();
    }

    // private void Update() {
    //     if(Input.GetKeyDown(KeyCode.Alpha1))
    //     {
    //         PlaySound(SoundClips.Sound_01);
    //     }
    //     else if(Input.GetKeyDown(KeyCode.Alpha2))
    //     {
    //         PlaySound(SoundClips.Sound_02);
    //     }
    // }

    //-----------------------------Audio Controller Functions--------------------------------

    //====================Private Function===================
    private void CreateAudioSourceGO()
    {
        _targetGameObject = new GameObject("Audio Source Holder");
        _targetGameObject.transform.SetParent(this.transform);

    }

    private AudioSource GetAudioSource()
    {
        int sourcesAmount = audioSources.Count;
        for (int i = 0; i < sourcesAmount; i++)
        {
            if (!audioSources[i].isPlaying)
            {
                return audioSources[i];
            }
        }

        AudioSource createdSource = CreateAudioSourceObject();
        audioSources.Add(createdSource);

        return createdSource;
    }

    private AudioSource CreateAudioSourceObject()
    {
        AudioSource audioSource = _targetGameObject.AddComponent<AudioSource>();
        SetSourceDefaultSettings(audioSource);

        return audioSource;
    }

    private static void SetSourceDefaultSettings(AudioSource source, AudioType type = AudioType.Sound)
    {
        if (type == AudioType.Sound)
        {
            source.loop = false;
            source.mute = !audioController._soundState;
        }
        else if (type == AudioType.Music)
        {
            source.loop = true;
            source.mute = !audioController._musicState;
        }

        source.clip = null;

        source.volume = 1f;
        source.pitch = 1.0f;
        source.spatialBlend = 0; // 2D Sound
        source.playOnAwake = false;
        source.outputAudioMixerGroup = null;
    }

    private static void AddSound(AudioSource source)
    {
        if (!audioController.activeSounds.Contains(source))
        {
            audioController.activeSounds.Add(source);
        }
    }

    private static void AddMusic(AudioSource source)
    {
        if (!audioController.activeMusic.Contains(source))
        {
            audioController.activeMusic.Add(source);
        }
    }

    private void StopSound(AudioSource source, float fadeTime = 0)
    {
        int streamID = activeSounds.FindIndex(x => x == source);
        if (streamID != -1)
        {
            if (fadeTime == 0)
            {
                activeSounds[streamID].Stop();
                activeSounds[streamID].clip = null;
                activeSounds.RemoveAt(streamID);
            }
            else
            {
                activeSounds[streamID].DOVolume(0f, fadeTime, callBack: () =>
                {
                    activeSounds.Remove(source);
                    source.Stop();
                });
            }
        }
    }

    private void StopMusic(AudioSource source, float fadeTime = 0)
    {
        int streamID = activeMusic.FindIndex(x => x == source);
        if (streamID != -1)
        {
            if (fadeTime == 0)
            {
                activeMusic[streamID].Stop();
                activeMusic[streamID].clip = null;
                activeMusic.RemoveAt(streamID);
            }
            else
            {
                activeMusic[streamID].DOVolume(0f, fadeTime, callBack: () =>
                {
                    activeMusic.Remove(source);
                    source.Stop();
                });
            }
        }
    }

    // Get sound audio
    private Sound GetSound(SoundClips soundClip)
    {
        return Sounds.Find(s => s.soundName == soundClip.ToString());
    }

    // Get music audio
    private Music GetMusic(MusicClips musicClip)
    {
        return Musics.Find(m => m.musicName == musicClip.ToString());
    }

    // Save and load
    private void SaveAudioSetting()
    {
        PrefsSettings.SetBool(AUDIO_SETTING_SOUND, _soundState);
        PrefsSettings.SetBool(AUDIO_SETTING_MUSIC, _musicState);
    }

    private void LoadAudioSetting()
    {
        // Load sound
        if(PlayerPrefs.HasKey(AUDIO_SETTING_SOUND))
        {
            _soundState = PrefsSettings.GetBool(AUDIO_SETTING_SOUND);
        }
        else
        {
            _soundState = true;
            PrefsSettings.SetBool(AUDIO_SETTING_SOUND, _soundState);
        }

        // Load music
        if(PlayerPrefs.HasKey(AUDIO_SETTING_MUSIC))
        {
            _musicState = PrefsSettings.GetBool(AUDIO_SETTING_MUSIC);
        }
        else
        {
            _musicState = true;
            PrefsSettings.SetBool(AUDIO_SETTING_MUSIC, _musicState);
        }
    }

    //====================Public Function===================
    public static Coroutine InvokeAudioCoroutine(IEnumerator enumerator)
    {
        return audioController.StartCoroutine(enumerator);
    }

    public static void StopAudioCoroutine(Coroutine coroutine)
    {
        audioController.StopCoroutine(coroutine);
    }

    public static void PlaySound(SoundClips clip, float volumePercentage = 1.0f, float pitch = 1.0f)
    {
        Sound sound = audioController.GetSound(clip);

        if (sound.clip == null)
            Debug.LogError("[AudioController]: Audio clip is null");

        AudioSource source = audioController.GetAudioSource();

        SetSourceDefaultSettings(source, AudioType.Sound);

        source.clip = sound.clip;
        source.volume *= volumePercentage;
        source.pitch = pitch;
        source.Play();

        AddSound(source);
    }

    public static void PlayMusic(MusicClips clip, float volumePercentage = 1.0f)
    {
        Music music = audioController.GetMusic(clip);

        if (music.clip == null)
            Debug.LogError("[AudioController]: Audio clip is null");

        AudioSource source = audioController.GetAudioSource();

        SetSourceDefaultSettings(source, AudioType.Music);

        source.clip = music.clip;
        source.volume *= volumePercentage;
        source.Play();

        AddMusic(source);
    }

    public static AudioCase PlaySmartSound(SoundClips clip, float volumePercentage = 1.0f, float pitch = 1.0f)
    {
        Sound sound = audioController.GetSound(clip);

        if (sound.clip == null)
            Debug.LogError("[AudioController]: Audio clip is null");

        AudioSource source = audioController.GetAudioSource();

        SetSourceDefaultSettings(source, AudioType.Sound);

        source.clip = sound.clip;
        source.volume *= volumePercentage;
        source.pitch = pitch;

        AudioCase audioCase = new AudioCase(sound.clip, source, AudioType.Sound);
        audioCase.Play();

        AddSound(source);

        return audioCase;
    }

    public static AudioCase PlaySmartMusic(MusicClips clip, float volumePercentage = 1.0f, float pitch = 1.0f)
    {
        Music music = audioController.GetMusic(clip);

        if (music.clip == null)
            Debug.LogError("[AudioController]: Audio clip is null");

        AudioSource source = audioController.GetAudioSource();

        SetSourceDefaultSettings(source, AudioType.Music);

        source.clip = music.clip;
        source.volume *= volumePercentage;
        source.pitch = pitch;

        AudioCase audioCase = new AudioCase(music.clip, source, AudioType.Music);

        audioCase.Play();

        AddMusic(source);

        return audioCase;
    }

    public static void PlayRandomMusic()
    {
        if (audioController.Musics.Count > 0)
        {
            MusicClips musicClip = Extensions.RandomEnumValue<MusicClips>();

            PlayMusic(musicClip);
        }

    }

    // Releasing all active sounds.
    private static void ReleaseSounds()
    {
        int activeStreamsCount = audioController.activeSounds.Count - 1;
        for (int i = activeStreamsCount; i >= 0; i--)
        {
            audioController.activeSounds[i].Stop();
            audioController.activeSounds[i].clip = null;
            audioController.activeSounds.RemoveAt(i);
        }
    }

    // Releasing all active music.
    private static void ReleaseMusic()
    {
        int activeMusicCount = audioController.activeMusic.Count - 1;
        for (int i = activeMusicCount; i >= 0; i--)
        {
            audioController.activeMusic[i].Stop();
            audioController.activeMusic[i].clip = null;
            audioController.activeMusic.RemoveAt(i);
        }
    }

    public static void StopStream(AudioCase audioCase, float fadeTime = 0)
    {
        if (audioCase.type == AudioType.Sound)
        {
            audioController.StopSound(audioCase.source, fadeTime);
        }
        else
        {
            audioController.StopMusic(audioCase.source, fadeTime);
        }
    }

    public static void TurnOnSound()
    {
        audioController._soundState = true;
        audioController.SaveAudioSetting();
    }

    public static void TurnOffSound()
    {
        audioController._soundState = false;
        audioController.SaveAudioSetting();

        ReleaseSounds();
    }

    public static void TurnOnMusic()
    {
        audioController._musicState = true;
        audioController.SaveAudioSetting();
    }

    public static void TurnOffMusic()
    {
        audioController._musicState = false;
        audioController.SaveAudioSetting();

        ReleaseMusic();
    }

}
