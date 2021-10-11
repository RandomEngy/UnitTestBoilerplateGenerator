using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitBoilerplate.Sandbox.Classes.Cases;

namespace UnitTestBoilerplate.SelfTest.Cases
{
	[TestClass]
	public class DerivedPropertyInjectedClassTests
	{


		[TestInitialize]
		public void TestInitialize()
		{

		}

		private DerivedPropertyInjectedClass CreateDerivedPropertyInjectedClass()
		{
			return new DerivedPropertyInjectedClass();
		}

		[TestMethod]
		public void TestMethod1()
		{
			// Arrange
			var derivedPropertyInjectedClass = this.CreateDerivedPropertyInjectedClass();

			// Act


			// Assert
			Assert.Fail();
		}
	}
}
