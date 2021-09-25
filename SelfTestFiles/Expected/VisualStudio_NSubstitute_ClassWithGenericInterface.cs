using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Collections.Generic;
using UnitBoilerplate.Sandbox.Classes;
using UnitBoilerplate.Sandbox.Classes.Cases;

namespace UnitTestBoilerplate.SelfTest.Cases
{
	[TestClass]
	public class ClassWithGenericInterfaceTests
	{
		private IGenericInterface<bool> subGenericInterfaceBool;
		private IGenericInterface<List<string>> subGenericInterfaceListString;
		private ISomeInterface subSomeInterface;

		[TestInitialize]
		public void TestInitialize()
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
