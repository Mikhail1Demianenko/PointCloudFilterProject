using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using PointCloudFilterService.EventHandlers;
using PointCloudFilterService.Helpers;
using PointCloudFilterWPF;

namespace PointCloudFilterService
{

    public class PointCloudFilterExternalCommand
    {
        public UIDocument uidoc { get; set; }
        public FaceServices FaceService { get; set; }

        public static MainWindow myForm; 

        public void Init(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            uidoc = commandData.Application.ActiveUIDocument;

            PickFaceEventHandler pickHandler = new PickFaceEventHandler();
            ExternalEvent pickEvent = ExternalEvent.Create(pickHandler);

            ResetViewEventHandler resetHandler = new ResetViewEventHandler();
            ExternalEvent resetEvent = ExternalEvent.Create(resetHandler);




            myForm = new MainWindow(pickEvent, resetEvent);
            FaceService = new FaceServices(uidoc);

            #region Events subscription
            myForm.PickPCClickedEvent += MyForm_PickPCClickedEvent;
            myForm.UpdateAvgdistEvent += MyForm_UpdateAvgdistEvent;
            myForm.UpdateNumberOfPoints += MyForm_UpdateNumberOfPoints;
            myForm.UpdateNeglectFartherEvent += MyForm_UpdateNeglectFartherEvent;
            myForm.SetNull += MyForm_SetNull;
            #endregion

            myForm.Show();
        }
        #region Events
        private void MyForm_SetNull()
        {
            PCFilterDTO.FinalDistance = 0;
        }  

        private void MyForm_UpdateNeglectFartherEvent(object source, UpdateEventArgs args)
        {
            PCFilterDTO.NeglectFarther = args.NeglectFarther;
        }

        private void MyForm_UpdateNumberOfPoints(object source, UpdateEventArgs args)
        {
            PCFilterDTO.NumberOfPoints = args.NumberOfPoints;
        }

        private void MyForm_UpdateAvgdistEvent(object source, UpdateEventArgs args)
        {
            PCFilterDTO.AverageDistance = args.AverageDistance;
        }

        private void MyForm_PickPCClickedEvent(object source, EventArgs args)
        {
            PointCloudServices.Pick(uidoc);
        }
        #endregion
    }
}
