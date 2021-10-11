using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;
using UnitBoilerplate.Sandbox.Classes;
using UnitBoilerplate.Sandbox.Classes.Cases;

namespace UnitTestBoilerplate.SelfTest.Cases
{
	[TestClass]
	public class FileScopedNamespaceTests
	{
		private ISomeInterface mockSomeInterface;

		[TestInitialize]
		public void TestInitialize()
		{
			this.mockSomeInterface = Mock.Create<ISomeInterface>();
		}

		private FileScopedNamespace CreateFileScopedNamespace()
		{
			return new FileScopedNamespace(
				this.mockSomeInterface);
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
