using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PointCloudFilterService.Helpers
{
    public class ResetView
    {
        public static void ResetPointCloudView()

        {
            try
            {
                PCFilterDTO.PointCloud.FilterAction = Autodesk.Revit.DB.SelectionFilterAction.None;
            }
            catch
            {
            }
        }
    }
}
