using UnityEngine;
using System.Collections;
using SmartMaker;


namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("SmartMaker")]
    [Tooltip("UiListView.SelectedIndex()")]
    public class UiListViewSelectedIndex : FsmStateAction
    {
        [RequiredField]
        public UiListView listView;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        public FsmInt storedValue;

        public override void Reset()
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();

            storedValue.Value = listView.selectedIndex;

            Finish();
        }
    }
}