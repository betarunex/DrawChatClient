using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DrawingClient
{
    public class MyLine
    {
        public double X1;
        public double Y1;
        public double X2;
        public double Y2;
        public double thickness;
        public byte[] rgb = new byte[3];

        public static Line newLine(double x1, double y1, double x2, double y2, double thickness,byte[] rgb)
        {
            Line line = new Line();
            line.X1 = x1;
            line.Y1 = y1;
            line.X2 = x2;
            line.Y2 = y2;
            line.StrokeThickness = thickness;
            line.Stroke = new SolidColorBrush(Color.FromRgb(rgb[0], rgb[1], rgb[2]));
            return line;
        }
    }
}
