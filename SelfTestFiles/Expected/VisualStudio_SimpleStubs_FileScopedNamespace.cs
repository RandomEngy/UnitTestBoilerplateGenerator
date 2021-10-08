using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitBoilerplate.Sandbox.Classes;
using UnitBoilerplate.Sandbox.Classes.Cases;

namespace UnitTestBoilerplate.SelfTest.Cases
{
	[TestClass]
	public class FileScopedNamespaceTests
	{
		private StubISomeInterface stubSomeInterface;

		[TestInitialize]
		public void TestInitialize()
		{
			this.stubSomeInterface = new StubISomeInterface();
		}

		private FileScopedNamespace CreateFileScopedNamespace()
		{
			return new FileScopedNamespace(
				this.stubSomeInterface);
		}

		[TestMethod]
		public void TestMethod1()
		{
			// Arrange
			var fileScopedNamespace = this.CreateFileScopedNamespace();

			// Act


			// Assert
			Assert.Fail();
		}
	}
}
