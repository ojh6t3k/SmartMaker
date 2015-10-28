using UnityEngine;
using System.Collections;
using UnityEditor;
using HutongGames.PlayMaker;
using SmartMaker;
using SmartMaker.PlayMaker;


[CustomEditor(typeof(UiListViewProxy))]
public class UiListViewProxyInspector : Editor
{
    public override void OnInspectorGUI()
    {
        UiListViewProxy proxy = (UiListViewProxy)target;

        if (proxy.GetComponent<UiListView>() == null)
        {
            EditorGUILayout.HelpBox("There is no UiListView!", MessageType.Error);
        }
        else
        {
            proxy.eventOnChangedSelection = ProxyInspectorUtil.EventField(target, "OnChangedSelection", proxy.eventOnChangedSelection, proxy.builtInOnChangedSelection);
        }
    }
}
