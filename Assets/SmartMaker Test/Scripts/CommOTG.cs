using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace SmartMaker
{
	[AddComponentMenu("SmartMaker/Unity3D/Communication/CommOTG")]
	public class CommOTG : CommObject
	{
#if UNITY_ANDROID
		private AndroidJavaObject _androidOTG;
		private AndroidJavaObject _activityContext;
		private bool _isOpen;

		void Awake()
		{
			_isOpen = false;

			using(AndroidJavaClass activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
			{
				_activityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
			}

			using(AndroidJavaClass pluginClass = new AndroidJavaClass("com.smartmaker.android.CommOTG"))
			{
				if(pluginClass != null)
				{
					_androidOTG = pluginClass.CallStatic<AndroidJavaObject>("instance");
					_androidOTG.Call("SetContext", _activityContext);
				}
			}
		}
		
		public override void Open()
		{
			if(_androidOTG != null)
			{
				_isOpen = _androidOTG.Call<bool>("open");
			}

			if(_isOpen == false)
			{
				if(OnOpenFailed != null)
					OnOpenFailed(this, null);
			}
		}
		
		public override void Close()
		{
			if(_androidOTG != null)
			{
				_androidOTG.Call("close");
				_isOpen = false;
			}
		}
		
		public override void Write(byte[] bytes)
		{
			if(_androidOTG != null)
			{
				if(_androidOTG.Call<bool>("write", bytes) == false)
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
			if(_androidOTG != null)
			{
				return _androidOTG.Call<byte[]>("read");
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
		
#endif
	}
}