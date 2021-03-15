using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Reflection;
using System.Windows.Media.Imaging;
using PointCloudFilterApp.Properties;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using PointCloudFilterService;
using System.Windows.Media;
using System.Windows.Interop;
using System.Windows;

namespace PointCloudFilterApp
{
    public class App : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication a)
        {
            AddButton(a);

            a.Idling += A_Idling;

            return Result.Succeeded;
        }

        private void A_Idling(object sender, Autodesk.Revit.UI.Events.IdlingEventArgs e)
        {
            PointCloudFilterExternalCommand.myForm.UpdateDistance(Math.Round(PCFilterDTO.FinalDistance * 304.8).ToString());
        }

        public Result OnShutdown(UIControlledApplication a)
        {
            return Result.Succeeded;
        }

        public void AddButton(UIControlledApplication application)
        {

            string PanelName = "My Tools";
            RibbonPanel PanelRibbon = application.CreateRibbonPanel(PanelName);

            string msg = "Allows to simplify the fine-tuting process of a roughly placed model to better fit a Point Cloud";

            PushButtonData wkst = new PushButtonData("revitFilter", "Point Cloud" + Environment.NewLine + "Filter", Assembly.GetExecutingAssembly().Location, "PointCloudFilterApp.ExternalCommand")
            {
                ToolTip = msg
            };


   
            PushButton pb = PanelRibbon.AddItem(wkst) as PushButton;

            Bitmap bitmap = Resources.MainIcon;
            IntPtr hBitmap = bitmap.GetHbitmap();

            ImageSource wpfBitmap =
                 Imaging.CreateBitmapSourceFromHBitmap(
                      hBitmap, IntPtr.Zero, Int32Rect.Empty,
                      BitmapSizeOptions.FromEmptyOptions());




            // Apply image to button 
            pb.LargeImage = wpfBitmap;


            
        }

    }
}

