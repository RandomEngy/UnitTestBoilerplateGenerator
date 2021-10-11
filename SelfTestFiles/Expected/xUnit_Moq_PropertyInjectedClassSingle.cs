using Moq;
using System;
using UnitBoilerplate.Sandbox.Classes.Cases;
using Xunit;

namespace UnitTestBoilerplate.SelfTest.Cases
{
	public class PropertyInjectedClassSingleTests
	{
		private MockRepository mockRepository;



		public PropertyInjectedClassSingleTests()
		{
			this.mockRepository = new MockRepository(MockBehavior.Strict);


		}

		private PropertyInjectedClassSingle CreatePropertyInjectedClassSingle()
		{
			return new PropertyInjectedClassSingle();
		}

		[Fact]
		public void TestMethod1()
		{
			// Arrange
			var propertyInjectedClassSingle = this.CreatePropertyInjectedClassSingle();

			// Act


			// Assert
			Assert.True(false);
			this.mockRepository.VerifyAll();
		}
	}
}
