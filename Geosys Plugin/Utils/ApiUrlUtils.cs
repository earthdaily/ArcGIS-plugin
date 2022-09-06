using Geosys_Plugin.Models;
using Geosys_Plugin.Models.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geosys_Plugin.Utils
{
    public static class ApiUrlUtils
    {
        public static string getUrl(MapImageInput input, MapImageParameters parameters)
        {
            CatalogImageryUrls _catalogImageryUrls = new APIUrls().catalogImagery;
            string url = null;
            bool hasUrlParameters = false;
            List<string> ModelMapValues = new List<string> { "INSEASONFIELD_AVERAGE_REVERSE_NDVI", "INSEASONFIELD_AVERAGE_NDVI", "INSEASONFIELD_AVERAGE_LAI", "INSEASONFIELD_AVERAGE_REVERSE_LAI" };
            if (ModelMapValues.Contains(input.MapProductType))
            {
                string apiUrl = string.Format(@"{0}/{1}", _catalogImageryUrls.GetBaseUrl(), _catalogImageryUrls.ModelMap);
                url = String.Format(apiUrl, input.SeasonField, input.ImageId, input.MapProductType, input.NPlanned, input.ImageFormat);
            }
            else if (input.MapProductType == "SAMZ")
            {
                string apiUrl = string.Format(@"{0}/{1}", _catalogImageryUrls.GetBaseUrl(), _catalogImageryUrls.ManagementZonesMap);
                url = String.Format(apiUrl, input.SeasonField, input.MapProductType, input.ImageFormat);
                url = String.Format("{0}?imageReferences={1}", url, input.ImageId);
            }
            else if (input.MapProductType == "OM")
            {
                string apiUrl = string.Format(@"{0}/{1}", _catalogImageryUrls.GetBaseUrl(), _catalogImageryUrls.OrganicMatterMap);
                url = String.Format(apiUrl, input.SeasonField, input.ImageId, input.ImageFormat);
            }
            else if (input.MapProductType == "ELEVATION")
            {
                string apiUrl = string.Format(@"{0}/{1}", _catalogImageryUrls.GetBaseUrl(), _catalogImageryUrls.TopologyMap);
                url = String.Format(apiUrl, input.SeasonField, input.ImageFormat);
            }
            else if (input.MapProductType == "YGM")
            {
                string apiUrl = string.Format(@"{0}/{1}", _catalogImageryUrls.GetBaseUrl(), _catalogImageryUrls.YgmMap);
                url = String.Format(apiUrl, input.SeasonField, input.ImageId, input.ImageFormat);
            }
            else if (input.MapProductType == "YPM")
            {
                string apiUrl = string.Format(@"{0}/{1}", _catalogImageryUrls.GetBaseUrl(), _catalogImageryUrls.YpmMap);
                url = String.Format(apiUrl, input.SeasonField, input.ImageId, 80, input.ImageFormat);
            }
            else
            {
                string apiUrl = string.Format(@"{0}/{1}", _catalogImageryUrls.GetBaseUrl(), _catalogImageryUrls.BaseReferenceMapImage);
                url = String.Format(apiUrl, input.SeasonField, input.ImageId, input.MapProductType, input.ImageFormat);
            }
            if (parameters != null && parameters.NumberOfZones > 0)
            {
                if (hasUrlParameters)
                {
                    url = String.Format("{0}&zoning=true&zoneCount={1}", url, parameters.NumberOfZones);
                }
                else
                {
                    url = String.Format("{0}?zoning=true&zoneCount={1}", url, parameters.NumberOfZones);
                }
            }
            return url;
        }
    }
}
