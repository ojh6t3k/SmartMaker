using UnityEngine;
using System.Collections;
using SmartMaker;


namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("SmartMaker")]
    [Tooltip("UiListView.Add()")]
    public class UiListViewAdd : FsmStateAction
    {
        [RequiredField]
        public UiListView listView;
        [RequiredField]
        public UiListItem listItem;

        public Sprite image;
        public FsmString[] text;
        public FsmObject data;

        public override void Reset()
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();

            UiListItem item = GameObject.Instantiate(listItem);
            if (item.image != null)
                item.image.sprite = image;
            for(int i=0; i<item.textList.Length; i++)
            {
                if (i < text.Length)
                    item.textList[i].text = text[i].Value;
            }
            item.data = data.Value;

            listView.AddItem(item);            

            Finish();
        }
    }
}