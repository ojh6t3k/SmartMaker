using UnityEngine;
using System.Collections;
using UnityEditor;
using SmartMaker;

[CustomEditor(typeof(CommSerial))]
public class CommSerialInspector : Editor
{
	SerializedProperty portNames;
	SerializedProperty portName;

	void OnEnable()
	{
		portNames = serializedObject.FindProperty("portNames");
		portName = serializedObject.FindProperty("portName");
	}
	
	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();

		CommSerial serial = (CommSerial)target;

		GUI.enabled = !serial.IsOpen;

		EditorGUILayout.BeginHorizontal();
		int index = -1;
		string[] list = new string[portNames.arraySize];
		for(int i=0; i<list.Length; i++)
		{
			list[i] = portNames.GetArrayElementAtIndex(i).stringValue;
			if(portName.stringValue.Equals(list[i]) == true)
				index = i;
		}
		index = EditorGUILayout.Popup("Port Name", index, list);
		if(index >= 0)
			portName.stringValue = list[index];
		else
			portName.stringValue = "";
		if(GUILayout.Button("Search") == true)
			serial.PortSearch();
		EditorGUILayout.EndHorizontal();

		this.serializedObject.ApplyModifiedProperties();
	}
}
