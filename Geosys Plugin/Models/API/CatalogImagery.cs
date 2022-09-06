using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Geosys_Plugin.Models.API
{
    class CatalogImagery
    {
    }

    public class MapImageInput
    {
        public string SeasonField { get; set; }

        public string ImageId { get; set; }

        public string DiffImageId { get; set; }

        public string ImageFormat { get; set; }

        public string MapProductType { get; set; }

        public float NPlanned { get; set; }

        public string ImageDate { get; set; }
    }

    public class MapImageParameters
    {
        public float YieldAverage { get; set; }

        public float YieldMinimum { get; set; }

        public float YieldMaximum { get; set; }

        public float OrganicAverage { get; set; }

        public int NumberOfZones { get; set; }

        public float Gain { get; set; }

        public float Offset { get; set; }
    }

    public class HotspotParameters
    {
        public string zoningSegmentation { get; set; }

        public string hotSpotPosition { get; set; }
    }

    public class CatalogImageryInput
    {
        public string Geometry { get; set; }

        public Crop Crop { get; set; }

        public string SowingDate { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Sensor { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string MapProductType { get; set; }


    }
    public class Crop
    {
        public string Id { get; set; }
    }

    public class CatalogImageryOutput
    {
        public string CoverageType { get; set; }

        public bool IsSelected { get; set; }

        public Image Image { get; set; }

        public Map[] Maps { get; set; }

        public SeasonField SeasonField { get; set; }

        public BitmapImage ImageSrc { get; set; }
    }

    public class Image
    {

        public string Id { get; set; }

        public string[] AvailableBands { get; set; }

        public string Sensor { get; set; }

        public string SoilMaterial { get; set; }

        public long SpatialResolution { get; set; }

        public string Weather { get; set; }

        public double ZenithAngleInDegree { get; set; }

        public string Date { get; set; }
    }

    public partial class Map
    {
        public string Type { get; set; }
    }

    public partial class SeasonField
    {
        public string Id { get; set; }

        public string CustomerExternalId { get; set; }
    }
}
