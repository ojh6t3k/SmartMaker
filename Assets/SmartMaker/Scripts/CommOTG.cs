using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace SmartMaker
{
	[AddComponentMenu("SmartMaker/Communication/CommOTG")]
	public class CommOTG : CommObject
	{
		void Awake()
		{
		}
		
		public override void Open()
		{
		}
		
		public override void Close()
		{
		}
		
		public override void Write(byte[] bytes)
		{
			try
			{
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
				return false;
			}
		}
	}
}