using System;
using System.Text;

namespace UnitTestBoilerplate.Utilities
{
	public static class StringUtilities
	{
		#region ReplaceTokens

		public static string ReplaceTokens(string template, Action<string, int, StringBuilder> tokenReplacementAction)
		{
			var builder = new StringBuilder();

			for (int i = 0; i < template.Length; i++)
			{
				char c = template[i];
				if (c == '$')
				{
					int endIndex = -1;
					for (int j = i + 1; j < template.Length; j++)
					{
						if (template[j] == '$')
						{
							endIndex = j;
							break;
						}
					}

					if (endIndex < 0)
					{
						// We couldn't find the end index for the replacement property name. Continue.
						builder.Append(c);
					}
					else
					{
						// Calculate values on demand from switch statement. Some are preset values, some need a bit of calc like base name,
						// some are dependent on the test framework (attributes), some need to pull down other templates and loop through mock fields
						string tokenText = template.Substring(i + 1, endIndex - i - 1);

						ProcessTokenWithAnyModifiers(tokenText, tokenReplacementAction, i, builder);
						i = endIndex;
					}
				}
				else
				{
					builder.Append(c);
				}
			}

			return builder.ToString();
		}

		#region Process Functions

		private static void ProcessTokenWithAnyModifiers(string tokenText, Action<string, int, StringBuilder> tokenReplacementAction, int propertyIndex, StringBuilder builder)
		{
			string modifier;
			SplitToken(ref tokenText, out modifier);

			var tokenValueBuilder = new StringBuilder();
			tokenReplacementAction(tokenText, propertyIndex, tokenValueBuilder);
			string tokenValue = tokenValueBuilder.ToString();

			tokenValue = ProcessModifiers(tokenValue, modifier);

			builder.Append(tokenValue);
		}

		private static string ProcessModifiers(string tokenValue, string myModifier)
		{
			if(string.IsNullOrEmpty(myModifier))
			{
				return tokenValue;
			}

			string nextModifier;
			string arg;
			SplitToken(ref myModifier, out nextModifier);
			SplitModifierArg(ref myModifier, out arg);

			switch (myModifier)
			{
				case "CamelCase":
					tokenValue = RunCamelCaseReplacement(tokenValue);
					break;
				case "NewLineIfPopulated":
					tokenValue = RunNewLineIfPopulatedReplacement(tokenValue);
					break;
				case "Remove":
					tokenValue = RunRemoveReplacement(tokenValue, arg);
					break;
				case "RemoveGeneric":
					tokenValue = RunRemoveGenericReplacement(tokenValue, arg);
					break;
				case "LowerCase":
					tokenValue = RunLowerCaseReplacement(tokenValue);
					break;
				case "AddOnParams":
					tokenValue = RunAddOnParamsReplacement(tokenValue);
					break;
				case "SingleLine":
					tokenValue = RunSingleLineReplacement(tokenValue);
					break;
				case "StdHttpFuncName":
					tokenValue = RunStdHttpFuncNameReplacement(tokenValue);
					break;
				default:
					// Ignore the modifier
					break;
			}

			return ProcessModifiers(tokenValue, nextModifier);
		}

		#endregion Process Functions

		#region Split Functions

		private static void SplitToken(ref string tokenText, out string modifier)
		{
			int periodIndex = tokenText.IndexOf(".", StringComparison.Ordinal);
			if (periodIndex <= 0)
			{
				modifier = string.Empty;
			}
			else
			{
				modifier = tokenText.Substring(periodIndex + 1);
				tokenText = tokenText.Substring(0, periodIndex);
			}
		}

		private static void SplitModifierArg(ref string modifier, out string arg)
		{
			int parenthesisIndex = modifier.IndexOf("(", StringComparison.Ordinal);
			if (parenthesisIndex <= 0)
			{
				arg = string.Empty;
			}
			else
			{
				arg = modifier.Substring(parenthesisIndex + 1, modifier.Length - parenthesisIndex - 2);
				modifier = modifier.Substring(0, parenthesisIndex);
			}
		}

		#endregion Split Functions

		#region Modifier Functions

		private static string RunCamelCaseReplacement(string tokenValue)
		{
			return tokenValue.Substring(0, 1).ToLowerInvariant() + tokenValue.Substring(1);
		}

		private static string RunNewLineIfPopulatedReplacement(string tokenValue)
		{
			var tokenValueBuilder = new StringBuilder(tokenValue);

			if (!string.IsNullOrEmpty(tokenValue))
			{
				tokenValueBuilder.AppendLine();
			}
			return tokenValueBuilder.ToString();
		}

		private static string RunRemoveReplacement(string tokenValue, string textToRemove)
		{
			return tokenValue.Replace(textToRemove, "");
		}

		private static string RunRemoveGenericReplacement(string tokenValue, string genericToRemove)
		{
			int genericStartPos;
			if (string.IsNullOrEmpty(tokenValue) || (genericStartPos = tokenValue.IndexOf($"{genericToRemove}<", StringComparison.Ordinal)) < 0)
			{
				return tokenValue; //Generic isn't in this token
			}

			//Find appropriate closing bracket to remove
			int genericMiddlePos = genericStartPos + genericToRemove.Length + 1;
			int unpairedOpenArrows = 1;
			int genericEndPos = -1;
			for (int charInMiddle = genericMiddlePos; charInMiddle < tokenValue.Length; ++charInMiddle)
			{
				switch (tokenValue[charInMiddle])
				{
					case '<': ++unpairedOpenArrows; break;
					case '>': --unpairedOpenArrows; break;
				}
				if (unpairedOpenArrows == 0)
				{
					genericEndPos = charInMiddle;
					break;
				}
			}

			if (genericEndPos == -1)
			{
				return tokenValue; //Unclosed Generic.  Can't remove.
			}

			var bufferWOutGeneric = new StringBuilder();
			bufferWOutGeneric.Append(tokenValue.Substring(0, genericStartPos));
			bufferWOutGeneric.Append(tokenValue.Substring(genericMiddlePos, genericEndPos - genericMiddlePos));
			bufferWOutGeneric.Append(tokenValue.Substring(genericEndPos+1));

			return bufferWOutGeneric.ToString();
		}

		private static string RunLowerCaseReplacement(string tokenValue)
		{
			tokenValue = tokenValue.ToLower();
			return tokenValue;
		}

		private static string RunAddOnParamsReplacement(string tokenValue)
		{
			if (!string.IsNullOrEmpty(tokenValue))
			{
				return $",{tokenValue}";
			}

			return tokenValue;
		}

		private static string RunSingleLineReplacement(string tokenValue)
		{
			return tokenValue.Replace(Environment.NewLine, "");
		}

		private static string RunStdHttpFuncNameReplacement(string tokenValue)
		{
			switch (tokenValue)
			{
				case "Get":
					return "GetAsync";
				case "Post":
					return "PostAsJsonAsync";
			}
			return tokenValue;
		}

		#endregion Modifier Functions

		#endregion ReplaceTokens

		public static string GetShortNameFromFullTypeName(string fullName)
		{
			int lastDotIndex = fullName.LastIndexOf('.');
			return lastDotIndex > 0 ? fullName.Substring(lastDotIndex + 1) : fullName;
		}
	}
}
