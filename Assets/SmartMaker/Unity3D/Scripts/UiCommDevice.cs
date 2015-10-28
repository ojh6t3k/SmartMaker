using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


namespace SmartMaker
{
    [AddComponentMenu("SmartMaker/Unity3D/Internal/UiCommDevice")]
    public class UiCommDevice : MonoBehaviour
    {
        public CommObject commObject;
        public Canvas popup;
        public RectTransform commDevice;
        public Button commDeviceOK;
        public Button commDeviceCancel;

        public UnityEvent OnCommDeviceOK;
        public UnityEvent OnCommDeviceCancel;

        protected virtual void OnInitialize() { }
        protected virtual void OnShow() { }
        protected virtual void OnOK() { }
        protected virtual void OnCancel() { }
        protected virtual void OnStartSearch() { }
        protected virtual void OnFoundDevice() { }
        protected virtual void OnStopSearch() { }

        void Awake()
        {
            commObject.OnStartSearch.AddListener(OnStartSearch);
            commObject.OnFoundDevice.AddListener(OnFoundDevice);
            commObject.OnStopSearch.AddListener(OnStopSearch);

            commDeviceOK.onClick.AddListener(OnClickOK);
            commDeviceCancel.onClick.AddListener(OnClickCancel);

            OnInitialize();
        }

        // Use this for initialization
        void Start()
        {
            popup.gameObject.SetActive(false);
            commDevice.gameObject.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {

        }
        
        public void ShowUI()
        {
            popup.gameObject.SetActive(true);
            commDevice.gameObject.SetActive(true);

            OnShow();
        }

        private void OnClickOK()
        {
            popup.gameObject.SetActive(false);
            commDevice.gameObject.SetActive(false);

            OnOK();
            OnCommDeviceOK.Invoke();
        }

        private void OnClickCancel()
        {
            popup.gameObject.SetActive(false);
            commDevice.gameObject.SetActive(false);

            OnCancel();
            OnCommDeviceCancel.Invoke();
        }
    }
}
