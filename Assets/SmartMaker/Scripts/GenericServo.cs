using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace SmartMaker
{
	[AddComponentMenu("SmartMaker/AppActions/GenericServo")]
	public class GenericServo : AppAction
	{
		public int pin;
		public float offsetAngle = 0f;
		
		[SerializeField] private byte _angle;
		
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
		
		public float Angle
		{
			get
			{
				return (float)_angle - 90f;
			}
			set
			{
				int newAngle = (int)(Mathf.Clamp(value, -90f, 90f) + 90f);
				if(_angle != (byte)newAngle)
				{
					_angle = (byte)newAngle;
					SetDirty();
				}
			}
		}

		public override string[] SketchExternalIncludes()
		{
			List<string> includes = new List<string>();
			includes.Add("#include <Servo.h>");
			return includes.ToArray();
		}
		
		public override string SketchDeclaration()
		{
			return string.Format("{0} {1}({2:d}, {3:d});", this.GetType().Name, this.name, id, pin);
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
			int angle = (int)Mathf.Clamp((float)_angle + offsetAngle, 0f, 180f);
			Push((byte)angle);
		}
	}
}
