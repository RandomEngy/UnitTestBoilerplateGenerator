using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using System;
using UnitBoilerplate.Sandbox.Classes.Cases;

namespace UnitTestBoilerplate.SelfTest.Cases
{
	[TestClass]
	public class RecordWithMethodTests
	{


		[TestInitialize]
		public void TestInitialize()
		{

		}

		private RecordWithMethod CreateRecordWithMethod()
		{
			return new RecordWithMethod();
		}

		[TestMethod]
		public void AddStuff_StateUnderTest_ExpectedBehavior()
		{
			// Arrange
			var recordWithMethod = this.CreateRecordWithMethod();
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
