using Rhino.Mocks;
using System;
using System.Collections.Generic;
using UnitBoilerplate.Sandbox.Classes;
using UnitBoilerplate.Sandbox.Classes.Cases;
using Xunit;

namespace UnitTestBoilerplate.SelfTest.Cases
{
	public class ClassWithGenericInterfaceTests
	{
		private IGenericInterface<bool> stubGenericInterfaceBool;
		private IGenericInterface<List<string>> stubGenericInterfaceListString;
		private ISomeInterface stubSomeInterface;

		public ClassWithGenericInterfaceTests()
		{
			this.stubGenericInterfaceBool = MockRepository.GenerateStub<IGenericInterface<bool>>();
			this.stubGenericInterfaceListString = MockRepository.GenerateStub<IGenericInterface<List<string>>>();
			this.stubSomeInterface = MockRepository.GenerateStub<ISomeInterface>();
		}

		private ClassWithGenericInterface CreateClassWithGenericInterface()
		{
			return new ClassWithGenericInterface(
				this.stubGenericInterfaceBool,
				this.stubGenericInterfaceListString,
				this.stubSomeInterface);
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
