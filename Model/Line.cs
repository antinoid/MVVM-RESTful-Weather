using System.Windows;

namespace Model
{
    public class Line
    {
        public Point From { get; set; }
        public Point To { get; set; }
        public string Color { get; set; }

        public Line(Point from, Point to, string color)
        {
            From = from;
            To = to;
            Color = color;
        }
    }
}
