using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PointCloudFilterApp
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    class ExternalCommand : IExternalCommand
    {

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {


            PointCloudFilterService.PointCloudFilterExternalCommand comm = new PointCloudFilterService.PointCloudFilterExternalCommand();
            comm.Init(commandData, ref message, elements);

            return Result.Succeeded;
        }
    }
}
