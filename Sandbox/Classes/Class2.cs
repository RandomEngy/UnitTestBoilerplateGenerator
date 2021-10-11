using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace UnitBoilerplate.Sandbox.Classes
{
    public class Class2
    {
        [Dependency]
        public ISomeInterface MyProperty { protected get; set; }

        [Dependency]
        public ISomeOtherInterface Property2 { protected get; set; }
    }
}
