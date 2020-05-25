using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BubbleBot.Views.Accounts.MapViewer
{
    public class MapViewerCell : UIElement
    {
        #region UIElementHeritage
        private readonly UIElementCollection children;
        public void AddChild(UIElement element)
        {
            children.Add(element);
        }

        public void RemoveChild(UIElement element)
        {
            children.Remove(element);
        }

        protected override int VisualChildrenCount
        {
            get { return children.Count; }
        }

        protected override Visual GetVisualChild(int index)
        {
            return children[index];
        }

        protected override Size MeasureCore(Size availableSize)
        {
            foreach (UIElement element in children)
            {
                element.Measure(availableSize);
            }

            return new Size();
        }

        protected override void ArrangeCore(Rect finalRect)
        {
            foreach (UIElement element in children)
            {
                element.Arrange(finalRect);
            }
        }

        #endregion

        // Constructor
        public MapViewerCell(Point[] points)
        {
            children = new UIElementCollection(this, null);
            Points = points;
        }

        // Properties
        public Point[] Points { get; }


        public void Draw(DrawingContext drawingContext, Brush brush, Pen pen, bool realmap)
        {
            DrawPolygonOrPolyline(drawingContext, brush, pen, Points, realmap);
        }

        public void DrawObstacle(DrawingContext drawingContext, Brush brush, Pen pen)
        {
            var newPoints = new Point[15];

            newPoints[0] = new Point(Points[3].X, Points[3].Y - 10);
            newPoints[1] = new Point(Points[0].X, Points[0].Y - 10);
            newPoints[2] = new Point(Points[1].X, Points[1].Y - 10);
            newPoints[3] = new Point(Points[2].X, Points[2].Y - 10);
            newPoints[4] = new Point(Points[3].X, Points[3].Y - 10);

            newPoints[5] = new Point(Points[3].X, Points[3].Y - 10);
            newPoints[6] = new Point(Points[2].X, Points[2].Y - 10);
            newPoints[7] = Points[2];
            newPoints[8] = Points[3];
            newPoints[9] = new Point(Points[3].X, Points[3].Y - 10);

            newPoints[10] = new Point(Points[2].X, Points[2].Y - 10);
            newPoints[11] = new Point(Points[1].X, Points[1].Y - 10);
            newPoints[12] = Points[1];
            newPoints[13] = Points[2];
            newPoints[14] = new Point(Points[2].X, Points[2].Y - 10);

            DrawPolygonOrPolyline(drawingContext, brush, pen, newPoints, false);
        }

        public void DrawPie(DrawingContext drawingContext, Brush brush)
        {
            drawingContext.DrawEllipse(brush, null, new Point(Points[0].X, Points[1].Y), 5, 5);
        }

        public void DrawPlayer(DrawingContext drawingContext, Brush brush)
        {

            drawingContext.DrawEllipse(brush, null, new Point(Points[0].X, Points[1].Y), 5, 5);
        }

        public void DrawRectangle(DrawingContext drawingContext, Brush brush)
        {
            drawingContext.DrawRectangle(brush, null, new Rect(Points[0].X - 5, Points[1].Y - 5, 10, 10));
        }

        public void DrawCross(DrawingContext drawingContext, Pen pen)
        {
            drawingContext.DrawLine(pen, new Point(Points[0].X - 4, Points[0].Y + 8),
                new Point(Points[0].X + 4, Points[1].Y + 2)); ;
            drawingContext.DrawLine(pen, new Point(Points[0].X + 4, Points[0].Y + 8),
                new Point(Points[0].X - 4, Points[1].Y + 2));
        }

        public void DrawImage(DrawingContext drawingContext, ImageSource image)
        {
            drawingContext.DrawImage(image, new Rect(Points[0].X - 10, Points[1].Y - 10, 20, 20));
        }

        public bool IsPointInside(Point pos)
        {
            var inside = false;

            var j = Points.Length - 1;
            for (var i = 0; i < Points.Length; i++)
            {
                if (Points[i].Y < pos.Y && Points[j].Y >= pos.Y || Points[j].Y < pos.Y && Points[i].Y >= pos.Y)
                    if (Points[i].X + (pos.Y - Points[i].Y) / (Points[j].Y - Points[i].Y) *
                        (Points[j].X - Points[i].X) < pos.X)
                        inside = !inside;
                j = i;
            }

            return inside;
        }


        private static void DrawPolygonOrPolyline(DrawingContext drawingContext, Brush brush, Pen pen, Point[] points, bool realMap)
        {
            // Make a StreamGeometry to hold the drawing objects.
            var geo = new StreamGeometry();
            //geo.FillRule = fill_rule;

            // Open the context to use for drawing.
            using (var context = geo.Open())
            {
                // Start at the first point.
                context.BeginFigure(points[0], true, true);

                // Add the points after the first one.
                context.PolyLineTo(points.Skip(1).ToArray(), true, false);
            }

            if (realMap)
            {
                drawingContext.DrawGeometry(brush, new Pen(new SolidColorBrush(Colors.White) { Opacity = 0.5 }, 1), geo);
            }
            else
            {
                drawingContext.DrawGeometry(brush, pen, geo);
            }
            // Draw.
        }
    }
}