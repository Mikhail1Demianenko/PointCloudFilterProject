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
    public class ResetViewEventHandler : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            using(Transaction trans = new Transaction(app.ActiveUIDocument.Document))
            {
                trans.Start("Reset View");
                ResetView.ResetPointCloudView();
                trans.Commit();
            }
        }

        public string GetName()
        {
            return "Reset View";
        }
    }
}
