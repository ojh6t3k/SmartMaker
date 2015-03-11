using UnityEngine;
using System.Collections;


namespace SmartMaker
{
	[AddComponentMenu("SmartMaker/AppActions/AnalogOutput")]
	public class AnalogOutput : AppAction
	{
		public int pin;
		
		[SerializeField] private byte _value;
		
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
		
		public float Value
		{
			get
			{
				return (float)_value / 255f;
			}
			set
			{
				int newValue = (int)(value * 255f);
				if(_value != (byte)newValue)
				{
					_value = (byte)newValue;
					SetDirty();
				}
			}
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
			Push(_value);
		}
	}
}
