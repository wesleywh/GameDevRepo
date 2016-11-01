using System;

namespace System 
{
	public static class StringOperators {
		public static bool IsNullOrWhiteSpace(string value)
		{
			if (value != null)
			{
				for (int i = 0; i < value.Length; i++)
				{
					if (!char.IsWhiteSpace(value[i]))
					{
						return false;
					}
				}
			}
			return true;
		}
	}
}