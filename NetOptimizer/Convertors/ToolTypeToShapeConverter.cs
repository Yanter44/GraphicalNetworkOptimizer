using NetOptimizer.Models.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace NetOptimizer.Convertors
{
    public class ToolTypeToShapeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case UIToolElementType.Label:
                    return new TextBlock { Text = "T", Foreground = Brushes.Black, FontSize = 18, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };

                case UIToolElementType.Line:
                    return new Line { X1 = 10, Y1 = 20, X2 = 35, Y2 = 20, Stroke = Brushes.Black, StrokeThickness = 2 };
                case UIToolElementType.Сurve:
                    {
                        PathFigure figure = new PathFigure
                        {
                            StartPoint = new Point(5, 20)
                        };

                        double x = 5;
                        double amplitude = 15;   
                        double step = 30;       
                        double y = 20;

                        Random rnd = new Random();

                        for (int i = 0; i < 5; i++)
                        {
                            double x1 = x + step * 0.3;
                            double x2 = x + step * 0.6;
                            double xEnd = x + step;

                            double y1 = y - amplitude + rnd.NextDouble() * 10;
                            double y2 = y + amplitude + rnd.NextDouble() * 10;
                            double yEnd = y + (rnd.NextDouble() * 10 - 5);

                            figure.Segments.Add(new BezierSegment(
                                new Point(x1, y1),
                                new Point(x2, y2),
                                new Point(xEnd, yEnd),
                                true
                            ));

                            x = xEnd;
                            y = yEnd;
                        }

                        PathGeometry geometry = new PathGeometry();
                        geometry.Figures.Add(figure);

                        return new Path
                        {
                            Stroke = Brushes.Black,
                            StrokeThickness = 2,
                            Data = geometry
                        };
                    }

                case UIToolElementType.Square:
                    return new Rectangle { Width = 20, Height = 20, Stroke = Brushes.Black, StrokeThickness = 2 };

                case UIToolElementType.Elipse:
                    return new Ellipse { Width = 20, Height = 20, Stroke = Brushes.Black, StrokeThickness = 2 };

                case UIToolElementType.Arrow:
                    return new Viewbox
                    {
                        Width = 24,
                        Height = 24,
                        Child = new Path
                        {
                            Stroke = Brushes.Black,
                            StrokeThickness = 6, 
                            Stretch = Stretch.Uniform,
                            Data = Geometry.Parse("M 10 50 L 90 50 M 65 30 L 90 50 L 65 70")
                        }
                    };
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
