namespace UnitTestBoilerplate.Model
{
	public class MethodDescriptor
	{
		public MethodDescriptor(string name, MethodArgumentDescriptor[] methodParameters, bool isAsync, bool hasReturnType, string returnType, HttpType http)
		{
			Name = name;
			MethodParameters = methodParameters;
			IsAsync = isAsync;
			HasReturnType = hasReturnType;
			ReturnType = returnType;
			Http = http;
		}

		public string Name { get; }

		public MethodArgumentDescriptor[] MethodParameters { get; }

		public bool IsAsync { get; }

		public bool HasReturnType { get; }

		public string ReturnType { get; }

		public HttpType Http { get; }
	}

	public enum HttpType
	{
		Get,
		Post,
		Put,
		Patch,
		Delete,
		Options,
		Head,
		None
	}
}
