using UnityEngine;
using System.Collections;
using System;
using SmartMaker;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("SmartMaker")]
	[Tooltip("NeoPixel.SetPixel()")]
	public class NeoPixelSetPixel : FsmStateAction
	{
		[RequiredField]
		public NeoPixel neoPixel;
		public FsmInt index;
		public FsmColor color;
		public FsmFloat red;
		public FsmFloat green;
		public FsmFloat blue;

		public override void Reset()
		{
			neoPixel = null;
			// default axis to variable dropdown with None selected.
			index = new FsmInt { UseVariable = true };
			color = new FsmColor { UseVariable = true };
			red = new FsmFloat { UseVariable = true };
			green = new FsmFloat { UseVariable = true };
			blue = new FsmFloat { UseVariable = true };
		}
		
		public override void OnEnter()
		{
			base.OnEnter();
			
			if(neoPixel != null)
			{
				if(!index.IsNone)
				{
					if(!color.IsNone)
					{
						neoPixel.SetPixel(index.Value, color.Value);
					}
					else
					{
						if(!red.IsNone && !green.IsNone && !blue.IsNone)
							neoPixel.SetPixel(index.Value, red.Value, green.Value, blue.Value);
					}
				}
			}
			
			Finish();
		}
	}
}
