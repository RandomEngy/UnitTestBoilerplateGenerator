using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using UnitBoilerplate.Sandbox.Classes.Cases;

namespace UnitTestBoilerplate.SelfTest.Cases
{
	[TestClass]
	public class RecordWithMethodTests
	{
		private MockRepository mockRepository;



		[TestInitialize]
		public void TestInitialize()
		{
			this.mockRepository = new MockRepository(MockBehavior.Strict);


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
			this.mockRepository.VerifyAll();
		}
	}
}
