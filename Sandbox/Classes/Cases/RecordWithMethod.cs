using System;
using System.Collections.Generic;
using System.Text;

namespace UnitBoilerplate.Sandbox.Classes.Cases
{
	public record RecordWithMethod
	{
		public int AddStuff(int a, int b)
		{
			return a + b;
		}
	}
}
