using UnityEngine;
using System.Collections;
using UnityEditor;
using SmartMaker;

[CustomEditor(typeof(CommWiFi))]
public class CommWiFiInspector : Editor
{
	SerializedProperty ipAddress;
	SerializedProperty port;
    SerializedProperty OnOpen;
    SerializedProperty OnClose;
    SerializedProperty OnOpenFailed;
    SerializedProperty OnErrorClosed;

    void OnEnable()
	{
		ipAddress = serializedObject.FindProperty("ipAddress");
		port = serializedObject.FindProperty("port");
        OnOpen = serializedObject.FindProperty("OnOpen");
        OnClose = serializedObject.FindProperty("OnClose");
        OnOpenFailed = serializedObject.FindProperty("OnOpenFailed");
        OnErrorClosed = serializedObject.FindProperty("OnErrorClosed");
    }

	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
		CommWiFi wifi = (CommWiFi)target;
		
		GUI.enabled = !wifi.IsOpen;
		EditorGUILayout.PropertyField(ipAddress, new GUIContent("IP Address"));
		EditorGUILayout.PropertyField(port, new GUIContent("Port"));

        EditorGUILayout.Separator();
        EditorGUILayout.PropertyField(OnOpen, new GUIContent("OnOpen"));
        EditorGUILayout.PropertyField(OnClose, new GUIContent("OnClose"));
        EditorGUILayout.PropertyField(OnOpenFailed, new GUIContent("OnOpenFailed"));
        EditorGUILayout.PropertyField(OnErrorClosed, new GUIContent("OnErrorClosed"));

        this.serializedObject.ApplyModifiedProperties();
	}
}
