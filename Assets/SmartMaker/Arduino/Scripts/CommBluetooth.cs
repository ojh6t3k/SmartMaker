using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;
using UnityEngine.UI;
#if UNITY_STANDALONE_WIN
using System.IO.Ports;
#endif

namespace SmartMaker
{
	[AddComponentMenu("SmartMaker/Arduino/Communication/CommBluetooth")]
	public class CommBluetooth : CommObject
	{
        [SerializeField]
        public List<string> devNames = new List<string>();
        public string devName;
        public int baudrate = 57600;
        public string streamClass = "Serial";
        
        public Text uiText;
        public RectTransform uiPanel;
        public GameObject uiItem;

#if UNITY_ANDROID
		private AndroidJavaObject _activity;
		private AndroidJavaObject _activityContext;
#elif UNITY_STANDALONE

#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
        private SerialPort _serialPort;
#endif

#endif
		
		void Awake()
		{
#if UNITY_ANDROID
			using(AndroidJavaClass activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
			{
				_activityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
			}
			
			using(AndroidJavaClass pluginClass = new AndroidJavaClass("com.smartmaker.android.CommBluetooth"))
			{
				if(pluginClass != null)
				{
					_activity = pluginClass.CallStatic<AndroidJavaObject>("GetInstance");
					_activity.Call("SetContext", _activityContext);
				}
			}
#elif UNITY_STANDALONE

#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
            _serialPort = new SerialPort();
            _serialPort.DtrEnable = true; // win32 hack to try to get DataReceived event to fire
            _serialPort.RtsEnable = true;
            _serialPort.DataBits = 8;
            _serialPort.Parity = Parity.None;
            _serialPort.StopBits = StopBits.One;
            _serialPort.ReadTimeout = 1; // since on windows we *cannot* have a separate read thread
            _serialPort.WriteTimeout = 1000;
#endif

#endif

			if(uiText != null)
				uiText.text = devName;
		}
		
		public void DeviceSearch()
		{
            devNames.Clear();

#if UNITY_ANDROID
			if(_activity != null)
                devNames.AddRange(_activity.Call<string[]>("DeviceSearch"));
#elif UNITY_STANDALONE

#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
            devNames.AddRange(SerialPort.GetPortNames());
#endif		

#endif

			if(uiPanel != null && uiItem != null)
			{
				List<GameObject> items = new List<GameObject>();
				foreach(RectTransform rect in uiPanel)
				{
					if(rect.gameObject.Equals(uiItem) == false)
						items.Add(rect.gameObject);						
				}
				
				foreach(GameObject go in items)
					GameObject.DestroyImmediate(go);
				
				Text t = uiItem.GetComponent<Text>();
				if(t == null)
					t = uiItem.GetComponentInChildren<Text>();
				
				if(devNames.Count == 0)
				{
					if(t != null)
						t.text = "";
				}
				else
				{
					if(t != null)
						t.text = devNames[0];
					
					for(int i=1; i<devNames.Count; i++)
					{
						GameObject item = GameObject.Instantiate(uiItem);
						item.transform.SetParent(uiPanel.transform);
						t = item.GetComponent<Text>();
						if(t == null)
							t = item.GetComponentInChildren<Text>();
						if(t != null)
							t.text = devNames[i];
					}
				}
			}
		}
		
		public override void Open()
		{
#if UNITY_ANDROID
			if(_activity != null)
            {
    			if(_activity.Call<bool>("Open", devName) == true)
    			{
    				StartCoroutine(WaitOpenComplete());
    			}
    			else
    			{
    				if(OnOpenFailed != null)
    					OnOpenFailed(this, null);
    			}
            }
            else
            {
                if(OnOpenFailed != null)
                    OnOpenFailed(this, null);
            }
#elif UNITY_STANDALONE

#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
            _serialPort.PortName = "//./" + devName;
            
            try
            {
                _serialPort.BaudRate = baudrate;
                _serialPort.Open();
                if(_serialPort.IsOpen == true)
                {
                    if(OnOpened != null)
                        OnOpened(this, null);
                }
            }
            catch(Exception)
            {
                if(OnOpenFailed != null)
                    OnOpenFailed(this, null);
            }
#else
            if(OnOpenFailed != null)
                OnOpenFailed(this, null);
#endif

#else
            if(OnOpenFailed != null)
                OnOpenFailed(this, null);
#endif
		}

#if UNITY_ANDROID
		private IEnumerator WaitOpenComplete()
		{
			while(IsOpen == false)
			{
				if(_activity.Call<bool>("IsOpening") == true)
					yield return new WaitForSeconds(0.5f);
				else
					break;
			}

			if(IsOpen == true)
			{
				if(OnOpened != null)
					OnOpened(this, null);

				StartCoroutine(WatchClosed());
			}
			else
			{
				if(OnOpenFailed != null)
					OnOpenFailed(this, null);
			}
		}

		private IEnumerator WatchClosed()
		{
			while(IsOpen == true)
			{
				yield return new WaitForSeconds(1f);
			}

			if(OnErrorClosed != null)
				OnErrorClosed(this, null);
		}
#endif
		
		public override void Close()
		{
#if UNITY_ANDROID
			StopCoroutine(WatchClosed());

			if(_activity != null)
				_activity.Call("Close");
#elif UNITY_STANDALONE

#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
            try
            {
                _serialPort.Close();
            }
            catch(Exception)
            {
            }
#endif

#endif
		}
		
		public override void Write(byte[] bytes)
		{
            if(bytes == null)
                return;
            if(bytes.Length == 0)
                return;

#if UNITY_ANDROID
			if(_activity != null)
				_activity.Call("Write", bytes);
            else
            {
                if(OnErrorClosed != null)
                    OnErrorClosed(this, null);
            }
#elif UNITY_STANDALONE

#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
            try
            {
                _serialPort.Write(bytes, 0, bytes.Length);
            }
            catch(Exception)
            {
                if(OnErrorClosed != null)
                    OnErrorClosed(this, null);
            }
#else
            if(OnErrorClosed != null)
                OnErrorClosed(this, null);
#endif

#endif
		}
		
		public override byte[] Read()
		{
#if UNITY_ANDROID
			if(_activity != null)
				return _activity.Call<byte[]>("Read");
            else
                return null;
#elif UNITY_STANDALONE

#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
            List<byte> bytes = new List<byte>();
            
            while(true)
            {           
                try
                {
                    bytes.Add((byte)_serialPort.ReadByte());
                }
                catch(TimeoutException)
                {
                    break;
                }
                catch(Exception)
                {
                    if(OnErrorClosed != null)
                        OnErrorClosed(this, null);
                    return null;
                }
            }
            
            if(bytes.Count == 0)
                return null;
            else
                return bytes.ToArray();
#else
            return null;
#endif

#else
            return null;
#endif
		}
		
		public override bool IsOpen
		{
			get
			{
#if UNITY_ANDROID
				if(_activity != null)
					return _activity.Call<bool>("IsOpen");
                else
                    return false;
#elif UNITY_STANDALONE

#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
                if(_serialPort != null)
                    return _serialPort.IsOpen;
                else
                    return false;
#else
                return false;
#endif

#else
                return false;
#endif
			}
		}
		
		public void SelectDeviceName(Text text)
		{
			devName = text.text;
			if(uiText != null)
				uiText.text = devName;
		}

        public override string SketchSetup ()
        {
            StringBuilder source = new StringBuilder();
            
            if(streamClass.Equals("Serial") == true || streamClass.Equals("Serial0") == true)
            {
                source.AppendLine(string.Format("  UnityApp.begin({0:d});", baudrate));
            }
            else
            {
                source.AppendLine(string.Format("  {0}.begin({1:d});", streamClass, baudrate));
                source.AppendLine(string.Format("  UnityApp.begin((Stream*)&{0});", streamClass));
            }
            
            return source.ToString();
        }
        
        public override string SketchLoop ()
        {
            return "  UnityApp.process();";
        }
	}
}
