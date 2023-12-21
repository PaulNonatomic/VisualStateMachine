using System;
using System.Collections.Generic;
using VisualStateMachine.Attributes;

namespace ExampleOne.States
{
	public class StoreMenuState : MenuState
	{
		[Transition("Back")] 
		public event Action OnBack;

		[Transition("Purchase Item")] 
		public event Action OnPurchaseItem;

		protected override Dictionary<string, Action> MapActions()
		{
			return new Dictionary<string, Action>
			{
				{ "Back", () => OnBack?.Invoke() }
			};
		}
		
		protected override void HandleMenuSelected(string buttonId)
		{
			if (buttonId.Contains("Item"))
			{
				PurchaseItem(buttonId);
				return;
			}
			
			base.HandleMenuSelected(buttonId);
		}

		private void PurchaseItem(string itemId)
		{
			StateMachineCore.Root.GetComponent<MenuStateMachineData>().PuchaseItemId = itemId;
			OnPurchaseItem?.Invoke();
		}
	}
}