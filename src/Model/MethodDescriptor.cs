namespace UnitTestBoilerplate.Model
{
	public class MethodDescriptor
	{
		public MethodDescriptor(string name, MethodArgumentDescriptor[] methodParameters, bool isAsync, bool hasReturnType, string returnType)
		{
			Name = name;
			MethodParameters = methodParameters;
			IsAsync = isAsync;
			HasReturnType = hasReturnType;
			ReturnType = returnType;
		}

		public string Name { get; }

		public MethodArgumentDescriptor[] MethodParameters { get; }

		public bool IsAsync { get; }

		public bool HasReturnType { get; }

		public string ReturnType { get; }
	}
}
