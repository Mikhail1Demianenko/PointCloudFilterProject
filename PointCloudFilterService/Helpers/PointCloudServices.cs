using Autodesk.Revit.DB;
using Autodesk.Revit.DB.PointClouds;
using Autodesk.Revit.UI;
using PointCloudFilterWPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PointCloudFilterService.Helpers
{
    public class PointCloudServices
    {
        public static void Pick(UIDocument uidoc)
        {
            try
            {
                Reference pickedPc = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element, "Pick Point Cloud");

                PointCloudInstance pc = uidoc.Document.GetElement(pickedPc.ElementId) as PointCloudInstance;

                PCFilterDTO.PointCloud = pc;

                PCFilterDTO.PointCloudScale = pc.GetTransform().Scale;
            }
            catch { }


        }

        public static PointCollection FilterPointCloud(List<Plane> filteringPlanes)
        {


                PointCloudInstance pc = PCFilterDTO.PointCloud;

                PointCloudFilter filter = PointCloudFilterFactory.CreateMultiPlaneFilter(filteringPlanes);

                PointCollection pointCollection = pc.GetPoints(filter, PCFilterDTO.AverageDistance, PCFilterDTO.NumberOfPoints);
                pc.SetSelectionFilter(filter);
                pc.FilterAction = SelectionFilterAction.Isolate;

                return pointCollection;


        }
    }
}
