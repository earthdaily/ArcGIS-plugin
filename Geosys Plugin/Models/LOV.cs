using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Geosys_Plugin.Models
{
    public class LOV
    {
        public List<string> MapProductTypes { get; set; }
        public List<string> Sensors { get; set; }

        public List<string> CropTypes { get; set; }

        public static LOV ReadLovFile()
        {
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Geosys_Plugin.Config.LOV.json"))
            using (StreamReader reader = new StreamReader(stream))
            {
                var jsonContent = reader.ReadToEnd();
                var config = JsonConvert.DeserializeObject<LOV>(jsonContent);
                return config;
            }
        }
    }
}
