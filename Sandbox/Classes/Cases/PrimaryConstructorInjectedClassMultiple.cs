using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitBoilerplate.Sandbox.Classes.Cases
{
	public class PrimaryConstructorInjectedClassMultiple(ISomeInterface someInterface, ISomeOtherInterface someOtherInterface)
	{
		private readonly ISomeInterface someInterface = someInterface;
		private readonly ISomeOtherInterface someOtherInterface = someOtherInterface;
	}
}
