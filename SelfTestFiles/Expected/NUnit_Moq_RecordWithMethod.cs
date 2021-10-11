using Moq;
using NUnit.Framework;
using System;
using UnitBoilerplate.Sandbox.Classes.Cases;

namespace UnitTestBoilerplate.SelfTest.Cases
{
	[TestFixture]
	public class RecordWithMethodTests
	{
		private MockRepository mockRepository;



		[SetUp]
		public void SetUp()
		{
			this.mockRepository = new MockRepository(MockBehavior.Strict);


		}

		private RecordWithMethod CreateRecordWithMethod()
		{
			return new RecordWithMethod();
		}

		[Test]
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
