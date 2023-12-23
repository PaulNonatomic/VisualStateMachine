using System;
using System.Collections.Generic;
using UnityEngine;
using VisualStateMachine.Attributes;
using VisualStateMachine.States;

namespace Testing.Test
{
	public class ManyPropsState : State
	{
		[Transition]
		public event Action Exit;
		
		[SerializeField] private List<string> _myStrings;
		[SerializeField] private ScriptableObject _myObject;
		[SerializeField] private int _myInt;
		[SerializeField] private bool _myBool;
		[SerializeField] private List<int> _myInts;
		[SerializeField] private ScriptableObject _myNextObject;
 		
		public override void EnterState()
		{
			
		}

		public override void ExitState()
		{
			
		}
	}
}