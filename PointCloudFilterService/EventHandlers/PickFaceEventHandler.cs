using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using PointCloudFilterService.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PointCloudFilterService.EventHandlers
{
    class PickFaceEventHandler : IExternalEventHandler
    {


        public void Execute(UIApplication app)
        {


            FaceServices FaceService = new FaceServices(app.ActiveUIDocument);

            using (Transaction trans = new Transaction(app.ActiveUIDocument.Document))
            {
                trans.Start("Isolate Points");
                FaceService.Initialize();
                trans.Commit();
            }
          




        }

        public string GetName()
        {
            return "Pick Face";
        }
    }
}
