using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL
{
    public class MyDropShadow : Decorator
    {
        public Color Color
        {
            get
            {
                object value = base.GetValue(MyDropShadow.ColorProperty);
                if (value == null)
                {
                    return default(Color);
                }
                return (Color)value;
            }
            set
            {
                base.SetValue(MyDropShadow.ColorProperty, value);
            }
        }

        public double ShadowRadius
        {
            get
            {
                return Conversions.ToDouble(base.GetValue(MyDropShadow.ShadowRadiusProperty));
            }
            set
            {
                base.SetValue(MyDropShadow.ShadowRadiusProperty, value);
            }
        }

        public CornerRadius CornerRadius
        {
            get
            {
                object value = base.GetValue(MyDropShadow.CornerRadiusProperty);
                if (value == null)
                {
                    return default(CornerRadius);
                }
                return (CornerRadius)value;
            }
            set
            {
                base.SetValue(MyDropShadow.CornerRadiusProperty, value);
            }
        }

        private static bool IsCornerRadiusValid(object value)
        {
            CornerRadius cornerRadius = ((value != null) ? ((CornerRadius)value) : default(CornerRadius));
            return cornerRadius.TopLeft >= 0.0 && cornerRadius.TopRight >= 0.0 && cornerRadius.BottomLeft >= 0.0 && cornerRadius.BottomRight >= 0.0 && !double.IsNaN(cornerRadius.TopLeft) && !double.IsNaN(cornerRadius.TopRight) && !double.IsNaN(cornerRadius.BottomLeft) && !double.IsNaN(cornerRadius.BottomRight) && !double.IsInfinity(cornerRadius.TopLeft) && !double.IsInfinity(cornerRadius.TopRight) && !double.IsInfinity(cornerRadius.BottomLeft) && !double.IsInfinity(cornerRadius.BottomRight);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            CornerRadius cornerRadius = this.CornerRadius;
            Rect rect = new Rect(0.0, 0.0, base.RenderSize.Width, base.RenderSize.Height);
            Color color = this.Color;
            if (rect.Width > 0.0 && rect.Height > 0.0 && color.A > 0)
            {
                double num = rect.Right - rect.Left - 2.0 * this.ShadowRadius;
                double num2 = rect.Bottom - rect.Top - 2.0 * this.ShadowRadius;
                double num3 = Math.Min(num * 0.5, num2 * 0.5);
                cornerRadius.TopLeft = Math.Min(cornerRadius.TopLeft, num3);
                cornerRadius.TopRight = Math.Min(cornerRadius.TopRight, num3);
                cornerRadius.BottomLeft = Math.Min(cornerRadius.BottomLeft, num3);
                cornerRadius.BottomRight = Math.Min(cornerRadius.BottomRight, num3);
                Brush[] brushes = this.GetBrushes(color, cornerRadius);
                double num4 = rect.Top + this.ShadowRadius;
                double num5 = rect.Left + this.ShadowRadius;
                double num6 = rect.Right - this.ShadowRadius;
                double num7 = rect.Bottom - this.ShadowRadius;
                double[] array = new double[]
                {
                    num5,
                    num5 + cornerRadius.TopLeft,
                    num6 - cornerRadius.TopRight,
                    num5 + cornerRadius.BottomLeft,
                    num6 - cornerRadius.BottomRight,
                    num6
                };
                double[] array2 = new double[]
                {
                    num4,
                    num4 + cornerRadius.TopLeft,
                    num4 + cornerRadius.TopRight,
                    num7 - cornerRadius.BottomLeft,
                    num7 - cornerRadius.BottomRight,
                    num7
                };
                drawingContext.PushGuidelineSet(new GuidelineSet(array, array2));
                cornerRadius.TopLeft += this.ShadowRadius;
                cornerRadius.TopRight += this.ShadowRadius;
                cornerRadius.BottomLeft += this.ShadowRadius;
                cornerRadius.BottomRight += this.ShadowRadius;
                Rect rect2 = new Rect(rect.Left, rect.Top, cornerRadius.TopLeft, cornerRadius.TopLeft);
                drawingContext.DrawRectangle(brushes[0], null, rect2);
                double num8 = array[2] - array[1];
                if (num8 > 0.0)
                {
                    Rect rect3 = new Rect(array[1], rect.Top, num8, this.ShadowRadius);
                    drawingContext.DrawRectangle(brushes[1], null, rect3);
                }
                Rect rect4 = new Rect(array[2], rect.Top, cornerRadius.TopRight, cornerRadius.TopRight);
                drawingContext.DrawRectangle(brushes[2], null, rect4);
                double num9 = array2[3] - array2[1];
                if (num9 > 0.0)
                {
                    Rect rect5 = new Rect(rect.Left, array2[1], this.ShadowRadius, num9);
                    drawingContext.DrawRectangle(brushes[3], null, rect5);
                }
                double num10 = array2[4] - array2[2];
                if (num10 > 0.0)
                {
                    Rect rect6 = new Rect(array[5], array2[2], this.ShadowRadius, num10);
                    drawingContext.DrawRectangle(brushes[5], null, rect6);
                }
                Rect rect7 = new Rect(rect.Left, array2[3], cornerRadius.BottomLeft, cornerRadius.BottomLeft);
                drawingContext.DrawRectangle(brushes[6], null, rect7);
                double num11 = array[4] - array[3];
                if (num11 > 0.0)
                {
                    Rect rect8 = new Rect(array[3], array2[5], num11, this.ShadowRadius);
                    drawingContext.DrawRectangle(brushes[7], null, rect8);
                }
                Rect rect9 = new Rect(array[4], array2[4], cornerRadius.BottomRight, cornerRadius.BottomRight);
                drawingContext.DrawRectangle(brushes[8], null, rect9);
                if (cornerRadius.TopLeft == this.ShadowRadius && cornerRadius.TopLeft == cornerRadius.TopRight && cornerRadius.TopLeft == cornerRadius.BottomLeft && cornerRadius.TopLeft == cornerRadius.BottomRight)
                {
                    Rect rect10 = new Rect(array[0], array2[0], num, num2);
                    drawingContext.DrawRectangle(brushes[4], null, rect10);
                }
                else
                {
                    PathFigure pathFigure = new PathFigure();
                    if (cornerRadius.TopLeft > this.ShadowRadius)
                    {
                        pathFigure.StartPoint = new Point(array[1], array2[0]);
                        pathFigure.Segments.Add(new LineSegment(new Point(array[1], array2[1]), true));
                        pathFigure.Segments.Add(new LineSegment(new Point(array[0], array2[1]), true));
                    }
                    else
                    {
                        pathFigure.StartPoint = new Point(array[0], array2[0]);
                    }
                    if (cornerRadius.BottomLeft > this.ShadowRadius)
                    {
                        pathFigure.Segments.Add(new LineSegment(new Point(array[0], array2[3]), true));
                        pathFigure.Segments.Add(new LineSegment(new Point(array[3], array2[3]), true));
                        pathFigure.Segments.Add(new LineSegment(new Point(array[3], array2[5]), true));
                    }
                    else
                    {
                        pathFigure.Segments.Add(new LineSegment(new Point(array[0], array2[5]), true));
                    }
                    if (cornerRadius.BottomRight > this.ShadowRadius)
                    {
                        pathFigure.Segments.Add(new LineSegment(new Point(array[4], array2[5]), true));
                        pathFigure.Segments.Add(new LineSegment(new Point(array[4], array2[4]), true));
                        pathFigure.Segments.Add(new LineSegment(new Point(array[5], array2[4]), true));
                    }
                    else
                    {
                        pathFigure.Segments.Add(new LineSegment(new Point(array[5], array2[5]), true));
                    }
                    if (cornerRadius.TopRight > this.ShadowRadius)
                    {
                        pathFigure.Segments.Add(new LineSegment(new Point(array[5], array2[2]), true));
                        pathFigure.Segments.Add(new LineSegment(new Point(array[2], array2[2]), true));
                        pathFigure.Segments.Add(new LineSegment(new Point(array[2], array2[0]), true));
                    }
                    else
                    {
                        pathFigure.Segments.Add(new LineSegment(new Point(array[5], array2[0]), true));
                    }
                    pathFigure.IsClosed = true;
                    pathFigure.Freeze();
                    PathGeometry pathGeometry = new PathGeometry();
                    pathGeometry.Figures.Add(pathFigure);
                    pathGeometry.Freeze();
                    drawingContext.DrawGeometry(brushes[4], null, pathGeometry);
                }
                drawingContext.Pop();
            }
        }

        private static void ClearBrushes(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((MyDropShadow)o)._brushes = null;
        }

        private GradientStopCollection CreateStops(Color c, double cornerRadius)
        {
            double num = 1.0 / (this.ShadowRadius + cornerRadius);
            GradientStopCollection gradientStopCollection = new GradientStopCollection();
            Color color = c;
            gradientStopCollection.Add(new GradientStop(color, (this.ShadowRadius * 0.1 + cornerRadius) * num));
            checked
            {
                color.A = (byte)Math.Round(unchecked(0.74336 * (double)c.A));
                gradientStopCollection.Add(new GradientStop(color, unchecked((this.ShadowRadius * 0.3 + cornerRadius) * num)));
                color.A = (byte)Math.Round(unchecked(0.38053 * (double)c.A));
                gradientStopCollection.Add(new GradientStop(color, unchecked((this.ShadowRadius * 0.5 + cornerRadius) * num)));
                color.A = (byte)Math.Round(unchecked(0.12389 * (double)c.A));
                gradientStopCollection.Add(new GradientStop(color, unchecked((this.ShadowRadius * 0.7 + cornerRadius) * num)));
                color.A = (byte)Math.Round(unchecked(0.02654 * (double)c.A));
            }
            gradientStopCollection.Add(new GradientStop(color, (this.ShadowRadius * 0.9 + cornerRadius) * num));
            color.A = 0;
            gradientStopCollection.Add(new GradientStop(color, (this.ShadowRadius + cornerRadius) * num));
            gradientStopCollection.Freeze();
            return gradientStopCollection;
        }

        private Brush[] CreateBrushes(Color c, CornerRadius cornerRadius)
        {
            Brush[] array = new Brush[9];
            array[4] = new SolidColorBrush(c);
            array[4].Freeze();
            GradientStopCollection gradientStopCollection = this.CreateStops(c, 0.0);
            LinearGradientBrush linearGradientBrush = new LinearGradientBrush(gradientStopCollection, new Point(0.0, 1.0), new Point(0.0, 0.0));
            linearGradientBrush.Freeze();
            array[1] = linearGradientBrush;
            LinearGradientBrush linearGradientBrush2 = new LinearGradientBrush(gradientStopCollection, new Point(1.0, 0.0), new Point(0.0, 0.0));
            linearGradientBrush2.Freeze();
            array[3] = linearGradientBrush2;
            LinearGradientBrush linearGradientBrush3 = new LinearGradientBrush(gradientStopCollection, new Point(0.0, 0.0), new Point(1.0, 0.0));
            linearGradientBrush3.Freeze();
            array[5] = linearGradientBrush3;
            LinearGradientBrush linearGradientBrush4 = new LinearGradientBrush(gradientStopCollection, new Point(0.0, 0.0), new Point(0.0, 1.0));
            linearGradientBrush4.Freeze();
            array[7] = linearGradientBrush4;
            GradientStopCollection gradientStopCollection2;
            if (cornerRadius.TopLeft == 0.0)
            {
                gradientStopCollection2 = gradientStopCollection;
            }
            else
            {
                gradientStopCollection2 = this.CreateStops(c, cornerRadius.TopLeft);
            }
            RadialGradientBrush radialGradientBrush = new RadialGradientBrush(gradientStopCollection2)
            {
                RadiusX = 1.0,
                RadiusY = 1.0,
                Center = new Point(1.0, 1.0),
                GradientOrigin = new Point(1.0, 1.0)
            };
            radialGradientBrush.Freeze();
            array[0] = radialGradientBrush;
            GradientStopCollection gradientStopCollection3;
            if (cornerRadius.TopRight == 0.0)
            {
                gradientStopCollection3 = gradientStopCollection;
            }
            else if (cornerRadius.TopRight == cornerRadius.TopLeft)
            {
                gradientStopCollection3 = gradientStopCollection2;
            }
            else
            {
                gradientStopCollection3 = this.CreateStops(c, cornerRadius.TopRight);
            }
            RadialGradientBrush radialGradientBrush2 = new RadialGradientBrush(gradientStopCollection3)
            {
                RadiusX = 1.0,
                RadiusY = 1.0,
                Center = new Point(0.0, 1.0),
                GradientOrigin = new Point(0.0, 1.0)
            };
            radialGradientBrush2.Freeze();
            array[2] = radialGradientBrush2;
            GradientStopCollection gradientStopCollection4;
            if (cornerRadius.BottomLeft == 0.0)
            {
                gradientStopCollection4 = gradientStopCollection;
            }
            else if (cornerRadius.BottomLeft == cornerRadius.TopLeft)
            {
                gradientStopCollection4 = gradientStopCollection2;
            }
            else if (cornerRadius.BottomLeft == cornerRadius.TopRight)
            {
                gradientStopCollection4 = gradientStopCollection3;
            }
            else
            {
                gradientStopCollection4 = this.CreateStops(c, cornerRadius.BottomLeft);
            }
            RadialGradientBrush radialGradientBrush3 = new RadialGradientBrush(gradientStopCollection4)
            {
                RadiusX = 1.0,
                RadiusY = 1.0,
                Center = new Point(1.0, 0.0),
                GradientOrigin = new Point(1.0, 0.0)
            };
            radialGradientBrush3.Freeze();
            array[6] = radialGradientBrush3;
            GradientStopCollection gradientStopCollection5;
            if (cornerRadius.BottomRight == 0.0)
            {
                gradientStopCollection5 = gradientStopCollection;
            }
            else if (cornerRadius.BottomRight == cornerRadius.TopLeft)
            {
                gradientStopCollection5 = gradientStopCollection2;
            }
            else if (cornerRadius.BottomRight == cornerRadius.TopRight)
            {
                gradientStopCollection5 = gradientStopCollection3;
            }
            else if (cornerRadius.BottomRight == cornerRadius.BottomLeft)
            {
                gradientStopCollection5 = gradientStopCollection4;
            }
            else
            {
                gradientStopCollection5 = this.CreateStops(c, cornerRadius.BottomRight);
            }
            RadialGradientBrush radialGradientBrush4 = new RadialGradientBrush(gradientStopCollection5)
            {
                RadiusX = 1.0,
                RadiusY = 1.0,
                Center = new Point(0.0, 0.0),
                GradientOrigin = new Point(0.0, 0.0)
            };
            radialGradientBrush4.Freeze();
            array[8] = radialGradientBrush4;
            return array;
        }

        private Brush[] GetBrushes(Color c, CornerRadius cornerRadius)
        {
            if (MyDropShadow._commonBrushes == null)
            {
                object resourceAccess = MyDropShadow._resourceAccess;
                ObjectFlowControl.CheckForSyncLockOnValueType(resourceAccess);
                lock (resourceAccess)
                {
                    if (MyDropShadow._commonBrushes == null)
                    {
                        MyDropShadow._commonBrushes = this.CreateBrushes(c, cornerRadius);
                        MyDropShadow._commonCornerRadius = cornerRadius;
                    }
                }
            }
            Brush[] array;
            if (c == ((SolidColorBrush)MyDropShadow._commonBrushes[4]).Color && cornerRadius == MyDropShadow._commonCornerRadius)
            {
                this._brushes = null;
                array = MyDropShadow._commonBrushes;
            }
            else
            {
                if (this._brushes == null)
                {
                    this._brushes = this.CreateBrushes(c, cornerRadius);
                }
                array = this._brushes;
            }
            return array;
        }

        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register("Color", typeof(Color), typeof(MyDropShadow), new FrameworkPropertyMetadata(Color.FromArgb(113, 0, 0, 0), FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(MyDropShadow.ClearBrushes)));

        public static readonly DependencyProperty ShadowRadiusProperty = DependencyProperty.Register("ShadowRadius", typeof(double), typeof(MyDropShadow), new FrameworkPropertyMetadata(5.0, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(MyDropShadow.ClearBrushes)));

        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(MyDropShadow), new FrameworkPropertyMetadata(default(CornerRadius), FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(MyDropShadow.ClearBrushes)), new ValidateValueCallback(MyDropShadow.IsCornerRadiusValid));

        private static Brush[] _commonBrushes;

        private static CornerRadius _commonCornerRadius;

        private static readonly object _resourceAccess = RuntimeHelpers.GetObjectValue(new object());

        private Brush[] _brushes;

        private enum Placement
        {
            TopLeft,
            Top,
            TopRight,
            Left,
            Center,
            Right,
            BottomLeft,
            Bottom,
            BottomRight
        }
    }
}
