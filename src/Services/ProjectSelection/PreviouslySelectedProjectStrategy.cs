using System;
using System.Collections.Generic;
using System.Linq;
using EnvDTE80;
using UnitTestBoilerplate.Model;

namespace UnitTestBoilerplate.Services.ProjectSelection
{
	internal class PreviouslySelectedProjectStrategy : ITestProjectSelectionStrategy
	{
		private readonly IBoilerplateCache cache;
		private readonly DTE2 dte;
		private readonly TestProject previouslySelectedProject;

		public PreviouslySelectedProjectStrategy(IBoilerplateCache cache, DTE2 dte, TestProject previouslySelectedProject)
		{
			this.cache = cache;
			this.dte = dte;
			this.previouslySelectedProject = previouslySelectedProject;
		}

		public TestProject Apply(IEnumerable<TestProject> testProjects)
		{
			if (this.previouslySelectedProject != null)
				return this.previouslySelectedProject;

			string lastSelectedProject = this.cache.GetLastSelectedProject(this.dte.Solution.FileName);
			if (lastSelectedProject == null)
				return null;

			return testProjects
				?.FirstOrDefault(project => 
					string.Equals(
						lastSelectedProject, 
						project.Project.FullName,
						StringComparison.OrdinalIgnoreCase));
		}
	}
}
