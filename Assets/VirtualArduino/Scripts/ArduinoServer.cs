using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Sockets;

namespace SmartMaker
{
	[AddComponentMenu("SmartMaker/ArduinoServer")]
	public class ArduinoServer : MonoBehaviour
	{
		public int port;

		private enum CMD
		{
			Start = 0x80, //128
			Exit = 0x81,  //129
			Update = 0x82, //130
			Action = 0x83, //131
			Ready = 0x84, //132
			Ping = 0x85 //133
		}

		private AppAction[] _actions;
		private Socket _socket;
		private Socket _client;
		private bool _readyReceived;
		private int _processUpdate;
		private byte _id;
		private byte _numData;
		private byte _currentNumData;
		private byte MAX_ARGUMENT_BYTES = 116;
		private byte[] _storedData;

		void Awake()
		{
			_storedData = new byte[MAX_ARGUMENT_BYTES + (MAX_ARGUMENT_BYTES / 8) + 1];
		}

		// Use this for initialization
		void Start ()
		{
			_actions = appActions;
			foreach(AppAction action in _actions)
				action.ActionSetup();

			_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			_socket.ReceiveBufferSize = 4096;

			try
			{
				_socket.Bind(new IPEndPoint(IPAddress.Any, port));
				_socket.Listen(1);
				SocketAsyncEventArgs e = new SocketAsyncEventArgs();
				e.Completed += new EventHandler<SocketAsyncEventArgs>(AcceptCompleted);
				_socket.AcceptAsync(e);
			}
			catch(Exception)
			{
				Debug.Log("Failed to init!");
			}
		}
		
		// Update is called once per frame
		void Update ()
		{
			if(_client != null)
			{
				byte[] rcvBytes = Read();
				for(int i=0; i<rcvBytes.Length; i++)
				{
					byte bit = 1;
					byte inputData = rcvBytes[i];
					if(inputData >= 0)
					{
						if((inputData & (byte)0x80) == 0x80)
						{
							if(inputData == (byte)CMD.Ping)
							{
								Write(new byte[] { (byte)CMD.Ping });
							}
							else if(inputData == (byte)CMD.Start)
							{
								foreach(AppAction action in _actions)
									action.ActionStart();

								Write(new byte[] { (byte)CMD.Ready });
							}
							else if(inputData == (byte)CMD.Exit)
							{
								foreach(AppAction action in _actions)
									action.ActionStop();
							}
							else if(inputData == (byte)CMD.Ready)
							{
								_readyReceived = true;
							}
							else if(inputData == (byte)CMD.Action)
							{
								if(_processUpdate > 0)
								{
									foreach(AppAction action in _actions)
										action.ActionExcute();

									Write(new byte[] { (byte)CMD.Ready });
									_processUpdate = 0;
								}
							}
							
							if(inputData == (byte)CMD.Update)
							{
								_processUpdate = 1;
							}
							else
								Reset();
						}
						else if(_processUpdate > 0)
						{
							if(_processUpdate == 1)
							{
								_id = inputData;
								_processUpdate = 2;
							}
							else if(_processUpdate == 2)
							{
								_numData = inputData;
								if(_numData > MAX_ARGUMENT_BYTES)
									Reset();
								else
								{
									_processUpdate = 3;
									_currentNumData = 0;
								}
							}
							else if(_processUpdate == 3)
							{
								if(_currentNumData < _numData)
									_storedData[_currentNumData++] = inputData;
								
								if(_currentNumData >= _numData)
								{
									// Decoding 7bit bytes
									_numData = 0;
									for(int j=0; j<_currentNumData; j++)
									{
										if(bit == 1)
										{
											_storedData[_numData] = (byte)(_storedData[j] << bit);
											bit++;
										}
										else if(bit == 8)
										{
											_storedData[_numData] |= _storedData[j];
											bit = 1;
										}
										else
										{
											_storedData[_numData++] |= (byte)(_storedData[j] >> (7 - bit + 1));
											_storedData[_numData] = (byte)(_storedData[j] << bit);
											bit++;
										}
									}
									
									_currentNumData = 0;

									foreach(AppAction action in _actions)
									{
										if(action.id == _id)
											action.dataBytes = _storedData;
									}
																		
									_processUpdate = 1;
								}
							}
						}
						else
							Reset();
					}
				}
				
				if(_readyReceived == true)
				{
					if(_actions.Length > 0)
					{
						List<byte> writeBytes = new List<byte>();
						foreach(AppAction action in _actions)
						{
							byte[] dataBytes = action.dataBytes;
							if(dataBytes != null)
							{
								writeBytes.Add((byte)(action.id & 0x7F));
								
								// Encoding 7bit bytes
								List<byte> data7bitBytes = new List<byte>();
								byte bit = 1;
								byte temp = 0;
								for(int i=0; i<dataBytes.Length; i++)
								{
									data7bitBytes.Add((byte)((temp | (dataBytes[i] >> bit)) & 0x7F));
									if(bit == 7)
									{
										data7bitBytes.Add((byte)(dataBytes[i] & 0x7F));
										bit = 1;
										temp = 0;
									}
									else
									{
										temp = (byte)(dataBytes[i] << (7 - bit));
										if(i == (dataBytes.Length - 1))
											data7bitBytes.Add((byte)(temp & 0x7F));
										bit++;
									}
								}
								
								writeBytes.Add((byte)data7bitBytes.Count); // num bytes
								writeBytes.AddRange(data7bitBytes.ToArray());
							}
						}
						
						if(writeBytes.Count > 0)
						{
							writeBytes.Insert(0, (byte)CMD.Update); // Update
							writeBytes.Add((byte)CMD.Action); // Action
							Write (writeBytes.ToArray());
						}
						else
							Write(new byte[] { (byte)CMD.Update, (byte)CMD.Action });
						
						_readyReceived = false;
					}
				}
			}
		}

		void OnDestroy()
		{
			_socket.Close();
		}

		private void Reset()
		{
			if(_processUpdate > 0)
				Write(new byte[] { (byte)CMD.Ready });
			
			_processUpdate = 0;
			_numData = 0;
			_currentNumData = 0;
		}

		public void Write(byte[] bytes)
		{
			try
			{
				_client.Send(bytes);
			}
			catch(Exception e)
			{
				Debug.Log(e);
				try
				{
					_client.Close();
				}
				catch(Exception e2)
				{
				}
				_client = null;
			}
		}
		
		public byte[] Read()
		{
			List<byte> bytes = new List<byte>();
			
			try
			{
				if(_client.Available > 0)
				{
					byte[] rcvData = new byte[256];
					int count = _client.Receive(rcvData);
					for(int i=0; i<count; i++)
						bytes.Add(rcvData[i]);
				}
			}
			catch(Exception e)
			{
				Debug.Log(e);
				try
				{
					_client.Close();
				}
				catch(Exception e2)
				{
				}
				_client = null;
			}
			
			return bytes.ToArray();
		}

		public AppAction[] appActions
		{
			get
			{
				return GetComponentsInChildren<AppAction>();
			}
		}

		private void AcceptCompleted(object sender, SocketAsyncEventArgs e)
		{
			Debug.Log("Accept client!");

			_client = e.AcceptSocket;
			_client.NoDelay = true;
			_client.LingerState = new LingerOption(true, 0);
		}
	}
}
