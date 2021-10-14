using System;
using System.Collections.Generic;
using System.Linq;
using EnvDTE80;
using UnitTestBoilerplate.Model;
using UnitTestBoilerplate.Utilities;

namespace UnitTestBoilerplate.Services.ProjectSelection
{
	internal class TemplateBasedStrategy : ITestProjectSelectionStrategy
	{
		private readonly IBoilerplateSettings settings;
		private readonly DTE2 dte;

		public TemplateBasedStrategy(IBoilerplateSettings settings, DTE2 dte)
		{
			this.settings = settings;
			this.dte = dte;
		}

		public TestProject Apply(IEnumerable<TestProject> testProjects)
		{
			var format = this.settings.TestProjectNameFormat;
			if (string.IsNullOrWhiteSpace(format))
			{
				return null;
			}

			IList<string> selectedProjectNames = SolutionUtilities
				.GetSelectedFiles(this.dte)
				.Select(item => item.ProjectName)
				.ToList();

			if (selectedProjectNames.Count != 1)
			{
				return null; // we cannot reliably predict, which test project is correct, when multiple source projects are selected
			}

			var expectedTestProject = format.Replace("$ProjectName$", selectedProjectNames.First());
			return testProjects
				?.FirstOrDefault(testProject =>
					string.Equals(
						testProject.Name, 
						expectedTestProject, 
						StringComparison.CurrentCultureIgnoreCase));
		}
	}
}
