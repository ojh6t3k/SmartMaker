using UnityEngine;
using System.Collections;
using UnityEditor;
using SmartMaker;


[CustomEditor(typeof(GenericServo))]
public class GenericServoInspector : Editor
{
	bool foldout = true;
	SerializedProperty id;
	SerializedProperty pin;
	SerializedProperty offsetAngle;
	
	void OnEnable()
	{
		id = serializedObject.FindProperty("id");
		pin = serializedObject.FindProperty("pin");
		offsetAngle = serializedObject.FindProperty("offsetAngle");
	}
	
	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
		GenericServo servo = (GenericServo)target;
		
		foldout = EditorGUILayout.Foldout(foldout, "Sketch Options");
		if(foldout == true)
		{
			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField(id, new GUIContent("id"));
			EditorGUILayout.PropertyField(pin, new GUIContent("pin"));
			EditorGUI.indentLevel--;
		}

		EditorGUILayout.PropertyField(offsetAngle, new GUIContent("Offset Angle"));
		servo.Angle = (int)EditorGUILayout.Slider("Angle", servo.Angle, -90f, 90f);
		
		this.serializedObject.ApplyModifiedProperties();
	}
}
