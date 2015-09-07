using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

namespace SmartMaker
{
    [AddComponentMenu("SmartMaker/Arduino/Internal")]
    public class HostApp : MonoBehaviour
    {
        public float timeoutSec = 5f;

        public UnityEvent OnConnected;
        public UnityEvent OnConnectionFailed;
        public UnityEvent OnDisconnected;

        private CommObject _commObject;
        private bool _opened = false;
        private bool _connected = false;
        private float _timeout = 0;
        private float _fpsPreTime;
        private float _fpsDeltaTime;

        protected virtual void OnAwake() {}
        protected virtual void OnStart() {}
        protected virtual void OnUpdate() {}
        protected virtual void OnCommOpen() {}
        protected virtual void OnCommOpenFailed() {}
        protected virtual void OnCommClose() {}
        protected virtual void OnConnect() {}
        protected virtual void OnDisconnect() {}
        protected virtual void OnErrorDisconnect() {}

        void Awake()
        {
            if(commObject != null)
            {
                _commObject.OnOpened += CommOpenEventHandler;
                _commObject.OnOpenFailed += CommOpenFailEventHandler;
                _commObject.OnErrorClosed += CommErrorCloseEventHandler;
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
            if(_opened == true)
            {
                OnUpdate();

                // Check timeout
                if(_timeout > timeoutSec) // wait until timeout seconds
                    ErrorDisconnect();
                else
                    _timeout += Time.deltaTime;
            }
    	}

        public CommObject commObject
        {
            get
            {
                if( _commObject != null)
                    return _commObject;
                
                List<CommObject> listComms = new List<CommObject>(GameObject.FindObjectsOfType<CommObject>());
                for(int i=0; i<listComms.Count; i++)
                {
                    if(listComms[i].owner == null)
                    {
                        listComms.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        if(listComms[i].enabled == false || listComms[i].owner.Equals(this) == false)
                        {
                            listComms.RemoveAt(i);
                            i--;
                        }
                    }
                }
                
                if(listComms.Count > 0)
                {
                    _commObject = listComms[0];
                    return _commObject;
                }
                else
                    return null;
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
            if(_commObject == null)
                return;
            
            _commObject.Open();

            OnConnect();
        }

        public void Disconnect()
        {
            if(_commObject == null)
                return;

            _connected = false;
            OnDisconnect();

            _opened = false;
            _commObject.Close();

            OnDisconnected.Invoke();
        }

        private void ErrorDisconnect()
        {
            bool state = _opened;

            _connected = false;
            OnErrorDisconnect();

            _commObject.Close();
            _opened = false;
             
            if(state == false)
            {
                Debug.Log("Failed to open CommObject!");
                OnConnectionFailed.Invoke();
            }
            else
            {
                Debug.Log("Lost connection!");
                OnDisconnected.Invoke();
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

        private void CommOpenEventHandler(object sender, EventArgs e)
        {
            _opened = true;
            TimeoutReset();

            OnCommOpen();
        }
        
        private void CommOpenFailEventHandler(object sender, EventArgs e)
        {
            ErrorDisconnect();
            OnCommOpenFailed();
        }
        
        private void CommErrorCloseEventHandler(object sender, EventArgs e)
        {
            ErrorDisconnect();
            OnCommClose();
        }
    }
}
