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
using System.Threading.Tasks;
using Newtonsoft.Json;
using Geosys_Plugin.Models;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using ArcGIS.Core.Events;
using ArcGIS.Desktop.Mapping.Events;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using System.Text.RegularExpressions;

namespace Geosys_Plugin.Views.Plugin
{
    internal class CoverageParametersViewModel : BaseViewModel, INotifyPropertyChanged
    {
        #region Private Properties
        SubscriptionToken _mmpcToken = null;

        private FeatureLayer _selectedLayer;
        private ObservableCollection<FeatureLayer> _allMapLayers;
        private List<MapProductConfig> _mapProductConfigList;
        private List<MapProductConfig> _mapProductsTypes;
        // Sensors LOV from config
        private List<string> _sensorsLOV;
        // Sensors LOV for Combobox
        private List<string> _sensorsLOVComboBox;
        private MapProductConfig _mapProductType;
        private string _sensor;
        private bool _selectedFeaturesOnly;
        private bool _bulkRequest;
        private DateTime? _startDate;
        private DateTime? _endDate;
        private float _nPlanned;
        private SubscriptionToken _activeMapViewChangedEvent = null;

        private bool _isNplannedEnabled;
        #endregion Properties

        /// <summary>
        /// Consume the passed in CIMView. Call the base constructor to wire up the CIMView.
        /// </summary>
        public CoverageParametersViewModel()
        {
            MapView activeView = MapView.Active;
            //get all the layers in the active map
            _allMapLayers = new ObservableCollection<FeatureLayer>(new List<FeatureLayer>());
            var activeMapView = MapView.Active;
            _activeMapViewChangedEvent = ActiveMapViewChangedEvent.Subscribe(OnActiveMapViewChangedEvent);
            LayersAddedEvent.Subscribe(onLayersAddRem);
            LayersRemovedEvent.Subscribe(onLayersAddRem);
            MapMemberPropertiesChangedEvent.Subscribe(onLayersPropertiesChanged);
            if (activeMapView != null)
            {
                AllMapLayers = new ObservableCollection<FeatureLayer>(filterOutputsLayer(activeView.Map.GetLayersAsFlattenedList().OfType<FeatureLayer>().ToList()));
                // the map view is not active yet
            }
            _mapProductsTypes = new List<MapProductConfig>();
            _sensorsLOV = new List<string>();
            _sensorsLOVComboBox = new List<string>();
            _selectedFeaturesOnly = false;

            //set the selected layer to be the first one from the list
            if (_allMapLayers.Count > 0)
                SelectedLayer = _allMapLayers[0];

            LOV listOfValues = LOV.ReadLovFile();
            _mapProductConfigList = MapProductConfig.ReadConfigFile();
            _mapProductsTypes.AddRange(_mapProductConfigList);
            _sensorsLOV = listOfValues.Sensors;
            _sensorsLOVComboBox = listOfValues.Sensors;

            _mapProductType = _mapProductConfigList[0];
            _isNplannedEnabled = false;
            _sensor = "ALL_SENSORS";
            _startDate = DateTime.Now;
            _endDate = DateTime.Now;
            _nPlanned = .01F;
        }

        #region Public Properties

        public ObservableCollection<FeatureLayer> AllMapLayers
        {
            get { return _allMapLayers; }
            set
            {
                _selectedLayer = null;
                _allMapLayers = value;

                NotifyPropertyChanged("AllMapLayers"); // method implemented below
            }
        }


        public List<MapProductConfig> MapProductsTypes
        {
            get { return _mapProductsTypes; }
        }

        public List<string> SensorsLOV
        {
            get { return _sensorsLOV; }
        }

        public List<string> SensorsLOVComboBox
        {
            get { return _sensorsLOVComboBox; }

            set
            {
                SetProperty(ref _sensorsLOVComboBox, value, () => SensorsLOVComboBox);

            }
        }

