using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geosys_Plugin.Views.Plugin
{
    internal class ErrorPanelViewModel : BaseViewModel
    {

        private string _errorMessage;

        public ErrorPanelViewModel()
        {
        }

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                SetProperty(ref _errorMessage, value, () => ErrorMessage);
            }
        }
    }
}
