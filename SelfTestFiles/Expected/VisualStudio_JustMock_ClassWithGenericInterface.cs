using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Telerik.JustMock;
using UnitBoilerplate.Sandbox.Classes;
using UnitBoilerplate.Sandbox.Classes.Cases;

namespace UnitTestBoilerplate.SelfTest.Cases
{
	[TestClass]
	public class ClassWithGenericInterfaceTests
	{
		private IGenericInterface<bool> mockGenericInterfaceBool;
		private IGenericInterface<List<string>> mockGenericInterfaceListString;
		private ISomeInterface mockSomeInterface;

		[TestInitialize]
		public void TestInitialize()
		{
			this.mockGenericInterfaceBool = Mock.Create<IGenericInterface<bool>>();
			this.mockGenericInterfaceListString = Mock.Create<IGenericInterface<List<string>>>();
			this.mockSomeInterface = Mock.Create<ISomeInterface>();
		}

		private ClassWithGenericInterface CreateClassWithGenericInterface()
		{
			return new ClassWithGenericInterface(
				this.mockGenericInterfaceBool,
				this.mockGenericInterfaceListString,
				this.mockSomeInterface);
		}

		[TestMethod]
		public void TestMethod1()
		{
			// Arrange
			var classWithGenericInterface = this.CreateClassWithGenericInterface();

			// Act


			// Assert
			Assert.Fail();
		}
	}
}
