using System;
using UnityEngine;
using VisualStateMachine.Attributes;
using VisualStateMachine.States;

namespace Samples.ExampleTwo.Source
{
	public class FooBarState : State
	{
		[Transition]
		public event Action ExitOne;

		[Transition] 
		public event Action ExitTwo;
		
		private float _remainingTime;
		private float _duration;
		
		public override void EnterState()
		{
			_duration = StateMachineCore.Root.GetComponent<Duration>().DurationTime;
			_remainingTime = _duration;
		}

		public override void UpdateState()
		{
			if (_remainingTime > 0)
			{
				_remainingTime -= Time.deltaTime;
			}
			else
			{
				ExitOne?.Invoke();
			}
		}

		public override void ExitState()
		{
			
		}
	}
}