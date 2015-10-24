using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SmartMaker
{
    [AddComponentMenu("SmartMaker/Unity3D/UI/UiListItem")]
    [RequireComponent(typeof(Toggle))]
    public class UiListItem : UIBehaviour, IPointerClickHandler
    {
        public UiListView owner;
        public Image image;
        public Text[] textList;
        public System.Object data;

        private Toggle _toggle;

        public int index
        {
            get
            {
                return this.transform.GetSiblingIndex();
            }
        }

        // This property is for ListView
        public bool selected
        {
            get
            {
                if (_toggle == null)
                    _toggle = GetComponent<Toggle>();

                return _toggle.isOn;
            }
            set
            {
                if (_toggle == null)
                    _toggle = GetComponent<Toggle>();

                _toggle.isOn = value;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            owner.selectedItem = this;
        }
    }
}
