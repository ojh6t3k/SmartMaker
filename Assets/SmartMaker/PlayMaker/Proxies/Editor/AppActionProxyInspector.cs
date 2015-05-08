using UnityEngine;
using System.Collections;
using UnityEditor;
using HutongGames.PlayMaker;
using SmartMaker;
using SmartMaker.PlayMaker;


[CustomEditor(typeof(AppActionProxy))]
public class AppActionProxyInspector : Editor
{
	public override void OnInspectorGUI()
	{
		AppActionProxy proxy = (AppActionProxy)target;
		
		if(proxy.GetComponent<AppAction>() == null)
		{
			EditorGUILayout.HelpBox("There is no AppAction!", MessageType.Error);
		}
		else
		{
			proxy.eventOnStarted = EventField("OnStarted", proxy.eventOnStarted, proxy.builtInOnStarted);
			proxy.eventOnExcuted = EventField("OnExcuted", proxy.eventOnExcuted, proxy.builtInOnExcuted);
			proxy.eventOnStopped = EventField("OnStopped", proxy.eventOnStopped, proxy.builtInOnStopped);
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
