using UnityEngine;
using System.Collections;
using UnityEditor;
using HutongGames.PlayMaker;
using SmartMaker;
using SmartMaker.PlayMaker;


[CustomEditor(typeof(HostAppProxy))]
public class HostAppProxyInspector : Editor
{
	public override void OnInspectorGUI()
	{
        HostAppProxy proxy = (HostAppProxy)target;

		if(proxy.GetComponent<ArduinoApp>() == null)
		{
			EditorGUILayout.HelpBox("There is no HostApp!", MessageType.Error);
		}
		else
		{
			proxy.eventOnConnected = ProxyInspectorUtil.EventField(target, "OnConnected", proxy.eventOnConnected, proxy.builtInOnConnected);
			proxy.eventOnConnectionFailed = ProxyInspectorUtil.EventField(target, "OnConnectionFailed", proxy.eventOnConnectionFailed, proxy.builtInOnConnectionFailed);
			proxy.eventOnDisconnected = ProxyInspectorUtil.EventField(target, "OnDisonnected", proxy.eventOnDisconnected, proxy.builtInOnDisconnected);
            proxy.eventOnLostConnection = ProxyInspectorUtil.EventField(target, "OnLostConnection", proxy.eventOnLostConnection, proxy.builtInOnLostConnection);
        }
	}
}
