using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using UnitBoilerplate.Sandbox.Classes.Cases;

namespace UnitTestBoilerplate.SelfTest.Cases
{
	[TestClass]
	public class RecordWithMethodTests
	{
		[TestMethod]
		public void AddStuff_StateUnderTest_ExpectedBehavior()
		{
			// Arrange
			var recordWithMethod = new RecordWithMethod();
			int a = 0;
			int b = 0;

			// Act
			var result = recordWithMethod.AddStuff(
				a,
				b);

			// Assert
			Assert.Fail();
		}
	}
}
