using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public static class EditorUtil  {
    public static void ToggleDefine(bool status, string define)
    {
        string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
        List<string> definesList = definesString.Split(';').ToList();

        if (status && !definesList.Contains(define))
        {
            definesList.Add(define);
            Debug.Log(define + " added");
        }
        else if (!status && definesList.Contains(define))
        {
            definesList.Remove(define);
            Debug.Log(define + " removed");
        }
        else
        {
            return;
        }
        PlayerSettings.SetScriptingDefineSymbolsForGroup(
            EditorUserBuildSettings.selectedBuildTargetGroup,
            string.Join(";", definesList.ToArray()));
    }

    public static void DrawDefaultInspector(SerializedObject serializedObject, bool includeScriptName = false){
        EditorGUI.BeginChangeCheck();
        serializedObject.UpdateIfRequiredOrScript();
       
        SerializedProperty property = serializedObject.GetIterator();
        bool expanded = true;
        while (property.NextVisible(expanded))
        {
 
            if (!includeScriptName && property.propertyPath == "m_Script")
            {
                continue;
            }
            using (new EditorGUI.DisabledScope("m_Script" == property.propertyPath))
            {
                EditorGUILayout.PropertyField(property, true);
            }
            expanded = false;
        }

        serializedObject.ApplyModifiedProperties();
        EditorGUI.EndChangeCheck();
    }
}
