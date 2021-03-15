using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PointCloudFilterWPF
{
    public class UpdateEventArgs : EventArgs
    {
        public double AverageDistance { get; set; }
        public double NeglectFarther { get; set; }
        public int NumberOfPoints { get; set; }
    }
}
