using UnityEngine;
using System.Collections;
using UnityEditor;
using SmartMaker;

[CustomEditor(typeof(CommBluetooth))]
public class CommBluetoothInspector : Editor
{
	bool foldout = true;
	
	SerializedProperty devNames;
	SerializedProperty devName;
	SerializedProperty uiText;
	SerializedProperty uiPanel;
	SerializedProperty uiItem;

	void OnEnable()
	{
		devNames = serializedObject.FindProperty("devNames");
		devName = serializedObject.FindProperty("devName");
		uiText = serializedObject.FindProperty("uiText");
		uiPanel = serializedObject.FindProperty("uiPanel");
		uiItem = serializedObject.FindProperty("uiItem");
	}
	
	public override void OnInspectorGUI()
	{
#if !UNITY_ANDROID
		EditorGUILayout.HelpBox("This component only can work on android", MessageType.Error);
#endif
		this.serializedObject.Update();
		
		CommBluetooth bluetooth = (CommBluetooth)target;
		
		GUI.enabled = !bluetooth.IsOpen;
		
		EditorGUILayout.BeginHorizontal();
		int index = -1;
		string[] list = new string[devNames.arraySize];
		for(int i=0; i<list.Length; i++)
		{
			list[i] = devNames.GetArrayElementAtIndex(i).stringValue;
			if(devName.stringValue.Equals(list[i]) == true)
				index = i;
		}
		index = EditorGUILayout.Popup("Device Name", index, list);
		if(index >= 0)
			devName.stringValue = list[index];
		else
			devName.stringValue = "";
		if(GUILayout.Button("Search") == true)
			bluetooth.DeviceSearch();
		EditorGUILayout.EndHorizontal();
		
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
