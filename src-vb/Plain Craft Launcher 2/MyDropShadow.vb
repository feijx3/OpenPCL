Imports System
Imports System.Runtime.CompilerServices
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Media
Imports Microsoft.VisualBasic.CompilerServices

Namespace PCL
    Public Class MyDropShadow
        Inherits Decorator

        Public Property Color As Color
            Get
                Dim value As Object = MyBase.GetValue(MyDropShadow.ColorProperty)
                If value Is Nothing Then
                    Return Nothing
                End If
                Return CType(value, Color)
            End Get
            Set(value As Color)
                MyBase.SetValue(MyDropShadow.ColorProperty, value)
            End Set
        End Property

        Public Property ShadowRadius As Double
            Get
                Return Conversions.ToDouble(MyBase.GetValue(MyDropShadow.ShadowRadiusProperty))
            End Get
            Set(value As Double)
                MyBase.SetValue(MyDropShadow.ShadowRadiusProperty, value)
            End Set
        End Property

        Public Property CornerRadius As CornerRadius
            Get
                Dim value As Object = MyBase.GetValue(MyDropShadow.CornerRadiusProperty)
                If value Is Nothing Then
                    Return Nothing
                End If
                Return CType(value, CornerRadius)
            End Get
            Set(value As CornerRadius)
                MyBase.SetValue(MyDropShadow.CornerRadiusProperty, value)
            End Set
        End Property

        Private Shared Function IsCornerRadiusValid(value As Object) As Boolean
            Dim cornerRadius As CornerRadius = If((value IsNot Nothing), CType(value, CornerRadius), Nothing)
            Return cornerRadius.TopLeft >= 0.0 AndAlso cornerRadius.TopRight >= 0.0 AndAlso cornerRadius.BottomLeft >= 0.0 AndAlso cornerRadius.BottomRight >= 0.0 AndAlso Not Double.IsNaN(cornerRadius.TopLeft) AndAlso Not Double.IsNaN(cornerRadius.TopRight) AndAlso Not Double.IsNaN(cornerRadius.BottomLeft) AndAlso Not Double.IsNaN(cornerRadius.BottomRight) AndAlso Not Double.IsInfinity(cornerRadius.TopLeft) AndAlso Not Double.IsInfinity(cornerRadius.TopRight) AndAlso Not Double.IsInfinity(cornerRadius.BottomLeft) AndAlso Not Double.IsInfinity(cornerRadius.BottomRight)
        End Function

        Protected Overrides Sub OnRender(drawingContext As DrawingContext)
            Dim cornerRadius As CornerRadius = Me.CornerRadius
            Dim rect As Rect = New Rect(0.0, 0.0, MyBase.RenderSize.Width, MyBase.RenderSize.Height)
            Dim color As Color = Me.Color
            If rect.Width > 0.0 AndAlso rect.Height > 0.0 AndAlso color.A > 0 Then
                Dim num As Double = rect.Right - rect.Left - 2.0 * Me.ShadowRadius
                Dim num2 As Double = rect.Bottom - rect.Top - 2.0 * Me.ShadowRadius
                Dim num3 As Double = Math.Min(num * 0.5, num2 * 0.5)
                cornerRadius.TopLeft = Math.Min(cornerRadius.TopLeft, num3)
                cornerRadius.TopRight = Math.Min(cornerRadius.TopRight, num3)
                cornerRadius.BottomLeft = Math.Min(cornerRadius.BottomLeft, num3)
                cornerRadius.BottomRight = Math.Min(cornerRadius.BottomRight, num3)
                Dim brushes As Brush() = Me.GetBrushes(color, cornerRadius)
                Dim num4 As Double = rect.Top + Me.ShadowRadius
                Dim num5 As Double = rect.Left + Me.ShadowRadius
                Dim num6 As Double = rect.Right - Me.ShadowRadius
                Dim num7 As Double = rect.Bottom - Me.ShadowRadius
                Dim array As Double() = New Double() { num5, num5 + cornerRadius.TopLeft, num6 - cornerRadius.TopRight, num5 + cornerRadius.BottomLeft, num6 - cornerRadius.BottomRight, num6 }
                Dim array2 As Double() = New Double() { num4, num4 + cornerRadius.TopLeft, num4 + cornerRadius.TopRight, num7 - cornerRadius.BottomLeft, num7 - cornerRadius.BottomRight, num7 }
                drawingContext.PushGuidelineSet(New GuidelineSet(array, array2))
                cornerRadius.TopLeft += Me.ShadowRadius
                cornerRadius.TopRight += Me.ShadowRadius
                cornerRadius.BottomLeft += Me.ShadowRadius
                cornerRadius.BottomRight += Me.ShadowRadius
                Dim rect2 As Rect = New Rect(rect.Left, rect.Top, cornerRadius.TopLeft, cornerRadius.TopLeft)
                drawingContext.DrawRectangle(brushes(0), Nothing, rect2)
                Dim num8 As Double = array(2) - array(1)
                If num8 > 0.0 Then
                    Dim rect3 As Rect = New Rect(array(1), rect.Top, num8, Me.ShadowRadius)
                    drawingContext.DrawRectangle(brushes(1), Nothing, rect3)
                End If
                Dim rect4 As Rect = New Rect(array(2), rect.Top, cornerRadius.TopRight, cornerRadius.TopRight)
                drawingContext.DrawRectangle(brushes(2), Nothing, rect4)
                Dim num9 As Double = array2(3) - array2(1)
                If num9 > 0.0 Then
                    Dim rect5 As Rect = New Rect(rect.Left, array2(1), Me.ShadowRadius, num9)
                    drawingContext.DrawRectangle(brushes(3), Nothing, rect5)
                End If
                Dim num10 As Double = array2(4) - array2(2)
                If num10 > 0.0 Then
                    Dim rect6 As Rect = New Rect(array(5), array2(2), Me.ShadowRadius, num10)
                    drawingContext.DrawRectangle(brushes(5), Nothing, rect6)
                End If
                Dim rect7 As Rect = New Rect(rect.Left, array2(3), cornerRadius.BottomLeft, cornerRadius.BottomLeft)
                drawingContext.DrawRectangle(brushes(6), Nothing, rect7)
                Dim num11 As Double = array(4) - array(3)
                If num11 > 0.0 Then
                    Dim rect8 As Rect = New Rect(array(3), array2(5), num11, Me.ShadowRadius)
                    drawingContext.DrawRectangle(brushes(7), Nothing, rect8)
                End If
                Dim rect9 As Rect = New Rect(array(4), array2(4), cornerRadius.BottomRight, cornerRadius.BottomRight)
                drawingContext.DrawRectangle(brushes(8), Nothing, rect9)
                If cornerRadius.TopLeft = Me.ShadowRadius AndAlso cornerRadius.TopLeft = cornerRadius.TopRight AndAlso cornerRadius.TopLeft = cornerRadius.BottomLeft AndAlso cornerRadius.TopLeft = cornerRadius.BottomRight Then
                    Dim rect10 As Rect = New Rect(array(0), array2(0), num, num2)
                    drawingContext.DrawRectangle(brushes(4), Nothing, rect10)
                Else
                    Dim pathFigure As PathFigure = New PathFigure()
                    If cornerRadius.TopLeft > Me.ShadowRadius Then
                        pathFigure.StartPoint = New Point(array(1), array2(0))
                        pathFigure.Segments.Add(New LineSegment(New Point(array(1), array2(1)), True))
                        pathFigure.Segments.Add(New LineSegment(New Point(array(0), array2(1)), True))
                    Else
                        pathFigure.StartPoint = New Point(array(0), array2(0))
                    End If
                    If cornerRadius.BottomLeft > Me.ShadowRadius Then
                        pathFigure.Segments.Add(New LineSegment(New Point(array(0), array2(3)), True))
                        pathFigure.Segments.Add(New LineSegment(New Point(array(3), array2(3)), True))
                        pathFigure.Segments.Add(New LineSegment(New Point(array(3), array2(5)), True))
                    Else
                        pathFigure.Segments.Add(New LineSegment(New Point(array(0), array2(5)), True))
                    End If
                    If cornerRadius.BottomRight > Me.ShadowRadius Then
                        pathFigure.Segments.Add(New LineSegment(New Point(array(4), array2(5)), True))
                        pathFigure.Segments.Add(New LineSegment(New Point(array(4), array2(4)), True))
                        pathFigure.Segments.Add(New LineSegment(New Point(array(5), array2(4)), True))
                    Else
                        pathFigure.Segments.Add(New LineSegment(New Point(array(5), array2(5)), True))
                    End If
                    If cornerRadius.TopRight > Me.ShadowRadius Then
                        pathFigure.Segments.Add(New LineSegment(New Point(array(5), array2(2)), True))
                        pathFigure.Segments.Add(New LineSegment(New Point(array(2), array2(2)), True))
                        pathFigure.Segments.Add(New LineSegment(New Point(array(2), array2(0)), True))
                    Else
                        pathFigure.Segments.Add(New LineSegment(New Point(array(5), array2(0)), True))
                    End If
                    pathFigure.IsClosed = True
                    pathFigure.Freeze()
                    Dim pathGeometry As PathGeometry = New PathGeometry()
                    pathGeometry.Figures.Add(pathFigure)
                    pathGeometry.Freeze()
                    drawingContext.DrawGeometry(brushes(4), Nothing, pathGeometry)
                End If
                drawingContext.Pop()
            End If
        End Sub

        Private Shared Sub ClearBrushes(o As DependencyObject, e As DependencyPropertyChangedEventArgs)
            CType(o, MyDropShadow)._brushes = Nothing
        End Sub

        Private Function CreateStops(c As Color, cornerRadius As Double) As GradientStopCollection
            Dim num As Double = 1.0 / (Me.ShadowRadius + cornerRadius)
            Dim gradientStopCollection As GradientStopCollection = New GradientStopCollection()
            Dim color As Color = c
            gradientStopCollection.Add(New GradientStop(color, (Me.ShadowRadius * 0.1 + cornerRadius) * num))
            color.A = CByte(Math.Round(0.74336 * CDbl(c.A)))
            gradientStopCollection.Add(New GradientStop(color, (Me.ShadowRadius * 0.3 + cornerRadius) * num))
            color.A = CByte(Math.Round(0.38053 * CDbl(c.A)))
            gradientStopCollection.Add(New GradientStop(color, (Me.ShadowRadius * 0.5 + cornerRadius) * num))
            color.A = CByte(Math.Round(0.12389 * CDbl(c.A)))
            gradientStopCollection.Add(New GradientStop(color, (Me.ShadowRadius * 0.7 + cornerRadius) * num))
            color.A = CByte(Math.Round(0.02654 * CDbl(c.A)))
            gradientStopCollection.Add(New GradientStop(color, (Me.ShadowRadius * 0.9 + cornerRadius) * num))
            color.A = 0
            gradientStopCollection.Add(New GradientStop(color, (Me.ShadowRadius + cornerRadius) * num))
            gradientStopCollection.Freeze()
            Return gradientStopCollection
        End Function

        Private Function CreateBrushes(c As Color, cornerRadius As CornerRadius) As Brush()
            Dim array As Brush() = New Brush(8) {}
            array(4) = New SolidColorBrush(c)
            array(4).Freeze()
            Dim gradientStopCollection As GradientStopCollection = Me.CreateStops(c, 0.0)
            Dim linearGradientBrush As LinearGradientBrush = New LinearGradientBrush(gradientStopCollection, New Point(0.0, 1.0), New Point(0.0, 0.0))
            linearGradientBrush.Freeze()
            array(1) = linearGradientBrush
            Dim linearGradientBrush2 As LinearGradientBrush = New LinearGradientBrush(gradientStopCollection, New Point(1.0, 0.0), New Point(0.0, 0.0))
            linearGradientBrush2.Freeze()
            array(3) = linearGradientBrush2
            Dim linearGradientBrush3 As LinearGradientBrush = New LinearGradientBrush(gradientStopCollection, New Point(0.0, 0.0), New Point(1.0, 0.0))
            linearGradientBrush3.Freeze()
            array(5) = linearGradientBrush3
            Dim linearGradientBrush4 As LinearGradientBrush = New LinearGradientBrush(gradientStopCollection, New Point(0.0, 0.0), New Point(0.0, 1.0))
            linearGradientBrush4.Freeze()
            array(7) = linearGradientBrush4
            Dim gradientStopCollection2 As GradientStopCollection
            If cornerRadius.TopLeft = 0.0 Then
                gradientStopCollection2 = gradientStopCollection
            Else
                gradientStopCollection2 = Me.CreateStops(c, cornerRadius.TopLeft)
            End If
            Dim radialGradientBrush As RadialGradientBrush = New RadialGradientBrush(gradientStopCollection2) With { .RadiusX = 1.0, .RadiusY = 1.0, .Center = New Point(1.0, 1.0), .GradientOrigin = New Point(1.0, 1.0) }
            radialGradientBrush.Freeze()
            array(0) = radialGradientBrush
            Dim gradientStopCollection3 As GradientStopCollection
            If cornerRadius.TopRight = 0.0 Then
                gradientStopCollection3 = gradientStopCollection
            ElseIf cornerRadius.TopRight = cornerRadius.TopLeft Then
                gradientStopCollection3 = gradientStopCollection2
            Else
                gradientStopCollection3 = Me.CreateStops(c, cornerRadius.TopRight)
            End If
            Dim radialGradientBrush2 As RadialGradientBrush = New RadialGradientBrush(gradientStopCollection3) With { .RadiusX = 1.0, .RadiusY = 1.0, .Center = New Point(0.0, 1.0), .GradientOrigin = New Point(0.0, 1.0) }
            radialGradientBrush2.Freeze()
            array(2) = radialGradientBrush2
            Dim gradientStopCollection4 As GradientStopCollection
            If cornerRadius.BottomLeft = 0.0 Then
                gradientStopCollection4 = gradientStopCollection
            ElseIf cornerRadius.BottomLeft = cornerRadius.TopLeft Then
                gradientStopCollection4 = gradientStopCollection2
            ElseIf cornerRadius.BottomLeft = cornerRadius.TopRight Then
                gradientStopCollection4 = gradientStopCollection3
            Else
                gradientStopCollection4 = Me.CreateStops(c, cornerRadius.BottomLeft)
            End If
            Dim radialGradientBrush3 As RadialGradientBrush = New RadialGradientBrush(gradientStopCollection4) With { .RadiusX = 1.0, .RadiusY = 1.0, .Center = New Point(1.0, 0.0), .GradientOrigin = New Point(1.0, 0.0) }
            radialGradientBrush3.Freeze()
            array(6) = radialGradientBrush3
            Dim gradientStopCollection5 As GradientStopCollection
            If cornerRadius.BottomRight = 0.0 Then
                gradientStopCollection5 = gradientStopCollection
            ElseIf cornerRadius.BottomRight = cornerRadius.TopLeft Then
                gradientStopCollection5 = gradientStopCollection2
            ElseIf cornerRadius.BottomRight = cornerRadius.TopRight Then
                gradientStopCollection5 = gradientStopCollection3
            ElseIf cornerRadius.BottomRight = cornerRadius.BottomLeft Then
                gradientStopCollection5 = gradientStopCollection4
            Else
                gradientStopCollection5 = Me.CreateStops(c, cornerRadius.BottomRight)
            End If
            Dim radialGradientBrush4 As RadialGradientBrush = New RadialGradientBrush(gradientStopCollection5) With { .RadiusX = 1.0, .RadiusY = 1.0, .Center = New Point(0.0, 0.0), .GradientOrigin = New Point(0.0, 0.0) }
            radialGradientBrush4.Freeze()
            array(8) = radialGradientBrush4
            Return array
        End Function

        Private Function GetBrushes(c As Color, cornerRadius As CornerRadius) As Brush()
            If MyDropShadow._commonBrushes Is Nothing Then
                Dim resourceAccess As Object = MyDropShadow._resourceAccess
                ObjectFlowControl.CheckForSyncLockOnValueType(resourceAccess)
                SyncLock resourceAccess
                    If MyDropShadow._commonBrushes Is Nothing Then
                        MyDropShadow._commonBrushes = Me.CreateBrushes(c, cornerRadius)
                        MyDropShadow._commonCornerRadius = cornerRadius
                    End If
                End SyncLock
            End If
            Dim array As Brush()
            If c = CType(MyDropShadow._commonBrushes(4), SolidColorBrush).Color AndAlso cornerRadius = MyDropShadow._commonCornerRadius Then
                Me._brushes = Nothing
                array = MyDropShadow._commonBrushes
            Else
                If Me._brushes Is Nothing Then
                    Me._brushes = Me.CreateBrushes(c, cornerRadius)
                End If
                array = Me._brushes
            End If
            Return array
        End Function

        Public Shared ColorProperty As DependencyProperty = DependencyProperty.Register("Color", GetType(Color), GetType(MyDropShadow), New FrameworkPropertyMetadata(Color.FromArgb(113, 0, 0, 0), FrameworkPropertyMetadataOptions.AffectsRender, AddressOf MyDropShadow.ClearBrushes))

        Public Shared ShadowRadiusProperty As DependencyProperty = DependencyProperty.Register("ShadowRadius", GetType(Double), GetType(MyDropShadow), New FrameworkPropertyMetadata(5.0, FrameworkPropertyMetadataOptions.AffectsRender, AddressOf MyDropShadow.ClearBrushes))

        Public Shared CornerRadiusProperty As DependencyProperty = DependencyProperty.Register("CornerRadius", GetType(CornerRadius), GetType(MyDropShadow), New FrameworkPropertyMetadata(Nothing, FrameworkPropertyMetadataOptions.AffectsRender, AddressOf MyDropShadow.ClearBrushes), AddressOf MyDropShadow.IsCornerRadiusValid)

        Private Shared _commonBrushes As Brush()

        Private Shared _commonCornerRadius As CornerRadius

        Private Shared _resourceAccess As Object = RuntimeHelpers.GetObjectValue(New Object())

        Private _brushes As Brush()

        Private Enum Placement
            TopLeft
            Top
            TopRight
            Left
            Center
            Right
            BottomLeft
            Bottom
            BottomRight
        End Enum
    End Class
End Namespace
