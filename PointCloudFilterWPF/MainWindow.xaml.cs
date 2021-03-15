using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PointCloudFilterWPF
{
    public partial class MainWindow : Window
    {

        #region Delegate and events
        // the usual way with delegates and events will work since thehe is no transaction. The other two are done through IExtEventHandlers
        public delegate void PickPCClickedEventHandler(object source, EventArgs args);
        public event PickPCClickedEventHandler PickPCClickedEvent;


        public delegate void UpdateAverageDistanceEventHandler(object source, UpdateEventArgs args);
        public event UpdateAverageDistanceEventHandler UpdateAvgdistEvent;

        public delegate void UpdateNumberOfPointsEventHandler(object source, UpdateEventArgs args);
        public event UpdateNumberOfPointsEventHandler UpdateNumberOfPoints;

        public delegate void UpdateNeglectFartherEventHandler(object source, UpdateEventArgs args);
        public event UpdateNeglectFartherEventHandler UpdateNeglectFartherEvent;

        public delegate void SetDistanceNullEventHandler();
        public event SetDistanceNullEventHandler SetNull;
        #endregion

        private static readonly Regex _regex = new Regex("[^0-9]+");

        private ExternalEvent _pickEvent { get; set; }

        private ExternalEvent _resetEvent { get; set; }

        public static double AvgDist { get; private set; }
        public static int NumberOfPoints { get; private set; }
        public static double NeglectFarther { get; private set; }

        public MainWindow(ExternalEvent pickEvent, ExternalEvent resetEvent)
        {
            InitializeComponent();

            _pickEvent = pickEvent;
            _resetEvent = resetEvent;

            
        }


        public void UpdateDistance(string valueToUpdate)
        {
            distLabel.Content = valueToUpdate; // todo: dont forget conversion
        }

        #region Input Validation
        private void avgDist_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = _regex.IsMatch(e.Text);
        }

        private void numPoints_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = _regex.IsMatch(e.Text);
            
        }

        private void neglectFarther_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = _regex.IsMatch(e.Text);
        }




        #endregion

        private void pickPcButton_Click_1(object sender, RoutedEventArgs e)
        {
            PickPCClickedEvent(this, EventArgs.Empty);
        }

        private void pickFaceButton_Click_1(object sender, RoutedEventArgs e)
        {


            NumberOfPoints = Int32.Parse(numPoints.Text);
            AvgDist = Int32.Parse(avgDist.Text) * 0.00328084;
            NeglectFarther = Int32.Parse(neglectFarther.Text) * 0.00328084;


            _pickEvent.Raise();
        }

        private void resetViewButton_Click_1(object sender, RoutedEventArgs e)
        {
            _resetEvent.Raise();
            SetNull();
        }

        private void avgDist_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdateAvgdistEvent(this, new UpdateEventArgs() { AverageDistance = Int32.Parse(avgDist.Text) * 0.00328084});
        }

        private void numPoints_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdateNumberOfPoints(this, new UpdateEventArgs() { NumberOfPoints = Int32.Parse(numPoints.Text) });
        }

        private void neglectFarther_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdateNeglectFartherEvent(this, new UpdateEventArgs() { NeglectFarther = Int32.Parse(neglectFarther.Text) * 0.00328084 });
        }
    }
}
