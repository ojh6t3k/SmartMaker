using UnityEngine;
using System.Collections.Generic;


namespace SmartMaker
{
    [AddComponentMenu("SmartMaker/Unity3D/UI/UiCommSerial")]
    public class UiCommSerial : UiCommDevice
    {
        public UiListView commDeviceListView;
        public UiListItem commDeviceListItem;


        protected override void OnInitialize()
        {
            commDeviceListView.OnChangedSelection.AddListener(OnChangedSelection);
        }

        protected override void OnShow()
        {
            commDeviceOK.interactable = false;
            commDeviceListView.ClearItem();
            commObject.StartSearch();
        }

        protected override void OnOK()
        {
            UiListItem item = commDeviceListView.selectedItem;
            if (item != null)
            {
                commObject.StopSearch();
                commObject.device = new CommDevice((CommDevice)item.data);
            }                
        }

        protected override void OnCancel()
        {
            commObject.StopSearch();
        }

        protected override void OnStartSearch()
        {
            
        }

        protected override void OnFoundDevice()
        {
            List<CommDevice> foundDevices = commObject.foundDevices;
            UiListItem[] items = commDeviceListView.items;
            List<CommDevice> addList = new List<CommDevice>();

            for (int i = 0; i < foundDevices.Count; i++)
            {
                bool add = true;
                for (int j = 0; j < items.Length; j++)
                {
                    if (foundDevices[i].Equals((CommDevice)items[j].data))
                    {
                        add = false;
                        break;
                    }
                }
                if (add)
                    addList.Add(foundDevices[i]);
            }

            CommDevice device = commObject.device;
            for (int i = 0; i < addList.Count; i++)
            {
                UiListItem item = GameObject.Instantiate(commDeviceListItem);
                item.textList[0].text = addList[i].name;
                item.data = addList[i];
                commDeviceListView.AddItem(item);
                if (device.Equals(addList[i]))
                    commDeviceListView.selectedItem = item;
            }
        }

        protected override void OnStopSearch()
        {
            
        }

        private void OnChangedSelection()
        {
            if(commDeviceListView.selectedItem == null)
                commDeviceOK.interactable = false;
            else
                commDeviceOK.interactable = true;
        }
    }
}
