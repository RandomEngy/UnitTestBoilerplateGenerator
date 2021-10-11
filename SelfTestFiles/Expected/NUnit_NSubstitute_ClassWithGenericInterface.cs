using NSubstitute;
using NUnit.Framework;
using System.Collections.Generic;
using UnitBoilerplate.Sandbox.Classes;
using UnitBoilerplate.Sandbox.Classes.Cases;

namespace UnitTestBoilerplate.SelfTest.Cases
{
	[TestFixture]
	public class ClassWithGenericInterfaceTests
	{
		private IGenericInterface<bool> subGenericInterfaceBool;
		private IGenericInterface<List<string>> subGenericInterfaceListString;
		private ISomeInterface subSomeInterface;

		[SetUp]
		public void SetUp()
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
