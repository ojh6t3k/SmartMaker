using UnityEngine;
using System.Collections;


namespace SmartMaker
{
	[AddComponentMenu("SmartMaker/AppActions/DigitalOutput")]
	public class DigitalOutput : AppAction
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

		public bool Value
		{
			get
			{
				if(_value == 0)
					return false;
				else
					return true;
			}
			set
			{
				byte bValue = 0;
				if(value == true)
					bValue = 1;
					
				if(_value != bValue)
				{
					_value = bValue;
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
