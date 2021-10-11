using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using UnitBoilerplate.Sandbox.Classes;
using UnitBoilerplate.Sandbox.Classes.Cases;

namespace UnitTestBoilerplate.SelfTest.Cases
{
	[TestClass]
	public class ClassWithGenericInterfaceTests
	{
		private MockRepository mockRepository;

		private Mock<IGenericInterface<bool>> mockGenericInterfaceBool;
		private Mock<IGenericInterface<List<string>>> mockGenericInterfaceListString;
		private Mock<ISomeInterface> mockSomeInterface;

		[TestInitialize]
		public void TestInitialize()
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

		[TestMethod]
		public void TestMethod1()
		{
			// Arrange
			var classWithGenericInterface = this.CreateClassWithGenericInterface();

			// Act


			// Assert
			Assert.Fail();
			this.mockRepository.VerifyAll();
		}
	}
}
