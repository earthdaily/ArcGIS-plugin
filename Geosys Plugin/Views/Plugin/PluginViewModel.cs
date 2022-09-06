using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using System.Windows.Input;
using Geosys_Plugin.WebServices;
using Geosys_Plugin.Models.API;
using ArcGIS.Desktop.Editing.Attributes;
using System.Reflection;
using Geosys_Plugin.Models;
using System.Windows.Media.Imaging;
using Geosys_Plugin.Utils;

namespace Geosys_Plugin.Views.Plugin
{
    internal class PluginViewModel : DockPane
    {
        private const string _dockPaneID = "Geosys_Plugin_Views_Plugin_Plugin";

        // View Models
        private CoverageParametersViewModel _coverageParametersVM;
        private SearchResultsViewModel _searchResultsVM;
        private ErrorPanelViewModel _errorPanelVM;
        private CreateMapViewModel _createMapVM;
        private InformationPanelViewModel _informationPanelVM;

        private CurrentPageEnum _currentPageValue;

        private CatalogImageryService _catalogImageryService;

        // Visibility buttons
        private bool _isSearchMapVisible;

        // Buttons
        private PluginButton _button1;
        private PluginButton _button2;
        private PluginButton _button3;

        WebUrls _webUrls;

        protected PluginViewModel()
        {
            _currentPageValue = CurrentPageEnum.CoverageParameters;
            this.setCoverageParametersButtons();
            _coverageParametersVM = new CoverageParametersViewModel();
            _searchResultsVM = new SearchResultsViewModel(onSelectionChanged);
            _createMapVM = new CreateMapViewModel();
            CurrentPage = _coverageParametersVM;
            _catalogImageryService = new CatalogImageryService();
            _errorPanelVM = new ErrorPanelViewModel();
            _informationPanelVM = new InformationPanelViewModel();
            _isSearchMapVisible = true;
            _webUrls = new APIUrls().web;
        }

        /// <summary>
        /// Show the DockPane.
        /// </summary>
        internal static void Show()
        {
            DockPane pane = FrameworkApplication.DockPaneManager.Find(_dockPaneID);
            if (pane == null)
                return;

            pane.Activate();
        }

        /// <summary>
        /// Text shown near the top of the DockPane.
        /// </summary>
        private string _heading = "Coverage parameters";
        public string Heading
        {
            get { return _heading; }
            set
            {
                SetProperty(ref _heading, value, () => Heading);
            }
        }

        private BaseViewModel _currentPage;
        public BaseViewModel CurrentPage
        {
            get { return _currentPage; }
            set
            {
                SetProperty(ref _currentPage, value, () => CurrentPage);
            }
        }

        public bool IsSearchMapVisible
        {
            get { return _isSearchMapVisible; }
            set
            {
                SetProperty(ref _isSearchMapVisible, value, () => IsSearchMapVisible);
                //OnPropertyChanged("IsEnabled");
            }
        }


        public PluginButton Button1
        {
            get { return _button1; }
            set
            {
                SetProperty(ref _button1, value, () => Button1);
                //OnPropertyChanged("IsEnabled");
            }
        }

        public PluginButton Button2
        {
            get { return _button2; }
            set
            {
                SetProperty(ref _button2, value, () => Button2);
                //OnPropertyChanged("IsEnabled");
            }
        }

        public PluginButton Button3
        {
            get { return _button3; }
            set
            {
                SetProperty(ref _button3, value, () => Button3);
                //OnPropertyChanged("IsEnabled");
            }
        }

        #region Commands

        public ICommand LeftButtonCmd
        {
            get
            {
                return new RelayCommand(() =>
               {
                   if (_currentPageValue == CurrentPageEnum.Error || _currentPageValue == CurrentPageEnum.SearchResults)
                   {
                       CurrentPage = _coverageParametersVM;
                       _currentPageValue = CurrentPageEnum.CoverageParameters;
                       Button2 = new PluginButton
                       {
                           Label = "Search Map",
                           Visible = true
                       };
                   }
                   else if (_currentPageValue == CurrentPageEnum.CreateMap)
                   {
                       CurrentPage = _coverageParametersVM;
                       _currentPageValue = CurrentPageEnum.SearchResults;
                   }
                   //this.IsSearchMapVisible = true;
               }, true);
            }
        }


