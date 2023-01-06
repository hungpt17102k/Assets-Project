using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BravestarsAdsManager))]
public class BravestarsAdsManagerEditor : Editor
{
    string adsDefine = "ADS";

    BravestarsAdsManager adsManager;

    BuildTarget activeBuildTarget = BuildTarget.NoTarget;

    public override void OnInspectorGUI()
    {
         adsManager = (BravestarsAdsManager)target;

        if (EditorUserBuildSettings.activeBuildTarget != activeBuildTarget)
        {
            EditorUtil.ToggleDefine(adsManager.isAdEnabled, adsDefine);
            activeBuildTarget = EditorUserBuildSettings.activeBuildTarget;
        }

        GUILayout.BeginVertical(EditorStyles.helpBox);

        GUILayout.Label("Requirement:");

        EditorGUILayout.HelpBox("• To use Ads, you must import the MAX sdk, Firebase, AppFlyer." +
                                    "\n**IMPORTANT** Enabling ad without doing so will produce compile errors. **IMPORTANT**\n" +
                                    "\n• Do not enable ads in services (Legacy)\n"+
                                    "\n• Platform must be Android/iOS", MessageType.Info);

        adsManager.isAdEnabled = EditorGUILayout.Toggle("Enable Ad", adsManager.isAdEnabled);

        GUILayout.EndVertical();

        GUI.enabled = adsManager.isAdEnabled;
        EditorUtil.DrawDefaultInspector(serializedObject);
 
        if (GUI.changed) {
            EditorUtil.ToggleDefine(adsManager.isAdEnabled, adsDefine);
            EditorUtility.SetDirty(adsManager);  
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty (adsManager.gameObject.scene);
        }
    }
}
