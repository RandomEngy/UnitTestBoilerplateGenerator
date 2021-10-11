using Moq;
using System;
using UnitBoilerplate.Sandbox.Classes.Cases;
using Xunit;

namespace UnitTestBoilerplate.SelfTest.Cases
{
	public class RecordWithMethodTests
	{
		private MockRepository mockRepository;



		public RecordWithMethodTests()
		{
			this.mockRepository = new MockRepository(MockBehavior.Strict);


		}

		private RecordWithMethod CreateRecordWithMethod()
		{
			return new RecordWithMethod();
		}

		[Fact]
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
			Assert.True(false);
			this.mockRepository.VerifyAll();
		}
	}
}
