using Geosys_Plugin.Models.API;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geosys_Plugin.Utils
{
    public static class FileUtils
    {
        public static string MapImageToFileName(MapImageInput mapImage, MapImageParameters parameters, string hotspotPrefix = null)
        {

            string fileName = null;
            if (hotspotPrefix != null)
            {
                fileName = string.Format("{0}_{1}_{2}", hotspotPrefix, mapImage.SeasonField, mapImage.ImageDate);
            }
            else
            {
                fileName = string.Format("{0}_{1}_zones_{2}_{3}", mapImage.MapProductType, parameters.NumberOfZones, mapImage.SeasonField, mapImage.ImageDate);
            }

            return fileName;
        }

        public static FileInfo incrementNameIfFileExists(string fullPath)
        {
            FileInfo fileInfo = new FileInfo(fullPath);
            string fileExtension = fileInfo.Extension;
            string fileNameWithoutExtension = Path.ChangeExtension(fullPath, null);
            int fileIncrement = 1;
            while (fileInfo.Exists)
            {
                fileInfo = new FileInfo(fileNameWithoutExtension + "_" + fileIncrement + fileExtension);
                fileIncrement++;
            }
            return fileInfo;
        }
    }
}
