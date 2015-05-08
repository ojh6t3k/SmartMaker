using UnityEngine;
using System.Collections;
using UnityEditor;
using HutongGames.PlayMaker;
using SmartMaker;
using SmartMaker.PlayMaker;


[CustomEditor(typeof(DigitalInputProxy))]
public class DigitalInputProxyInspector : Editor
{
	public override void OnInspectorGUI()
	{
		DigitalInputProxy proxy = (DigitalInputProxy)target;
		
		if(proxy.GetComponent<DigitalInput>() == null)
		{
			EditorGUILayout.HelpBox("There is no DigitalInput!", MessageType.Error);
		}
		else
		{
			proxy.eventOnStarted = EventField("OnStarted", proxy.eventOnStarted, proxy.builtInOnStarted);
			proxy.eventOnStopped = EventField("OnStopped", proxy.eventOnStopped, proxy.builtInOnStopped);
			proxy.eventOnChangedValue = EventField("OnChangedValue", proxy.eventOnChangedValue, proxy.builtInOnChangedValue);
		}
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
