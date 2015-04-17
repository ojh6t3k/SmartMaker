using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace SmartMaker
{
	[AddComponentMenu("SmartMaker/Communication/CommOTG")]
	public class CommOTG : CommObject
	{
#if UNITY_ANDROID && !UNITY_EDITOR
		private AndroidJavaObject _activity;
		private bool _isOpen;

		void Awake()
		{
			AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			_activity = jc.GetStatic<AndroidJavaObject>("currentActivity");
			_isOpen = false;
		}
		
		public override void Open()
		{
			if(_activity != null)
			{
				_isOpen = _activity.Call<bool>("OTG_Open");
			}

			if(_isOpen == false)
			{
				if(OnOpenFailed != null)
					OnOpenFailed(this, null);
			}
		}
		
		public override void Close()
		{
			if(_activity != null)
			{
				_activity.Call("OTG_Close");
				_isOpen = false;
			}
		}
		
		public override void Write(byte[] bytes)
		{
			if(_activity != null)
			{
				if(_activity.Call<bool>("OTG_Write", bytes) == false)
				{
					_isOpen = false;
					if(OnErrorClosed != null)
						OnErrorClosed(this, null);
				}
			}
			else
			{
				if(OnErrorClosed != null)
					OnErrorClosed(this, null);
			}
		}
		
		public override byte[] Read()
		{
			if(_activity != null)
			{
				return _activity.Call<byte[]>("OTG_Read");
			}
			else
			{
				if(OnErrorClosed != null)
					OnErrorClosed(this, null);
				return null;
			}
		}
		
		public override bool IsOpen
		{
			get
			{
				return _isOpen;
			}
		}
#else
		public override void Open()
		{
			if(OnOpenFailed != null)
				OnOpenFailed(this, null);
		}
		
		public override void Close()
		{
		}
		
		public override void Write(byte[] bytes)
		{
			if(OnErrorClosed != null)
				OnErrorClosed(this, null);
		}
		
		public override byte[] Read()
		{
			if(OnErrorClosed != null)
				OnErrorClosed(this, null);
			return null;
		}
		
		public override bool IsOpen
		{
			get
			{
				return false;
			}
		}
#endif
	}
}