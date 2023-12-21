using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VisualStateMachine.States;
using VisualStateMachine.Tools;

namespace VisualStateMachine
{
	[Serializable]
	public class StateNode
	{
		public bool IsEntryNode => _isEntryNode;
		public bool IsActive => _isActive;
		public Vector2 Position => _position;
		public State State => _state;
		public string Id => _id;
		public IReadOnlyList<StateConnection> Connections => _connections;
		public float LastActive => _lastActive;

		[SerializeField] private State _state;
		[SerializeField] private string _id = Guid.NewGuid().ToString();
		[SerializeField] private Vector2 _position;
		[SerializeField] private bool _isEntryNode;
		[SerializeField] private List<StateConnection> _connections = new();

		[NonSerialized]
		private bool _isActive;

		[NonSerialized] 
		private float _lastActive = -1;
		
		public StateNode(Type stateType)
		{
			if (!stateType.IsSubclassOf(typeof(State)))
			{
				throw new ArgumentException("The provided type must be a subclass of State.");
			}
			
			_state = ScriptableObject.CreateInstance(stateType) as State;
			_state.name = _id;
		}
		
		public void SetAsEntryNode(bool isEntryNode)
		{
			_isEntryNode = isEntryNode;
		}

		public void SetPosition(Vector2 position)
		{
			_position = position;
		}
		
		public void AddConnection(StateConnection connection)
		{
			_connections.Add(connection);
		}
		
		public void RemoveConnection(StateConnection connection)
		{
			_connections.Remove(connection);
		}
		
		public List<StateConnection> GetConnectionsToNode(string nodeId)
		{
			return _connections.FindAll(connection => connection.ToNodeId == nodeId);
		}
		
		public void RemoveConnections(IEnumerable<StateConnection> connections)
		{
			foreach (var connection in connections)
			{
				_connections.Remove(connection);
			}
		}
		
		public void RemoveConnectionToNode(string nodeId)
		{
			_connections.RemoveAll(connection => connection.ToNodeId == nodeId);
		}
		
		
		public void Awake(StateMachineCore stateMachineCore)
		{
			_state = _state.Clone(stateMachineCore);
			_state.AwakeState();
		}

		public void Start(StateMachineCore stateMachineCore)
		{
			_state.StartState();
		}

		public void Enter()
		{
			ToggleConnectionSubscription(subscribe: true);
			_isActive = true;
			_lastActive = Time.time;
			State.EnterState();
		}

		public void Update()
		{
			if (!_isActive) return;
			
			_lastActive = Time.time;
			State.UpdateState();
		}

		public void FixedUpdate()
		{
			if (!_isActive) return;

			_lastActive = Time.time;
			State.FixedUpdateState();
		}

		public void Exit()
		{
			ToggleConnectionSubscription(subscribe: false);
			_isActive = false;
			_lastActive = Time.time;
			State.ExitState();
		}

		private void ToggleConnectionSubscription(bool subscribe)
		{
			foreach (var connection in _connections) 
			{
				ToggleEventSubscriptionByName(State, connection.FromPortName, connection, subscribe);
			}
		}

		private void ToggleEventSubscriptionByName(object targetObject, string eventName, StateConnection connection, bool subscribe)
		{
			var eventInfo = targetObject.GetType().GetEvent(eventName);
			if (eventInfo == null) return;
			
			var handler = Delegate.CreateDelegate(eventInfo.EventHandlerType, connection, nameof(connection.Transition));

			if (subscribe)
			{
				eventInfo.AddEventHandler(targetObject, handler);
			}
			else
			{
				eventInfo.RemoveEventHandler(targetObject, handler);
			}
		}

		public void RemoveAll(Predicate<StateConnection> match)
		{
			_connections.RemoveAll(match);
		}

		public void Reset()
		{
			_isActive = false;
		}
	}
}