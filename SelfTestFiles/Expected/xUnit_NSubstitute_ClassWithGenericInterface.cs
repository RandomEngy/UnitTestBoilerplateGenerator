using NSubstitute;
using System;
using System.Collections.Generic;
using UnitBoilerplate.Sandbox.Classes;
using UnitBoilerplate.Sandbox.Classes.Cases;
using Xunit;

namespace UnitTestBoilerplate.SelfTest.Cases
{
	public class ClassWithGenericInterfaceTests
	{
		private IGenericInterface<bool> subGenericInterfaceBool;
		private IGenericInterface<List<string>> subGenericInterfaceListString;
		private ISomeInterface subSomeInterface;

		public ClassWithGenericInterfaceTests()
		{
			this.subGenericInterfaceBool = Substitute.For<IGenericInterface<bool>>();
			this.subGenericInterfaceListString = Substitute.For<IGenericInterface<List<string>>>();
			this.subSomeInterface = Substitute.For<ISomeInterface>();
		}

		private ClassWithGenericInterface CreateClassWithGenericInterface()
		{
			return new ClassWithGenericInterface(
				this.subGenericInterfaceBool,
				this.subGenericInterfaceListString,
				this.subSomeInterface);
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
