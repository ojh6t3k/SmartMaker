using UnityEngine;
using System.Collections.Generic;
using System;
#if UNITY_STANDALONE
using System.Threading;
using System.IO;
using System.IO.Ports;
#endif


namespace SmartMaker
{
	[AddComponentMenu("SmartMaker/Unity3D/Communication/CommSerial")]
	public class CommSerial : CommObject
	{
        public int baudrate = 115200;

        private bool _threadOnOpen = false;
        private bool _threadOnOpenFailed = false;

#if UNITY_STANDALONE
        private SerialPort _serialPort;
        private Thread _openThread;
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

            platformSupport = true;
#endif
        }

        void Update()
        {
            if (_threadOnOpen)
            {
                OnOpen.Invoke();
                _threadOnOpen = false;
            }

            if (_threadOnOpenFailed)
            {
                OnOpenFailed.Invoke();
                _threadOnOpenFailed = false;
            }
        }

        #region Override
        public override void Open()
        {
            if (IsOpen)
                return;

            _openThread = new Thread(openThread);
            _openThread.Start();
        }

        public override void Close()
        {
            if (!IsOpen)
                return;

            ErrorClose();
            OnClose.Invoke();
        }

        protected override void ErrorClose()
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

        public override bool IsOpen
        {
            get
            {
#if UNITY_STANDALONE
                if (_serialPort == null)
                    return false;

                return _serialPort.IsOpen;
#else
                return false;
#endif
            }
        }

        public override void StartSearch()
        {
            foundDevices.Clear();
            OnStartSearch.Invoke();            

#if UNITY_STANDALONE
#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                CommDevice foundDevice = new CommDevice();
                foundDevice.name = port;
                foundDevice.address = "//./" + port;
                foundDevices.Add(foundDevice);
            }
            System.Management.ManagementObjectSearcher Searcher = new System.Management.ManagementObjectSearcher("Select * from WIN32_SerialPort");
            foreach (System.Management.ManagementObject Port in Searcher.Get())
            {
                foreach (System.Management.PropertyData Property in Port.Properties)
                {
                    Debug.Log(Property.Name + " " + (Property.Value == null ? null : Property.Value.ToString()));
                }
            }
#elif (UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX)
            string prefix = "/dev/";
            string[] ports = Directory.GetFiles(prefix, "*.*");
            foreach (string port in ports)
            {
                if (port.StartsWith("/dev/cu."))
                {
                    if (port.Contains("usb"))
                    {
                        CommDevice foundDevice = new CommDevice();
                        foundDevice.name = port.Substring(prefix.Length);
                        foundDevice.address = port;
                        foundDevices.Add(foundDevice);
                    }
                }
            }
#endif
#endif
            if (foundDevices.Count > 0)
                OnFoundDevice.Invoke();

            OnStopSearch.Invoke();
        }

        public override void Write(byte[] data)
        {
            if (data == null)
                return;
            if (data.Length == 0)
                return;

#if UNITY_STANDALONE
            try
            {
                _serialPort.Write(data, 0, data.Length);
            }
            catch (Exception)
            {
                ErrorClose();
                OnErrorClosed.Invoke();
            }
#endif
        }

        public override byte[] Read()
        {
#if UNITY_STANDALONE
            List<byte> bytes = new List<byte>();

            while (true)
            {
                try
                {
                    bytes.Add((byte)_serialPort.ReadByte());
                }
                catch (TimeoutException)
                {
                    break;
                }
                catch (Exception)
                {
                    ErrorClose();
                    OnErrorClosed.Invoke();
                    return null;
                }
            }

            if (bytes.Count == 0)
                return null;
            else
                return bytes.ToArray();
#else
            return null;
#endif
        }
        #endregion

        private void openThread()
        {
#if UNITY_STANDALONE
            try
            {
                _serialPort.PortName = device.address;
                _serialPort.BaudRate = baudrate;
                _serialPort.Open();
                _threadOnOpen = true;
            }
            catch (Exception)
            {
                _threadOnOpenFailed = true;
            }
#else
            _threadOnOpenFailed = true;
#endif
            _openThread.Abort();
            return;
        }
    }
}
