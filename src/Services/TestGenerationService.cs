﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using UnitTestBoilerplate.Model;
using UnitTestBoilerplate.Utilities;

namespace UnitTestBoilerplate.Services
{
	[Export(typeof(ITestGenerationService))]
    public class TestGenerationService : ITestGenerationService
    {
		#region Properties

		private static readonly HashSet<string> PropertyInjectionAttributeNames = new HashSet<string>
		{
			"Microsoft.Practices.Unity.DependencyAttribute",
			"Ninject.InjectAttribute",
			"Grace.DependencyInjection.Attributes.ImportAttribute"
		};

		private static readonly IList<string> ClassSuffixes = new List<string>
		{
			"ViewModel",
			"Service",
			"Provider",
			"Factory",
			"Manager",
			"Component"
		};

		[Import]
		internal IBoilerplateSettingsFactory SettingsFactory { get; set; }

		#endregion Properties

		#region Public Functions

		public async Task<string> GenerateUnitTestFileAsync(
			ProjectItemSummary selectedFile,
			EnvDTE.Project targetProject, 
			TestFramework testFramework,
			MockFramework mockFramework)
		{
			string sourceProjectDirectory = Path.GetDirectoryName(selectedFile.ProjectFilePath);
			string selectedFileDirectory = Path.GetDirectoryName(selectedFile.FilePath);

			if (sourceProjectDirectory == null || selectedFileDirectory == null || !selectedFileDirectory.StartsWith(sourceProjectDirectory, StringComparison.OrdinalIgnoreCase))
			{
				throw new InvalidOperationException("Error with selected file paths.");
			}

			string relativePath = this.GetRelativePath(selectedFile);

			TestGenerationContext context = await this.CollectTestGenerationContextAsync(selectedFile, targetProject, testFramework, mockFramework, this.SettingsFactory.Get());

			string unitTestContents = this.GenerateUnitTestContents(context);

			string testFolder = Path.Combine(Path.GetDirectoryName(targetProject.FullName), relativePath);

			string testFileNameBase = StringUtilities.ReplaceTokens(
				context.Settings.FileNameTemplate,
				(tokenName, propertyIndex, builder) =>
				{
					if (WriteGlobalToken(tokenName, builder, context))
					{
						return;
					}

					WriteTokenPassthrough(tokenName, builder);
				});

			testFileNameBase = FileUtilities.CleanFileName(testFileNameBase);

			string testPath = Path.Combine(testFolder, testFileNameBase + ".cs");

			if (File.Exists(testPath))
			{
				throw new InvalidOperationException("Test file already exists.");
			}

			if (!Directory.Exists(testFolder))
			{
				Directory.CreateDirectory(testFolder);
			}

			File.WriteAllText(testPath, unitTestContents, new UTF8Encoding(true));

			return testPath;
		}

		public async Task GenerateUnitTestFileAsync(
			ProjectItemSummary selectedFile,
			string targetFilePath,
			string targetProjectNamespace,
			TestFramework testFramework,
			MockFramework mockFramework)
		{
			string sourceProjectDirectory = Path.GetDirectoryName(selectedFile.ProjectFilePath);
			string selectedFileDirectory = Path.GetDirectoryName(selectedFile.FilePath);

			if (sourceProjectDirectory == null || selectedFileDirectory == null || !selectedFileDirectory.StartsWith(sourceProjectDirectory, StringComparison.OrdinalIgnoreCase))
			{
				throw new InvalidOperationException("Error with selected file paths.");
			}

			TestGenerationContext context = await this.CollectTestGenerationContextAsync(selectedFile, targetProjectNamespace, testFramework, mockFramework, this.SettingsFactory.Get());

			string unitTestContents = this.GenerateUnitTestContents(context);

			string testFolder = Path.GetDirectoryName(targetFilePath);

			if (File.Exists(targetFilePath))
			{
				throw new InvalidOperationException("Test file already exists.");
			}

			if (!Directory.Exists(testFolder))
			{
				Directory.CreateDirectory(testFolder);
			}

			File.WriteAllText(targetFilePath, unitTestContents);
		}

		public string GetRelativePath(ProjectItemSummary selectedFile)
		{
			string projectDirectory = Path.GetDirectoryName(selectedFile.ProjectFilePath);
			string selectedFileDirectory = Path.GetDirectoryName(selectedFile.FilePath);

			if (projectDirectory == null || selectedFileDirectory == null || !selectedFileDirectory.StartsWith(projectDirectory, StringComparison.OrdinalIgnoreCase))
			{
				throw new InvalidOperationException("Error with selected file paths.");
			}

			string relativePath = selectedFileDirectory.Substring(projectDirectory.Length);
			if (relativePath.StartsWith("\\", StringComparison.Ordinal))
			{
				relativePath = relativePath.Substring(1);
			}

			return relativePath;
		}

