using UnityEngine;
using System.Collections;
using UnityEngine.Networking;


namespace SmartMaker
{
	[AddComponentMenu("SmartMaker/Unity3D/Network/NetDigitalOutput")]
	public class NetDigitalOutput : NetworkBehaviour
	{
		public int id;

		private DigitalOutput _dOut;

		[SyncVar]
		private bool _value;

		// Use this for initialization
		void Start ()
		{
			if(isServer == true)
			{
				DigitalOutput[] dOutList = GameObject.FindObjectsOfType<DigitalOutput>();
				foreach(DigitalOutput dO in dOutList)
				{
					if(dO.id == id)
					{
						_dOut = dO;
						break;
					}
				}
			}
		}
		
		// Update is called once per frame
		void Update ()
		{
			if(_dOut != null)
				_dOut.value = _value;
		}

		public bool Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
			}
		}
	}
}
