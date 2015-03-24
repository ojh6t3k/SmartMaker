using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;
using System.IO;
using System.Text;
using UnityEditor.Events;
using SmartMaker;

[CustomEditor(typeof(ArduinoApp))]
public class ArduinoAppInspector : Editor
{
	SerializedProperty commObject;
	SerializedProperty timeoutSec;
	SerializedProperty OnConnected;
	SerializedProperty OnConnectionFailed;
	SerializedProperty OnDisconnected;

	void OnEnable()
	{
		commObject = serializedObject.FindProperty("commObject");
		timeoutSec = serializedObject.FindProperty("timeoutSec");
		OnConnected = serializedObject.FindProperty("OnConnected");
		OnConnectionFailed = serializedObject.FindProperty("OnConnectionFailed");
		OnDisconnected = serializedObject.FindProperty("OnDisconnected");
	}

	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();

		ArduinoApp arduino = (ArduinoApp)target;

		if(Application.isPlaying == false)
		{
			if(GUILayout.Button("Create Sketch") == true)
				CreateSketch(EditorUtility.SaveFilePanel("Create Sketch", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "", "ino"), arduino.appActions);
		}
		else
		{
			if(arduino.commObject != null)
			{
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
			else
			{
				EditorGUILayout.HelpBox("CommObject is Null!", MessageType.Error);
			}
		}

		EditorGUILayout.PropertyField(commObject, new GUIContent("CommObject"));
		EditorGUILayout.PropertyField(timeoutSec, new GUIContent("Timeout(sec)"));

		EditorGUILayout.Separator();
		EditorGUILayout.PropertyField(OnConnected);
		EditorGUILayout.PropertyField(OnConnectionFailed);
		EditorGUILayout.PropertyField(OnDisconnected);

		this.serializedObject.ApplyModifiedProperties();
	}

	private void CreateSketch(string file, AppAction[] actions)
	{
		List<Type> types = new List<Type>();
		List<string> exIncludes = new List<string>();
		foreach(AppAction action in actions)
		{
			Type type = action.GetType();
			if(types.IndexOf(type) < 0)
			{
				types.Add(type);
				string[] includes = action.SketchExternalIncludes();
				if(includes != null)
				{
					foreach(string include in includes)
					{
						if(exIncludes.IndexOf(include) < 0)
							exIncludes.Add(include);
					}
				}
			}
		}

		StringBuilder source = new StringBuilder();

		foreach(string include in exIncludes)
			source.AppendLine(include);
		source.AppendLine("#include \"UnityApp.h\"");
		foreach(Type type in types)
			source.AppendLine(string.Format("#include \"{0}.h\"", type.Name));
		source.AppendLine();

		foreach(AppAction action in actions)
			source.AppendLine(action.SketchDeclaration());
		source.AppendLine();

		source.AppendLine("void setup()");
		source.AppendLine("{");
		foreach(AppAction action in actions)
			source.AppendLine(string.Format("  UnityApp.attachAction((AppAction*)&{0});", action.name));
		source.AppendLine("  UnityApp.begin(115200);");
		source.AppendLine("}");
		source.AppendLine();

		source.AppendLine("void loop()");
		source.AppendLine("{");
		source.AppendLine("  UnityApp.process();");
		source.AppendLine("}");

		string path = Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file));
		Directory.CreateDirectory(path);
		StreamWriter sw = new StreamWriter(Path.Combine(path, Path.GetFileName(file)));
		sw.Write(source.ToString());
		sw.Close();

		string srcPath = "Assets/SmartMaker/Arduino";
		CopyLibrary("UnityApp", srcPath, path);
		CopyLibrary("AppAction", srcPath, path);
		foreach(Type type in types)
			CopyLibrary(type.Name, srcPath, path);
	}

	private void CopyLibrary(string name, string srcPath, string destPath)
	{
		File.Copy(Path.Combine(srcPath, name + ".h"), Path.Combine(destPath, name + ".h"), true);
		File.Copy(Path.Combine(srcPath, name + ".cpp"), Path.Combine(destPath, name + ".cpp"), true);
	}
}
