using FakeItEasy;
using System;
using System.Collections.Generic;
using UnitBoilerplate.Sandbox.Classes;
using UnitBoilerplate.Sandbox.Classes.Cases;
using Xunit;

namespace UnitTestBoilerplate.SelfTest.Cases
{
	public class ClassWithGenericInterfaceTests
	{
		private IGenericInterface<bool> fakeGenericInterfaceBool;
		private IGenericInterface<List<string>> fakeGenericInterfaceListString;
		private ISomeInterface fakeSomeInterface;

		public ClassWithGenericInterfaceTests()
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
