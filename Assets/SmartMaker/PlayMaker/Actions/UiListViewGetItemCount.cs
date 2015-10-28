using UnityEngine;
using System.Collections;
using SmartMaker;


namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("SmartMaker")]
    [Tooltip("UiListView.GetItemCount()")]
    public class UiListViewGetItemCount : FsmStateAction
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

            storedValue.Value = listView.itemCount;

            Finish();
        }
    }
}