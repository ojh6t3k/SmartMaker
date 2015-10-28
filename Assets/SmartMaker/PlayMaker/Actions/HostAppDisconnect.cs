using UnityEngine;
using System.Collections;
using System;
using SmartMaker;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("SmartMaker")]
	[Tooltip("HostApp.Disconnect()")]
	public class HostAppDisconnect : FsmStateAction
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
                hostApp.Disconnect();
			
			Finish();
		}
	}
}
