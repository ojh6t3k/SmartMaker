using UnityEngine;
using System.Collections;

namespace SmartMaker
{
	[AddComponentMenu("SmartMaker/Arduino/AppActions/Add-on/NeoPixel")]
	public class NeoPixel : AppAction
	{
		public enum CONFIG
		{
			NEO_RGB_KHZ400 = 0x00,
			NEO_RGB_KHZ800 = 0x02,
			NEO_GRB_KHZ400 = 0x01,
			NEO_GRB_KHZ800 = 0x03
		}

		public int pin;
		public int num;
		public CONFIG config;

		void Awake()
		{
		}

		// Use this for initialization
		void Start ()
		{
		}
		
		// Update is called once per frame
		void Update ()
		{
		}
	}
}