using ArcGIS.Core.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geosys_Plugin.Utils
{
    public static class GeometryUtils
    {
        public static Geometry CombineFeatures(List<Geometry> shapes)
        {
            Geometry combinedShape = null;
            for (int i = 0; i < shapes.Count; i++)
            {
                if (i == 0)
                {
                    combinedShape = shapes[0];
                }
                else
                {

                    combinedShape = GeometryEngine.Instance.Union(combinedShape, shapes[i]);
                }
            }
            return combinedShape;
        }
    }
}
