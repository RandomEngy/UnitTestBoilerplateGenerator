using System.Collections.Generic;
using UnitTestBoilerplate.Model;

namespace UnitTestBoilerplate.Services.ProjectSelection
{
	internal interface ITestProjectSelectionStrategy
	{
		TestProject Apply(IEnumerable<TestProject> testProjects);
	}
}