﻿using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace VisualStateMachine.Editor.Utils
{
	public class StringUtils
	{
		public static string RemoveStateSuffix(string title)
		{
			return !title.EndsWith("State") ? title : title[..^5];
		}
		
		public static string ApplyEllipsis(string text, int maxLength)
		{
			return text.Length <= maxLength ? text : text[..(maxLength - 3)] + "...";
		}
		
		public static string PascalCaseToTitleCase(string pascalCaseString)
		{
			var withSpaces = Regex.Replace(pascalCaseString, "(\\B[A-Z])", " $1");
			var textInfo = CultureInfo.CurrentCulture.TextInfo;
			
			return textInfo.ToTitleCase(withSpaces);
		}
		
		public static int FindLevenshteinDistance(string s, string t)
		{
			int n = s.Length;
			int m = t.Length;
			int[,] d = new int[n + 1, m + 1];

			if (n == 0)
			{
				return m;
			}

			if (m == 0)
			{
				return n;
			}

			for (int i = 0; i <= n; d[i, 0] = i++) { }
			for (int j = 0; j <= m; d[0, j] = j++) { }

			for (int i = 1; i <= n; i++)
			{
				for (int j = 1; j <= m; j++)
				{
					int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

					d[i, j] = Math.Min(
						Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
						d[i - 1, j - 1] + cost);
				}
			}

			return d[n, m];
		}
	}
}