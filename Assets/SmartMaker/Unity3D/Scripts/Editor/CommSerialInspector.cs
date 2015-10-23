using UnityEngine;
using System.Collections;
using UnityEditor;
using SmartMaker;

[CustomEditor(typeof(CommSerial))]
public class CommSerialInspector : Editor
{
    SerializedProperty baudrate;
    SerializedProperty OnOpen;
    SerializedProperty OnClose;
    SerializedProperty OnOpenFailed;
    SerializedProperty OnErrorClosed;
    SerializedProperty OnStartSearch;
    SerializedProperty OnStopSearch;
    SerializedProperty OnFoundDevice;

    void OnEnable()
	{
        baudrate = serializedObject.FindProperty("baudrate");
        OnOpen = serializedObject.FindProperty("OnOpen");
        OnClose = serializedObject.FindProperty("OnClose");
        OnOpenFailed = serializedObject.FindProperty("OnOpenFailed");
        OnErrorClosed = serializedObject.FindProperty("OnErrorClosed");
        OnStartSearch = serializedObject.FindProperty("OnStartSearch");
        OnStopSearch = serializedObject.FindProperty("OnStopSearch");
        OnFoundDevice = serializedObject.FindProperty("OnFoundDevice");
    }
	
	public override void OnInspectorGUI()
	{
        this.serializedObject.Update();

        CommSerial serial = (CommSerial)target;

#if UNITY_STANDALONE
        GUI.enabled = !serial.IsOpen;

#if (UINTY_STANDALONE_WIN || UNITY_EDITOR_WIN)
        EditorGUILayout.LabelField(string.Format("Port Name: {0}", serial.device.name));
#elif (UINTY_STANDALONE_OSX || UNITY_EDITOR_OSX)
        EditorGUILayout.LabelField(string.Format("Port Name: {0}", serial.device.address));
#endif
        EditorGUILayout.BeginHorizontal();
        int index = -1;
        string[] list = new string[serial.foundDevices.Count];
        for (int i = 0; i < list.Length; i++)
        {
            list[i] = serial.foundDevices[i].name;
            if (serial.device.Equals(serial.foundDevices[i]))
                index = i;
        }
        index = EditorGUILayout.Popup(" ", index, list);
        if (index >= 0)
            serial.device = new CommDevice(serial.foundDevices[index]);
        if (GUILayout.Button("Search", GUILayout.Width(60f)) == true)
            serial.StartSearch();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.PropertyField(baudrate, new GUIContent("Baudrate"));

        GUI.enabled = true;
#else
        EditorGUILayout.HelpBox("This component only can work on standalone platform(windows, osx, linux..)", MessageType.Error);
#endif

        EditorGUILayout.Separator();
        EditorGUILayout.PropertyField(OnOpen, new GUIContent("OnOpen"));
        EditorGUILayout.PropertyField(OnClose, new GUIContent("OnClose"));
        EditorGUILayout.PropertyField(OnOpenFailed, new GUIContent("OnOpenFailed"));
        EditorGUILayout.PropertyField(OnErrorClosed, new GUIContent("OnErrorClosed"));
        EditorGUILayout.PropertyField(OnStartSearch, new GUIContent("OnStartSearch"));
        EditorGUILayout.PropertyField(OnStopSearch, new GUIContent("OnStopSearch"));
        EditorGUILayout.PropertyField(OnFoundDevice, new GUIContent("OnFoundDevice"));

        this.serializedObject.ApplyModifiedProperties();
	}
}
