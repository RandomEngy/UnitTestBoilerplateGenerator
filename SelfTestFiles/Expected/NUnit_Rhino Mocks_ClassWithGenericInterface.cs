using NUnit.Framework;
using Rhino.Mocks;
using System.Collections.Generic;
using UnitBoilerplate.Sandbox.Classes;
using UnitBoilerplate.Sandbox.Classes.Cases;

namespace UnitTestBoilerplate.SelfTest.Cases
{
	[TestFixture]
	public class ClassWithGenericInterfaceTests
	{
		private IGenericInterface<bool> stubGenericInterfaceBool;
		private IGenericInterface<List<string>> stubGenericInterfaceListString;
		private ISomeInterface stubSomeInterface;

		[SetUp]
		public void SetUp()
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

		[Test]
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
