using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


namespace SmartMaker
{
    [AddComponentMenu("SmartMaker/Unity3D/UI/UiHostApp")]
    public class UiHostApp : MonoBehaviour
    {
        public HostApp hostApp;
        public Button quit;
        public RectTransform start;
        public Button connect;
        public RectTransform run;
        public Button disconnect;
        public Canvas popup;
        public RectTransform commDevice;
        public UiListView commDeviceListView;
        public UiListItem commDeviceListItem;
        public Button commDeviceOK;
        public Button commDeviceCancel;
        public Canvas message;
        public RectTransform connecting;
        public RectTransform connectionFailed;
        public Button connectionFailedOK;
        public RectTransform lostConnection;
        public Button lostConnectionOK;

        void Awake()
        {
            hostApp.OnConnected.AddListener(OnHostAppConnected);
            hostApp.OnConnectionFailed.AddListener(OnHostAppConnectionFailed);
            hostApp.OnDisconnected.AddListener(OnHostAppDisconnected);
            hostApp.OnLostConnection.AddListener(OnHostAppLostConnection);
            hostApp.commObject.OnFoundDevice.AddListener(OnCommObjectFoundDevice);
            quit.onClick.AddListener(OnQuitClick);
            connect.onClick.AddListener(OnConnectClick);
            disconnect.onClick.AddListener(OnDisconnectClick);
            commDeviceOK.onClick.AddListener(OnCommDeviceOKClick);
            commDeviceCancel.onClick.AddListener(OnCommDeviceCancelClick);
            connectionFailedOK.onClick.AddListener(OnConnectionFailedOKClick);
            lostConnectionOK.onClick.AddListener(OnLostConnectionOKClick);
        }

        // Use this for initialization
        void Start()
        {
            start.gameObject.SetActive(true);
            run.gameObject.SetActive(false);
            popup.gameObject.SetActive(false);
            commDevice.gameObject.SetActive(false);
            message.gameObject.SetActive(false);
            connecting.gameObject.SetActive(false);
            connectionFailed.gameObject.SetActive(false);
            lostConnection.gameObject.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnHostAppConnected()
        {
            start.gameObject.SetActive(false);
            run.gameObject.SetActive(true);
            message.gameObject.SetActive(false);
            connecting.gameObject.SetActive(false);
        }

        private void OnHostAppConnectionFailed()
        {
            message.gameObject.SetActive(true);
            connecting.gameObject.SetActive(false);
            connectionFailed.gameObject.SetActive(true);
        }

        private void OnHostAppDisconnected()
        {
            start.gameObject.SetActive(true);
            run.gameObject.SetActive(false);
        }

        private void OnHostAppLostConnection()
        {
            message.gameObject.SetActive(true);
            lostConnection.gameObject.SetActive(true);
        }

        private void OnCommObjectFoundDevice()
        {
            List<CommDevice> foundDevices = hostApp.commObject.foundDevices;            
            UiListItem[] items = commDeviceListView.items;
            List<CommDevice> addList = new List<CommDevice>();
            
            for(int i=0; i<foundDevices.Count; i++)
            {
                bool add = true;
                for(int j=0; j<items.Length; j++)
                {
                    if(foundDevices[i].Equals((CommDevice)items[j].data))
                    {
                        add = false;
                        break;
                    }
                }
                if (add)
                    addList.Add(foundDevices[i]);
            }

            CommDevice device = hostApp.commObject.device;
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

        private void OnQuitClick()
        {
            hostApp.Disconnect();
            Application.Quit();
        }

        private void OnConnectClick()
        {
            popup.gameObject.SetActive(true);
            commDevice.gameObject.SetActive(true);

            commDeviceListView.ClearItem();
            hostApp.commObject.StartSearch();
        }

        private void OnDisconnectClick()
        {
            hostApp.Disconnect();
        }

        private void OnCommDeviceOKClick()
        {
            UiListItem item = commDeviceListView.selectedItem;
            if(item != null)
            {
                popup.gameObject.SetActive(false);
                commDevice.gameObject.SetActive(false);
                message.gameObject.SetActive(true);
                connecting.gameObject.SetActive(true);

                hostApp.commObject.StopSearch();
                hostApp.commObject.device = new CommDevice((CommDevice)item.data);
                hostApp.Connect();
            }
        }

        private void OnCommDeviceCancelClick()
        {
            hostApp.commObject.StopSearch();
            popup.gameObject.SetActive(false);
            commDevice.gameObject.SetActive(false);
        }

        private void OnConnectionFailedOKClick()
        {
            message.gameObject.SetActive(false);
            connectionFailed.gameObject.SetActive(false);
        }

        private void OnLostConnectionOKClick()
        {
            message.gameObject.SetActive(false);
            lostConnection.gameObject.SetActive(false);
            start.gameObject.SetActive(true);
            run.gameObject.SetActive(false);
        }
    }
}
