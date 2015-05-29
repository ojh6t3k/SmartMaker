using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.IO.Ports;
using UnityEngine.UI;

namespace SmartMaker
{
	[AddComponentMenu("SmartMaker/Communication/CommBluetooth")]
	public class CommBluetooth : CommObject
	{
#if UNITY_ANDROID
		[SerializeField]
		public List<string> devNames = new List<string>();
		public string devName;

		public Text uiText;
		public RectTransform uiPanel;
		public GameObject uiItem;

		private AndroidJavaObject _activity;
		private AndroidJavaObject _activityContext;
		
		void Awake()
		{
			using(AndroidJavaClass activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
			{
				_activityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
			}
			
			using(AndroidJavaClass pluginClass = new AndroidJavaClass("com.smartmaker.android.CommBluetooth"))
			{
				if(pluginClass != null)
				{
					_activity = pluginClass.CallStatic<AndroidJavaObject>("GetInstance");
					_activity.Call("SetContext", _activityContext);
				}
			}

			if(uiText != null)
				uiText.text = devName;
		}
		
		public void DeviceSearch()
		{
			if(_activity == null)
				return;

			devNames.Clear();
			devNames.AddRange(_activity.Call<string[]>("DeviceSearch"));

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
				
				if(devNames.Count == 0)
				{
					if(t != null)
						t.text = "";
				}
				else
				{
					if(t != null)
						t.text = devNames[0];
					
					for(int i=1; i<devNames.Count; i++)
					{
						GameObject item = GameObject.Instantiate(uiItem);
						item.transform.SetParent(uiPanel.transform);
						t = item.GetComponent<Text>();
						if(t == null)
							t = item.GetComponentInChildren<Text>();
						if(t != null)
							t.text = devNames[i];
					}
				}
			}
		}
		
		public override void Open()
		{
			if(_activity == null)
				return;

			if(_activity.Call<bool>("Open", devName) == true)
			{
				StartCoroutine(WaitOpenComplete());
			}
			else
			{
				if(OnOpenFailed != null)
					OnOpenFailed(this, null);
			}
		}

		private IEnumerator WaitOpenComplete()
		{
			while(IsOpen == false)
			{
				if(_activity.Call<bool>("IsOpening") == true)
					yield return new WaitForSeconds(0.5f);
				else
					break;
			}

			if(IsOpen == true)
			{
				if(OnOpened != null)
					OnOpened(this, null);

				StartCoroutine(WatchClosed());
			}
			else
			{
				if(OnOpenFailed != null)
					OnOpenFailed(this, null);
			}
		}

		private IEnumerator WatchClosed()
		{
			while(IsOpen == true)
			{
				yield return new WaitForSeconds(1f);
			}

			if(OnErrorClosed != null)
				OnErrorClosed(this, null);
		}
		
		public override void Close()
		{
			StopCoroutine(WatchClosed());

			if(_activity == null)
				return;

			_activity.Call("Close");
		}
		
		public override void Write(byte[] bytes)
		{
			if(_activity == null)
				return;

			if(bytes == null)
				return;
			if(bytes.Length == 0)
				return;
			
			_activity.Call("Write", bytes);
		}
		
		public override byte[] Read()
		{
			if(_activity == null)
				return null;

			return _activity.Call<byte[]>("Read");
		}
		
		public override bool IsOpen
		{
			get
			{
				if(_activity == null)
					return false;

				return _activity.Call<bool>("IsOpen");
			}
		}
		
		public void SelectDeviceName(Text text)
		{
			devName = text.text;
			if(uiText != null)
				uiText.text = devName;
		}

#else
		[SerializeField]
		public List<string> devNames = new List<string>();
		public string devName;

		public Text uiText;
		public RectTransform uiPanel;
		public GameObject uiItem;

		void Awake()
		{
			if(uiText != null)
				uiText.text = devName;
		}
		
		public void DeviceSearch()
		{
			devNames.Clear();

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
				if(t != null)
					t.text = "";
			}
		}
		
		public void SelectDeviceName(Text text)
		{
			devName = text.text;
			if(uiText != null)
				uiText.text = devName;
		}
#endif
	}
}
