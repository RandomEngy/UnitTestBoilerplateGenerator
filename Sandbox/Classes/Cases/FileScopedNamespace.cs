namespace UnitBoilerplate.Sandbox.Classes.Cases;

public class FileScopedNamespace
{
	private readonly ISomeInterface someInterface;

	public FileScopedNamespace(ISomeInterface someInterface)
	{
		this.someInterface = someInterface;
	}
}
