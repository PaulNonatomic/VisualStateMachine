﻿using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using VisualStateMachine.Attributes;
using VisualStateMachine.Editor.Ports;
using VisualStateMachine.Editor.Services;
using VisualStateMachine.Editor.Utils;
using VisualStateMachine.States;

namespace VisualStateMachine.Editor.Nodes
{
	public class StateNodeView : Node
	{
		public StateNode Data;
		public StateMachine StateMachine;

		public StateNodeView(StateNode stateNode, string stateTitle, string stateName, StateMachineGraphView graphView)
		{
			Data = stateNode;
			this.title = ProcessTitle(stateTitle);
			this.name = stateNode.Id;
			
			if(stateNode.State is not EntryState) StateMachineNodeFactory.CreateInputPort(this, graphView);
			StateMachineNodeFactory.CreateOutputPorts(this, graphView);
			
			this.RefreshPorts();
			this.RefreshExpandedState();
			this.SetPosition(new Rect(stateNode.Position, Vector2.one));
			
			var container = new VisualElement();
			container.name = "title-container";
			
			var title = this.Query<VisualElement>("title").First();
			var titleLabel = title.Query<VisualElement>("title-label").First();
			var titleButton = title.Query<VisualElement>("title-button-container").First();
			title.Remove(titleButton);
			
			var icon = new Image();
			icon.name = "title-icon";
			icon.scaleMode = ScaleMode.ScaleToFit;
			
			title.Add(container);
			container.Add(icon);
			container.Add(titleLabel);

			var progressBar = new ProgressBar();
			progressBar.name = "progress-bar";
			title.Add(progressBar);
			
			var contents = this.Query<VisualElement>("contents").First();
			var propertyContainer = new VisualElement()
			{
				name = "property-container"
			};
			
			propertyContainer.AddToClassList("full-width");
			contents.Insert(0, propertyContainer);

			if (stateNode.State != null)
			{
				var stateInspector = StateMachineNodeFactory.CreateUIElementInspector(stateNode.State);
				stateInspector.name = "state-inspector";
				propertyContainer.Add(stateInspector);
				
				if (stateInspector.childCount > 0)
				{
					propertyContainer.AddToClassList("has-properties");
				}
				
				//adjust the node size
				var stateType = stateNode.State.GetType();
				var nodeWidth = stateType.GetCustomAttribute<NodeWidthAttribute>();
				if (nodeWidth != null)
				{
					propertyContainer.style.width = nodeWidth.Width;
				}
			}
		}

		private string ProcessTitle(string title)
		{
			title = StringUtils.RemoveStateSuffix(title);
			title = StringUtils.ApplyEllipsis(title, 32);

			return title;
		}
		
		public virtual void Update()
		{
			if (Data == null) return;
			
			DrawEntryPoint();
			DrawCustomNodeColor();
			DrawActiveNode();
			SetCustomLabelText();
			CreateCustomIcon();
		}
		
		private void CreateCustomIcon()
		{
			var stateType = Data.State.GetType();
			var image = titleContainer.Q<Image>("title-icon");
			
			var nodeIcon = AttributeUtils.GetInheritedCustomAttribute<NodeIconAttribute>(stateType);
			if (nodeIcon == null)
			{
				if(image != null) image.parent.Remove(image);
				return;
			}
			
			var icon = ImageService.FetchTexture(nodeIcon.Path, nodeIcon.Source);
			if (icon == null)
			{
				if(image != null) image.parent.Remove(image);
				return;
			}
			
			if (image == null) return;
			image.image = icon;
		}

		private void SetCustomLabelText()
		{
			var stateType = Data.State.GetType();
			
			var nodeLabel = stateType.GetCustomAttribute<NodeLabelAttribute>();
			if (nodeLabel == null) return;
			
			titleContainer.Q<Label>().text = nodeLabel.Text;
		}

		private void DrawCustomNodeColor()
		{
			var stateType = Data.State.GetType();
			
			var nodeColor = AttributeUtils.GetInheritedCustomAttribute<NodeColorAttribute>(stateType);
			var color = ColorUtils.HexToColor(NodeColor.Grey);
			
			if (nodeColor != null)
			{
				color = ColorUtils.HexToColor(nodeColor.HexColor);
			}

			color.a = 0.8f;
			
			titleContainer.style.backgroundColor = color;

			var selectionBorder = this.Q("selection-border");
			selectionBorder.style.borderBottomColor = color;
			selectionBorder.style.borderTopColor = color;
			selectionBorder.style.borderLeftColor = color;
			selectionBorder.style.borderRightColor = color;
		}

		private void DrawActiveNode()
		{
			if (!Application.isPlaying) return;
			
			if (Data.IsActive)
			{
				AddToClassList("active-node");
				var progressBar = this.Query<ProgressBar>().First();
				if(progressBar != null) progressBar.value = (Time.time % 1f) * 100f;
			}
			else
			{
				RemoveFromClassList("active-node");
			}
		}

		private void DrawEntryPoint()
		{
			if (Data.IsEntryNode)
			{
				AddToClassList("entry-node");
			}
			else
			{
				RemoveFromClassList("entry-node");
			}
		}
		
		public override Port InstantiatePort(
			Orientation orientation,
			Direction direction,
			Port.Capacity capacity,
			System.Type type)
		{
			return StatePort.Create<Edge>(orientation, direction, capacity, type);
		}
	}
}