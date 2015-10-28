using UnityEngine;
using System.Collections;
using SmartMaker;


namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("SmartMaker")]
    [Tooltip("UiListView.SelectedText()")]
    public class UiListViewSelectedText : FsmStateAction
    {
        [RequiredField]
        public UiListView listView;

        public FsmInt index;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        public FsmString storedValue;

        public override void Reset()
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();

            storedValue.Value = "";

            UiListItem item = listView.selectedItem;
            if (item != null)
            {
                if (index.Value >= 0 && index.Value < item.textList.Length)
                    storedValue.Value = item.textList[index.Value].text;
            }

            Finish();
        }
    }
}