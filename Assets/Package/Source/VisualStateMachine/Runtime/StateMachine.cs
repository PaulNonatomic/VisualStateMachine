using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using VisualStateMachine.States;
using VisualStateMachine.Tools;
using Object = UnityEngine.Object;

namespace VisualStateMachine
{
	[Serializable]
	public class GraphViewState
	{
		public Vector3 Position = Vector3.zero;
		public float Scale = 0f;
	}
	
	[CreateAssetMenu(fileName = "StateMachine", menuName = "StateMachine/StateMachine")]
	public class StateMachine : ScriptableObject
	{
		public GraphViewState GraphViewState => _graphViewState ??= new GraphViewState();
		public State CurrentState => _currentNode?.State ?? null;
		
		public StateMachine Base { get; set; }
		public IReadOnlyCollection<StateNode> Nodes => _nodes;
		public bool IsComplete { get; private set; }

		[SerializeField] 
		private string _entryStateId;

		[SerializeField] 
		private GraphViewState _graphViewState;
		
		[SerializeField]
		private List<StateNode> _nodes = new();

		[SerializeField]
		private List<Object> _references = new();

		[NonSerialized]
		private Dictionary<string, StateNode> _nodeLookup = new();
		
		[NonSerialized]
		private Dictionary<JumpId, StateNode> _jumpNodeLookup = new();

		[NonSerialized]
		private StateNode _currentNode;

		[NonSerialized]
		private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();


		public static StateMachine CreateInstance(StateMachine stateMachine)
		{
			DevLog.Log("StateMachine.CreateInstance");
			var instance = Instantiate(stateMachine);
			instance.name = instance.name + instance.GetInstanceID();
			instance.Base = stateMachine;
			
			return instance;
		}

		public void UpdateGraphViewState(Vector3 position, float scale)
		{
			_graphViewState ??= new GraphViewState();
			_graphViewState.Position = position;
			_graphViewState.Scale = scale;
		}
		
		public void AwakeStateMachine(StateMachineCore stateMachineCore)
		{
			DevLog.Log("StateMachine.Awake");
			
			AwakeNodes(stateMachineCore);
			CreateNodeLookupTable();
			
			if(_nodeLookup.Count == 0) throw new Exception($"StateMachine {this.name} has 0 nodes.");
			
			_currentNode = _nodeLookup[_entryStateId];
		}

		public void StartStateMachine(StateMachineCore stateMachineCore)
		{
			DevLog.Log($"StateMachine.Start: {name}");
			ResetNodes();
			IsComplete = false;
			
			StartNodes(stateMachineCore);
			_currentNode = _nodeLookup[_entryStateId];
			SubscribeToNode(_currentNode);
			
			_currentNode?.Enter();
		}

		private void ResetNodes()
		{
			foreach(var node in _nodes)
			{
				node.Reset();
			}
		}

		public void Update()
		{
			_currentNode?.Update();
		}

		public void Complete()
		{
			IsComplete = true;
		}
	
		public void AddEntryNode()
		{
			if (Application.isPlaying) return;
			
			DevLog.Log("StateMachine.AddEntryNode");
			
			#if UNITY_EDITOR
			{
				if (!AssetDatabase.Contains(this)) return;
				if (_nodes.Count > 0) return;

				var stateNode = new StateNode(typeof(EntryState));
				stateNode.SetAsEntryNode(true);
			
				_entryStateId = stateNode.Id;
			
				AddNode(stateNode);
			}
			#endif
		}

		private void AwakeNodes(StateMachineCore stateMachineCore)
		{
			DevLog.Log("StateMachine.AwakeNodes");
			
			foreach (var node in _nodes)
			{
				node.Awake(stateMachineCore);
			}
		}

		private void StartNodes(StateMachineCore stateMachineCore)
		{
			DevLog.Log("StateMachine.StartNodes");
			
			foreach (var node in _nodes)
			{
				node.Start(stateMachineCore);
			}
		}

		private void CreateNodeLookupTable()
		{
			DevLog.Log("StateMachine.CreateNodeLookupTable");
			
			_nodeLookup.Clear();

			foreach (var node in _nodes)
			{
				if (node.State == null) continue;
				
				_nodeLookup.Add(node.Id, node);
				
				if (node.State is JumpInState)
				{
					var jumpIn = node.State as JumpInState;
					_jumpNodeLookup.Add(jumpIn.JumpId, node);
				}
			}
		}

		#region StateMachine Management
		
		private void SubscribeToNode(StateNode node)
		{
			DevLog.Log("StateMachine.SubscribeToNode");
			
			var connections = node.Connections;

			foreach (var connection in node.Connections)
			{
				connection.OnTransition += OnTransition;
			}
		}
		
		private void Unsubscribe(StateNode node)
		{
			DevLog.Log("StateMachine.Unsubscribe");
			
			var connections = node?.Connections;
			if (connections == null) return;
			
			foreach (var connection in node.Connections)
			{
				if (connection == null) continue;
				connection.OnTransition -= OnTransition;
			}
		}

		private void OnTransition(StateConnection connection)
		{
			DevLog.Log("StateMachine.OnTransition");
			var nextNode = _nodeLookup[connection.ToNodeId];
			TriggerAsyncTransition(nextNode, connection.PortData.FrameDelay);
		}

