using UnityEngine;
using System.Collections;
using UnityEditor;
using SmartMaker;

[CustomEditor(typeof(CommBluetooth))]
public class CommBluetoothInspector : Editor
{
	bool foldout = true;
    bool foldout2 = true;
	
    SerializedProperty owner;
	SerializedProperty devNames;
	SerializedProperty devName;
    SerializedProperty baudrate;
    SerializedProperty streamClass;
	SerializedProperty uiText;
	SerializedProperty uiPanel;
	SerializedProperty uiItem;

	void OnEnable()
	{
        owner = serializedObject.FindProperty("owner");
		devNames = serializedObject.FindProperty("devNames");
		devName = serializedObject.FindProperty("devName");
        baudrate = serializedObject.FindProperty("baudrate");
        streamClass = serializedObject.FindProperty("streamClass");
		uiText = serializedObject.FindProperty("uiText");
		uiPanel = serializedObject.FindProperty("uiPanel");
		uiItem = serializedObject.FindProperty("uiItem");
	}
	
	public override void OnInspectorGUI()
	{
#if !(UNITY_ANDROID || UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
		EditorGUILayout.HelpBox("This component only can work on window/android", MessageType.Error);
#endif
		this.serializedObject.Update();
		
		CommBluetooth bluetooth = (CommBluetooth)target;

        EditorGUILayout.PropertyField(owner, new GUIContent("Owner"));
		
		GUI.enabled = !bluetooth.IsOpen;

        EditorGUILayout.PropertyField(devName, new GUIContent("Device Name"));
        EditorGUILayout.BeginHorizontal();
        int index = -1;
        string[] list = new string[devNames.arraySize];
        for(int i=0; i<list.Length; i++)
        {
            list[i] = devNames.GetArrayElementAtIndex(i).stringValue;
            if(devName.stringValue.Equals(list[i]) == true)
                index = i;
        }
        index = EditorGUILayout.Popup(" ", index, list);
        if(index >= 0)
            devName.stringValue = list[index];
        if(GUILayout.Button("Search", GUILayout.Width(60f)) == true)
            bluetooth.DeviceSearch();
        EditorGUILayout.EndHorizontal();
		
        if(Application.isPlaying == false)
        {
            foldout2 = EditorGUILayout.Foldout(foldout2, "Sketch Options");
            if(foldout2 == true)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(streamClass, new GUIContent("Stream Class"));
                EditorGUILayout.PropertyField(baudrate, new GUIContent("Baudrate"));
                EditorGUI.indentLevel--;
            }
        }
		
		foldout = EditorGUILayout.Foldout(foldout, "UI objects");
		if(foldout == true)
		{
			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField(uiText, new GUIContent("UI Text"));
			EditorGUILayout.PropertyField(uiPanel, new GUIContent("UI Panel"));
			EditorGUILayout.PropertyField(uiItem, new GUIContent("UI Item"));
			EditorGUI.indentLevel--;
		}

		this.serializedObject.ApplyModifiedProperties();
	}
}
