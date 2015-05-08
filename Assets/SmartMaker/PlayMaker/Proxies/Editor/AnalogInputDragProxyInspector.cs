using UnityEngine;
using System.Collections;
using UnityEditor;
using HutongGames.PlayMaker;
using SmartMaker;
using SmartMaker.PlayMaker;


[CustomEditor(typeof(AnalogInputDragProxy))]
public class AnalogInputDragProxyInspector : Editor
{
	SerializedProperty analogInputDrag;

	void OnEnable()
	{
		analogInputDrag = serializedObject.FindProperty("analogInputDrag");
	}

	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();

		AnalogInputDragProxy proxy = (AnalogInputDragProxy)target;

		EditorGUILayout.PropertyField(analogInputDrag, new GUIContent("analogInputDrag"));
		
		if(proxy.analogInputDrag == null)
		{
			EditorGUILayout.HelpBox("There is no AnalogInputDrag!", MessageType.Error);
		}
		else
		{
			proxy.eventOnDragStart = EventField("OnDragStart", proxy.eventOnDragStart, proxy.builtInOnDragStart);
			proxy.eventOnDragMove = EventField("OnDragMove", proxy.eventOnDragMove, proxy.builtInOnDragMove);
			proxy.eventOnDragEnd = EventField("OnDragEnd", proxy.eventOnDragEnd, proxy.builtInOnDragEnd);
		}

		this.serializedObject.ApplyModifiedProperties();
	}
	
	private string EventField(string eventName, string customEvent, string builtInEvent)
	{
		EditorGUILayout.LabelField(string.Format("Event: {0}", eventName));
		EditorGUI.indentLevel++;
		GUILayout.BeginHorizontal();
		customEvent = EditorGUILayout.TextField(customEvent);
		if(customEvent.Equals(builtInEvent) == false)
		{
			if(GUILayout.Button("Reset", GUILayout.Width(50f)) == true)
			{
				customEvent = builtInEvent;
				EditorUtility.SetDirty(target);
			}
		}
		GUILayout.EndHorizontal();
		EditorGUI.indentLevel--;
		
		return customEvent;
	}
}
