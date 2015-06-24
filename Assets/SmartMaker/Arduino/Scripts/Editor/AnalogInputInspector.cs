using UnityEngine;
using System.Collections;
using UnityEditor;
using SmartMaker;


[CustomEditor(typeof(AnalogInput))]
public class AnalogInputInspector : Editor
{
	bool foldout = true;

	SerializedProperty owner;
	SerializedProperty id;
	SerializedProperty pin;
	SerializedProperty resolution;
	SerializedProperty OnStarted;
	SerializedProperty OnExcuted;
	SerializedProperty OnStopped;
	
	void OnEnable()
	{
		owner = serializedObject.FindProperty("owner");
		id = serializedObject.FindProperty("id");
		pin = serializedObject.FindProperty("pin");
		resolution = serializedObject.FindProperty("resolution");
		OnStarted = serializedObject.FindProperty("OnStarted");
		OnExcuted = serializedObject.FindProperty("OnExcuted");
		OnStopped = serializedObject.FindProperty("OnStopped");
	}
	
	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
		AnalogInput aInput = (AnalogInput)target;

		EditorGUILayout.PropertyField(owner, new GUIContent("Owner"));
		
		foldout = EditorGUILayout.Foldout(foldout, "Sketch Options");
		if(foldout == true)
		{
			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField(id, new GUIContent("id"));
			EditorGUILayout.PropertyField(pin, new GUIContent("pin (A_)"));
			EditorGUI.indentLevel--;
		}
		aInput.autoUpdate = EditorGUILayout.Toggle("Auto update", aInput.autoUpdate);

		EditorGUILayout.PropertyField(resolution, new GUIContent("Resolution"));
		EditorGUILayout.FloatField("Analog Value", aInput.Value);

		EditorGUILayout.Separator();
		EditorGUILayout.PropertyField(OnStarted);
		EditorGUILayout.PropertyField(OnExcuted);
		EditorGUILayout.PropertyField(OnStopped);

		if(Application.isPlaying == true && aInput.autoUpdate == true)
			EditorUtility.SetDirty(target);
		
		this.serializedObject.ApplyModifiedProperties();
	}
}
