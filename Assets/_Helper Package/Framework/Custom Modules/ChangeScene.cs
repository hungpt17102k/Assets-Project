#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;


public class ChangeScene : Editor
{

    [MenuItem("Open Scene/Game Scene")]
    public static void OpenGame()
    {
        OpenScene("GameScene");
    }

    // [MenuItem("Open Scene/Game #2")]
    // public static void OpenGame()
    // {
    //     OpenScene("Main-Update");
    // }

    private static void OpenScene(string sceneName)
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene("Assets/Scenes/" + sceneName + ".unity");
        }
    }
}
#endif