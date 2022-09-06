using ArcGIS.Core.CIM;
using ArcGIS.Core.Data;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Catalog;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Editing;
using ArcGIS.Desktop.Extensions;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Dialogs;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Layouts;
using ArcGIS.Desktop.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geosys_Plugin.Views.Settings
{
    internal class ShowSettings : Button
    {

        private Settings _settings = null;

        protected override void OnClick()
        {
            //already open?
            if (_settings != null)
                return;
            _settings = new Settings();
            _settings.Owner = FrameworkApplication.Current.MainWindow;
            _settings.Closed += (o, e) => { _settings = null; };
            _settings.Show();
            //uncomment for modal
            //_settings.ShowDialog();
        }

    }
}
