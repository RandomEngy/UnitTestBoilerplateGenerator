using Moq;
using System;
using UnitBoilerplate.Sandbox.Classes.Cases;
using Xunit;

namespace UnitTestBoilerplate.SelfTest.Cases
{
	public class PropertyInjectedClassMultipleTests
	{
		private MockRepository mockRepository;



		public PropertyInjectedClassMultipleTests()
		{
			this.mockRepository = new MockRepository(MockBehavior.Strict);


		}

		private PropertyInjectedClassMultiple CreatePropertyInjectedClassMultiple()
		{
			return new PropertyInjectedClassMultiple();
		}

		[Fact]
		public void TestMethod1()
		{
			// Arrange
			var propertyInjectedClassMultiple = this.CreatePropertyInjectedClassMultiple();

			// Act


			// Assert
			Assert.True(false);
			this.mockRepository.VerifyAll();
		}
	}
}
