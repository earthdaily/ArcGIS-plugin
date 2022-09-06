using ArcGIS.Core.CIM;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;
using Geosys_Plugin.Models;
using Geosys_Plugin.Models.API;
using Geosys_Plugin.WebServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace Geosys_Plugin.Views.Plugin
{
    class CreateMapViewModel : BaseViewModel
    {

        #region Private Properties
        private bool _isFetchHotSpotActive;

        private RadioButtonState _pngState;
        private RadioButtonState _shpState;
        private RadioButtonState _tiffState;
        private RadioButtonState _kmzState;

        private List<CatalogImageryOutput> _selectedMaps;
        private MapProductConfig _mapProductConfig;
        private float _nPlanned;
        private float _yieldAverage;
        private float _yieldMinimum;
        private float _yieldMaximum;
        private float _organicAverage;
        private int _numberOfZones;
        private float _gain;
        private float _offset;
        private MapFormatEnum _mapFormat = MapFormatEnum.PNG;
        private HotspotConfig _hotspotConfig;

        private CatalogImageryService _catalogImageryService;
        #endregion Properties

        public CreateMapViewModel()
        {
            _catalogImageryService = new CatalogImageryService();
            _yieldAverage = 80.00F;
            _yieldMinimum = 10.00F;
            _yieldMaximum = 100.00F;
            _organicAverage = 0.00F;
            _numberOfZones = 0;
            _gain = 1.00F;
            _offset = 0.00F;
            this._isFetchHotSpotActive = false;
            _pngState = new RadioButtonState();
            _kmzState = new RadioButtonState();
            _shpState = new RadioButtonState();
            _tiffState = new RadioButtonState();
            initHotspotConfig();
        }

        public bool ISFetchHotspotActive
        {
            get { return _isFetchHotSpotActive; }
            set
            {
                SetProperty(ref _isFetchHotSpotActive, value, () => ISFetchHotspotActive);
            }
        }

        public RadioButtonState PngState
        {
            get { return _pngState; }
            set
            {
                SetProperty(ref _pngState, value, () => PngState);
            }
        }

        public RadioButtonState ShpState
        {
            get { return _shpState; }
            set
            {
                SetProperty(ref _shpState, value, () => ShpState);
            }
        }

        public RadioButtonState TiffState
        {
            get { return _tiffState; }
            set
            {
                SetProperty(ref _tiffState, value, () => TiffState);
            }
        }

        public RadioButtonState KmzState
        {
            get { return _kmzState; }
            set
            {
                SetProperty(ref _kmzState, value, () => KmzState);
            }
        }

        public HotspotConfig HotspotConfig
        {
            get { return _hotspotConfig; }
            set
            {
                SetProperty(ref _hotspotConfig, value, () => HotspotConfig);
            }
        }


        public async Task addShapeToMapAsync(string shapePath)
        {
            Layer shapeLayer = null;
            if (MapView.Active != null && MapView.Active.Map != null)
            {
                try
                {
                    await QueuedTask.Run(() =>
                    {
                        shapeLayer = LayerFactory.Instance.CreateLayer(new Uri(shapePath), getAndCreateGroupLayerIfNeeded(), 0);
                    });
                }
                catch (Exception exc)
                {
                    // Catch any exception found and display a message box.
                    ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("Exception caught while trying to add layer: " + exc.Message);
                    return;
                }
            }
        }

        /*
         * imageFormat : PNG or TIFF
         */
        public async Task addRasterToMapAsync(string rasterPath, string imageFormat)
        {
            BasicRasterLayer imageServiceLayer = null;
            if (MapView.Active != null && MapView.Active.Map != null)
            {
                try
                {
                    await QueuedTask.Run(() =>
                    {
                        imageServiceLayer = (BasicRasterLayer)LayerFactory.Instance.CreateRasterLayer(new Uri(rasterPath), getAndCreateGroupLayerIfNeeded(), 0);
                        if (imageFormat == "PNG")
                        {
                            CIMRasterColorizer newColorizer = imageServiceLayer.GetColorizer();
                            if (newColorizer is CIMRasterRGBColorizer)
                            {
                                ((CIMRasterRGBColorizer)newColorizer).StretchType = 0;
                            }
                            else if (newColorizer is CIMRasterStretchColorizer)
                            {
                                ((CIMRasterStretchColorizer)newColorizer).StretchType = 0;
                            }
                            imageServiceLayer.SetColorizer(newColorizer);
                        }
                    });
                }
                catch (Exception exc)
                {
                    // Catch any exception found and display a message box.
                    ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("Exception caught while trying to add layer: " + exc.Message);
                    return;
                }
            }
        }

        private GroupLayer getAndCreateGroupLayerIfNeeded()
        {
            GroupLayer groupLayer = null;
            string outputGroupName = "outputs";
            if (MapView.Active != null && MapView.Active.Map != null)
            {

                IReadOnlyList<GroupLayer> groupLayers = MapView.Active.Map.Layers.OfType<GroupLayer>().ToList();
                foreach (var group in groupLayers)
                {
                    if (group.Name == outputGroupName)
                    {
                        groupLayer = group;
                    }
                }
                if (groupLayer == null)
                {
                    groupLayer = LayerFactory.Instance.CreateGroupLayer(MapView.Active.Map, 0, outputGroupName);
                }
            }
            return groupLayer;
        }

        public void initHotspotConfig()
        {
            HotspotConfig = new HotspotConfig
            {
                Checked = false,
                Polygon = true,
                PositionChecked = false,
                NonePosition = false,
                PointPosition = false,
                MinimumPosition = false,
                AveragePosition = false,
                MedianPosition = false,
                MaximumPosition = false,
                AllPosition = false,
                FiltersChecked = false,
                Top = 0,
                Bottom = 0
            };
        }

        public async Task CreateMapAsync(bool isDifferenceMap = false)
        {
            if (SelectedMaps.Count > 0)
            {
                for (int i = 0; i < SelectedMaps.Count; i++)
                {
                    MapImageParameters MapImageParameters = new MapImageParameters
                    {
                        YieldAverage = YieldAverage,
                        YieldMinimum = YieldMinimum,
                        YieldMaximum = YieldMaximum,
                        OrganicAverage = OrganicAverage,
                        NumberOfZones = NumberOfZones,
                        Gain = Gain,
                        Offset = Offset
                    };

                    MapImageInput MapInput = new MapImageInput
                    {
                        ImageId = SelectedMaps[i].Image.Id,
                        ImageFormat = null,
                        SeasonField = SelectedMaps[i].SeasonField.Id,
                        MapProductType = MapProductConfig.Label,
                        NPlanned = NPlanned,
                        ImageDate = SelectedMaps[i].Image.Date
                    };
                    if (isDifferenceMap)
                    {
                        MapInput.MapProductType = "DIFFERENCE_" + MapProductConfig.Label;
                        MapInput.DiffImageId = SelectedMaps[1].Image.Id;
                        i = SelectedMaps.Count;
                    }
                    if (MapFormat == MapFormatEnum.TIFF)
                    {
                        // Tif
                        MapInput.ImageFormat = "image.tiff.zip";
                        string res = null;
                        try
                        {
                            res = await MapProductConfig.saveTiffImageAndGetPath(MapInput, MapImageParameters, HotspotConfig);
                        }
                        catch (Exception e)
                        {
                            ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("Exception calling Geosys API : " + e.Message);
                        }
                        if (res != null)
                        {
                            await this.addRasterToMapAsync(res, "TIFF");
                        }
                    }
                    else if (MapFormat == MapFormatEnum.SHP)
                    {

                        //Shape image.shp.zip
                        MapInput.ImageFormat = "image.shp.zip";
                        //SHP
                        string res = null;
                        try
                        {
                            res = await MapProductConfig.saveShapeImageAndGetPath(MapInput, MapImageParameters, HotspotConfig);
                        }
                        catch (Exception e)
                        {
                            ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("Exception calling Geosys API : " + e.Message);
                        }
                        if (res != null)
                        {
                            await this.addShapeToMapAsync(res);
                        }
                    }
                    else if (MapFormat == MapFormatEnum.PNG)
                    {

                        MapInput.ImageFormat = "image.png";

                        //SHP
                        string res = null;
                        try
                        {
                            res = await MapProductConfig.savePngImageAndGetPath(MapInput, MapImageParameters, HotspotConfig);
                        }
                        catch (Exception e)
                        {
                            ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("Exception calling Geosys API : " + e.Message);
                        }
                        if (res != null)
                        {
                            await this.addRasterToMapAsync(res, "PNG");
                        }



                    }
                    else if (MapFormat == MapFormatEnum.KMZ)
                    {
                        MapInput.ImageFormat = "image.kmz";

                        //KMZ
                        string res = null;
                        if (GetHotspotParametersFromConfig().Count > 0)
                        {

                        }
                        else
                        {
                            try
                            {
                                res = await MapProductConfig.saveKmzAndGetPath(MapInput, MapImageParameters, HotspotConfig);
                            }
                            catch (Exception e)
                            {
                                ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("Exception calling Geosys API : " + e.Message);
                            }
                            if (res != null)
                            {
                                await this.addShapeToMapAsync(res);
                            }
                        }
                    }

                    string hotSpotPath = null;
                    if (HotspotConfig.Checked && MapImageParameters.NumberOfZones > 0)
                    {
                        hotSpotPath = await MapProductConfig.saveShapeHotspotAndGetPath(MapInput, MapImageParameters, HotspotConfig);
                    }
                    if (hotSpotPath != null)
                    {
                        await this.addShapeToMapAsync(hotSpotPath);
                    }
                }

            }
        }

        List<HotspotParameters> GetHotspotParametersFromConfig()
        {
            List<HotspotParameters> hotspotParametersList = new List<HotspotParameters>();
            if (HotspotConfig.Checked && NumberOfZones > 0)
            {
                string zoningSegmentation;
                if (HotspotConfig.Polygon)
                {
                    zoningSegmentation = "Polygon";
                }
                else
                {
                    zoningSegmentation = "None";
                }
                if (HotspotConfig.PositionChecked)
                {
                    if (HotspotConfig.NonePosition)
                    {
                        hotspotParametersList.Add(new HotspotParameters { hotSpotPosition = "None", zoningSegmentation = zoningSegmentation });
                    }
                    else if (HotspotConfig.PointPosition)
                    {
                        hotspotParametersList.Add(new HotspotParameters { hotSpotPosition = "PointOnSurface", zoningSegmentation = zoningSegmentation });
                    }
                    else if (HotspotConfig.MinimumPosition)
                    {
                        hotspotParametersList.Add(new HotspotParameters { hotSpotPosition = "Min", zoningSegmentation = zoningSegmentation });
                    }
                    else if (HotspotConfig.AveragePosition)
                    {
                        hotspotParametersList.Add(new HotspotParameters { hotSpotPosition = "Average", zoningSegmentation = zoningSegmentation });
                    }
                    else if (HotspotConfig.MedianPosition)
                    {
                        hotspotParametersList.Add(new HotspotParameters { hotSpotPosition = "Median", zoningSegmentation = zoningSegmentation });
                    }
                    else if (HotspotConfig.MaximumPosition)
                    {
                        hotspotParametersList.Add(new HotspotParameters { hotSpotPosition = "Max", zoningSegmentation = zoningSegmentation });
                    }
                    else if (HotspotConfig.AllPosition)
                    {
                        hotspotParametersList.Add(new HotspotParameters { hotSpotPosition = "All", zoningSegmentation = zoningSegmentation });
                    }
                }
            }
            return hotspotParametersList;
        }


        public ICommand SwitchMapFormatCommand => new RelayCommand(SwitchMapFormat, () => true);

        private void SwitchMapFormat(object choice)
        {
            Enum.TryParse(choice.ToString(), out MapFormatEnum enumChoice);
            MapFormat = enumChoice;
        }

        public ICommand SwitchHotspotCommand => new RelayCommand(SwitchHotspot, () => true);

        private void SwitchHotspot(object choice)
        {
            if (choice.ToString() == "Polygon")
            {
                HotspotConfig.Polygon = true;
                HotspotConfig.PolygonPart = false;
            }
            else
            {
                HotspotConfig.Polygon = false;
                HotspotConfig.PolygonPart = true;
            }
        }

        public List<CatalogImageryOutput> SelectedMaps
        {
            get
            {
                return _selectedMaps;
            }
            set
            {
                SetProperty(ref _selectedMaps, value, () => SelectedMaps);
            }
        }

        public MapProductConfig MapProductConfig
        {
            get
            {
                return _mapProductConfig;
            }
            set
            {
                bool _valueChecked = false;
                PngState.Checked = false;
                ShpState.Checked = false;
                TiffState.Checked = false;
                KmzState.Checked = false;

                PngState.Enabled = value.MapFormats.Contains("PNG");
                if (!_valueChecked && PngState.Enabled)
                {
                    MapFormat = MapFormatEnum.PNG;
                    PngState.Checked = true;
                    _valueChecked = true;
                }
                ShpState.Enabled = value.MapFormats.Contains("SHP");
                if (!_valueChecked && ShpState.Enabled)
                {
                    MapFormat = MapFormatEnum.SHP;
                    ShpState.Checked = true;
                    _valueChecked = true;
                }
                TiffState.Enabled = value.MapFormats.Contains("TIFF");
                if (!_valueChecked && TiffState.Enabled)
                {
                    MapFormat = MapFormatEnum.TIFF;
                    TiffState.Checked = true;
                    _valueChecked = true;
                }
                KmzState.Enabled = value.MapFormats.Contains("KMZ");
                if (!_valueChecked && KmzState.Enabled)
                {
                    MapFormat = MapFormatEnum.KMZ;
                    KmzState.Checked = true;
                    _valueChecked = true;
                }

                SetProperty(ref _mapProductConfig, value, () => MapProductConfig);
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

        public float YieldAverage
        {
            get
            {
                return _yieldAverage;
            }
            set
            {
                SetProperty(ref _yieldAverage, value, () => YieldAverage);
            }
        }

        public float YieldMinimum
        {
            get
            {
                return _yieldMinimum;
            }
            set
            {
                SetProperty(ref _yieldMinimum, value, () => YieldMinimum);
            }
        }

        public float YieldMaximum
        {
            get
            {
                return _yieldMaximum;
            }
            set
            {
                SetProperty(ref _yieldMaximum, value, () => YieldMaximum);
            }
        }

        public float OrganicAverage
        {
            get
            {
                return _organicAverage;
            }
            set
            {
                SetProperty(ref _organicAverage, value, () => OrganicAverage);
            }
        }

        public int NumberOfZones
        {
            get
            {
                return _numberOfZones;
            }
            set
            {
                SetProperty(ref _numberOfZones, value, () => NumberOfZones);
            }
        }

        public float Gain
        {
            get
            {
                return _gain;
            }
            set
            {
                SetProperty(ref _gain, value, () => Gain);
            }
        }

        public float Offset
        {
            get
            {
                return _offset;
            }
            set
            {
                SetProperty(ref _offset, value, () => Offset);
            }
        }

        public MapFormatEnum MapFormat
        {
            get
            {
                return _mapFormat;
            }
            set
            {
                SetProperty(ref _mapFormat, value, () => MapFormat);
            }
        }

        public enum MapFormatEnum
        {
            PNG,
            TIFF,
            SHP,
            KMZ
        }
    }


    class RadioButtonState
    {
        public bool Checked { get; set; }
        public bool Enabled { get; set; }

        public RadioButtonState()
        {
            Checked = true;
            Enabled = true;
        }
    }

    class HotspotConfig
    {
        public bool Checked { get; set; }

        public bool Polygon { get; set; }

        public bool PolygonPart { get; set; }

        public bool PositionChecked { get; set; }

        public bool NonePosition { get; set; }

        public bool PointPosition { get; set; }

        public bool MinimumPosition { get; set; }

        public bool AveragePosition { get; set; }

        public bool MedianPosition { get; set; }

        public bool MaximumPosition { get; set; }

        public bool AllPosition { get; set; }

        public bool FiltersChecked { get; set; }

        public int Top { get; set; }

        public int Bottom { get; set; }
    }
}
