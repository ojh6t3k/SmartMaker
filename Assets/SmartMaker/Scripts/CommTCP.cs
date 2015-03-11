using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Sockets;


namespace SmartMaker
{
	[AddComponentMenu("SmartMaker/Communication/CommTCP")]
	public class CommTCP : CommObject
	{
		public string ipAddress;
		public int port;
		public bool localHost;

		private Socket _socket;

		void Awake()
		{
			_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			_socket.NoDelay = true;
			_socket.ReceiveBufferSize = 4096;
			_socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);
		}		

		public override void Open()
		{
			try
			{
				SocketAsyncEventArgs e = new SocketAsyncEventArgs();
				if(localHost == false)
					e.RemoteEndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
				else
					e.RemoteEndPoint = new IPEndPoint(Dns.GetHostEntry("localhost").AddressList[0], port);
				e.UserToken = _socket;			
				e.Completed += new EventHandler<SocketAsyncEventArgs>(ConnectCompleted);
				_socket.ConnectAsync(e);
			}
			catch(Exception e)
			{
				Debug.Log(e);
				if(OnOpenFailed != null)
					OnOpenFailed(this, null);
			}
		}
		
		public override void Close()
		{
			try
			{
				_socket.Shutdown(SocketShutdown.Both);
				_socket.Close();
			}
			catch(Exception)
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
				Debug.Log("send error");
				Debug.Log(e);
				if(OnErrorClosed != null)
					OnErrorClosed(this, null);
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
				if(OnErrorClosed != null)
					OnErrorClosed(this, null);
			}

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
			{
				if(OnOpened != null)
					OnOpened(this, null);
			}
			else
			{
				if(OnOpenFailed != null)
					OnOpenFailed(this, null);
			}
		}
	}
}
