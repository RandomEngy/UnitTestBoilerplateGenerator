using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace UnitTestBoilerplate.Model
{
	public class MethodDescriptor
	{
		public MethodDescriptor(string name, MethodArgumentDescriptor[] methodParameters, bool isAsync, bool hasReturnType, string returnType, IEnumerable<AttributeSyntax> attributeSyntaxes)
		{
			Name = name;
			MethodParameters = methodParameters;
			IsAsync = isAsync;
			HasReturnType = hasReturnType;
			ReturnType = returnType;
			MethodAttributes = new MethodAttributesDescriptor(attributeSyntaxes);
		}

		public string Name { get; }

		public MethodArgumentDescriptor[] MethodParameters { get; }

		public bool IsAsync { get; }

		public bool HasReturnType { get; }

		public string ReturnType { get; }

		public MethodAttributesDescriptor MethodAttributes { get; }
	}
}
