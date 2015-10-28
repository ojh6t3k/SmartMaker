using UnityEngine;
using System.Collections;
using System;
using SmartMaker;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("SmartMaker")]
	[Tooltip("HostApp.Connect()")]
	public class HostAppConnect : FsmStateAction
	{
		[RequiredField]
		public HostApp hostApp;

		public override void Reset()
		{
            hostApp = null;
		}
		
		public override void OnEnter()
		{
			base.OnEnter();
			
			if(hostApp != null)
                hostApp.Connect();
			
			Finish();
		}
	}
}
