using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitBoilerplate.Sandbox.Classes.Cases;

namespace UnitTestBoilerplate.SelfTest.Cases
{
	[TestClass]
	public class PropertyInjectedClassSingleTests
	{


		[TestInitialize]
		public void TestInitialize()
		{

		}

		private PropertyInjectedClassSingle CreatePropertyInjectedClassSingle()
		{
			return new PropertyInjectedClassSingle();
		}

		[TestMethod]
		public void TestMethod1()
		{
			// Arrange
			var propertyInjectedClassSingle = this.CreatePropertyInjectedClassSingle();

			// Act


			// Assert
			Assert.Fail();
		}
	}
}
