using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using HutongGames.PlayMaker;

namespace SmartMaker.PlayMaker
{
    [AddComponentMenu("SmartMaker/PlayMaker/HostAppProxy")]
    public class HostAppProxy : MonoBehaviour
    {
        public readonly string builtInOnConnected = "HOST APP / ON CONNECTED";
        public readonly string builtInOnConnectionFailed = "HOST APP / ON CONNECTION FAILED";
        public readonly string builtInOnDisconnected = "HOST APP / ON DISCONNECTED";
        public readonly string builtInOnLostConnection = "HOST APP / ON LOST CONNECTION";

        public string eventOnConnected = "HOST APP / ON CONNECTED";
        public string eventOnConnectionFailed = "HOST APP / ON CONNECTION FAILED";
        public string eventOnDisconnected = "HOST APP / ON DISCONNECTED";
        public string eventOnLostConnection = "HOST APP / ON LOST CONNECTION";

        private PlayMakerFSM _fsm;
        private HostApp _hostApp;
        private FsmEventTarget _fsmEventTarget;

        void Awake()
        {
            _fsm = FindObjectOfType<PlayMakerFSM>();
            if (_fsm == null)
                _fsm = gameObject.AddComponent<PlayMakerFSM>();

            _hostApp = GetComponent<HostApp>();
            if (_hostApp != null)
            {
                _hostApp.OnConnected.AddListener(OnConnected);
                _hostApp.OnConnectionFailed.AddListener(OnConnectionFailed);
                _hostApp.OnDisconnected.AddListener(OnDisconnected);
                _hostApp.OnLostConnection.AddListener(OnLostConnection);
            }

            _fsmEventTarget = new FsmEventTarget();
            _fsmEventTarget.target = FsmEventTarget.EventTarget.BroadcastAll;
            _fsmEventTarget.excludeSelf = false;
        }

        void Start()
        {

        }	
        	
		// Update is called once per frame
		void Update ()
		{
		
		}

		private void OnConnected()
		{
			_fsm.Fsm.Event(_fsmEventTarget, eventOnConnected);
		}

		private void OnConnectionFailed()
		{
			_fsm.Fsm.Event(_fsmEventTarget, eventOnConnectionFailed);
		}

		private void OnDisconnected()
		{
			_fsm.Fsm.Event(_fsmEventTarget, eventOnDisconnected);
		}

        private void OnLostConnection()
        {
            _fsm.Fsm.Event(_fsmEventTarget, eventOnLostConnection);
        }
    }
}
