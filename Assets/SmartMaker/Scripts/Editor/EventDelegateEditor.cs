using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Reflection;
using System.Collections.Generic;

namespace SmartMaker
{
	public static class EventDelegateEditor
	{
		static public void Field(string eventName, List<EventDelegate> list)
		{
			EditorGUILayout.Space();
			GUILayout.Label(eventName);

			for(int i=0; i<list.Count; i++)
			{
				GUILayout.BeginHorizontal();
				EditorGUILayout.ObjectField(list[i].target, typeof(MonoBehaviour), true);
				GUILayout.Label(list[i].methodName);
				GUILayout.EndHorizontal();
			}

			MonoBehaviour component = (MonoBehaviour)EditorGUILayout.ObjectField("Handler", null, typeof(MonoBehaviour), true);
			if(component != null)
				EventDelegate.Add(list, component);
		}
	}
}
