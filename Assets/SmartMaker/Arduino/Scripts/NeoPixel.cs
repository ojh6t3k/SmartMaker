using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace SmartMaker
{
	[AddComponentMenu("SmartMaker/Arduino/AppActions/Add-on/NeoPixel")]
	public class NeoPixel : AppAction
	{
		public class Pixel
		{
			public byte red;
			public byte green;
			public byte blue;
			public bool dirty = false;
		}

		public enum CONFIG
		{
			NEO_RGB_KHZ400,
			NEO_RGB_KHZ800,
			NEO_GRB_KHZ400,
			NEO_GRB_KHZ800
		}

		public int pin;
		public int num;
		public CONFIG config;
		[Range(0f, 1f)]
		public float brightness = 1f;

		private Pixel[] _pixels;
		private byte _index;
		private byte _red;
		private byte _green;
		private byte _blue;
		private float _brightness;

		void Awake()
		{
			_pixels = new Pixel[num];
			for(int i=0; i<_pixels.Length; i++)
				_pixels[i] = new Pixel();
		}

		// Use this for initialization
		void Start ()
		{
		}
		
		// Update is called once per frame
		void Update ()
		{
			if(brightness != _brightness)
			{
				_brightness = brightness;
				SetDirty();
			}
		}

		public void SetPixel(int index, Color color)
		{
			SetPixel(index, color.r, color.g, color.b);
		}

		public void SetPixel(int index, float red, float green, float blue)
		{
			for(int i=0; i<_pixels.Length; i++)
			{
				if(i == index)
				{
					_pixels[i].red = (byte)(red * 255f);
					_pixels[i].green = (byte)(green * 255f);
					_pixels[i].blue = (byte)(blue * 255f);
					_pixels[i].dirty = true;
					SetDirty();
					break;
				}
			}
		}
/*
		public override string[] SketchIncludes()
		{
			List<string> includes = new List<string>();
			includes.Add("#include <Adafruit_NeoPixel.h>");
			return includes.ToArray();
		}
*/
		public override string SketchDeclaration()
		{
			string configString = "";
			switch(config)
			{
			case CONFIG.NEO_GRB_KHZ400:
				configString = "NEO_GRB+NEO_KHZ400";
				break;

			case CONFIG.NEO_GRB_KHZ800:
				configString = "NEO_GRB+NEO_KHZ800";
				break;

			case CONFIG.NEO_RGB_KHZ400:
				configString = "NEO_RGB+NEO_KHZ400";
				break;

			case CONFIG.NEO_RGB_KHZ800:
				configString = "NEO_RGB+NEO_KHZ800";
				break;
			}

			return string.Format("{0} {1}({2:d}, {3:d}, {4:d}, (int)({5}));", this.GetType().Name, SketchVarName, id, num, pin, configString);
		}
		
		public override string SketchVarName
		{
			get
			{
				return string.Format("neoPixel{0:d}", id);
			}
		}
		
		protected override void OnActionSetup ()
		{
		}
		
		protected override void OnActionStart ()
		{
			autoUpdate = false;
		}
		
		protected override void OnActionExcute ()
		{
		}
		
		protected override void OnActionStop ()
		{
		}
		
		protected override void OnPop ()
		{
		}
		
		protected override void OnPush ()
		{
			for(int i=0; i<_pixels.Length; i++)
			{
				if(_pixels[i].dirty == true)
				{
					_pixels[i].dirty = false;
					_index = (byte)i;
					_red = _pixels[i].red;
					_green = _pixels[i].green;
					_blue = _pixels[i].blue;
					break;
				}
			}

			Push(_index);
			Push(_red);
			Push(_green);
			Push(_blue);
			int ibrightness = (int)(_brightness * 255f);
			Push((byte)ibrightness);

			for(int i=0; i<_pixels.Length; i++)
			{
				if(_pixels[i].dirty == true)
				{
					SetDirty();
					break;
				}
			}
		}
	}
}