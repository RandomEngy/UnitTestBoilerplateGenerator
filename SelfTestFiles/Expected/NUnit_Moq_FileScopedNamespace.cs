using Moq;
using NUnit.Framework;
using UnitBoilerplate.Sandbox.Classes;
using UnitBoilerplate.Sandbox.Classes.Cases;

namespace UnitTestBoilerplate.SelfTest.Cases
{
	[TestFixture]
	public class FileScopedNamespaceTests
	{
		private MockRepository mockRepository;

		private Mock<ISomeInterface> mockSomeInterface;

		[SetUp]
		public void SetUp()
		{
			this.mockRepository = new MockRepository(MockBehavior.Strict);

			this.mockSomeInterface = this.mockRepository.Create<ISomeInterface>();
		}

		private FileScopedNamespace CreateFileScopedNamespace()
		{
			return new FileScopedNamespace(
				this.mockSomeInterface.Object);
		}

		[Test]
		public void TestMethod1()
		{
			// Arrange
			var fileScopedNamespace = this.CreateFileScopedNamespace();

			// Act


			// Assert
			Assert.Fail();
			this.mockRepository.VerifyAll();
		}
	}
}
