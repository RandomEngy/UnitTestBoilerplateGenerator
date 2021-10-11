using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using UnitBoilerplate.Sandbox.Classes;
using UnitBoilerplate.Sandbox.Classes.Cases;

namespace UnitTestBoilerplate.SelfTest.Cases
{
	[TestClass]
	public class ClassWithGenericInterfaceTests
	{
		private IGenericInterface<bool> fakeGenericInterfaceBool;
		private IGenericInterface<List<string>> fakeGenericInterfaceListString;
		private ISomeInterface fakeSomeInterface;

		[TestInitialize]
		public void TestInitialize()
		{
			this.fakeGenericInterfaceBool = A.Fake<IGenericInterface<bool>>();
			this.fakeGenericInterfaceListString = A.Fake<IGenericInterface<List<string>>>();
			this.fakeSomeInterface = A.Fake<ISomeInterface>();
		}

		private ClassWithGenericInterface CreateClassWithGenericInterface()
		{
			return new ClassWithGenericInterface(
				this.fakeGenericInterfaceBool,
				this.fakeGenericInterfaceListString,
				this.fakeSomeInterface);
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
