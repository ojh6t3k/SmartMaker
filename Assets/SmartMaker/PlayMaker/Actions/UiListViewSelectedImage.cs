using UnityEngine;
using System.Collections;
using SmartMaker;


namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("SmartMaker")]
    [Tooltip("UiListView.SelectedImage()")]
    public class UiListViewSelectedImage : FsmStateAction
    {
        [RequiredField]
        public UiListView listView;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        public FsmTexture storedValue;

        public override void Reset()
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();

            UiListItem item = listView.selectedItem;
            if (item != null)
                storedValue.Value = item.image.sprite.texture;
            else
                storedValue.Value = null;

            Finish();
        }
    }
}