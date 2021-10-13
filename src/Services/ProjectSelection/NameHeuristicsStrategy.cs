using System.Collections.Generic;
using System.Linq;
using UnitTestBoilerplate.Model;

namespace UnitTestBoilerplate.Services.ProjectSelection
{
	internal class NameHeuristicsStrategy : ITestProjectSelectionStrategy
	{
		public TestProject Apply(IEnumerable<TestProject> testProjects)
		{
			return testProjects?.FirstOrDefault(project => 
				project.Name.ToLowerInvariant().Contains("test"));
		}
	}
}
