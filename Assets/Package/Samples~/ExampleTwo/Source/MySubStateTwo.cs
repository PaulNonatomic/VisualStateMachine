using UnityEngine;
using VisualStateMachine.States;

namespace Samples.ExampleTwo.Source
{
	public class MySubStateTwo : State
	{
		[SerializeField] private float _duration = 1f;
		
		private float _entryTime;
		
		public override void EnterState()
		{
			_entryTime = Time.time;
		}

		public override void UpdateState()
		{
			if (Time.time - _entryTime > _duration)
			{
				StateMachineCore.Complete(this);
			}
		}

		public override void ExitState()
		{
			
		}
	}
}