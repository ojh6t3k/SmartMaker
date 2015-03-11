using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Events;


namespace SmartMaker
{
	[AddComponentMenu("SmartMaker/AppActions/DigitalInput")]
	public class DigitalInput : AppAction
	{
		public int pin;
		public bool pullup;

		public UnityEvent OnChangedValue;

		private byte _newValue;
		private byte _value;
		
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

		public bool Value
		{
			get
			{
				if(_value == 0)
					return false;
				else
					return true;
			}
		}

		public override string SketchDeclaration()
		{
			string code = string.Format("{0} {1}({2:d}, {3:d}, ", this.GetType().Name, this.name, id, pin);
			if(pullup == true)
				code += "true);";
			else
				code += "false);";

			return code;
		}
		
		protected override void OnActionSetup ()
		{
		}
		
		protected override void OnActionStart ()
		{
			_newValue = _value;
		}
		
		protected override void OnActionExcute ()
		{
			if(_newValue != _value)
			{
				_value = _newValue;
				OnChangedValue.Invoke();
			}
		}
		
		protected override void OnActionStop ()
		{
		}
		
		protected override void OnPop ()
		{
			Pop(ref _newValue);
		}
		
		protected override void OnPush ()
		{
		}
	}
}
