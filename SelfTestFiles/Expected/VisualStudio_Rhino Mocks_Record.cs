using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using UnitBoilerplate.Sandbox.Classes.Cases;

namespace UnitTestBoilerplate.SelfTest.Cases
{
	[TestClass]
	public class RecordTests
	{


		[TestInitialize]
		public void TestInitialize()
		{

		}

		private Record CreateRecord()
		{
			return new Record();
		}

		[TestMethod]
		public void TestMethod1()
		{
			// Arrange
			var record = this.CreateRecord();

			// Act


			// Assert
			Assert.Fail();
		}
	}
}
