using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geosys_Plugin.Views.Plugin
{
    class InformationPanelViewModel : BaseViewModel
    {

        private string _informationMessage;

        public InformationPanelViewModel()
        {
            _informationMessage = "Searching ...";
        }

        public string InformationMessage
        {
            get { return _informationMessage; }
            set
            {
                SetProperty(ref _informationMessage, value, () => InformationMessage);
            }
        }
    }
}
