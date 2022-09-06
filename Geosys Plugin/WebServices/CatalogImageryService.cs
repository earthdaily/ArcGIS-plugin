using Geosys_Plugin.Models;
using Geosys_Plugin.Models.API;
using Geosys_Plugin.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Geosys_Plugin.WebServices
{
    public class CatalogImageryService : WebServiceBase
    {
        private AuthenticationService _authenticationService;
        CatalogImageryUrls _catalogImageryUrls;

        public CatalogImageryService()
        {
            _catalogImageryUrls = new APIUrls().catalogImagery;
            _authenticationService = new AuthenticationService();
        }

        public async Task<List<CatalogImageryOutput>> GetCatalogImagery(CatalogImageryInput input, string startDate, string endDate)
        {
            string imageDateParam = null;
            string mapsTypeParam = null;

            //Map Product Type Values for which Maps.Type parameter can be used
            var MapsTypeParameterValues = new List<string> { "INSEASON_NDVI", "INSEASON_EVI", "INSEASON_CVI", "INSEASON_GNDVI", "INSEASONPARTIAL_NDVI", "INSEASONPARTIAL_EVI", "INSEASON_LAI", "INSEASONFIELD_AVERAGE_REVERSE_NDVI", "INSEASONFIELD_AVERAGE_NDVI", "OM", "YGM", "YPM", "COLORCOMPOSITION", "INSEASON_S2REP", "SAMZ" };
            var match = MapsTypeParameterValues.FirstOrDefault(val => val.Contains(input.MapProductType));
            if (match != null)
            {
                mapsTypeParam = string.Format("{0}={1}", "Maps.Type", input.MapProductType);
            }

            if (startDate != null)
            {
                if (endDate != null)
                {
                    imageDateParam = string.Format("{0}{1}|{2}", "Image.Date=$between:", startDate, endDate);
                }
                else
                {
                    imageDateParam = string.Format("{0}{1}", "Image.Date=$gte:", startDate);
                }
            }
            else if (endDate != null)
            {
                imageDateParam = string.Format("{0}{1}", "Image.Date=$lte:", endDate);
            }
            await _authenticationService.RenewAuthenticationTokenIfNeeded();
            string url = string.Format(@"{0}/{1}", _catalogImageryUrls.GetBaseUrl(), _catalogImageryUrls.CatalogImagery);
            string coverageTypeParam;
            if (input.MapProductType == "INSEASONPARTIAL_EVI" || input.MapProductType == "INSEASONPARTIAL_NDVI")
            {
                coverageTypeParam = String.Format("{0}={1}", "coverageType", "CLOUD");
            } else
            {
                coverageTypeParam = String.Format("{0}={1}", "coverageType", "CLEAR");
            }
            if (input.Sensor == null || input.Sensor == "ALL_SENSORS")
            {
                if (imageDateParam != null)
                {
                    url = string.Format(@"{0}?{1}&{2}", url, imageDateParam, coverageTypeParam);
                }
                else
                {
                    url = string.Format(@"{0}?{1}", url, coverageTypeParam);
                }
            }
            else
            {
                if (imageDateParam != null)
                {
                    url = string.Format(@"{0}?{1}&{2}&Image.Sensor={3}", url, imageDateParam, coverageTypeParam, input.Sensor);
                }
                else
                {
                    url = string.Format(@"{0}?{1}&Image.Sensor={2}", url, coverageTypeParam, input.Sensor);
                }
            }
            if (mapsTypeParam != null)
            {
                url = string.Format(@"{0}&{1}", url, mapsTypeParam);
            }
            input.Sensor = null;
            var json = JsonConvert.SerializeObject(input);
            List<CatalogImageryOutput> outputs = null;
            outputs = await PostRequestWithAuthentication<List<CatalogImageryOutput>>(url, json);
            return outputs;
        }

        public async Task<BitmapImage> GetBaseReferenceMapImage(MapImageInput input)
        {
            await _authenticationService.RenewAuthenticationTokenIfNeeded();
            BitmapImage img = null;
            string apiUrl = string.Format(@"{0}/{1}", _catalogImageryUrls.GetBaseUrl(), _catalogImageryUrls.BaseReferenceMapImage);
            string urlWithParams = String.Format(apiUrl, input.SeasonField, input.ImageId, input.MapProductType, input.ImageFormat);

            img = await GetImageRequestByAuthentication<BitmapImage>(urlWithParams);
            return img;
        }

        public async Task<BitmapImage> GetManagementZonesMap(MapImageInput input)
        {
            await _authenticationService.RenewAuthenticationTokenIfNeeded();
            BitmapImage img = null;
            string apiUrl = string.Format(@"{0}/{1}", _catalogImageryUrls.GetBaseUrl(), _catalogImageryUrls.ManagementZonesMap);
            string urlWithParams = String.Format(apiUrl, input.SeasonField, input.MapProductType, input.ImageFormat);
            urlWithParams = String.Format("{0}?imageReferences={1}", urlWithParams, input.ImageId);
            img = await GetImageRequestByAuthentication<BitmapImage>(urlWithParams);
            return img;
        }

        public async Task<BitmapImage> GetOrganicMatterMap(MapImageInput input)
        {
            await _authenticationService.RenewAuthenticationTokenIfNeeded();
            BitmapImage img = null;
            string apiUrl = string.Format(@"{0}/{1}", _catalogImageryUrls.GetBaseUrl(), _catalogImageryUrls.OrganicMatterMap);
            string urlWithParams = String.Format(apiUrl, input.SeasonField, input.ImageId, input.ImageFormat);
            img = await GetImageRequestByAuthentication<BitmapImage>(urlWithParams);
            return img;
        }

        public async Task<BitmapImage> GetTopologyMap(MapImageInput input)
        {
            await _authenticationService.RenewAuthenticationTokenIfNeeded();
            BitmapImage img = null;
            string apiUrl = string.Format(@"{0}/{1}", _catalogImageryUrls.GetBaseUrl(), _catalogImageryUrls.TopologyMap);
            string urlWithParams = String.Format(apiUrl, input.SeasonField, input.ImageFormat);
            img = await GetImageRequestByAuthentication<BitmapImage>(urlWithParams);
            return img;
        }

        public async Task<BitmapImage> GetYGMMap(MapImageInput input)
        {
            await _authenticationService.RenewAuthenticationTokenIfNeeded();
            BitmapImage img = null;
            string apiUrl = string.Format(@"{0}/{1}", _catalogImageryUrls.GetBaseUrl(), _catalogImageryUrls.YgmMap);
            string urlWithParams = String.Format(apiUrl, input.SeasonField, input.ImageId, input.ImageFormat);
            img = await GetImageRequestByAuthentication<BitmapImage>(urlWithParams);
            return img;
        }

        public async Task<BitmapImage> GetYPMMap(MapImageInput input)
        {
            await _authenticationService.RenewAuthenticationTokenIfNeeded();
            BitmapImage img = null;
            string apiUrl = string.Format(@"{0}/{1}", _catalogImageryUrls.GetBaseUrl(), _catalogImageryUrls.YpmMap);
            string urlWithParams = String.Format(apiUrl, input.SeasonField, input.ImageId, 80, input.ImageFormat);
            img = await GetImageRequestByAuthentication<BitmapImage>(urlWithParams);
            return img;
        }

        public async Task<BitmapImage> GetImageMapByUrl(string url)
        {
            await _authenticationService.RenewAuthenticationTokenIfNeeded();
            BitmapImage img = null;

            img = await GetImageRequestByAuthentication<BitmapImage>(url);
            return img;
        }

        public async Task<BitmapImage> GetModelMap(string url)
        {
            await _authenticationService.RenewAuthenticationTokenIfNeeded();
            BitmapImage img = null;

            img = await GetImageRequestByAuthentication<BitmapImage>(url);
            return img;
        }

        public async Task<BitmapImage> GetReflectanceMap(MapImageInput input)
        {
            await _authenticationService.RenewAuthenticationTokenIfNeeded();
            BitmapImage image = null;
            string apiUrl = string.Format(@"{0}/{1}", _catalogImageryUrls.GetBaseUrl(), _catalogImageryUrls.ReflectanceMap);
            string urlWithParams = String.Format(apiUrl, input.SeasonField, input.ImageId, input.MapProductType);
            var tifPath = await GetTifZipAndSaveAsTif<string>(urlWithParams, null);
            image = new BitmapImage(new Uri(tifPath));
            return image;
        }

        public async Task<string> SaveMapTiffImageByUrl(string url, string fileName)
        {
            await _authenticationService.RenewAuthenticationTokenIfNeeded();
            string tifPath = null;

            tifPath = await GetTifZipAndSaveAsTif<string>(url, fileName);
            return tifPath;
        }

        public async Task<string> SaveMapShapeByUrl(string url, string fileName)
        {
            await _authenticationService.RenewAuthenticationTokenIfNeeded();
            string shapePath = null;
            shapePath = await GetShapeZipAndSaveLocally<string>(url, fileName);
            return shapePath;
        }

        public async Task<string> SavePngShapeByUrl(string pngUrl, string pgwUrl, string legendUrl, string fileName)
        {
            string shapePath = await GetPngAndSaveLocally<string>(pngUrl, pgwUrl, legendUrl, fileName);
            return shapePath;
        }

        public async Task<string> SaveKmzShapeByUrl(string url, string fileName)
        {
            string shapePath = await GetKmzAndSaveLocally<string>(url, fileName);
            return shapePath;
        }

        public async Task<string> SaveMapTiffImage(MapImageInput input, MapImageParameters parameters)
        {
            await _authenticationService.RenewAuthenticationTokenIfNeeded();
            string tifPath = null;
            string urlWithParams = ApiUrlUtils.getUrl(input, parameters);
            string fileName = FileUtils.MapImageToFileName(input, parameters);
            tifPath = await GetTifZipAndSaveAsTif<string>(urlWithParams, fileName);
            return tifPath;
        }

        public async Task<string> SaveMapShape(MapImageInput input, MapImageParameters parameters)
        {
            await _authenticationService.RenewAuthenticationTokenIfNeeded();
            string shapePath = null;
            string urlWithParams = ApiUrlUtils.getUrl(input, parameters);
            string fileName = FileUtils.MapImageToFileName(input, parameters);
            shapePath = await GetShapeZipAndSaveLocally<string>(urlWithParams, fileName);
            return shapePath;
        }


        public async Task<string> SaveMapKmz(MapImageInput input, MapImageParameters parameters)
        {
            await _authenticationService.RenewAuthenticationTokenIfNeeded();
            string shapePath = null;
            string kmzUrl = ApiUrlUtils.getUrl(input, parameters);
            string fileName = FileUtils.MapImageToFileName(input, parameters);
            shapePath = await GetKmzAndSaveLocally<string>(kmzUrl, fileName);
            return shapePath;
        }

    }
}
