using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.IO;
using System.Reflection;

namespace Geosys_Plugin.Models
{
    class APIUrls
    {
        public AuthenticationUrls authentication { get; set; }
        public CatalogImageryUrls catalogImagery { get; set; }

        public WebUrls web { get; set; }

        public APIUrls()
        {
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Geosys_Plugin.Config.APIUrls.json"))
            using (StreamReader reader = new StreamReader(stream))
            {
                var jsonContent = reader.ReadToEnd();
                JsonConvert.PopulateObject(jsonContent, this);
            }
        }

    }


    public class AuthenticationUrls
    {
        public string BaseUrl { get; set; }

        public string BaseTestUrl { get; set; }

        public string GetToken { get; set; }
    }

    public class CatalogImageryUrls
    {
        public string BaseUrl { get; set; }

        public string BaseTestUrl { get; set; }

        public string BaseReferenceMapImage { get; set; }

        public string DifferenceMap { get; set; }

        public string ManagementZonesMap { get; set; }

        public string OrganicMatterMap { get; set; }

        public string ReflectanceMap { get; set; }

        public string SoilMap { get; set; }

        public string TopologyMap { get; set; }

        public string ModelMap { get; set; }

        public string YgmMap { get; set; }

        public string YpmMap { get; set; }

        public string CatalogImagery { get; set; }

        public string GetBaseUrl()
        {
            string baseUrl = null;
            if (Properties.Settings.Default.UseTestingService)
            {
                baseUrl = BaseTestUrl;
            }
            else
            {
                baseUrl = BaseUrl;
            }
            return baseUrl;
        }
    }

    public class WebUrls
    {
        public string AboutUrl { get; set; }

        public string CredentialUrl { get; set; }

        public string HelpUrl { get; set; }
    }

}




