using UnityEngine;
using System.Collections;
using UnityEditor;
using HutongGames.PlayMaker;
using SmartMaker;
using SmartMaker.PlayMaker;


[CustomEditor(typeof(ArduinoAppProxy))]
public class ArduinoAppProxyInspector : Editor
{
	public override void OnInspectorGUI()
	{
		ArduinoAppProxy proxy = (ArduinoAppProxy)target;

		if(proxy.GetComponent<ArduinoApp>() == null)
		{
			EditorGUILayout.HelpBox("There is no ArduinoApp!", MessageType.Error);
		}
		else
		{
			proxy.eventOnConnected = EventField("OnConnected", proxy.eventOnConnected, proxy.builtInOnConnected);
			proxy.eventOnConnectionFailed = EventField("OnConnectionFailed", proxy.eventOnConnectionFailed, proxy.builtInOnConnectionFailed);
			proxy.eventOnDisconnected = EventField("OnDisonnected", proxy.eventOnDisconnected, proxy.builtInOnDisconnected);
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
