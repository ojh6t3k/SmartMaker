using UnityEngine;
using System.Collections;
using SmartMaker;


namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("SmartMaker")]
    [Tooltip("UiListView.Remove()")]
    public class UiListViewRemove : FsmStateAction
    {
        [RequiredField]
        public UiListView listView;

        public override void Reset()
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();

            listView.RemoveItem();

            Finish();
        }
    }
}