using UnityEngine;
using System.Collections;
using SmartMaker;


namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("SmartMaker")]
    [Tooltip("UiListView.SelectedData()")]
    public class UiListViewSelectedData : FsmStateAction
    {
        [RequiredField]
        public UiListView listView;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        public FsmObject storedValue;

        public override void Reset()
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();

            UiListItem item = listView.selectedItem;
            if (item != null)
                storedValue.Value = (UnityEngine.Object)item.data;
            else
                storedValue.Value = null;

            Finish();
        }
    }
}