using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(AudioController), true)]
public class AudioControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        AudioController soundManager = (AudioController)target;

        EditorGUILayout.LabelField("");
        EditorGUILayout.HelpBox("After assigning Audio clips, Hit this button to get an enum of audioclip names created/updated. Makes method calls easy.\n ** NOTE : Make sure the name is a valid enum name **\n" +
                                " Example - AudioController.PlaySound(SoundClips.Demo);", MessageType.Info);

        if (GUILayout.Button("Generate Sound Clip Names Enum"))
        {
            GenerateSoundClipsEnum();
        }

        if (GUILayout.Button("Generate Music Clip Names Enum"))
        {
            GenerateMusicClipsEnum();
        }
    }

    /// <summary>
    /// Creates the soundClips enum at the soundManager's location
    /// </summary>
    public void GenerateSoundClipsEnum()
    {
        //Get the list of audios
        AudioController soundManager = (AudioController)target;
        List<Sound> sounds = soundManager.Sounds;

        //Get the script's path
        MonoScript thisScript = MonoScript.FromMonoBehaviour(soundManager);
        string ScriptFilePath = AssetDatabase.GetAssetPath(thisScript);

        //Create a path for the enum file
        string enumName = "SoundClips";
        string filePathAndName = ScriptFilePath.Replace(thisScript.name + ".cs", "") + "/" + enumName + ".cs";

        //Wrire the enum at above path
        using (StreamWriter streamWriter = new StreamWriter(filePathAndName))
        {
            streamWriter.WriteLine("public enum " + enumName);
            streamWriter.WriteLine("{");
            for (int i = 0; i < sounds.Count; i++)
            {
                streamWriter.WriteLine("\t" + sounds[i].soundName + ",");
            }
            streamWriter.WriteLine("}");
        }
        AssetDatabase.Refresh();

        Debug.Log("SoundClips  enum created/updated at " + ScriptFilePath);
    }

    /// <summary>
    /// Creates the musicClips enum at the soundManager's location
    /// </summary>
    public void GenerateMusicClipsEnum()
    {
        //Get the list of audios
        AudioController soundManager = (AudioController)target;
        List<Music> musics = soundManager.Musics;

        //Get the script's path
        MonoScript thisScript = MonoScript.FromMonoBehaviour(soundManager);
        string ScriptFilePath = AssetDatabase.GetAssetPath(thisScript);

        //Create a path for the enum file
        string enumName = "MusicClips";
        string filePathAndName = ScriptFilePath.Replace(thisScript.name + ".cs", "") + "/" + enumName + ".cs";

        //Wrire the enum at above path
        using (StreamWriter streamWriter = new StreamWriter(filePathAndName))
        {
            streamWriter.WriteLine("public enum " + enumName);
            streamWriter.WriteLine("{");
            for (int i = 0; i < musics.Count; i++)
            {
                streamWriter.WriteLine("\t" + musics[i].musicName + ",");
            }
            streamWriter.WriteLine("}");
        }
        AssetDatabase.Refresh();

        Debug.Log("MusicClips  enum created/updated at " + ScriptFilePath);
    }
}

#endif