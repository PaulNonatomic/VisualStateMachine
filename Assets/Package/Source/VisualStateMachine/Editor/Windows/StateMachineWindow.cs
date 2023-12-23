using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace VisualStateMachine.Editor.Windows
{
	[InitializeOnLoad]
	public class StateMachineWindow : GraphViewEditorWindow
	{
		public const int WindowWidth = 600;
		public const int WindowHeight = 400;
		
		private StateMachine _stateMachine;
		private StateMachine _stateMachineBase;
		private StateMachineGraphView _graphView;
		private StateMachineWindow _openWindow;

		private const string Title = "State Machine Editor";
		
		/**
		 * Static
		 */
		[MenuItem("Tools/State Machine Editor")]
		public static StateMachineWindow OpenWindow()
		{
			var window = GetWindow<StateMachineWindow>(false, Title, false);
			window.rootVisualElement.styleSheets.Add(Resources.Load<StyleSheet>("StateMachineEditor"));
			window.minSize = new Vector2(WindowWidth, WindowHeight);
			window.Draw();
			
			return window;
		}
		
		public static StateMachineWindow OpenWindow(StateMachine stateMachine)
		{
			var window = OpenWindow();
			window.Draw(stateMachine);

			return window;
		}

		static StateMachineWindow()
		{
			EditorApplication.projectWindowItemOnGUI += HandleProjectWindowItemGUI;
		}

		public void OnBecameVisible()
		{
			OpenWindow();
		}

		private void OnSelectionChange()
		{
			OpenWindow();
		}

		private static void HandleProjectWindowItemGUI(string guid, Rect selectionRect)
		{
			if (Event.current.type != EventType.MouseDown || Event.current.clickCount != 2) return;
			if (!TryGetSelectedStateMachine(out StateMachine stateMachine)) return;
			
			OpenWindow(stateMachine);
			Event.current.Use();
		}

		private static bool TryGetSelectedStateMachine(out StateMachine stateMachine)
		{
			stateMachine = null;

			var selected = Selection.activeObject;
			if (selected == null) return false;
			if (selected is not StateMachine selectedStateMachine) return false;

			stateMachine = selectedStateMachine;
			
			return true;
		}
		
		//@Todo: 
		private static bool TryGetOriginalStateMachine(out StateMachine stateMachine)
		{
			stateMachine = null;

			var selected = Selection.activeObject;
			if (selected == null) return false;
			if (selected is not StateMachineController stateMachineController) return false;

			stateMachine = stateMachineController.StateMachine;
			
			return true;
		}

		private static bool TryGetSelectedStateController(out StateMachineController stateMachineController)
		{
			stateMachineController = null;
			
			var selectedObject = Selection.activeGameObject;
			if (selectedObject == null) return false;
			
			stateMachineController = selectedObject.GetComponent<StateMachineController>();
			return stateMachineController != null;
		}

		private static bool TryGetStateMachine(out StateMachine stateMachine)
		{
			stateMachine = null;
			
			if (TryGetSelectedStateMachine(out var selectedStateMachine))
			{
				stateMachine = selectedStateMachine;
				return stateMachine != null;
			}

			if (TryGetSelectedStateController(out var selectedStateController))
			{
				stateMachine = selectedStateController.StateMachine;
				return stateMachine != null;
			}
			
			if(TryGetOriginalStateMachine(out var originalStateMachine))
			{
				stateMachine = originalStateMachine;
				return stateMachine != null;
			}

			return false;
		}
		
		/**
		 * Instance
		 */
		private void Draw()
		{
			if (_stateMachine != null)
			{
				Draw(_stateMachine);
				return;
			}

			if (_graphView == null)
			{
				_graphView = new StateMachineGraphView();
				rootVisualElement.Clear();
				rootVisualElement.Add(_graphView);
			}
		}
		
		private void Draw(StateMachine stateMachine)
		{
			_stateMachine = stateMachine;
			_stateMachineBase = stateMachine?.Base;
			
			if (_graphView == null)
			{
				_graphView = new StateMachineGraphView(_stateMachine);
				rootVisualElement.Clear();
				rootVisualElement.Add(_graphView);
			}
			else
			{
				_graphView.Update(_stateMachine);
			}
		}
		
		private void OnEnable()
		{
			EditorApplication.update += HandleEditorUpdate;
			EditorApplication.playModeStateChanged += HandlePlayModeStateChanged;
			AssemblyReloadEvents.beforeAssemblyReload += HandleBeforeAssemblyReload;
			AssemblyReloadEvents.afterAssemblyReload += HandleAfterAssemblyReload;
		}

		private void OnDisable()
		{
			EditorApplication.update -= HandleEditorUpdate;
			EditorApplication.playModeStateChanged -= HandlePlayModeStateChanged;
			AssemblyReloadEvents.beforeAssemblyReload -= HandleBeforeAssemblyReload;
			AssemblyReloadEvents.afterAssemblyReload -= HandleAfterAssemblyReload;
		}

		private void HandleEditorUpdate()
		{
			Redraw();
		}

		private void HandleAfterAssemblyReload()
		{
			Redraw();
		}

		private void HandleBeforeAssemblyReload()
		{
			Redraw();
		}

		private void HandlePlayModeStateChanged(PlayModeStateChange playMode)
		{
			if (playMode == PlayModeStateChange.ExitingEditMode)
			{
				if (_stateMachine == null) return;
				if (_stateMachine.Base == null) return;
				
				//.....
			}
		}

		private void Redraw()
		{
			if (TryGetStateMachine(out var stateMachine))
			{
				if (_stateMachine == null && stateMachine == null)
				{
					_graphView?.StateManager.ClearGraph();
				}
				
				Draw(stateMachine);
			}
			else
			{
				if (_stateMachine == null && _stateMachineBase == null)
				{
					Draw(null);
				}
				else if (_stateMachineBase != null)
				{
					Draw(_stateMachineBase);
				}
			}
		}
	}
}