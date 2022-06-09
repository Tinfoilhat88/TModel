using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace TModel
{
    public abstract class ModuleBase : ContentControl
    {
        public abstract string ModuleName { get; }
    }
}
