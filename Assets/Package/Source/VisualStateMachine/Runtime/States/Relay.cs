﻿using System;
using VisualStateMachine.Attributes;

namespace VisualStateMachine.States
{
	public enum RelayDirection
	{
		Left,
		Right,
		Down,
		Up
	}
	
	[NodeType(NodeType.Relay)]
	[PortOrientation(PortOrientation.Vertical)]
	public abstract class Relay : State
	{
		[Transition]
		public event Action Exit;
		
		public RelayDirection Direction { get; set; }
		
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