using Geosys_Plugin.Models.API;
using Geosys_Plugin.Utils;
using Geosys_Plugin.Views.Plugin;
using Geosys_Plugin.WebServices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Geosys_Plugin.Models
{
    class MapProductConfig
    {

        public string Id { get; set; }

        public string Label { get; set; }

        public string[] MapFormats { get; set; }

        public string[] Sensors { get; set; }

        public string MapType { get; set; }

        public bool NPlanned { get; set; }

        public bool Hotspot { get; set; }

        public bool YieldAverage { get; set; }

        public bool YieldMinMax { get; set; }

        public bool Organic { get; set; }

        public bool Zoning { get; set; }

        public bool Gain { get; set; }

        public bool Offset { get; set; }


        public async Task<BitmapImage> getThumbnail(MapImageInput input)
        {
            BitmapImage image = null;
            CatalogImageryUrls _catalogImageryUrls = new APIUrls().catalogImagery;
            CatalogImageryService _catalogImageryService = new CatalogImageryService();

            if (MapType == "ModelMap")
            {
                string apiUrl = string.Format(@"{0}/{1}", _catalogImageryUrls.GetBaseUrl(), _catalogImageryUrls.ModelMap);
                string urlWithParams = String.Format(apiUrl, input.SeasonField, input.ImageId, input.MapProductType, input.NPlanned, input.ImageFormat);
                image = await _catalogImageryService.GetImageMapByUrl(urlWithParams);
            }
            else if (MapType == "Reflectance")
            {
                image = new BitmapImage(new Uri("pack://application:,,,/Geosys Plugin;component/Images/no-thumbnail.jpg"));
            }
            else if (MapType == "Soil")
            {
                string apiUrl = string.Format(@"{0}/{1}", _catalogImageryUrls.GetBaseUrl(), _catalogImageryUrls.SoilMap);
                string urlWithParams = String.Format(apiUrl, input.SeasonField, "SOILMAP", "USA", input.ImageFormat);
                image = await _catalogImageryService.GetImageMapByUrl(urlWithParams);
            }
            else if (MapType == "Management")
            {
                string apiUrl = string.Format(@"{0}/{1}", _catalogImageryUrls.GetBaseUrl(), _catalogImageryUrls.ManagementZonesMap);
                string urlWithParams = String.Format(apiUrl, input.SeasonField, input.MapProductType, input.ImageFormat);
                urlWithParams = String.Format("{0}?imageReferences={1}", urlWithParams, input.ImageId);
                image = await _catalogImageryService.GetImageMapByUrl(urlWithParams);
            }
            else if (MapType == "OrganicMatter")
            {
                string apiUrl = string.Format(@"{0}/{1}", _catalogImageryUrls.GetBaseUrl(), _catalogImageryUrls.OrganicMatterMap);
                string urlWithParams = String.Format(apiUrl, input.SeasonField, input.ImageId, input.ImageFormat);
                image = await _catalogImageryService.GetImageMapByUrl(urlWithParams);
            }
            else if (MapType == "Topology")
            {
                string apiUrl = string.Format(@"{0}/{1}", _catalogImageryUrls.GetBaseUrl(), _catalogImageryUrls.TopologyMap);
                string urlWithParams = String.Format(apiUrl, input.SeasonField, input.ImageFormat);
                image = await _catalogImageryService.GetImageMapByUrl(urlWithParams);
            }
            else if (MapType == "Ygm")
            {
                string apiUrl = string.Format(@"{0}/{1}", _catalogImageryUrls.GetBaseUrl(), _catalogImageryUrls.YgmMap);
                string urlWithParams = String.Format(apiUrl, input.SeasonField, input.ImageId, 80, 100, 10, input.ImageFormat);
                image = await _catalogImageryService.GetImageMapByUrl(urlWithParams);
            }
            else if (MapType == "Ypm")
            {
                string apiUrl = string.Format(@"{0}/{1}", _catalogImageryUrls.GetBaseUrl(), _catalogImageryUrls.YpmMap);
                string urlWithParams = String.Format(apiUrl, input.SeasonField, input.ImageId, 80, input.ImageFormat);
                image = await _catalogImageryService.GetImageMapByUrl(urlWithParams);
            }
            else if (MapType == "Base")
            {
                string apiUrl = string.Format(@"{0}/{1}", _catalogImageryUrls.GetBaseUrl(), _catalogImageryUrls.BaseReferenceMapImage);
                string urlWithParams = String.Format(apiUrl, input.SeasonField, input.ImageId, input.MapProductType, input.ImageFormat);

                image = await _catalogImageryService.GetImageMapByUrl(urlWithParams);
            }
            return image;
        }

        public async Task<string> saveTiffImageAndGetPath(MapImageInput input, MapImageParameters parameters, HotspotConfig hotspotConfig)
        {
            CatalogImageryService _catalogImageryService = new CatalogImageryService();
            string urlWithParams = getProductImageUrl(input, parameters);
            string hotspotPrefix = null;
            if (hotspotConfig.Checked && parameters.NumberOfZones > 0)
            {
                hotspotPrefix = "SegmentsPerPolygon";
            }
            string fileName = FileUtils.MapImageToFileName(input, parameters, hotspotPrefix);
            return await _catalogImageryService.SaveMapTiffImageByUrl(urlWithParams, fileName);
        }

        public async Task<string> saveShapeImageAndGetPath(MapImageInput input, MapImageParameters parameters, HotspotConfig hotspotConfig)
        {
            CatalogImageryService _catalogImageryService = new CatalogImageryService();
            string urlWithParams = getProductImageUrl(input, parameters, hotspotConfig);
            string hotspotPrefix = null;
            if (hotspotConfig.Checked && parameters.NumberOfZones > 0)
            {
                hotspotPrefix = "SegmentsPerPolygon";
            }
            string fileName = FileUtils.MapImageToFileName(input, parameters, hotspotPrefix);
            return await _catalogImageryService.SaveMapShapeByUrl(urlWithParams, fileName);
        }

        public async Task<string> savePngImageAndGetPath(MapImageInput input, MapImageParameters parameters, HotspotConfig hotspotConfig)
        {
            CatalogImageryService _catalogImageryService = new CatalogImageryService();
            string urlWithParams = getProductImageUrl(input, parameters, hotspotConfig);
            string hotspotPrefix = null;
            if (hotspotConfig.Checked && parameters.NumberOfZones > 0)
            {
                hotspotPrefix = "SegmentsPerPolygon";
            }
            string fileName = FileUtils.MapImageToFileName(input, parameters, hotspotPrefix);
            input.ImageFormat = "world-file.pgw";
            string pgwUrl = getProductImageUrl(input, parameters); ;
            input.ImageFormat = "legend.png";
            string legendUrl = getProductImageUrl(input, parameters); ;
            return await _catalogImageryService.SavePngShapeByUrl(urlWithParams, pgwUrl, legendUrl, fileName);
        }

        public async Task<string> saveShapeHotspotAndGetPath(MapImageInput input, MapImageParameters parameters, HotspotConfig hotspotConfig)
        {
            input.ImageFormat = "hotspot.shp.zip";
            string hotspotPrefix = "HotspotsPerPolygon";
            CatalogImageryService _catalogImageryService = new CatalogImageryService();
            string urlWithParams = getProductImageUrl(input, parameters, hotspotConfig);
            string fileName = FileUtils.MapImageToFileName(input, parameters, hotspotPrefix);
            return await _catalogImageryService.SaveMapShapeByUrl(urlWithParams, fileName);
        }

        public async Task<string> saveKmzAndGetPath(MapImageInput input, MapImageParameters parameters, HotspotConfig hotspotConfig)
        {
            CatalogImageryService _catalogImageryService = new CatalogImageryService();
            string kmzUrl = getProductImageUrl(input, parameters);
            string hotspotPrefix = null;
            if (hotspotConfig.Checked && parameters.NumberOfZones > 0)
            {
                hotspotPrefix = "SegmentsPerPolygon";
            }
            string fileName = FileUtils.MapImageToFileName(input, parameters, hotspotPrefix);
            return await _catalogImageryService.SaveKmzShapeByUrl(kmzUrl, fileName);
        }

        public string getProductImageUrl(MapImageInput input, MapImageParameters parameters, HotspotConfig hotspotConfig = null)
        {
            CatalogImageryUrls _catalogImageryUrls = new APIUrls().catalogImagery;
            string url = null;
            bool hasUrlParameters = false;

            if (MapType == "ModelMap")
            {
                string apiUrl = string.Format(@"{0}/{1}", _catalogImageryUrls.GetBaseUrl(), _catalogImageryUrls.ModelMap);
                url = String.Format(apiUrl, input.SeasonField, input.ImageId, input.MapProductType, input.NPlanned, input.ImageFormat);
            }
            else if (MapType == "Reflectance")
            {
                string apiUrl = string.Format(@"{0}/{1}", _catalogImageryUrls.GetBaseUrl(), _catalogImageryUrls.ReflectanceMap);
                url = String.Format(apiUrl, input.SeasonField, input.ImageId);
            }
            else if (MapType == "Management")
            {
                string apiUrl = string.Format(@"{0}/{1}", _catalogImageryUrls.GetBaseUrl(), _catalogImageryUrls.ManagementZonesMap);
                url = String.Format(apiUrl, input.SeasonField, input.MapProductType, input.ImageFormat);
                url = String.Format("{0}?imageReferences={1}", url, input.ImageId);
                hasUrlParameters = true;
            }
            else if (MapType == "OrganicMatter")
            {
                string apiUrl = string.Format(@"{0}/{1}", _catalogImageryUrls.GetBaseUrl(), _catalogImageryUrls.OrganicMatterMap);
                url = String.Format(apiUrl, input.SeasonField, input.ImageId, input.ImageFormat);
            }
            else if (MapType == "Soil")
            {
                string apiUrl = string.Format(@"{0}/{1}", _catalogImageryUrls.GetBaseUrl(), _catalogImageryUrls.SoilMap);
                url = String.Format(apiUrl, input.SeasonField, "SOILMAP", "USA", input.ImageFormat);
            }
            else if (MapType == "Topology")
            {
                string apiUrl = string.Format(@"{0}/{1}", _catalogImageryUrls.GetBaseUrl(), _catalogImageryUrls.TopologyMap);
                url = String.Format(apiUrl, input.SeasonField, input.ImageFormat);
            }
            else if (MapType == "Ygm")
            {
                string apiUrl = string.Format(@"{0}/{1}", _catalogImageryUrls.GetBaseUrl(), _catalogImageryUrls.YgmMap);
                url = String.Format(apiUrl, input.SeasonField, input.ImageId, parameters.YieldAverage, parameters.YieldMaximum, parameters.YieldMinimum, input.ImageFormat);
            }
            else if (MapType == "Ypm")
            {
                string apiUrl = string.Format(@"{0}/{1}", _catalogImageryUrls.GetBaseUrl(), _catalogImageryUrls.YpmMap);
                url = String.Format(apiUrl, input.SeasonField, input.ImageId, parameters.YieldAverage, input.ImageFormat);
            }
            else if (MapType == "Base")
            {
                if (input.MapProductType.StartsWith("DIFFERENCE"))
                {
                    string apiUrl = string.Format(@"{0}/{1}", _catalogImageryUrls.GetBaseUrl(), _catalogImageryUrls.DifferenceMap);
                    url = String.Format(apiUrl, input.SeasonField, input.ImageId, input.MapProductType, input.DiffImageId, input.ImageFormat);
                    return url;
                }
                else
                {
                    string apiUrl = string.Format(@"{0}/{1}", _catalogImageryUrls.GetBaseUrl(), _catalogImageryUrls.BaseReferenceMapImage);
                    url = String.Format(apiUrl, input.SeasonField, input.ImageId, input.MapProductType, input.ImageFormat);

                }
            }
            if (Zoning && parameters != null && parameters.NumberOfZones > 0)
            {

                if (hasUrlParameters)
                {
                    url = String.Format("{0}&zoning=true&zoneCount={1}", url, parameters.NumberOfZones);
                }
                else
                {
                    url = String.Format("{0}?zoning=true&zoneCount={1}", url, parameters.NumberOfZones);
                }
                hasUrlParameters = true;

                //Do not use hotspot for Topology, Management, Reflectance MapTypes and ColorComposition MapProduct
                if (MapType != "Topology" && MapType != "Reflectance" && MapType != "Management" && input.MapProductType != "COLORCOMPOSITION")
                {
                    // Use hotspot only if it is checked from UI and if Number of Zones is higher than 0
                    if (hotspotConfig != null && hotspotConfig.Checked && parameters.NumberOfZones > 0)
                    {
                        url = String.Format("{0}&hotSpot={1}", url, true);
                        string zoningSegmentation = hotspotConfig.Polygon ? "None" : "Polygon";
                        url = String.Format("{0}&zoningSegmentation={1}", url, zoningSegmentation);
                        if (hotspotConfig.PositionChecked && HotspotConfigToHotspotPositionParam(hotspotConfig) != null)
                        {
                            url = String.Format("{0}&hotSpotPosition={1}", url, HotspotConfigToHotspotPositionParam(hotspotConfig));
                        }
                        if (hotspotConfig.FiltersChecked)
                        {
                            url = String.Format("{0}&hotSpotFilter=top({1})|bottom({2})", url, hotspotConfig.Top, hotspotConfig.Bottom);
                        }
                    }
                }

            }
            if (Gain && parameters != null && parameters.Gain != null)
            {
                if (hasUrlParameters)
                {
                    url = String.Format("{0}&gain={1}", url, parameters.Gain);
                }
                else
                {
                    url = String.Format("{0}?gain={1}", url, parameters.Gain);
                }
                hasUrlParameters = true;
            }

            if (Offset && parameters != null && parameters.Offset != null)
            {
                if (hasUrlParameters)
                {
                    url = String.Format("{0}&offset={1}", url, parameters.Offset);
                }
                else
                {
                    url = String.Format("{0}?offset={1}", url, parameters.Offset);
                }
                hasUrlParameters = true;
            }

            return url;
        }

        public string getProductHotspotUrl(MapImageInput input, MapImageParameters parameters, HotspotConfig hotspotConfig = null)
        {
            return null;
        }

        public static List<MapProductConfig> ReadConfigFile()
        {
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Geosys_Plugin.Config.ProductsConfig.json"))
            using (StreamReader reader = new StreamReader(stream))
            {
                var jsonContent = reader.ReadToEnd();
                var config = JsonConvert.DeserializeObject<List<MapProductConfig>>(jsonContent);
                return config;
            }
        }

        public static string HotspotConfigToHotspotPositionParam(HotspotConfig hotspotConfig)
        {
            string param = null;
            string concatChar = "";
            if (hotspotConfig.PositionChecked)
            {
                if (hotspotConfig.NonePosition)
                {
                    param = String.Format("{0}{1}{2}", param, concatChar, "None");
                    concatChar = "|";
                }
                if (hotspotConfig.PointPosition)
                {
                    param = String.Format("{0}{1}{2}", param, concatChar, "PointOnSurface");
                    concatChar = "|";
                }
                if (hotspotConfig.MinimumPosition)
                {
                    param = String.Format("{0}{1}{2}", param, concatChar, "Min");
                    concatChar = "|";
                }
                if (hotspotConfig.AveragePosition)
                {
                    param = String.Format("{0}{1}{2}", param, concatChar, "Average");
                    concatChar = "|";
                }
                if (hotspotConfig.MedianPosition)
                {
                    param = String.Format("{0}{1}{2}", param, concatChar, "Median");
                    concatChar = "|";
                }
                if (hotspotConfig.MaximumPosition)
                {
                    param = String.Format("{0}{1}{2}", param, concatChar, "Max");
                    concatChar = "|";
                }
                if (hotspotConfig.AllPosition)
                {
                    param = String.Format("{0}{1}{2}", param, concatChar, "All");
                }
            }
            return param;
        }

    }
}
