using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.Events;


namespace SmartMaker
{
    [AddComponentMenu("SmartMaker/Unity3D/UI/UiListView")]
    public class UiListView : UIBehaviour
    {
        public RectTransform itemRoot;

        public UnityEvent OnChangedSelection;

        private int _itemNum = 0;
        private UiListItem _selectedItem;

        public int itemCount
        {
            get
            {
                if (itemRoot == null)
                    return 0;
                else
                    return itemRoot.transform.childCount;
            }
        }

        public UiListItem[] items
        {
            get
            {
                List<UiListItem> list = new List<UiListItem>();
                foreach (Transform item in itemRoot.transform)
                    list.Add(item.GetComponent<UiListItem>());

                return list.ToArray();
            }
        }

        public UiListItem selectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                bool changed = false;
                if (_selectedItem != null)
                {
                    if (_selectedItem.Equals(value) == false)
                        changed = true;
                }
                else
                {
                    if (value != null)
                        changed = true;
                }

                if (_selectedItem != null)
                    _selectedItem.selected = false;

                _selectedItem = value;
                if (_selectedItem != null)
                    _selectedItem.selected = true;

                if (changed == true)
                    OnChangedSelection.Invoke();
            }
        }

        public int selectedIndex
        {
            get
            {
                if (_selectedItem == null)
                    return -1;

                return _selectedItem.index;
            }
            set
            {
                if (value < 0 || value >= itemCount)
                    return;

                selectedItem = itemRoot.transform.GetChild(value).GetComponent<UiListItem>();
            }
        }

        public void ClearItem()
        {
            if (_itemNum == 0)
                return;

            if(_selectedItem != null)
            {
                _selectedItem = null;
                OnChangedSelection.Invoke();
            }

            List<GameObject> list = new List<GameObject>();
            foreach (Transform item in itemRoot.transform)
                list.Add(item.gameObject);

            for (int i = 0; i < list.Count; i++)
                GameObject.DestroyImmediate(list[i]);

            _itemNum = 0;            
        }

        public void AddItem(UiListItem item)
        {
            if (item == null)
                return;

            item.transform.SetParent(itemRoot.transform);
            item.transform.localScale = Vector3.one;
            item.owner = this;
            _itemNum++;
        }

        public void InsertItem(UiListItem item)
        {
            if (_selectedItem == null || item == null)
                return;

            int index = _selectedItem.index;
            AddItem(item);
            item.transform.SetSiblingIndex(index);
        }

        public void RemoveItem()
        {
            if (_selectedItem == null)
                return;

            GameObject.DestroyImmediate(_selectedItem.gameObject);
            _itemNum--;
            _selectedItem = null;
            OnChangedSelection.Invoke();
        }
    }
}