        public MapProductConfig MapProductType
        {
            get { return _mapProductType; }
            set
            {
                //Check if mapProduct Type requires N-Planned attribute enabled
                if (value.NPlanned)
                {
                    IsNplannedEnabled = true;
                }
                else
                {
                    IsNplannedEnabled = false;
                }

                // Enable only some sensors values when Product Type is REFLECTANCE
                if (value.Sensors.Contains("ALL"))
                {
                    SensorsLOVComboBox = SensorsLOV;
                    Sensor = SensorsLOVComboBox[0];
                }
                else
                {
                    SensorsLOVComboBox = value.Sensors.ToList();
                    Sensor = SensorsLOVComboBox[0];
                }

                SetProperty(ref _mapProductType, value, () => MapProductType);

            }
        }

        public string Sensor
        {
            get { return _sensor; }
            set
            {
                SetProperty(ref _sensor, value, () => Sensor);

            }
        }

        public bool SelectedFeaturesOnly
        {
            get { return _selectedFeaturesOnly; }
            set
            {
                SetProperty(ref _selectedFeaturesOnly, value, () => SelectedFeaturesOnly);

            }
        }

        public bool BulkRequest
        {
            get { return _bulkRequest; }
            set
            {
                SetProperty(ref _bulkRequest, value, () => BulkRequest);
            }
        }
        public FeatureLayer SelectedLayer
        {
            get { return _selectedLayer; }
            set
            {
                SetProperty(ref _selectedLayer, value, () => SelectedLayer);
            }
        }


        public DateTime? StartDate
        {
            get
            {
                if (_startDate.HasValue)
                    return _startDate.Value;
                return null;
            }
            set
            {
                SetProperty(ref _startDate, value, () => StartDate);
            }
        }

        public DateTime? EndDate
        {
            get
            {
                if (_endDate.HasValue)
                    return _endDate.Value;
                return null;
            }
            set
            {
                SetProperty(ref _endDate, value, () => EndDate);
            }
        }

        public float NPlanned
        {
            get
            {
                return _nPlanned;
            }
            set
            {
                SetProperty(ref _nPlanned, value, () => NPlanned);
            }
        }

        public bool IsNplannedEnabled
        {

            get { return _isNplannedEnabled; }
            set
            {
                SetProperty(ref _isNplannedEnabled, value, () => IsNplannedEnabled);

            }
        }

        #endregion Public Properties

        private void OnActiveMapViewChangedEvent(ActiveMapViewChangedEventArgs args)
        {
            if (args.IncomingView != null)
            {
                if (args.IncomingView.Map != null)
                {
                    AllMapLayers = new ObservableCollection<FeatureLayer>(filterOutputsLayer(args.IncomingView.Map.GetLayersAsFlattenedList().OfType<FeatureLayer>().ToList()));
                    if (_allMapLayers.Count > 0)
                        SelectedLayer = _allMapLayers[0];
                }
            }
        }

        private void onLayersAddRem(LayerEventsArgs obj)
        {
            if (MapView.Active != null)
            {
                AllMapLayers = new ObservableCollection<FeatureLayer>(filterOutputsLayer(MapView.Active.Map.GetLayersAsFlattenedList().OfType<FeatureLayer>().ToList()));
                if (_allMapLayers.Count > 0)
                    SelectedLayer = _allMapLayers[0];
                // the map view is not active yet
            }
        }

        private void onLayersPropertiesChanged(MapMemberPropertiesChangedEventArgs evt)
        {
            if (MapView.Active != null)
            {

                AllMapLayers = new ObservableCollection<FeatureLayer>(filterOutputsLayer(MapView.Active.Map.GetLayersAsFlattenedList().OfType<FeatureLayer>().ToList()));
                if (_allMapLayers.Count > 0)
                    SelectedLayer = _allMapLayers[0];
                // the map view is not active yet
            }
        }


        private List<FeatureLayer> filterOutputsLayer(List<FeatureLayer> layersList)
        {
            List<FeatureLayer> newList = new List<FeatureLayer>();
            for (int i = 0; i < layersList.Count; i++)
            {
                if (layersList[i].Parent == null || layersList[i].Parent.GetType() != typeof(GroupLayer) || ((GroupLayer)layersList[i].Parent).Name != "outputs")
                {
                    newList.Add(layersList[i]);
                }
            }
            return newList;
        }

    }
}