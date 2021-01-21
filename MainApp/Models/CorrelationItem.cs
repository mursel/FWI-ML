using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace MainApp.Models
{
    public class CorrelationItem
    {
        public CorrelationItem() {
            ChildItems = new Dictionary<string, double>();
        }
        public string ColumnName { get; set; }
        public Dictionary<string, double> ChildItems { get; set; }

        public double OpacityLevel { get; set; }
    }
}
