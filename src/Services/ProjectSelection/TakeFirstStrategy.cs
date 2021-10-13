using System.Collections.Generic;
using System.Linq;
using UnitTestBoilerplate.Model;

namespace UnitTestBoilerplate.Services.ProjectSelection
{
	internal class TakeFirstStrategy : ITestProjectSelectionStrategy
	{
		public TestProject Apply(IEnumerable<TestProject> testProjects) => testProjects?.FirstOrDefault();
	}
}
