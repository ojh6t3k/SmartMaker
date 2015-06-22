using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;


namespace SmartMaker
{
	[AddComponentMenu("SmartMaker/Communication/CommBridge")]
	public class CommBridge : CommObject
	{
		public string ipAddress = "192.168.240.1"; // Arduino Yun default IP
		public int port = 5555; // Arduino Yun Bridge Port

		private Socket _socket;

		void Awake()
		{
		}		

		public override void Open()
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
			catch(Exception e)
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

		public override string[] SketchIncludes ()
		{
			List<string> includes = new List<string>();
			includes.Add("#include <Bridge.h>");
			includes.Add("#include <YunServer.h>");
			includes.Add("#include <YunClient.h>");
			return includes.ToArray();
		}

		public override string SketchDeclaration ()
		{
			StringBuilder source = new StringBuilder();
			
			source.AppendLine("YunServer server;");

			return source.ToString();
		}

		public override string SketchSetup ()
		{
			StringBuilder source = new StringBuilder();

			source.AppendLine("  Bridge.begin();");
			source.AppendLine("  server.noListenOnLocalhost();");
			source.AppendLine("  server.begin();");
			source.AppendLine("  UnityApp.begin();");

			return source.ToString();
		}

		public override string SketchLoop ()
		{
			StringBuilder source = new StringBuilder();

			source.AppendLine("  YunClient client = server.accept();");
			source.AppendLine("");
			source.AppendLine("  if(client)");
			source.AppendLine("  {");
			source.AppendLine("    while(client.connected())");
			source.AppendLine("      UnityApp.process((Stream*)&client);");
			source.AppendLine("    client.stop();");
			source.AppendLine("  }");
			source.AppendLine("  else");
			source.AppendLine("    UnityApp.process();");

			return source.ToString();
		}
	}
}
