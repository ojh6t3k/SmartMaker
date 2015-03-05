using UnityEngine;
using System.Collections;
using System.Reflection;
using System;
using System.Collections.Generic;


namespace SmartMaker
{
	[Serializable]
	public class EventDelegate
	{
		[SerializeField] MonoBehaviour _target;
		[SerializeField] string _methodName;

		public MonoBehaviour target
		{
			get
			{
				return _target;
			}
			set
			{
				_target = value;
				_methodName = null;
			}
		}

		public string methodName
		{
			get
			{
				return _methodName;
			}
			set
			{
				_methodName = value;
			}
		}

		public void Clear()
		{
			_target = null;
			_methodName = null;
		}

		public void Set(MonoBehaviour target, string methodName)
		{
			Clear();
			_target = target;
			_methodName = methodName;
		}

		public void Execute()
		{
			if(_target == null || _methodName == null)
				return;
		}

		static public void Execute(List<EventDelegate> list)
		{
			for(int i=0; i<list.Count; i++)
				list[i].Execute();
		}

		static public void Add(List<EventDelegate> list, MonoBehaviour target)
		{
			string[] methodNames = GetMethodNames(target);

			string methodName = null;
			if(methodNames.Length > 0)
				methodName = methodNames[0];

			EventDelegate eventDelegate = new EventDelegate();
			eventDelegate.Set(target, methodName);
			list.Add(eventDelegate);
		}

		static public string[] GetMethodNames(MonoBehaviour target)
		{
			List<string> methodNames = new List<string>();
			MethodInfo[] methods = target.GetType().GetMethods();

			for(int i=0; i<methods.Length; i++)
			{
				methodNames.Add(methods[i].Name);
			}

			return methodNames.ToArray();
		}
	}
}
