using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using UnitBoilerplate.Sandbox.Classes;
using UnitBoilerplate.Sandbox.Classes.Cases;

namespace UnitTestBoilerplate.SelfTest.Cases
{
	[TestClass]
	public class FileScopedNamespaceTests
	{
		private ISomeInterface stubSomeInterface;

		[TestInitialize]
		public void TestInitialize()
		{
			this.stubSomeInterface = MockRepository.GenerateStub<ISomeInterface>();
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
