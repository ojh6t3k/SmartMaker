using UnityEngine;
using UnityEngine.Events;

namespace SmartMaker
{
    [AddComponentMenu("SmartMaker/Unity3D/Internal/HostApp")]
    public class HostApp : MonoBehaviour
    {
        public CommObject commObject;
        public bool useTimeout = true;
        public float timeoutSec = 5f;

        public UnityEvent OnConnected;
        public UnityEvent OnConnectionFailed;
        public UnityEvent OnDisconnected;
        public UnityEvent OnLostConnection;
        
        private bool _connected = false;
        private float _timeout = 0;
        private float _fpsPreTime;
        private float _fpsDeltaTime;

        protected virtual void OnAwake() {}
        protected virtual void OnStart() {}
        protected virtual void OnUpdate() {}
        protected virtual void OnConnect() {}
        protected virtual void OnDisconnect() {}
        protected virtual void OnErrorDisconnect() {}

        void Awake()
        {
            if(commObject != null)
            {
                commObject.OnOpen.AddListener(OnCommOpen);
                commObject.OnOpenFailed.AddListener(OnCommOpenFailed);
                commObject.OnErrorClosed.AddListener(OnCommErrorClose);
            }

            OnConnected.AddListener(OnConnectedEventHandler);

            OnAwake();
        }

    	// Use this for initialization
    	void Start()
        {
            OnStart();    	
    	}
    	
    	// Update is called once per frame
    	void Update()
        {
            if(commObject != null)
            {
                if (commObject.IsOpen)
                {
                    OnUpdate();

                    // Check timeout
                    if (useTimeout == true)
                    {
                        if (_timeout > timeoutSec) // wait until timeout seconds
                            ErrorDisconnect();
                        else
                            _timeout += Time.deltaTime;
                    }
                }
            }            
    	}

        public bool connected
        {
            get
            {
                return _connected;
            }
        }

        public float fps
        {
            get
            {
                if(_connected == true)
                    return 1f / _fpsDeltaTime;
                
                return 0f;
            }
        }

        public void Connect()
        {
            if(commObject == null)
                return;

            TimeoutReset();
            commObject.Open();

            OnConnect();
        }

        public void Disconnect()
        {
            if(commObject == null)
                return;

            _connected = false;
            OnDisconnect();
            commObject.Close();

            OnDisconnected.Invoke();
        }

        private void ErrorDisconnect()
        {
            bool state = _connected;

            _connected = false;
            OnErrorDisconnect();
            commObject.Close();
             
            if(state == false)
            {
                Debug.Log("Failed to open CommObject!");
                OnConnectionFailed.Invoke();
            }
            else
            {
                Debug.Log("Lost connection!");
                OnLostConnection.Invoke();
            }
        }

        protected void TimeoutReset()
        {
            _timeout = 0;
        }

        protected void fpsRefresh()
        {
            float fpsCurTime = Time.time;
            _fpsDeltaTime = fpsCurTime - _fpsPreTime;
            _fpsPreTime = fpsCurTime;
        }

        private void OnConnectedEventHandler()
        {
            _connected = true;
            _fpsPreTime = Time.time;
        }

        protected virtual void OnCommOpen()
        {
            TimeoutReset();
        }

        protected virtual void OnCommOpenFailed()
        {
            ErrorDisconnect();
        }

        protected virtual void OnCommErrorClose()
        {
            ErrorDisconnect();
        }
    }
}
