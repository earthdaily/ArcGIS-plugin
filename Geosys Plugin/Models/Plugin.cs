using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Geosys_Plugin.Models
{
    class Plugin
    {

    }

    public class PluginButton
    {
        public string Label { get; set; }

        public bool Visible { get; set; }

        public ICommand CommandButton { get; set; }
    }
}
