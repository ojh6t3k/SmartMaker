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
        public UiCommDevice uiCommDevice;
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
            quit.onClick.AddListener(OnQuitClick);
            connect.onClick.AddListener(OnConnectClick);
            disconnect.onClick.AddListener(OnDisconnectClick);
            uiCommDevice.OnCommDeviceOK.AddListener(OnCommDeviceOKClick);
            uiCommDevice.OnCommDeviceCancel.AddListener(OnCommDeviceCancelClick);
            connectionFailedOK.onClick.AddListener(OnConnectionFailedOKClick);
            lostConnectionOK.onClick.AddListener(OnLostConnectionOKClick);
        }

        // Use this for initialization
        void Start()
        {
            start.gameObject.SetActive(true);
            run.gameObject.SetActive(false);            
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

        private void OnQuitClick()
        {
            hostApp.Disconnect();
            Application.Quit();
        }

        private void OnConnectClick()
        {
            uiCommDevice.ShowUI();            
        }

        private void OnDisconnectClick()
        {
            hostApp.Disconnect();
        }

        private void OnCommDeviceOKClick()
        {
            message.gameObject.SetActive(true);
            connecting.gameObject.SetActive(true);
            hostApp.Connect();
        }

        private void OnCommDeviceCancelClick()
        {            
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
