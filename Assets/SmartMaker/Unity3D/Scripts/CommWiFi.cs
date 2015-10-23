using UnityEngine;
using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;


namespace SmartMaker
{
	[AddComponentMenu("SmartMaker/Unity3D/Communication/CommWiFi")]
	public class CommWiFi : CommObject
	{
		public string ipAddress = "192.168.240.1"; // Arduino Yun default IP
		public int port = 5555; // Arduino Yun Bridge Port

		public string errorMessage;

		private Socket _socket;
        private bool _threadOnOpenFailed = false;
        private Thread _openThread;

        void Awake()
		{
		}

        void Update()
        {
            if (_threadOnOpenFailed)
            {
                OnOpenFailed.Invoke();
                _threadOnOpenFailed = false;
            }
        }

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
            try
            {
                _socket.Shutdown(SocketShutdown.Both);
                _socket.Close();
            }
            catch (Exception)
            {
            }
        }

        public override void Write(byte[] bytes)
		{
			try
			{
				_socket.Send(bytes);
			}
			catch(Exception e)
			{
                Debug.Log(e);
                ErrorClose();
                OnErrorClosed.Invoke();
            }
		}
		
		public override byte[] Read()
		{
			List<byte> bytes = new List<byte>();

			try
			{
				if(_socket.Available > 0)
				{
					byte[] rcvData = new byte[256];
					int count = _socket.Receive(rcvData);
					for(int i=0; i<count; i++)
						bytes.Add(rcvData[i]);
				}
			}
			catch(Exception)
			{
                ErrorClose();
                OnErrorClosed.Invoke();
                return null;
            }

            if (bytes.Count == 0)
                return null;
            else
                return bytes.ToArray();
        }
		
		public override bool IsOpen
		{
			get
			{
				if(_socket == null)
					return false;
				
				return _socket.Connected;
			}
		}

		private void ConnectCompleted(object sender, SocketAsyncEventArgs e)
		{
			if(_socket.Connected == true)
                OnOpen.Invoke();
			else
			{
				errorMessage = e.SocketError.ToString();
                OnOpenFailed.Invoke();
            }
		}

        private void openThread()
        {
            try
            {
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _socket.NoDelay = true;
                _socket.ReceiveBufferSize = 4096;
                _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);

                SocketAsyncEventArgs e = new SocketAsyncEventArgs();
                e.RemoteEndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
                e.UserToken = _socket;
                e.Completed += new EventHandler<SocketAsyncEventArgs>(ConnectCompleted);
                _socket.ConnectAsync(e);
            }
            catch (Exception e)
            {
                Debug.Log(e);
                _threadOnOpenFailed = true;
            }

            _openThread.Abort();
            return;
        }
    }
}
