using System;
using System.Collections.Generic;
using Telerik.JustMock;
using UnitBoilerplate.Sandbox.Classes;
using UnitBoilerplate.Sandbox.Classes.Cases;
using Xunit;

namespace UnitTestBoilerplate.SelfTest.Cases
{
	public class ClassWithGenericInterfaceTests
	{
		private IGenericInterface<bool> mockGenericInterfaceBool;
		private IGenericInterface<List<string>> mockGenericInterfaceListString;
		private ISomeInterface mockSomeInterface;

		public ClassWithGenericInterfaceTests()
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

		[Fact]
		public void TestMethod1()
		{
			// Arrange
			var classWithGenericInterface = this.CreateClassWithGenericInterface();

			// Act


			// Assert
			Assert.True(false);
		}
	}
}
