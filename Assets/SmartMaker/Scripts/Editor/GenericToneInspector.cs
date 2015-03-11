using UnityEngine;
using System.Collections;
using UnityEditor;
using SmartMaker;


[CustomEditor(typeof(GenericTone))]
public class GenericToneInspector : Editor
{
	bool foldout = true;
	ToneFrequency toneFrequency;
	int duration;

	SerializedProperty id;
	SerializedProperty pin;
	SerializedProperty playByAnimation;
	SerializedProperty OnCompletedPlay;
	
	void OnEnable()
	{
		id = serializedObject.FindProperty("id");
		pin = serializedObject.FindProperty("pin");
		playByAnimation = serializedObject.FindProperty("playByAnimation");
		OnCompletedPlay = serializedObject.FindProperty("OnCompletedPlay");
	}
	
	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
		GenericTone tone = (GenericTone)target;
		
		foldout = EditorGUILayout.Foldout(foldout, "Sketch Options");
		if(foldout == true)
		{
			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField(id, new GUIContent("id"));
			EditorGUILayout.PropertyField(pin, new GUIContent("pin"));
			EditorGUI.indentLevel--;
		}
		EditorGUILayout.PropertyField(playByAnimation, new GUIContent("Play by Animation"));

		toneFrequency = (ToneFrequency)EditorGUILayout.EnumPopup("Tone Frequency", toneFrequency);
		duration = EditorGUILayout.IntField("Duration(msec)", duration);
		if(GUILayout.Button("Play") == true)
			tone.Play(toneFrequency, duration);

		if(tone.remainTime > 0)
		{
			EditorGUILayout.IntField("Remain Time", tone.remainTime);
			EditorUtility.SetDirty(target);
		}
		
		EditorGUILayout.Separator();
		EditorGUILayout.PropertyField(OnCompletedPlay);
		
		this.serializedObject.ApplyModifiedProperties();
	}
}
