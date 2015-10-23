using UnityEngine;
using System;

public class Vibration : MonoBehaviour
{
#if UNITY_ANDROID
    private AndroidJavaObject _android;
#endif

    void Awake()
    {
#if UNITY_ANDROID
        try
        {
            AndroidJavaClass activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            if (activityClass != null)
            {
                AndroidJavaObject context = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
                if (context != null)
                {
                    AndroidJavaClass pluginClass = new AndroidJavaClass("com.smartmaker.android.Vibration");
                    if (pluginClass != null)
                    {
                        _android = pluginClass.CallStatic<AndroidJavaObject>("GetInstance");
                        if (!_android.Call<bool>("Initialize", context))
                            _android = null;
                    }
                }
            }
        }
        catch(Exception)
        {
            _android = null;
        }        

        if (_android == null)
            Debug.Log("Android Vibration Failed!");
#endif
    }

    // Use this for initialization
    void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    public void Vibrate(int milliseconds)
    {
#if UNITY_ANDROID
        if (_android != null)
            _android.Call("Vibrate", milliseconds);
#endif
    }
}
