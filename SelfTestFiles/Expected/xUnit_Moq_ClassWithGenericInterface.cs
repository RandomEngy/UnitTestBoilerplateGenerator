using Moq;
using System;
using System.Collections.Generic;
using UnitBoilerplate.Sandbox.Classes;
using UnitBoilerplate.Sandbox.Classes.Cases;
using Xunit;

namespace UnitTestBoilerplate.SelfTest.Cases
{
	public class ClassWithGenericInterfaceTests
	{
		private MockRepository mockRepository;

		private Mock<IGenericInterface<bool>> mockGenericInterfaceBool;
		private Mock<IGenericInterface<List<string>>> mockGenericInterfaceListString;
		private Mock<ISomeInterface> mockSomeInterface;

		public ClassWithGenericInterfaceTests()
		{
			this.mockRepository = new MockRepository(MockBehavior.Strict);

			this.mockGenericInterfaceBool = this.mockRepository.Create<IGenericInterface<bool>>();
			this.mockGenericInterfaceListString = this.mockRepository.Create<IGenericInterface<List<string>>>();
			this.mockSomeInterface = this.mockRepository.Create<ISomeInterface>();
		}

		private ClassWithGenericInterface CreateClassWithGenericInterface()
		{
			return new ClassWithGenericInterface(
				this.mockGenericInterfaceBool.Object,
				this.mockGenericInterfaceListString.Object,
				this.mockSomeInterface.Object);
		}

		[Fact]
		public void TestMethod1()
		{
			// Arrange
			var classWithGenericInterface = this.CreateClassWithGenericInterface();

			// Act


			// Assert
			Assert.True(false);
			this.mockRepository.VerifyAll();
		}
	}
}
