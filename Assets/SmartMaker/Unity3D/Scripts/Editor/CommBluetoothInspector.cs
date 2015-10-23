using UnityEngine;
using System.Collections;
using UnityEditor;
using SmartMaker;

[CustomEditor(typeof(CommBluetooth))]
public class CommBluetoothInspector : Editor
{
	SerializedProperty device;
    SerializedProperty searchTimeout;
    SerializedProperty OnOpen;
    SerializedProperty OnClose;
    SerializedProperty OnOpenFailed;
    SerializedProperty OnErrorClosed;
    SerializedProperty OnStartSearch;
    SerializedProperty OnStopSearch;
    SerializedProperty OnFoundDevice;


    void OnEnable()
	{
        device = serializedObject.FindProperty("device");
        searchTimeout = serializedObject.FindProperty("searchTimeout");
        OnOpen = serializedObject.FindProperty("OnOpen");
        OnOpenFailed = serializedObject.FindProperty("OnOpenFailed");
        OnErrorClosed = serializedObject.FindProperty("OnErrorClosed");
        OnStartSearch = serializedObject.FindProperty("OnStartSearch");
        OnStopSearch = serializedObject.FindProperty("OnStopSearch");
        OnFoundDevice = serializedObject.FindProperty("OnFoundDevice");
    }
	
	public override void OnInspectorGUI()
	{
#if !UNITY_ANDROID
		EditorGUILayout.HelpBox("This component only can work on android", MessageType.Error);
#endif
		this.serializedObject.Update();
		
	//	CommBluetooth bluetooth = (CommBluetooth)target;

        EditorGUILayout.PropertyField(device, new GUIContent("Device"), true);
        EditorGUILayout.PropertyField(searchTimeout, new GUIContent("Search Timeout"));

        EditorGUILayout.Separator();
        EditorGUILayout.PropertyField(OnOpen, new GUIContent("OnOpen"));
        EditorGUILayout.PropertyField(OnOpenFailed, new GUIContent("OnOpenFailed"));
        EditorGUILayout.PropertyField(OnErrorClosed, new GUIContent("OnErrorClosed"));
        EditorGUILayout.PropertyField(OnStartSearch, new GUIContent("OnStartSearch"));
        EditorGUILayout.PropertyField(OnStopSearch, new GUIContent("OnStopSearch"));
        EditorGUILayout.PropertyField(OnFoundDevice, new GUIContent("OnFoundDevice"));

        this.serializedObject.ApplyModifiedProperties();
	}
}
