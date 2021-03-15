using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PointCloudFilterService
{
    public static class PCFilterDTO
    {
        public static PointCloudInstance PointCloud { get; set; }
        public static double FinalDistance { get; set; }
        public static double PointCloudScale { get; set; }

        public static double AverageDistance { get; set; } = 0.00328084;
        public static int NumberOfPoints { get; set; } = 100000;
        public static double NeglectFarther { get; set; } = 3.28084;



    }
}
