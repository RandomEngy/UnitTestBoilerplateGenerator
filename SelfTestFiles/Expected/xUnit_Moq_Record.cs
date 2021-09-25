using Moq;
using System;
using UnitBoilerplate.Sandbox.Classes.Cases;
using Xunit;

namespace UnitTestBoilerplate.SelfTest.Cases
{
	public class RecordTests
	{
		private MockRepository mockRepository;



		public RecordTests()
		{
			this.mockRepository = new MockRepository(MockBehavior.Strict);


		}

		private Record CreateRecord()
		{
			return new Record();
		}

		[Fact]
		public void TestMethod1()
		{
			// Arrange
			var record = this.CreateRecord();

			// Act


			// Assert
			Assert.True(false);
			this.mockRepository.VerifyAll();
		}
	}
}
