using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.IO.Ports;

namespace SmartMaker
{
	[AddComponentMenu("SmartMaker/Communication/CommSerial")]
	public class CommSerial : CommObject
	{
		[SerializeField]
		public List<string> portNames = new List<string>();
		public string portName;

		private SerialPort _serialPort;

		void Awake()
		{
			_serialPort = new SerialPort();
			_serialPort.DtrEnable = true; // win32 hack to try to get DataReceived event to fire
			_serialPort.RtsEnable = true;
			_serialPort.DataBits = 8;
			_serialPort.BaudRate = 115200;
			_serialPort.Parity = Parity.None;
			_serialPort.StopBits = StopBits.One;
			_serialPort.ReadTimeout = 1; // since on windows we *cannot* have a separate read thread
			_serialPort.WriteTimeout = 1000;
		}

		public void PortSearch()
		{
			portNames.Clear();
			portNames.AddRange(SerialPort.GetPortNames());
		}

		public override void Open()
		{
			_serialPort.PortName = "//./" + portName;
			_serialPort.Open();

			if(_serialPort.IsOpen == true)
			{
				if(OnOpened != null)
					OnOpened(this, null);
			}
		}

		public override void Close()
		{
			try
			{
				_serialPort.Close();
			}
			catch(Exception)
			{
			}
		}

		public override void Write(byte[] bytes)
		{
			if(bytes == null)
				return;
			if(bytes.Length == 0)
				return;

			try
			{
				_serialPort.Write(bytes, 0, bytes.Length);
			}
			catch(Exception)
			{
				if(OnErrorClosed != null)
					OnErrorClosed(this, null);
			}
		}

		public override byte[] Read()
		{
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
		}

		public override bool IsOpen
		{
			get
			{
				if(_serialPort == null)
					return false;

				return _serialPort.IsOpen;
			}
		}
	}
}
