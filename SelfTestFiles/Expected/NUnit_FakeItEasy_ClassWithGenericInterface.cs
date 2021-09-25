using FakeItEasy;
using NUnit.Framework;
using System.Collections.Generic;
using UnitBoilerplate.Sandbox.Classes;
using UnitBoilerplate.Sandbox.Classes.Cases;

namespace UnitTestBoilerplate.SelfTest.Cases
{
	[TestFixture]
	public class ClassWithGenericInterfaceTests
	{
		private IGenericInterface<bool> fakeGenericInterfaceBool;
		private IGenericInterface<List<string>> fakeGenericInterfaceListString;
		private ISomeInterface fakeSomeInterface;

		[SetUp]
		public void SetUp()
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
