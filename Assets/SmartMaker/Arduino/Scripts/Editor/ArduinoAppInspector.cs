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
    bool foldout = true;

    SerializedProperty commObject;
    SerializedProperty streamClass;
    SerializedProperty baudrate;
    SerializedProperty timeoutSec;
	SerializedProperty OnConnected;
	SerializedProperty OnConnectionFailed;
	SerializedProperty OnDisconnected;
    SerializedProperty OnLostConnection;

    void OnEnable()
	{
        commObject = serializedObject.FindProperty("commObject");
        streamClass = serializedObject.FindProperty("streamClass");
        baudrate = serializedObject.FindProperty("baudrate");
        timeoutSec = serializedObject.FindProperty("timeoutSec");
		OnConnected = serializedObject.FindProperty("OnConnected");
		OnConnectionFailed = serializedObject.FindProperty("OnConnectionFailed");
		OnDisconnected = serializedObject.FindProperty("OnDisconnected");
        OnLostConnection = serializedObject.FindProperty("OnLostConnection");
    }

	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();

		ArduinoApp arduino = (ArduinoApp)target;

        EditorGUILayout.PropertyField(commObject, new GUIContent("CommObject"));

        if (Application.isPlaying == false)
		{
			EditorGUILayout.HelpBox("To connect the board is only possible in Play mode.", MessageType.Info);
			if(GUILayout.Button("Create Sketch") == true)
				CreateSketch(EditorUtility.SaveFilePanel("Create Sketch", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "", "ino"));
            foldout = EditorGUILayout.Foldout(foldout, "Sketch Options");
            if (foldout == true)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(streamClass, new GUIContent("Stream Class"));
                if(arduino.streamClass != ArduinoApp.StreamClass.Bridge)
                    EditorGUILayout.PropertyField(baudrate, new GUIContent("Baudrate"));
                EditorGUI.indentLevel--;
            }
        }
		else
		{
			if(arduino.commObject != null)
			{
				if(arduino.connected == true)
				{
					if(GUILayout.Button("Disconnect") == true)
						arduino.Disconnect();
				}
				else
				{
					if(GUILayout.Button("Connect") == true)
						arduino.Connect();
				}
				EditorGUILayout.LabelField(string.Format("FPS: {0:f1}", arduino.fps));
				
				EditorUtility.SetDirty(target);
			}
			else
			{
				EditorGUILayout.HelpBox("CommObject is Null!", MessageType.Error);
			}
		}

		EditorGUILayout.PropertyField(timeoutSec, new GUIContent("Timeout(sec)"));

		EditorGUILayout.Separator();
		EditorGUILayout.PropertyField(OnConnected);
		EditorGUILayout.PropertyField(OnConnectionFailed);
		EditorGUILayout.PropertyField(OnDisconnected);
        EditorGUILayout.PropertyField(OnLostConnection);

        this.serializedObject.ApplyModifiedProperties();
	}

	private void CreateSketch(string file)
	{
		if(file == null)
			return;

		if(file.Length == 0)
			return;

		ArduinoApp arduino = (ArduinoApp)target;
		AppAction[] actions = arduino.appActions;
		StringBuilder source = new StringBuilder();

		// Check id duplications
		for(int i=0; i<actions.Length; i++)
		{
			for(int j=i+1; j<actions.Length; j++)
			{
				if(actions[i].id == actions[j].id)
				{
					Debug.LogError("AppAction ID is duplicated!");
					return;
				}
			}
		}

		// #Includes
		List<Type> types = new List<Type>();
		List<string> exIncludes = new List<string>();
		string[] includes;
        if (arduino.streamClass == ArduinoApp.StreamClass.Bridge)
        {
            exIncludes.Add("#include <Bridge.h>");
            exIncludes.Add("#include <YunServer.h>");
            exIncludes.Add("#include <YunClient.h>");
        }
        foreach (AppAction action in actions)
		{
			Type type = action.GetType();
			if(types.IndexOf(type) < 0)
			{
				types.Add(type);
				includes = action.SketchIncludes();
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
		foreach(string include in exIncludes)
			source.AppendLine(include);
		source.AppendLine("#include \"UnityApp.h\"");
		foreach(Type type in types)
			source.AppendLine(string.Format("#include \"{0}.h\"", type.Name));
		source.AppendLine();

		// Declarations
        if (arduino.streamClass == ArduinoApp.StreamClass.Bridge)
            source.AppendLine("YunServer server;");
        foreach (AppAction action in actions)
			source.AppendLine(action.SketchDeclaration());
		source.AppendLine();

		// void Setup()
		source.AppendLine("void setup()");
		source.AppendLine("{");        
        foreach (AppAction action in actions)
			source.AppendLine(string.Format("  UnityApp.attachAction((AppAction*)&{0});", action.SketchVarName));
        if (arduino.streamClass == ArduinoApp.StreamClass.Bridge)
        {
            source.AppendLine("  Bridge.begin();");
            source.AppendLine("  server.noListenOnLocalhost();");
            source.AppendLine("  server.begin();");
            source.AppendLine("  UnityApp.begin();");
        }
        else
        {
            if (arduino.streamClass == ArduinoApp.StreamClass.Serial)
                source.AppendLine(string.Format("  UnityApp.begin({0:d});", arduino.baudrate));
            else
            {
                string serialClass = arduino.streamClass.ToString();
                source.AppendLine(string.Format("  {0}.begin({1:d});", serialClass, arduino.baudrate));
                source.AppendLine(string.Format("  UnityApp.begin((Stream*)&{0});", serialClass));
            }
        }
        source.AppendLine("}");
		source.AppendLine();

		// void loop()
		source.AppendLine("void loop()");
		source.AppendLine("{");
        if (arduino.streamClass == ArduinoApp.StreamClass.Bridge)
        {
            source.AppendLine("  YunClient client = server.accept();");
            source.AppendLine("");
            source.AppendLine("  if(client)");
            source.AppendLine("  {");
            source.AppendLine("    while(client.connected())");
            source.AppendLine("      UnityApp.process((Stream*)&client);");
            source.AppendLine("    client.stop();");
            source.AppendLine("  }");
            source.AppendLine("  else");
            source.AppendLine("    UnityApp.process();");
        }
        else
            source.AppendLine("  UnityApp.process();");
        source.AppendLine("}");

		// Create source
		string path = Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file));
		Directory.CreateDirectory(path);
		StreamWriter sw = new StreamWriter(Path.Combine(path, Path.GetFileName(file)));
		sw.Write(source.ToString());
		sw.Close();

		string[] results = Directory.GetDirectories("Assets/", "SmartMaker", SearchOption.AllDirectories);
		if(results.Length > 0)
		{
			string srcPath = Path.Combine(results[0], "Arduino/Library");
			try
			{
				CopyLibrary("UnityApp", srcPath, path);
				CopyLibrary("AppAction", srcPath, path);
				foreach(Type type in types)
				{
					CopyLibrary(type.Name, srcPath, path);
					string subPath = Path.Combine(srcPath, type.Name);
					if(Directory.Exists(subPath) == true)
					{
						string[] subFiles = Directory.GetFiles(subPath);
						foreach(string subFile in subFiles)
						{
							if(Path.GetExtension(subFile).Equals(".h") == true
							   || Path.GetExtension(subFile).Equals(".cpp") == true
							   || Path.GetExtension(subFile).Equals(".c") == true)
							{
								File.Copy(subFile, Path.Combine(path, Path.GetFileName(subFile)), true);
							}
						}
					}
				}
			}
			catch(Exception e)
			{
				Debug.LogError(e);
			}
		}
		else
		{
			Debug.LogError(string.Format("Can not find path of Arduino Library!"));
		}
	}

	private void CopyLibrary(string name, string srcPath, string destPath)
	{
		File.Copy(Path.Combine(srcPath, name + ".h"), Path.Combine(destPath, name + ".h"), true);
		File.Copy(Path.Combine(srcPath, name + ".cpp"), Path.Combine(destPath, name + ".cpp"), true);
	}
}
