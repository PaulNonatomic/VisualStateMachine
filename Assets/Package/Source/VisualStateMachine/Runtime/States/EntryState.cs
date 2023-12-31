﻿using System;
using VisualStateMachine.Attributes;

namespace VisualStateMachine.States
{ 
	[HideNode, NodeColor(NodeColor.Green), NoInputPort()]
	public class EntryState : State
	{
		[Transition(">>", NodeColor.Green, frameDelay:0)]
		public event Action Exit;

		public override void EnterState()
		{
			Exit?.Invoke();
		}

		public override void ExitState()
		{
			//Do nothing
		}
	}
}