		#endregion Public Functions

		#region Collect Tested Class Data

		private async Task<TestGenerationContext> CollectTestGenerationContextAsync(
			ProjectItemSummary selectedFile, 
			string targetProjectNamespace, 
			TestFramework testFramework,
			MockFramework mockFramework,
			IBoilerplateSettings settings)
		{
			Microsoft.CodeAnalysis.Solution solution = CreateUnitTestBoilerplateCommandPackage.VisualStudioWorkspace.CurrentSolution;
			DocumentId documentId = solution.GetDocumentIdsWithFilePath(selectedFile.FilePath).FirstOrDefault();
			if (documentId == null)
			{
				throw new InvalidOperationException("Could not find document in solution with file path " + selectedFile.FilePath);
			}

			var document = solution.GetDocument(documentId);

			SyntaxNode root = await document.GetSyntaxRootAsync();
			SemanticModel semanticModel = await document.GetSemanticModelAsync();

			SyntaxNode firstClassDeclaration = root.DescendantNodes().FirstOrDefault(node => node.Kind() == SyntaxKind.ClassDeclaration || node.Kind() == SyntaxKind.StructDeclaration);

			if (firstClassDeclaration == null)
			{
				throw new InvalidOperationException("Could not find class or struct declaration.");
			}

			if (firstClassDeclaration.ChildTokens().Any(node => node.Kind() == SyntaxKind.AbstractKeyword))
			{
				throw new InvalidOperationException("Cannot unit test an abstract class.");
			}

			SyntaxToken classIdentifierToken = firstClassDeclaration.ChildTokens().FirstOrDefault(n => n.Kind() == SyntaxKind.IdentifierToken);
			if (classIdentifierToken == default(SyntaxToken))
			{
				throw new InvalidOperationException("Could not find class identifier.");
			}

			NamespaceDeclarationSyntax namespaceDeclarationSyntax = null;
			if (!TypeUtilities.TryGetParentSyntax(firstClassDeclaration, out namespaceDeclarationSyntax))
			{
				throw new InvalidOperationException("Could not find class namespace.");
			}

			// Find property injection types
			var injectableProperties = new List<InjectableProperty>();

			string classFullName = namespaceDeclarationSyntax.Name + "." + classIdentifierToken;
			INamedTypeSymbol classType = semanticModel.Compilation.GetTypeByMetadataName(classFullName);

			foreach (ISymbol member in classType.GetBaseTypesAndThis().SelectMany(n => n.GetMembers()))
			{
				if (member.Kind == SymbolKind.Property)
				{
					IPropertySymbol property = (IPropertySymbol)member;

					foreach (AttributeData attribute in property.GetAttributes())
					{
						if (PropertyInjectionAttributeNames.Contains(attribute.AttributeClass.ToString()))
						{
							var injectableProperty = InjectableProperty.TryCreateInjectableProperty(property.Name, property.Type.ToString(), mockFramework);
							if (injectableProperty != null)
							{
								injectableProperties.Add(injectableProperty);
							}
						}
					}
				}
			}

			string className = classIdentifierToken.ToString();

			// Find constructor injection types
			List<InjectableType> constructorInjectionTypes = new List<InjectableType>();

			SyntaxNode constructorDeclaration = firstClassDeclaration.ChildNodes().FirstOrDefault(n => n.Kind() == SyntaxKind.ConstructorDeclaration);

			if (constructorDeclaration != null)
			{
				constructorInjectionTypes.AddRange(
					GetParameterListNodes(constructorDeclaration)
					.Select(node => InjectableType.TryCreateInjectableTypeFromParameterNode(node, semanticModel, mockFramework)));
			}

			// Find public method declarations
			IList<MethodDescriptor> methodDeclarations = new List<MethodDescriptor>();
			foreach (MethodDeclarationSyntax methodDeclaration in
				firstClassDeclaration.ChildNodes().Where(
					n => n.Kind() == SyntaxKind.MethodDeclaration
					&& ((MethodDeclarationSyntax)n).Modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword))))
			{
				var parameterList = GetParameterListNodes(methodDeclaration).ToList();
				var parameterTypes = GetArgumentDescriptors(parameterList, semanticModel, mockFramework);

				var attributeList = GetAttributeListNodes(methodDeclaration).ToList();
				var httpType = GetHttpType(attributeList);

				var isAsync =
					methodDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.AsyncKeyword)) ||
					DoesReturnTask(methodDeclaration);

				var hasReturnType = !DoesReturnNonGenericTask(methodDeclaration) && !DoesReturnVoid(methodDeclaration);

				string returnType = methodDeclaration.ReturnType.ToFullString();

				methodDeclarations.Add(new MethodDescriptor(methodDeclaration.Identifier.Text, parameterTypes, isAsync, hasReturnType, returnType, httpType));
			}

			string unitTestNamespace;

			string relativePath = this.GetRelativePath(selectedFile);

			if (string.IsNullOrEmpty(relativePath))
			{
				unitTestNamespace = targetProjectNamespace;
			}
			else
			{
				List<string> defaultNamespaceParts = targetProjectNamespace.Split('.').ToList();
				List<string> unitTestNamespaceParts = new List<string>(defaultNamespaceParts);
				unitTestNamespaceParts.AddRange(relativePath.Split('\\'));

				unitTestNamespace = string.Join(".", unitTestNamespaceParts);
			}

			List<InjectableType> injectedTypes = new List<InjectableType>(injectableProperties);
			injectedTypes.AddRange(constructorInjectionTypes.Where(t => t != null));

			GenerateMockNames(injectedTypes);

			return new TestGenerationContext(
				mockFramework,
				testFramework,
				document,
				settings,
				unitTestNamespace,
				className,
				namespaceDeclarationSyntax.Name.ToString(),
				injectableProperties,
				constructorInjectionTypes,
				injectedTypes,
				methodDeclarations);
		}

		private static bool DoesReturnTask(MethodDeclarationSyntax methodDeclaration)
		{
			return methodDeclaration.ReturnType is SimpleNameSyntax namedReturnType && namedReturnType.Identifier.Text == "Task";
		}

		private static bool DoesReturnNonGenericTask(MethodDeclarationSyntax methodDeclaration)
		{
			return DoesReturnTask(methodDeclaration) && !methodDeclaration.ReturnType.IsKind(SyntaxKind.GenericName);
		}

		private static bool DoesReturnVoid(MethodDeclarationSyntax methodDeclaration)
		{
			return methodDeclaration.ReturnType is PredefinedTypeSyntax predefinedType && predefinedType.Keyword.IsKind(SyntaxKind.VoidKeyword);
		}

		private static MethodArgumentDescriptor[] GetArgumentDescriptors(List<ParameterSyntax> argumentList, SemanticModel semanticModel, MockFramework mockFramework)
		{
			MethodArgumentDescriptor[] argumentDescriptors = new MethodArgumentDescriptor[argumentList.Count()];

			for (int i = 0; i < argumentDescriptors.Count(); i++)
			{
				string argumentName = argumentList[i].Identifier.Text;

				ParameterModifier modifier = GetArgumentModifier(argumentList[i]);

				SyntaxNode node = GetMainNodeFromParameterEntry(argumentList[i]);

				SymbolInfo symbolInfo = semanticModel.GetSymbolInfo(node);
				TypeDescriptor typeDescriptor;

				typeDescriptor = new TypeDescriptor(symbolInfo, argumentList[i].Type, node.Kind());

				argumentDescriptors[i] = new MethodArgumentDescriptor(typeDescriptor, argumentName, modifier);
			}

			return argumentDescriptors;
		}

		private static SyntaxNode GetMainNodeFromParameterEntry(ParameterSyntax parameter)
		{
			foreach (var child in parameter.ChildNodes())
			{
				SyntaxKind nodeKind = child.Kind();
				if (child.Kind() != SyntaxKind.AttributeList)
				{
					return child;
				}
			}

			throw new InvalidOperationException("Could not find valid syntax node inside parameter.");
		}

		private static ParameterModifier GetArgumentModifier(ParameterSyntax parameterSyntax)
		{
			if (parameterSyntax.Modifiers.Any(m => m.IsKind(SyntaxKind.OutKeyword)))
			{
				return ParameterModifier.Out;
			}

			if (parameterSyntax.Modifiers.Any(m => m.IsKind(SyntaxKind.RefKeyword)))
			{
				return ParameterModifier.Ref;
			}

			return ParameterModifier.None;
		}

		private static IEnumerable<ParameterSyntax> GetParameterListNodes(SyntaxNode memberNode)
		{
			SyntaxNode parameterListNode = memberNode.ChildNodes().First(n => n.Kind() == SyntaxKind.ParameterList);

			return parameterListNode.ChildNodes().Where(n => n.Kind() == SyntaxKind.Parameter).Cast<ParameterSyntax>();
		}

		private static HttpType GetHttpType(List<AttributeSyntax> attributeSyntaxList)
		{
			HttpType http = HttpType.None;
			foreach (AttributeSyntax attributeSyntax in attributeSyntaxList)
			{
				string attributeName = attributeSyntax.Name.ToString();
				if(attributeName.StartsWith("Http"))
				{
					if(Enum.TryParse(attributeName.Replace("Http",""), out http))
					{
						break;
					}
					else
					{
						http = HttpType.None;
					}
				}
			}

			return http;
		}

		private static IEnumerable<AttributeSyntax> GetAttributeListNodes(SyntaxNode memberNode)
		{
			SyntaxNode parameterListNode = memberNode.ChildNodes().FirstOrDefault(n => n.Kind() == SyntaxKind.AttributeList);

			return parameterListNode == null ? new List<AttributeSyntax>() : parameterListNode?.ChildNodes().Where(n => n.Kind() == SyntaxKind.Attribute).Cast<AttributeSyntax>();
		}

		private async Task<TestGenerationContext> CollectTestGenerationContextAsync(ProjectItemSummary selectedFile, EnvDTE.Project targetProject, TestFramework testFramework, MockFramework mockFramework, IBoilerplateSettings settings)
		{
			string targetProjectNamespace = targetProject.Properties.Item("DefaultNamespace").Value as string;
			return await this.CollectTestGenerationContextAsync(selectedFile, targetProjectNamespace, testFramework, mockFramework, settings);
		}

		#endregion Collect Tested Class Data

		#region Generate Test Class

		private string GenerateUnitTestContents(TestGenerationContext context)
		{
			TestFramework testFramework = context.TestFramework;
			MockFramework mockFramework = context.MockFramework;

			string fileTemplate = context.Settings.GetTemplate(testFramework, mockFramework, TemplateType.File);
			string filledTemplate = StringUtilities.ReplaceTokens(
				fileTemplate,
				(tokenName, propertyIndex, builder) =>
				{
					if (WriteGlobalToken(tokenName, builder, context))
					{
						return;
					}

					if (WriteContentToken(tokenName, propertyIndex, builder, context, fileTemplate))
					{
						return;
					}

					WriteTokenPassthrough(tokenName, builder);
				});

			SyntaxTree tree = CSharpSyntaxTree.ParseText(filledTemplate);
			SyntaxNode formattedNode = Formatter.Format(tree.GetRoot(), CreateUnitTestBoilerplateCommandPackage.VisualStudioWorkspace);

			return formattedNode.ToFullString();
		}

		private static string ReplaceInterfaceTokens(string template, InjectableType injectableType, TestGenerationContext context)
		{
			return StringUtilities.ReplaceTokens(
				template,
				(tokenName, propertyIndex, builder) =>
				{
					if (WriteGlobalToken(tokenName, builder, context))
					{
						return;
					}

					if (WriteInterfaceContentToken(injectableType, tokenName, builder))
					{
						return;
					}

					WriteTokenPassthrough(tokenName, builder);
				});
		}

		private static string ReplaceInterfaceTokensCustom(string template, InjectableType injectableType, TestGenerationContext context, string customMockClassName)
		{
			return StringUtilities.ReplaceTokens(
				template,
				(tokenName, propertyIndex, builder) =>
				{
					if (WriteGlobalToken(tokenName, builder, context))
					{
						return;
					}

					if (WriteInterfaceContentToken(injectableType, tokenName, builder))
					{
						return;
					}

					if (WriteInterfaceCustomContentToken(customMockClassName, tokenName, builder))
					{
						return;
					}

					WriteTokenPassthrough(tokenName, builder);
				});
		}

		private static bool WriteGlobalToken(string tokenName, StringBuilder builder, TestGenerationContext context)
		{
			switch (tokenName)
			{
				case "":
					builder.Append("$");
					break;

				case "Namespace":
					builder.Append(context.UnitTestNamespace);
					break;

				case "ClassName":
					builder.Append(context.ClassName);
					break;

				case "ClassNameShort":
					builder.Append(GetShortClassName(context.ClassName));
					break;

				case "ClassNameShortLower":
					// Legacy, new syntax is ClassNameShort.CamelCase
					builder.Append(GetShortClassNameLower(context.ClassName));
					break;

				default:
					return false;
			}

			return true;
		}

		private bool WriteContentToken(string tokenName, int propertyIndex, StringBuilder builder, TestGenerationContext context, string fileTemplate)
		{
			switch (tokenName)
			{
				case "UsingStatements":
					this.WriteUsings(builder, context);
					break;

				case "MockFieldDeclarations":
					this.WriteMockFieldDeclarations(builder, context);
					break;

				case "MockFieldInitializations":
					this.WriteMockFieldInitializations(builder, context);
					break;

				case "ExplicitConstructor":
					this.WriteExplicitConstructor(builder, context, FindIndent(fileTemplate, propertyIndex));
					break;

				case "TodoConstructor":
					WriteTodoConstructor(builder, context);
					break;

				case "TestMethods":
					this.WriteTestMethods(builder, context);
					break;

				default:
					return false;
			}

			return true;
		}

		private static bool WriteInterfaceContentToken(InjectableType injectableType, string tokenName, StringBuilder builder)
		{
			switch (tokenName)
			{
				case "InterfaceName":
					builder.Append(injectableType.TypeName);
					break;

				case "InterfaceNameBase":
					builder.Append(injectableType.TypeBaseName);
					break;

				case "InterfaceType":
					builder.Append(injectableType.ToString());
					break;

				case "InterfaceMockName":
					builder.Append(injectableType.MockName);
					break;

				default:
					return false;
			}

			return true;
		}

		private static bool WriteInterfaceCustomContentToken(string customMockClassName, string tokenName, StringBuilder builder)
		{
			if (tokenName == "CustomMockClass")
			{
				builder.Append(customMockClassName);
				return true;
			}

			return false;
		}

		private bool WriteTestMethodCommonToken(string tokenName, int propertyIndex, StringBuilder builder, TestGenerationContext context, string testTemplate)
		{
			switch (tokenName)
			{
				case "ExplicitConstructor":
					this.WriteExplicitConstructor(builder, context, FindIndent(testTemplate, propertyIndex));
					break;

				case "TodoConstructor":
					WriteTodoConstructor(builder, context);
					break;

				default:
					return false;
			}

			return true;
		}

		private bool WriteTestMethodInvocationToken(string tokenName, int propertyIndex, StringBuilder builder, TestGenerationContext context, string testTemplate, MethodDescriptor methodDescriptor)
		{
			int numberOfParameters = methodDescriptor.MethodParameters.Count();
			switch (tokenName)
			{
				case "TestedMethodName":
					builder.Append(methodDescriptor.Name);
					break;

				case "TestedMethodReturnType":
					builder.Append(methodDescriptor.ReturnType);
					break;

				case "HttpType":
					builder.Append(methodDescriptor.Http.ToString());
					break;

				case "TestMethodName":
					this.WriteTestMethodName(builder, context, methodDescriptor);
					break;

				case "AsyncModifier":
					if (methodDescriptor.IsAsync)
					{
						builder.Append("async");
					}
					break;

				case "AsyncReturnType":
					builder.Append(methodDescriptor.IsAsync ? "Task" : "void");
					break;

				case "ParameterSetupDefaults":
					WriteParameterSetupDefaults(builder, context, methodDescriptor);
					break;

				case "ParameterSetupTodo":
					for (int j = 0; j < numberOfParameters; j++)
					{
						builder.Append($"{methodDescriptor.MethodParameters[j].TypeInformation} {methodDescriptor.MethodParameters[j].ArgumentName} = TODO;");

						if (j < numberOfParameters - 1)
						{
							builder.AppendLine();
						}
					}
					break;

				case "MethodInvocationPrefix":
					if (methodDescriptor.HasReturnType)
					{
						builder.Append("var result = ");
					}

					if (methodDescriptor.IsAsync)
					{
						builder.Append("await ");
					}
					break;

				case "MethodInvocation":
					WriteMethodInvocation(builder, methodDescriptor, FindIndent(testTemplate, propertyIndex));
					break;

				case "MethodParameters":
					WriteMethodParameters(builder, methodDescriptor, FindIndent(testTemplate, propertyIndex));
					break;

				case "HttpMethodParameters":
					WriteHttpMethodParameters(builder, methodDescriptor, FindIndent(testTemplate, propertyIndex));
					break;

				case "URIParameters":
					WriteURIParameters(builder, methodDescriptor);
					break;

				default:
					return false;
			}

			return true;
		}

		private void WriteTestMethodName(StringBuilder builder, TestGenerationContext context, MethodDescriptor methodDescriptor)
		{
			string testMethodNameTemplate = context.Settings.GetTemplate(
				context.TestFramework,
				context.MockFramework,
				TemplateType.TestMethodName);

			string baseTestMethodName = StringUtilities.ReplaceTokens(
				testMethodNameTemplate,
				(tokenName2, propertyIndex2, builder2) =>
				{
					if (WriteGlobalToken(tokenName2, builder2, context))
					{
						return;
					}

					if (WriteTestMethodNameToken(tokenName2, builder2, methodDescriptor))
					{
						return;
					}

					WriteTokenPassthrough(tokenName2, builder2);
				});

			var testMethodName = CreateUniqueTestMethodName(context, baseTestMethodName);
			context.UsedTestMethodNames.Add(testMethodName);
			builder.Append(testMethodName);
		}

		private static bool WriteTestMethodNameToken(string tokenName, StringBuilder builder, MethodDescriptor methodDescriptor)
		{
			switch (tokenName)
			{
				case "TestedMethodName":
					builder.Append(methodDescriptor.Name);
					break;
				default:
					return false;
			}

			return true;
		}

		private static void WriteParameterSetupDefaults(StringBuilder builder, TestGenerationContext context, MethodDescriptor methodDescriptor)
		{
			int numberOfParameters = methodDescriptor.MethodParameters.Count();
			for (int j = 0; j < numberOfParameters; j++)
			{
				TypeDescriptor typeInformation = methodDescriptor.MethodParameters[j].TypeInformation;

				string argumentValue;
				if (typeInformation.TypeSymbol != null)
				{
					// If we have proper type information, generate the default expression for this type
					var generator = Microsoft.CodeAnalysis.Editing.SyntaxGenerator.GetGenerator(context.Document);
					argumentValue = generator.DefaultExpression(typeInformation.TypeSymbol).ToString();
				}
				else
				{
					argumentValue = "TODO";
				}

				builder.Append($"{typeInformation} {methodDescriptor.MethodParameters[j].ArgumentName} = {argumentValue};");
				if (j < numberOfParameters - 1)
				{
					builder.AppendLine();
				}
			}
		}

		private static void WriteMethodInvocation(StringBuilder builder, MethodDescriptor methodDescriptor, string currentIndent)
		{
			builder.Append($".{methodDescriptor.Name}(");
			WriteMethodParameters(builder, methodDescriptor, currentIndent);
			builder.Append(")");
		}

		private static void WriteMethodParameters(StringBuilder builder, MethodDescriptor methodDescriptor, string currentIndent)
		{
			var httpTypes = new List<HttpType>() { HttpType.None };

			int numberOfParameters = methodDescriptor.MethodParameters.Count();
			if (numberOfParameters == 0 || !httpTypes.Contains(methodDescriptor.Http))
			{
				return;
			}

			builder.AppendLine();

			for (int j = 0; j < numberOfParameters; j++)
			{
				builder.Append($"{currentIndent}	");

				switch (methodDescriptor.MethodParameters[j].Modifier)
				{
					case ParameterModifier.Out:
						builder.Append("out ");
						break;
					case ParameterModifier.Ref:
						builder.Append("ref ");
						break;
				}

				builder.Append($"{methodDescriptor.MethodParameters[j].ArgumentName}");

				if (j < numberOfParameters - 1)
				{
					builder.AppendLine(",");
				}
			}
		}

		private static void WriteHttpMethodParameters(StringBuilder builder, MethodDescriptor methodDescriptor, string currentIndent)
		{
			var httpTypes = new List<HttpType>() { HttpType.Post };

			int numberOfParameters = methodDescriptor.MethodParameters.Count();
			if (numberOfParameters == 0 || !httpTypes.Contains(methodDescriptor.Http))
			{
				return;
			}

			builder.AppendLine();
			builder.AppendLine($"{currentIndent}	{methodDescriptor.MethodParameters[0].ArgumentName}");
		}

		private static void WriteURIParameters(StringBuilder builder, MethodDescriptor methodDescriptor)
		{
			var httpTypes = new List<HttpType>() { HttpType.None };

			int numberOfParameters = methodDescriptor.MethodParameters.Count();
			if (numberOfParameters == 0 || httpTypes.Contains(methodDescriptor.Http) || (methodDescriptor.Http == HttpType.Post && numberOfParameters == 1))
			{
				return;
			}

			for (int j = 0; j < numberOfParameters; j++)
			{
				builder.Append(j==0 ? "?" : "&");
				string argumentName = methodDescriptor.MethodParameters[j].ArgumentName;
				builder.Append($"{argumentName}={{{argumentName}}}");
			}
		}

		private static void WriteTokenPassthrough(string tokenName, StringBuilder builder)
		{
			builder.Append($"${tokenName}$");
		}

		private void WriteUsings(StringBuilder builder, TestGenerationContext context)
		{
			List<string> namespaces = new List<string>();
			namespaces.AddRange(context.MockFramework.UsingNamespaces);
			namespaces.Add(context.TestFramework.UsingNamespace);
			namespaces.Add(context.ClassNamespace);

			if (context.TestFramework.TestCleanupStyle == TestCleanupStyle.Disposable)
			{
				namespaces.Add("System");
			}

			IDictionary<string, string> customMocks = context.Settings.CustomMocks;
			foreach (InjectableType injectedType in context.InjectedTypes)
			{
				namespaces.AddRange(injectedType.TypeNamespaces);

				// Add in namespaces for custom mock if needed.
				if (customMocks.TryGetValue(injectedType.FullName, out string customMockClassFull))
				{
					int lastDotIndex = customMockClassFull.LastIndexOf('.');
					if (lastDotIndex > 0)
					{
						namespaces.Add(customMockClassFull.Substring(0, lastDotIndex));
					}
				}
			}

			foreach (TypeDescriptor argumentDescriptor in
				context.MethodDeclarations.SelectMany(
					d => d.MethodParameters.Select(p => p.TypeInformation)))
			{
				if (argumentDescriptor is InjectableType injectableType)
				{
					namespaces.AddRange(injectableType.TypeNamespaces);

					continue;
				}

				namespaces.Add("System");
			}

			if (context.MethodDeclarations.Any(m => m.IsAsync))
			{
				namespaces.Add("System.Threading.Tasks");
			}

			string extraNamespacesSetting = context.Settings.GetTemplate(context.TestFramework, context.MockFramework, TemplateType.ExtraUsingNamespaces);
			string[] extraNamespaces = extraNamespacesSetting.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
			namespaces.AddRange(extraNamespaces);

			namespaces = namespaces.Distinct().ToList();
			namespaces.Sort(StringComparer.Ordinal);

			for (int i = 0; i < namespaces.Count; i++)
			{
				builder.Append($"using {namespaces[i]};");

				if (i < namespaces.Count - 1)
				{
					builder.AppendLine();
				}
			}
		}

		private void WriteMockFieldDeclarations(StringBuilder builder, TestGenerationContext context)
		{
			string template = context.Settings.GetTemplate(context.TestFramework, context.MockFramework, TemplateType.MockFieldDeclaration);
			IDictionary<string, string> customMocks = context.Settings.CustomMocks;
			for (int i = 0; i < context.InjectedTypes.Count; i++)
			{
				InjectableType injectedType = context.InjectedTypes[i];
				string line;

				if (customMocks.TryGetValue(injectedType.FullName, out string customMockClassFull))
				{
					string customMockClassName = StringUtilities.GetShortNameFromFullTypeName(customMockClassFull);
					line = ReplaceInterfaceTokensCustom(context.Settings.CustomMockFieldDeclarationTemplate, injectedType, context, customMockClassName);
				}
				else
				{
					line = ReplaceInterfaceTokens(template, injectedType, context);
				}

				builder.Append(line);

				if (i < context.InjectedTypes.Count - 1)
				{
					builder.AppendLine();
				}
			}
		}

		private void WriteMockFieldInitializations(StringBuilder builder, TestGenerationContext context)
		{
			string template = context.Settings.GetTemplate(context.TestFramework, context.MockFramework, TemplateType.MockFieldInitialization);
			IDictionary<string, string> customMocks = context.Settings.CustomMocks;
			for (int i = 0; i < context.InjectedTypes.Count; i++)
			{
				InjectableType injectedType = context.InjectedTypes[i];
				string line;
				if (customMocks.TryGetValue(injectedType.FullName, out string customMockClassFull))
				{
					string customMockClassName = StringUtilities.GetShortNameFromFullTypeName(customMockClassFull);
					line = ReplaceInterfaceTokensCustom(context.Settings.CustomMockFieldInitializationTemplate, injectedType, context, customMockClassName);
				}
				else
				{
					line = ReplaceInterfaceTokens(template, injectedType, context);
				}

				builder.Append(line);

				if (i < context.InjectedTypes.Count - 1)
				{
					builder.AppendLine();
				}
			}
		}

		private void WriteExplicitConstructor(StringBuilder builder, TestGenerationContext context, string currentIndent)
		{
			builder.Append($"new {context.ClassName}");

			if (context.ConstructorTypes.Count > 0)
			{
				builder.AppendLine("(");

				for (int i = 0; i < context.ConstructorTypes.Count; i++)
				{
					string mockReferenceStatement;
					InjectableType constructorType = context.ConstructorTypes[i];
					if (constructorType == null)
					{
						mockReferenceStatement = "TODO";
					}
					else
					{
						mockReferenceStatement = CreateMockReferenceStatement(context, constructorType);
					}

					builder.Append($"{currentIndent}    {mockReferenceStatement}");

					if (i < context.ConstructorTypes.Count - 1)
					{
						builder.AppendLine(",");
					}
				}

				builder.Append(")");
			}
			else if (context.Properties.Count == 0)
			{
				builder.Append("()");
			}

			if (context.Properties.Count > 0)
			{
				builder.AppendLine();
				builder.AppendLine("{");

				foreach (InjectableProperty property in context.Properties)
				{
					builder.AppendLine($"{property.PropertyName} = {CreateMockReferenceStatement(context, property)},");
				}

				builder.Append(@"}");
			}
		}

		private static string CreateMockReferenceStatement(TestGenerationContext context, InjectableType injectedType)
		{
			string template;
			if (context.Settings.CustomMocks.ContainsKey(injectedType.FullName))
			{
				template = context.Settings.CustomMockObjectReferenceTemplate;
			}
			else
			{
				template = context.Settings.GetTemplate(context.TestFramework, context.MockFramework, TemplateType.MockObjectReference);
			}

			return ReplaceInterfaceTokens(template, injectedType, context);
		}

		private static void WriteTodoConstructor(StringBuilder builder, TestGenerationContext context)
		{
			builder.Append("new ");
			builder.Append(context.ClassName);
			builder.Append("(");

			for (int i = 0; i < context.ConstructorTypes.Count; i++)
			{
				builder.Append("TODO");
				if (i < context.ConstructorTypes.Count - 1)
				{
					builder.Append(", ");
				}
			}

			builder.Append(")");
		}

		private void WriteTestMethods(StringBuilder builder, TestGenerationContext context)
		{
			if (context.MethodDeclarations.Count == 0)
			{
				string testMethodEmptyTemplate = context.Settings.GetTemplate(context.TestFramework, context.MockFramework, TemplateType.TestMethodEmpty);
				WriteTestMethod(builder, context, testMethodEmptyTemplate);

				return;
			}

			string testMethodInvokeTemplate = context.Settings.GetTemplate(context.TestFramework, context.MockFramework, TemplateType.TestMethodInvocation);
			for (int i = 0; i < context.MethodDeclarations.Count; i++)
			{
				var methodDescriptor = context.MethodDeclarations[i];

				WriteTestMethod(builder, context, testMethodInvokeTemplate, methodDescriptor);

				// Write separator line
				if (i < context.MethodDeclarations.Count - 1)
				{
					builder.AppendLine(); // Finish the line with the closing }
					builder.AppendLine(); // Make a blank line
				}
			}
		}

		private void WriteTestMethod(StringBuilder builder, TestGenerationContext context, string testTemplate, MethodDescriptor methodDescriptor = null)
		{
			string filledTemplate = StringUtilities.ReplaceTokens(
				testTemplate,
				(tokenName, propertyIndex, builder2) =>
				{
					if (WriteGlobalToken(tokenName, builder2, context))
					{
						return;
					}

					if (this.WriteTestMethodCommonToken(tokenName, propertyIndex, builder2, context, testTemplate))
					{
						return;
					}

					if (methodDescriptor != null && this.WriteTestMethodInvocationToken(tokenName, propertyIndex, builder2, context, testTemplate, methodDescriptor))
					{
						return;
					}

					WriteTokenPassthrough(tokenName, builder2);
				});

			builder.Append(filledTemplate);
		}

		private static string CreateUniqueTestMethodName(TestGenerationContext context, string baseTestMethodName)
		{
			string testMethodName = baseTestMethodName;

			int j = 1;

			while (context.UsedTestMethodNames.Contains(testMethodName))
			{
				testMethodName = baseTestMethodName + j;
				j++;
			}

			return testMethodName;
		}

		private static string FindIndent(string template, int currentIndex)
		{
			// Go back and find line start
			int lineStart = -1;
			for (int i = currentIndex - 1; i >= 0; i--)
			{
				char c = template[i];
				if (c == '\n')
				{
					lineStart = i + 1;
					break;
				}
			}

			// Go forward and find first non-whitespace character
			for (int i = lineStart; i <= currentIndex; i++)
			{
				char c = template[i];
				if (c != ' ' && c != '\t')
				{
					return template.Substring(lineStart, i - lineStart);
				}
			}

			return string.Empty;
		}

		private static string GetShortClassName(string className)
		{
			string pascalCaseShortClassName = null;
			foreach (string suffix in ClassSuffixes)
			{
				if (className.EndsWith(suffix))
				{
					pascalCaseShortClassName = suffix;
					break;
				}
			}

			if (pascalCaseShortClassName == null)
			{
				pascalCaseShortClassName = className;
			}

			return pascalCaseShortClassName;
		}

		private static string GetShortClassNameLower(string className)
		{
			string shortName = GetShortClassName(className);
			return shortName.Substring(0, 1).ToLowerInvariant() + shortName.Substring(1);
		}

		private static void GenerateMockNames(List<InjectableType> injectedTypes)
		{
			// Group them by TypeBaseName to see which ones need a more unique name
			var results = from t in injectedTypes
				group t by t.TypeBaseName into g
				select new { TypeBaseName = g.Key, Types = g.ToList() };

			foreach (var result in results)
			{
				if (result.Types.Count == 1)
				{
					result.Types[0].MockName = result.TypeBaseName;
				}
				else
				{
					foreach (var injectedType in result.Types)
					{
						injectedType.MockName = injectedType.LongMockName;
					}
				}
			}
		}

		#endregion Generate Test Class
	}
}
