using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FullStackWork.Models
{
    public class CRectangle()
    {
        public Point StartPoint { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }
        public CRectangle(Point start, double h, double w) : this()
        {
            this.StartPoint = start;
            this.Height = h;
            this.Width = w;
        }
    }
}
