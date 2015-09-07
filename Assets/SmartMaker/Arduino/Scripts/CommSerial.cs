﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;
using UnityEngine.UI;
#if UNITY_STANDALONE
using System.IO.Ports;
#endif


namespace SmartMaker
{
	[AddComponentMenu("SmartMaker/Arduino/Communication/CommSerial")]
	public class CommSerial : CommObject
	{
		[SerializeField]
		public List<string> portNames = new List<string>();
		public string portName;
		public int baudrate = 115200;
		public string streamClass = "Serial";
		
		public Text uiText;
		public RectTransform uiPanel;
		public GameObject uiItem;

#if UNITY_STANDALONE
		private SerialPort _serialPort;
#endif

		void Awake()
		{
#if UNITY_STANDALONE
			_serialPort = new SerialPort();
			_serialPort.DtrEnable = true; // win32 hack to try to get DataReceived event to fire
			_serialPort.RtsEnable = true;
			_serialPort.DataBits = 8;
			_serialPort.Parity = Parity.None;
			_serialPort.StopBits = StopBits.One;
			_serialPort.ReadTimeout = 1; // since on windows we *cannot* have a separate read thread
			_serialPort.WriteTimeout = 1000;
#endif

			if(uiText != null)
				uiText.text = portName;
	    }

		public void PortSearch()
		{
			portNames.Clear();
#if UNITY_STANDALONE

#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
            portNames.AddRange(SerialPort.GetPortNames());
#elif (UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX)
            string prefix = "/dev/";
            string[] ports = Directory.GetFiles("/dev/", "*.*");
            foreach (string p in ports)
            {
                if(p.StartsWith ("/dev/cu.usb") == true)
                    portNames.Add(p.Substring(prefix.Length));
                //  else if(p.StartsWith ("/dev/tty.usb") == true)
                //      portNames.Add(p.Substring(prefix.Length));
            }
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

				if(portNames.Count == 0)
				{
					if(t != null)
						t.text = "";
				}
				else
				{
					if(t != null)
						t.text = portNames[0];

					for(int i=1; i<portNames.Count; i++)
					{
						GameObject item = GameObject.Instantiate(uiItem);
						item.transform.SetParent(uiPanel.transform);
						t = item.GetComponent<Text>();
						if(t == null)
							t = item.GetComponentInChildren<Text>();
						if(t != null)
							t.text = portNames[i];
					}
				}
			}
		}

		public override void Open()
		{
#if UNITY_STANDALONE

#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
            _serialPort.PortName = "//./" + portName;
#elif (UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX)
            _serialPort.PortName = "/dev/" + portName;
#else
            _serialPort.PortName = portName;
#endif

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
		}

		public override void Close()
		{
#if UNITY_STANDALONE
			try
			{
				_serialPort.Close();
			}
			catch(Exception)
			{
			}
#endif
		}

		public override void Write(byte[] bytes)
		{
			if(bytes == null)
				return;
			if(bytes.Length == 0)
				return;

#if UNITY_STANDALONE
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
		}

		public override byte[] Read()
		{
#if UNITY_STANDALONE
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
		}

		public override bool IsOpen
		{
			get
			{
#if UNITY_STANDALONE
				if(_serialPort == null)
					return false;

				return _serialPort.IsOpen;
#else
                return false;
#endif
			}
		}

		public void SelectPortName(Text text)
		{
			portName = text.text;
			if(uiText != null)
				uiText.text = portName;
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
