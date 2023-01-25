using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AdsManager))]
public class AdsManagerEditor : Editor
{
    string adsDefine = "ADS";

    AdsManager adsManager;

    BuildTarget activeBuildTarget = BuildTarget.NoTarget;

    public override void OnInspectorGUI()
    {
         adsManager = (AdsManager)target;

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
        // Button for link download
        if (GUILayout.Button("Click here for Firebase package"))
        {
            Application.OpenURL("https://firebase.google.com/download/unity");
        }

        if (GUILayout.Button("Click here for Max AppLovin package"))
        {
            Application.OpenURL("https://dash.applovin.com/documentation/mediation/unity/getting-started/integration");
        }

        if (GUILayout.Button("Click here for AppFlyer package"))
        {
            Application.OpenURL("https://github.com/AppsFlyerSDK/appsflyer-unity-plugin/releases");
        }

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
