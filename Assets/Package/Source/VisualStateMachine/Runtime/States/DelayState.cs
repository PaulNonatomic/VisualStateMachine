﻿using System;
using UnityEngine;
using VisualStateMachine.Attributes;

namespace VisualStateMachine.States
{
	[NodeColor(NodeColor.Pink), NodeIcon(NodeIcon.VsmFlatTimeWhite, opacity:0.3f)]
	public class DelayState : State
	{
		[Transition]
		public event Action Exit;
		
		[SerializeField] 
		private float _duration = 1f;
		
		[NonSerialized]
		private float _time;

		public override void EnterState()
		{
			_time = Time.time;
		}

		public override void UpdateState()
		{
			if (Time.time - _time > _duration)
			{
				Exit?.Invoke();
			}
		}

		public override void ExitState()
		{
			//Do nothing
		}
	}
}