        public ICommand SearchMapCmd
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    await this.SearchMapAsync();
                    //this.IsSearchMapVisible = true;
                }, true);
            }
        }

        public ICommand HelpCmd
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    System.Diagnostics.Process.Start(_webUrls.HelpUrl);
                }, true);
            }
        }

        public ICommand ShowParametersCmd
        {
            get
            {
                return new RelayCommand(() =>
               {
                   showCoverageParametersView();
               }, true);
            }
        }

        public ICommand BackSearchResultsCmd
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    this.showSearchResultsView();
                    //this.IsSearchMapVisible = true;
                }, true);
            }
        }

        public ICommand ShowCreateMapViewCmd
        {
            get
            {
                return new RelayCommand(() =>
                {
                    showCreateMapView();
                }, true);
            }
        }

        public ICommand CreateMapCmd
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    this.CreateMap();
                }, true);
            }
        }

        public ICommand DifferenceMapCmd
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    this.CreateMap(true);
                }, true);
            }
        }



        public ICommand RightButtonCmd
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    if (_currentPageValue == CurrentPageEnum.CoverageParameters)
                    {
                        await this.SearchMapAsync();
                    }
                    else if (_currentPageValue == CurrentPageEnum.SearchResults)
                    {
                        this.CreateMap();
                    }
                }, true);
            }
        }

        /*public ICommand SearchMapCmd
        {
            get
            {
                return this.SearchMapAsync();

            }
        }*/

        #endregion Commands

        protected void showCoverageParametersView()
        {
            CurrentPage = _coverageParametersVM;
            _currentPageValue = CurrentPageEnum.CoverageParameters;
            this.setCoverageParametersButtons();
        }

        protected void showSearchResultsView()
        {
            CurrentPage = _searchResultsVM;
            _currentPageValue = CurrentPageEnum.SearchResults;
            setSearchResultsButtons();
        }

        protected void showCreateMapView()
        {
            CurrentPage = _createMapVM;
            _createMapVM.SelectedMaps = _searchResultsVM.GetSelectedResults();
            _createMapVM.MapProductConfig = _coverageParametersVM.MapProductType;
            _createMapVM.NPlanned = _coverageParametersVM.NPlanned;
            _currentPageValue = CurrentPageEnum.CreateMap;
            this.setCreateMapButtons();
        }

        protected void showErrorView()
        {
            CurrentPage = _errorPanelVM;
            _currentPageValue = CurrentPageEnum.Error;
            this.setErrorButtons();
        }

        void onSelectionChanged(List<CatalogImageryOutput> selection)
        {
            if (_currentPageValue == CurrentPageEnum.SearchResults)
            {
                Button3 = new PluginButton
                {
                    Label = "Next",
                    Visible = selection.Count > 0,
                    CommandButton = this.ShowCreateMapViewCmd
                };
            }
        }

        protected async Task SearchMapAsync()
        {

            try
            {
                CurrentPage = _informationPanelVM;

                FeatureLayer selectedLayer = _coverageParametersVM.SelectedLayer;
                if (selectedLayer == null)
                {
                    _errorPanelVM.ErrorMessage = "Error validating coverage parameters. Layer is not selected.";
                    this.showErrorView();
                    return;
                }
                //BasicFeatureLayer lala2 = (BasicFeatureLayer)lala;

                List<Geometry> _shapesList = new List<Geometry>();
                List<CatalogImageryOutput> coverages = new List<CatalogImageryOutput>();
                await QueuedTask.Run(async () =>
                {
                    //_coverageParametersVM.SelectedFeaturesOnly = true;
                    var featuresSelection = selectedLayer.GetSelection();
                    if (_coverageParametersVM.SelectedFeaturesOnly && featuresSelection.GetCount() > 0)
                    {


                        QueryFilter qf = new QueryFilter
                        {
                            ObjectIDs = featuresSelection.GetObjectIDs()
                        };
                        RowCursor rows = featuresSelection.Search(qf);
                        foreach (var oid in featuresSelection.GetObjectIDs())
                        {
                            var insp = new Inspector();
                            insp.Load(selectedLayer, oid);
                            _shapesList.Add(insp.Shape);
                        }
                    }
                    else
                    {
                        QueryFilter queryFilter = new QueryFilter { WhereClause = "1 = 1" };
                        RowCursor rowCursor = selectedLayer.Search(queryFilter);
                        using (rowCursor)
                        {
                            int featuresCount = 0;
                            while (rowCursor.MoveNext() && featuresCount < 10)
                            {
                                using (Row row = rowCursor.Current)
                                {
                                    Feature feat = row as Feature;
                                    var insp = new Inspector();
                                    insp.Load(selectedLayer, feat.GetObjectID());
                                    _shapesList.Add(insp.Shape);
                                }
                            }
                        }
                    }
                });
                //var sel = lala2.GetSelection();
                if (_coverageParametersVM.BulkRequest)
                {
                    Geometry combinedGeometry = GeometryUtils.CombineFeatures(_shapesList);
                    _shapesList = new List<Geometry>();
                    _shapesList.Add(combinedGeometry);
                }

                foreach (var shape in _shapesList)
                {
                    List<CatalogImageryOutput> coverage = await this.GetCatalogImageryForShapeAsync(shape);
                    coverages.AddRange(coverage);
                }

                _searchResultsVM.Results = coverages;
                if (coverages != null && coverages.Count > 0)
                {
                    for (int index = coverages.Count - 1; index >= 0; index--)
                    {
                        MapProductConfig MapProductConfig = _coverageParametersVM.MapProductType;
                        MapImageInput MapInput = new MapImageInput
                        {
                            ImageId = coverages[index].Image.Id,
                            ImageFormat = "thumbnail.png",
                            SeasonField = coverages[index].SeasonField.Id,
                            MapProductType = MapProductConfig.Label,
                            NPlanned = _coverageParametersVM.NPlanned,
                            ImageDate = coverages[index].Image.Date
                        };
                        BitmapImage image = await MapProductConfig.getThumbnail(MapInput);

                        if (image != null)
                        {
                            _searchResultsVM.Results[index].ImageSrc = image;
                        }
                        else
                        {
                            _searchResultsVM.Results.RemoveAt(index);
                        }
                    }

                }
                //_searchResultsVM.Results[0].ImageSrc = images;
                this.showSearchResultsView();
            }
            catch (Exception e)
            {
                if (e.Message == "Unauthorized")
                {
                    _errorPanelVM.ErrorMessage = "Error of processing ! Ensure your username, password, client id, and client secret are valid for the selected region service and then try again.";
                }
                else
                {
                    _errorPanelVM.ErrorMessage = "An error occured during processing";
                }

                this.showErrorView();
            }
        }

        protected void CreateMap(bool isDifferenceMap = false)
        {
            try
            {
                _createMapVM.CreateMapAsync(isDifferenceMap);
            }
            catch (Exception e)
            {

                this.IsSearchMapVisible = false;
                CurrentPage = _errorPanelVM;
                _currentPageValue = CurrentPageEnum.Error;
            }
            /// <summary>
            /// Coverage parameters .
            /// </summary>
        }

        private async Task<List<CatalogImageryOutput>> GetCatalogImageryForShapeAsync(Geometry shape)
        {
            string sensor = _coverageParametersVM.Sensor;
            string startDate = null;
            if (_coverageParametersVM.StartDate != null)
            {
                startDate = ((DateTime)_coverageParametersVM.StartDate).ToString("yyyy-MM-dd");
            }
            string endDate = null;
            if (_coverageParametersVM.EndDate != null)
            {
                endDate = ((DateTime)_coverageParametersVM.EndDate).ToString("yyyy-MM-dd");
            }
            string featuresWktGeometry = null;
            WKTExportFlags wktExportFlags = WKTExportFlags.wktExportDefaults;
            var projected = GeometryEngine.Instance.Project(shape, SpatialReferences.WGS84);
            featuresWktGeometry = GeometryEngine.Instance.ExportToWKT(wktExportFlags, projected);
            var input = new CatalogImageryInput
            {
                Geometry = featuresWktGeometry,
                Crop = new Crop { Id = Properties.Settings.Default.CropType },
                SowingDate = Properties.Settings.Default.SowingDate.ToString("yyyy-MM-dd"),
                Sensor = sensor,
                MapProductType = _coverageParametersVM.MapProductType.Label
            };
            List<CatalogImageryOutput> coverage = await _catalogImageryService.GetCatalogImagery(
                input, startDate, endDate
                );
            return coverage;
        }

        enum CurrentPageEnum
        {
            CoverageParameters,
            CreateMap,
            Error,
            SearchResults
        }


        private void setCoverageParametersButtons()
        {
            Button3 = new PluginButton
            {
                Label = "Search Map",
                Visible = true,
                CommandButton = this.SearchMapCmd
            };
            Button2 = new PluginButton
            {
                Label = "",
                Visible = false
            };
            Button1 = new PluginButton
            {
                Label = "",
                Visible = false
            };
        }

        private void setCreateMapButtons()
        {
            Button3 = new PluginButton
            {
                Label = "Create Map",
                Visible = true,
                CommandButton = this.CreateMapCmd
            };
            bool isDiffMapVisible = false;
            if (_createMapVM.SelectedMaps.Count == 2)
            {
                if (_createMapVM.MapProductConfig.Label == "INSEASON_NDVI" || _createMapVM.MapProductConfig.Label == "INSEASON_EVI")
                {
                    if (_createMapVM.SelectedMaps[0].SeasonField.Id == _createMapVM.SelectedMaps[1].SeasonField.Id)
                    {
                        isDiffMapVisible = true;
                    }
                }
            }
            Button2 = new PluginButton
            {
                Label = "Difference Map",
                Visible = isDiffMapVisible,
                CommandButton = this.DifferenceMapCmd
            };
            Button1 = new PluginButton
            {
                Label = "Back",
                Visible = true,
                CommandButton = this.BackSearchResultsCmd

            };
        }


        private void setErrorButtons()
        {
            Button3 = new PluginButton
            {
                Label = "Back",
                Visible = true,
                CommandButton = this.ShowParametersCmd
            };
            Button2 = new PluginButton
            {
                Label = "",
                Visible = false
            };
            Button1 = new PluginButton
            {
                Label = "",
                Visible = false
            };
        }

        private void setSearchResultsButtons()
        {
            Button3 = new PluginButton
            {
                Label = "Next",
                Visible = false,
                CommandButton = this.ShowCreateMapViewCmd
            };
            Button2 = new PluginButton
            {
                Label = "Back",
                Visible = true,
                CommandButton = this.ShowParametersCmd
            };
            Button1 = new PluginButton
            {
                Label = "",
                Visible = false
            };
        }
    }

    /// <summary>
    /// Button implementation to show the DockPane.
    /// </summary>
    internal class Plugin_ShowButton : Button
    {
        protected override void OnClick()
        {
            PluginViewModel.Show();
            //new CoverageParametersViewModel(null);
        }
    }
}