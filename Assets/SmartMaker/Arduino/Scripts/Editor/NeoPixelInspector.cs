using UnityEngine;
using System.Collections;
using UnityEditor;
using SmartMaker;


[CustomEditor(typeof(NeoPixel))]
public class NeoPixelInspector : Editor
{
	bool foldout = true;
	
	SerializedProperty owner;
	SerializedProperty id;
	SerializedProperty pin;
	SerializedProperty num;
	SerializedProperty config;
	SerializedProperty OnStarted;
	SerializedProperty OnStopped;

	private int _index;
	private Color _color;
	
	void OnEnable()
	{
		owner = serializedObject.FindProperty("owner");
		id = serializedObject.FindProperty("id");
		pin = serializedObject.FindProperty("pin");
		num = serializedObject.FindProperty("num");
		config = serializedObject.FindProperty("config");
		OnStarted = serializedObject.FindProperty("OnStarted");
		OnStopped = serializedObject.FindProperty("OnStopped");
	}
	
	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
		NeoPixel neoPixel = (NeoPixel)target;
		
		EditorGUILayout.PropertyField(owner, new GUIContent("Owner"));
		
		foldout = EditorGUILayout.Foldout(foldout, "Sketch Options");
		if(foldout == true)
		{
			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField(id, new GUIContent("id"));
			EditorGUILayout.PropertyField(pin, new GUIContent("pin"));
			EditorGUILayout.PropertyField(num, new GUIContent("num"));
			EditorGUILayout.PropertyField(config, new GUIContent("config"));
			EditorGUI.indentLevel--;
		}

		if(Application.isPlaying == true)
		{
			EditorGUILayout.Separator();
			float oldBrightness = neoPixel.Brightness;
			float newBrightness = EditorGUILayout.Slider("Brightness", oldBrightness, 0f, 1f);
			if(newBrightness != oldBrightness)
				neoPixel.Brightness = newBrightness;

			_index = EditorGUILayout.IntField("Index", _index);
			_color = EditorGUILayout.ColorField("Color", _color);
			if(GUILayout.Button("Set Pixel") == true)
			{
				neoPixel.SetPixel(_index, _color);
			}
		}
		
		EditorGUILayout.Separator();
		EditorGUILayout.PropertyField(OnStarted);
		EditorGUILayout.PropertyField(OnStopped);
		
		this.serializedObject.ApplyModifiedProperties();
	}
}
