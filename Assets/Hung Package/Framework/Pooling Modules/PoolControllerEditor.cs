using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(PoolController), true)]
public class PoolControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        PoolController soundManager = (PoolController)target;

        EditorGUILayout.LabelField("");
        EditorGUILayout.HelpBox("After assigning pool object, Hit this button to get an enum of audioclip names created/updated. Makes method calls easy.\n ** NOTE : Make sure the name is a valid enum name **\n" +
                                " Example - PoolController.ReuseObject(PoolKey.Demo);", MessageType.Info);

        if (GUILayout.Button("Generate PoolKey Names Enum"))
        {
            GeneratePoolKeyEnum();
        }
    }

    /// <summary>
    /// Creates the soundClips enum at the soundManager's location
    /// </summary>
    public void GeneratePoolKeyEnum()
    {
        //Get the list of audios
        PoolController poolController = (PoolController)target;
        List<PoolItem> listPool = poolController.ItemsToPool;

        //Get the script's path
        MonoScript thisScript = MonoScript.FromMonoBehaviour(poolController);
        string ScriptFilePath = AssetDatabase.GetAssetPath(thisScript);

        //Create a path for the enum file
        string enumName = "PoolKey";
        string filePathAndName = ScriptFilePath.Replace(thisScript.name + ".cs", "") + "/" + enumName + ".cs";

        //Wrire the enum at above path
        using (StreamWriter streamWriter = new StreamWriter(filePathAndName))
        {
            streamWriter.WriteLine("public enum " + enumName);
            streamWriter.WriteLine("{");
            for (int i = 0; i < listPool.Count; i++)
            {
                streamWriter.WriteLine("\t" + listPool[i].itemName + ",");
            }
            streamWriter.WriteLine("}");
        }
        AssetDatabase.Refresh();

        Debug.Log("PoolKey enum created/updated at " + ScriptFilePath);
    }
}

#endif