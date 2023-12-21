using System;
using System.Collections.Generic;
using UnityEngine;
using VisualStateMachine.Attributes;
using VisualStateMachine.States;

namespace Testing.Test
{
	public class TestingState : State
	{
		[Transition] public event Action Exit;
		
		[SerializeField] private List<string> _myList;
		
		public override void EnterState()
		{
			
		}

		public override void ExitState()
		{
			
		}
	}
}