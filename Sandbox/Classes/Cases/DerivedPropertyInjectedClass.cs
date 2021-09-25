using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace UnitBoilerplate.Sandbox.Classes.Cases
{
    public class DerivedPropertyInjectedClass : PropertyInjectedClassMultiple
    {
        [Dependency]
        public IInterface3 Interface3Property { protected get; set; }
    }
}
