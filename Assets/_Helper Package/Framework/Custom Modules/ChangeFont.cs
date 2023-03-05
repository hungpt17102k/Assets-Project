using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class ChangeFont : MonoBehaviour
{
    [Header("New Font to Change")]
    public Font newFont;

    [Header("Font Style")]
    public FontStyle style;

    [Header("Color of Font")]
    public Color colorFont;

    public void ChangeToNewFont()
    {
        var objsText = Resources.FindObjectsOfTypeAll(typeof(Text)) as Text[];

        foreach (var text in objsText)
        {
            text.font = newFont;
            text.fontStyle = style;
            text.color = colorFont;
        }
    }
}

#if UNITY_EDITOR 
[CustomEditor(typeof(ChangeFont))]
public class CustomInspectorFont : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ChangeFont fontChange = (ChangeFont)target;

        if (GUILayout.Button("Change Font"))
        {
            fontChange.ChangeToNewFont();
        }
    }
}

#endif