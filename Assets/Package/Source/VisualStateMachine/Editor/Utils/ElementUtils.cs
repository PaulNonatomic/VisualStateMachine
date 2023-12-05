﻿using System.Linq;
using UnityEngine.UIElements;

namespace VisualStateMachine.Editor.Utils
{
	public class ElementUtils
	{
		public static bool BothContainClass(VisualElement elementA, VisualElement elementB, string className)
		{
			var elementAHasClass = elementA.GetClasses().Contains(className);
			var elementBHasClass = elementB.GetClasses().Contains(className);
			return elementAHasClass == elementBHasClass;
		}
	}
}