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
    public class UserCanvas_Model
    {
        public Canvas CurrentCanvas { get; set; }
        public Point StartPoint { get; set; }
        public Point EndPoint { get; set; }      
        public Point UserPoint { get; set; }
        public UIElement SelectShape { get; set; }
        public string SelectAction { get; set; }      
    }
}
