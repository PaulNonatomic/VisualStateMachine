using System;
using UnityEngine;
using VisualStateMachine.Attributes;
using VisualStateMachine.States;

namespace Samples.ExampleTwo.Source
{
	public class RotationState : State
	{
		[Transition]
		public event Action Complete;
		
		[SerializeField] private float _speed = 1f;
		[SerializeField] private Vector3 _axis = Vector3.forward;
		[SerializeField] private float _duration = 1f;
		
		private StateMachineReferences _references;
		private float _entryTime;
		
		public override void EnterState()
		{
			_references = StateMachineCore.Root.GetComponentInParent<StateMachineReferences>();
			_entryTime = Time.time;
		}

		public override void UpdateState()
		{
			var speed = _speed * Time.deltaTime;
			var axis = _axis.normalized * _speed;
			
			_references.Cube.Rotate(axis);

			if (Time.time - _entryTime > _duration)
			{
				Complete?.Invoke();
			}
		}

		public override void ExitState()
		{
			
		}
	}
}