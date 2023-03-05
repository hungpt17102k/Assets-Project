using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemCheck : MonoBehaviour
{
    public static SystemCheck instance;

    private RuntimePlatform platformDevice;

    private float _frequency = 1.0f;
    private string _fps;

    [Header("Show FPS")]
    public bool showFPS;

    [Header("Is Connect Internet")] 
    public bool isConnectInternet;

    private void Awake() {
        Application.targetFrameRate = 60;

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        StartCoroutine(FPS());
    }

    private void LateUpdate() {
        CheckInternetConnection();
    }

    public bool IsMobileDevice()
    {
        platformDevice = Application.platform;

        if (platformDevice == RuntimePlatform.Android || platformDevice == RuntimePlatform.IPhonePlayer)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void CheckInternetConnection() {
        if(Application.internetReachability == NetworkReachability.NotReachable)
        {
            Debug.Log("Error. Check internet connection!");
            isConnectInternet = false;
        }
        else {
            isConnectInternet = true;
        }
    }

    private IEnumerator FPS() {
        for(;;){
            // Capture frame-per-second
            int lastFrameCount = Time.frameCount;
            float lastTime = Time.realtimeSinceStartup;
            yield return new WaitForSeconds(_frequency);
            float timeSpan = Time.realtimeSinceStartup - lastTime;
            int frameCount = Time.frameCount - lastFrameCount;
           
            // Display it
            _fps = string.Format("FPS: {0}" , Mathf.RoundToInt(frameCount / timeSpan));
        }
    }

    private void OnGUI()
    {
        if(!showFPS) {
            return;
        }

        GUIStyle style = new GUIStyle();
        style.fontSize = 50;
        //style.fontStyle = FontStyle.Bold;

        GUI.Label(new Rect(Screen.width - 250,10,150,50), _fps, style);
    }
}
