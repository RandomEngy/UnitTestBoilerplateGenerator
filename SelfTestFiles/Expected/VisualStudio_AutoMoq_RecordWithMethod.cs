using AutoMoq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
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
			var mocker = new AutoMoqer();
			var recordWithMethod = mocker.Create<RecordWithMethod>();
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
