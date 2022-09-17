﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using EnvDTE;
using EnvDTE80;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using UnitTestBoilerplate.Model;
using UnitTestBoilerplate.Services;
using UnitTestBoilerplate.Services.ProjectSelection;
using UnitTestBoilerplate.Utilities;
using Project = EnvDTE.Project;
using Task = System.Threading.Tasks.Task;
using Window = EnvDTE.Window;

namespace UnitTestBoilerplate.ViewModel
{
	public class CreateUnitTestBoilerplateViewModel : ViewModelBase
	{
		private readonly DTE2 dte;

		public CreateUnitTestBoilerplateViewModel()
		{
			this.dte = (DTE2)ServiceProvider.GlobalProvider.GetService(typeof(DTE));
		}

		public void Initialize()
		{
			this.TestProjects = new List<TestProject>();
			IList<Project> allProjects = SolutionUtilities.GetProjects(this.dte);

			var newProjectList = new List<TestProject>();
			foreach (Project project in allProjects)
			{
				TestProject testProject = new TestProject
				{
					Name = project.Name,
					Project = project
				};

				newProjectList.Add(testProject);
			}

			this.TestProjects = newProjectList.OrderBy(p => p.Name).ToList();

			var selectionStrategies = new ITestProjectSelectionStrategy[]
			{
				new TemplateBasedStrategy(this.SettingsFactory.Get(), this.dte),
				new PreviouslySelectedProjectStrategy(Cache, dte, this.selectedProject),
				new NameHeuristicsStrategy(),
				new TakeFirstStrategy()
			};

			this.selectedProject = selectionStrategies
				.Select(strategy => strategy.Apply(this.TestProjects))
				.FirstOrDefault(project => project != null);

			this.TestFrameworkChoices = TestFrameworks.List;
			this.MockFrameworkChoices = MockFrameworks.List;

			// Populate selected/detected test/mock frameworks based on selected project
			this.UpdateFrameworks();
		}

		[Import]
		internal IBoilerplateCache Cache { get; set; }

		[Import]
		internal IBoilerplateSettingsFactory SettingsFactory { get; set; }

		[Import]
		internal IFrameworkPickerService FrameworkPickerService { get; set; }

		[Import]
		internal ITestGenerationService TestGenerationService { get; set; }

		public ICreateUnitTestBoilerplateView View { get; set; }

		public List<TestProject> TestProjects { get; private set; }

		private TestProject selectedProject;
		public TestProject SelectedProject
		{
			get { return this.selectedProject; }
			set
			{
				this.Set(ref this.selectedProject, value);
				this.UpdateFrameworks();
			}
		}

		public IList<TestFramework> TestFrameworkChoices { get; private set; }

		private TestFramework selectedTestFramework;
		public TestFramework SelectedTestFramework
		{
			get { return this.selectedTestFramework; }
			set { this.Set(ref this.selectedTestFramework, value); }
		}

		private string detectedTestFrameworks;
		public string DetectedTestFrameworks
		{
			get { return this.detectedTestFrameworks; }
			set { this.Set(ref this.detectedTestFrameworks, value); }
		}

		public IList<MockFramework> MockFrameworkChoices { get; private set; }

		private MockFramework selectedMockFramework;

		public MockFramework SelectedMockFramework
		{
			get { return this.selectedMockFramework; }
			set { this.Set(ref this.selectedMockFramework, value); }
		}

		private string detectedMockFrameworks;
		public string DetectedMockFrameworks
		{
			get { return this.detectedMockFrameworks; }
			set { this.Set(ref this.detectedMockFrameworks, value); }
		}

		private RelayCommand createUnitTestCommand;
		public RelayCommand CreateUnitTestCommand
		{
			get
			{
				return this.createUnitTestCommand ?? (this.createUnitTestCommand = new RelayCommand(
					async () =>
					{
						try
						{
							IList<ProjectItemSummary> selectedFiles = SolutionUtilities.GetSelectedFiles(this.dte);
							await this.CreateUnitTestAsync(selectedFiles);
						}
						catch (Exception exception)
						{
							MessageBox.Show(exception.ToString());
						}
					}));
			}
		}

		internal async Task CreateUnitTestAsync(IList<ProjectItemSummary> selectedFiles, bool addToProject = true)
		{
			var createdTestPaths = new List<string>();
			foreach (ProjectItemSummary selectedFile in selectedFiles)
			{
				string generatedTestPath = await this.TestGenerationService.GenerateUnitTestFileAsync(
					selectedFile,
					this.SelectedProject.Project,
					this.SelectedTestFramework,
					this.SelectedMockFramework);

				if (generatedTestPath != null)
				{
					createdTestPaths.Add(generatedTestPath);
				}
			}

			if (addToProject)
			{
				bool focusSet = false;
				foreach (string createdTestPath in createdTestPaths)
				{
					// Add the file to project
					ProjectItem testItem = this.SelectedProject.Project.ProjectItems.AddFromFile(createdTestPath);

					Window testWindow = testItem.Open(EnvDTE.Constants.vsViewKindCode);
					testItem.ExpandView();
					testWindow.Visible = true;

					if (!focusSet)
					{
						testWindow.SetFocus();
						focusSet = true;
					}
				}

				this.Cache.SaveSelectedTestProject(this.dte.Solution.FileName, this.SelectedProject.Project.FullName);
			}

			this.View?.Close();
		}

		private void UpdateFrameworks()
		{
			if (this.selectedProject == null)
			{
				this.SelectedTestFramework = TestFrameworks.Default;
				this.SelectedMockFramework = MockFrameworks.Default;
			}
			else
			{
				IBoilerplateSettings settings = this.SettingsFactory.Get();

				List<TestFramework> testFrameworks = this.FrameworkPickerService.FindTestFrameworks(this.selectedProject.Project);
				List<MockFramework> mockFrameworks = this.FrameworkPickerService.FindMockFrameworks(this.selectedProject.Project);

				this.SelectedTestFramework = this.FrameworkPickerService.PickDefaultTestFramework(testFrameworks, settings);
				this.SelectedMockFramework = this.FrameworkPickerService.PickDefaultMockFramework(mockFrameworks, settings);

				if (testFrameworks.Count == 0)
				{
					this.DetectedTestFrameworks = "Detected: None";
				}
				else
				{
					this.DetectedTestFrameworks = "Detected: " + string.Join(", ", testFrameworks);
				}

				if (mockFrameworks.Count == 0)
				{
					this.DetectedMockFrameworks = "Detected: None";
				}
				else
				{
					this.DetectedMockFrameworks = "Detected: " + string.Join(", ", mockFrameworks);
				}
			}
		}
	}

	public interface ICreateUnitTestBoilerplateView
	{
		void Close();
	}
}
