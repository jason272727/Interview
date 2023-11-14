using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace ViewSonic
{
    public class MainWindow_Model
    {      
        public string UserAction { get; set; }       
        public int ResizingPoint { get; set; }       
        public Rectangle rectangle { get; set; }
        public Polygon Triangle { get; set; }
        public Ellipse ellipse { get; set; }
       
    }
}
