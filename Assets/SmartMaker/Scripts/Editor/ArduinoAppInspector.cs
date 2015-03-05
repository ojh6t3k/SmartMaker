using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.IO;
using System.Text;
using SmartMaker;

[CustomEditor(typeof(ArduinoApp))]
public class ArduinoAppInspector : Editor
{
	bool foldout = false;

	SerializedProperty commObject;
	SerializedProperty timeoutSec;

	void OnEnable()
	{
		commObject = serializedObject.FindProperty("commObject");
		timeoutSec = serializedObject.FindProperty("timeoutSec");
	}

	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();

		ArduinoApp arduino = (ArduinoApp)target;

		if(Application.isPlaying == false)
		{
			if(GUILayout.Button("Create Sketch") == true)
			{
				if(CreateSketch(EditorUtility.SaveFolderPanel("Select Folder", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "")) == false)
					Debug.LogError("Failed to create sketch!");
			}

			foldout = EditorGUILayout.Foldout(foldout, "Sketch Options");
			if(foldout == true)
			{
			}
		}

		EditorGUILayout.PropertyField(commObject, new GUIContent("CommObject"));
		EditorGUILayout.PropertyField(timeoutSec, new GUIContent("Timeout(sec)"));

		if(Application.isPlaying == true && arduino.commObject != null)
		{
			GUI.enabled = true;
			if(arduino.Connected == true)
			{
				if(GUILayout.Button("Disconnect") == true)
					arduino.Disconnect();
			}
			else
			{
				if(GUILayout.Button("Connect") == true)
					arduino.Connect();
			}
			
			EditorUtility.SetDirty(target);
		}

		EventDelegateEditor.Field("OnConnected", arduino.OnConnected);

		this.serializedObject.ApplyModifiedProperties();
	}

	private bool CreateSketch(string path)
	{
		StringBuilder source = new StringBuilder();

		source.AppendLine();
		source.AppendLine("void setup()");
		source.AppendLine("{");
		source.AppendLine("}");
		source.AppendLine("void loop()");
		source.AppendLine("{");
		source.AppendLine("}");

		path = Path.Combine(path, "SmartMaker");
		Directory.CreateDirectory(path);
		StreamWriter sw = new StreamWriter(Path.Combine(path, "SmartMaker.ino"));
		sw.Write(source.ToString());
		sw.Close();

		return true;
	}
}
