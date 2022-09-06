using ActiproSoftware.Windows.Input;
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
using Geosys_Plugin.Models.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Geosys_Plugin.Views.Plugin
{

    internal class SearchResultsViewModel : BaseViewModel
    {
        private List<CatalogImageryOutput> _results;
        private List<CatalogImageryOutput> _selectedResults;

        public DelegateCommand<object> ItemChangedCommand { get; private set; }

        /// <summary>
        /// Consume the passed in CIMView. Call the base constructor to wire up the CIMView.
        /// </summary>
        public SearchResultsViewModel(Action<List<CatalogImageryOutput>> onSelectionChanged)
        {

            _results = new List<CatalogImageryOutput>();

            ItemChangedCommand = new DelegateCommand<object>((selectedItem) =>
            {
                onSelectionChanged(GetSelectedResults());
            });
        }

        public List<CatalogImageryOutput> GetSelectedResults()
        {
            List<CatalogImageryOutput> Selection = new List<CatalogImageryOutput>();
            foreach (var result in Results)
            {
                if (result.IsSelected)
                {
                    Selection.Add(result);
                }
            }
            return Selection;
        }

        public List<CatalogImageryOutput> Results
        {
            get
            {
                return _results;
            }
            set
            {
                SetProperty(ref _results, value, () => Results);
            }
        }

        private void onSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (ListBox)sender;

        }
    }
}