		private async void TriggerAsyncTransition(StateNode nextNode, int frameDelay = 0)
		{
			try
			{
				await TransitionAsync(nextNode, frameDelay).ConfigureAwait(false);
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		private async Task TransitionAsync(StateNode nextNode, int frameDelay = 0)
		{
			try
			{
				Unsubscribe(_currentNode);
				_currentNode.Exit();
				
				if (!Application.isPlaying) return;
				
				if(frameDelay > 0)
				{
					await Task.Delay(TimeSpan.FromSeconds(Time.deltaTime), _cancellationTokenSource.Token);
				}
				
				_currentNode = nextNode;
				if (_currentNode == null) return;
				
				SubscribeToNode(_currentNode);
				_currentNode.Enter();
			}
			catch (OperationCanceledException canceledException)
			{
				Debug.Log("StateMachine.TransitionAsync cancelled: " + canceledException.Message);
			}
		}
		
		public void JumpTo(JumpId jumpId)
		{
			if (!_jumpNodeLookup.ContainsKey(jumpId))
			{
				throw new StateMachineException($"There is no JumpIn state with the id:{jumpId} in {this.name}");
			}
			
			var nextNode = _jumpNodeLookup[jumpId];
			if (nextNode == null) return;

			TriggerAsyncTransition(nextNode);
		}
		
		#endregion

		#region Node Management
		
		public void AddNode(StateNode node)
		{
			if (Application.isPlaying) return;
			
			ValidateSubAssets();
			
			if (node.State == null) return;
			
			DevLog.Log("StateMachine.AddNode");
			_nodes.Add(node);
			
			#if UNITY_EDITOR
			{
				_references.Add(node.State);
				AssetDatabase.AddObjectToAsset(node.State, this);
				AssetDatabase.SaveAssets();
			}
			#endif
		}
		
		public void RemoveNode(StateNode node)
		{
			if (Application.isPlaying) return;
			
			DevLog.Log("StateMachine.RemoveNode");
			
			ValidateSubAssets();

			if (node == null) return;
			if (node.State == null) return;
			if (!_nodes.Contains(node)) return;

			#if UNITY_EDITOR
			{
				var isEntryNode = _entryStateId == node.Id;

				AssetDatabase.RemoveObjectFromAsset(node.State);
				_references.Remove(node.State);
				RemoveConnectionsToNode(node.Id);
				
				_nodes.Remove(node);
				Save();
				
				if(isEntryNode) SelectNextEntryNode();
			}
			#endif
		}

		public void RemoveConnectionsToNode(string nodeId)
		{
			DevLog.Log("StateMachine.RemoveConnectionsToNode");
			foreach (var node in _nodes)
			{
				node.RemoveAll(connection => connection.ToNodeId == nodeId);
			}
		}

		public void RemoveConnection(string fromNodeId, string toNodeId)
		{
			DevLog.Log("StateMachine.RemoveConnection");
			
			var fromNode = _nodes.FirstOrDefault(node => node.Id == fromNodeId);
			if (fromNode == null) return;
			
			fromNode.RemoveAll(connection => connection.ToNodeId == toNodeId);
		}

		public void SetEntryNode(StateNode node)
		{
			DevLog.Log("StateMachine.SetEntryNode");
			
			if (!_nodes.Contains(node)) throw new Exception("Node is not part of this state machine");
			
			SetEntryNodeId(node.Id);
		}

		private void SetEntryNodeId(string entryNodeId)
		{
			DevLog.Log("StateMachine.SetEntryNodeId");
			_entryStateId = entryNodeId;

			foreach (var node in _nodes)
			{
				node.SetAsEntryNode(node.Id == entryNodeId);
			}
			
			Save();
		}

		public void Save()
		{
			if (Application.isPlaying) return;
			
			DevLog.Log("StateMachine.Save");
			ValidateSubAssets();
			
			#if UNITY_EDITOR
			{
				EditorUtility.SetDirty(this);
				AssetDatabase.SaveAssetIfDirty(this);
			}
			#endif
		}
		
		private void SelectNextEntryNode()
		{
			DevLog.Log("StateMachine.SelectNextEntryNode");
			
			_entryStateId = _nodes.Count > 0 ? _nodes[0].Id : null;
			Save();
		}

		public void RemoveAllNodes()
		{
			if (Application.isPlaying) return;
			
			DevLog.Log("StateMachine.RemoveAllNodes");
			
			#if UNITY_EDITOR
			{
				foreach(var state in _references)
				{
					AssetDatabase.RemoveObjectFromAsset(state);
				}

				_references.Clear();
				_nodes.Clear();
				Save();
			}
			#endif
		}
		
		#endregion
		
		private void ValidateSubAssets ()
		{
			if (Application.isPlaying) return;
			
			var thisPath = AssetDatabase.GetAssetPath(this);
			var subAssets = AssetDatabase.LoadAllAssetsAtPath(thisPath);
 
			for (var i = subAssets.Length - 1; i >= 0; i--) 
			{
				if(subAssets[i] != null) continue;
			 
				AssetDatabase.RemoveObjectFromAsset(_references[i]);
				_references.RemoveAt(i);
				_nodes.RemoveAt(i);
			}

			AssetDatabase.SaveAssets();
		}

		public void Dispose()
		{
			_cancellationTokenSource.Cancel();

			if (_currentNode != null)
			{
				try
				{
					Unsubscribe(_currentNode);
				}
				catch (Exception ex)
				{
					Debug.LogError("Error while unsubscribing: " + ex.Message);
				}
			}
			
			foreach(var node in _nodes)
			{
				if (node == null) continue;
				if (node.State == null) continue;
				
				node.State.DestroyState();
			}
		}
	}
}