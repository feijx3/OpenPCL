Imports System
Imports System.Windows
Imports Microsoft.VisualBasic.CompilerServices

Namespace PCL
    Public Class CustomEventService
        <AttachedPropertyBrowsableForType(GetType(DependencyObject))>
        Public Shared Sub SetEvents(d As DependencyObject, value As CustomEventCollection)
            d.SetValue(CustomEventService.EventsProperty, value)
        End Sub

        <AttachedPropertyBrowsableForType(GetType(DependencyObject))>
        Public Shared Function GetEvents(d As DependencyObject) As CustomEventCollection
            If d.GetValue(CustomEventService.EventsProperty) Is Nothing Then
                d.SetValue(CustomEventService.EventsProperty, New CustomEventCollection())
            End If
            Return CType(d.GetValue(CustomEventService.EventsProperty), CustomEventCollection)
        End Function

        <AttachedPropertyBrowsableForType(GetType(DependencyObject))>
        Public Shared Sub SetEventType(d As DependencyObject, value As CustomEvent.EventType)
            d.SetValue(CustomEventService.EventTypeProperty, value)
        End Sub

        <AttachedPropertyBrowsableForType(GetType(DependencyObject))>
        Public Shared Function GetEventType(d As DependencyObject) As CustomEvent.EventType
            Return CType(Conversions.ToInteger(d.GetValue(CustomEventService.EventTypeProperty)), CustomEvent.EventType)
        End Function

        <AttachedPropertyBrowsableForType(GetType(DependencyObject))>
        Public Shared Sub SetEventData(d As DependencyObject, value As String)
            d.SetValue(CustomEventService.EventDataProperty, value)
        End Sub

        <AttachedPropertyBrowsableForType(GetType(DependencyObject))>
        Public Shared Function GetEventData(d As DependencyObject) As String
            Return Conversions.ToString(d.GetValue(CustomEventService.EventDataProperty))
        End Function

        Public Shared EventsProperty As DependencyProperty = DependencyProperty.RegisterAttached("Events", GetType(CustomEventCollection), GetType(CustomEventService), New PropertyMetadata(Nothing))

        Public Shared EventTypeProperty As DependencyProperty = DependencyProperty.RegisterAttached("EventType", GetType(CustomEvent.EventType), GetType(CustomEventService), New PropertyMetadata(Nothing))

        Public Shared EventDataProperty As DependencyProperty = DependencyProperty.RegisterAttached("EventData", GetType(String), GetType(CustomEventService), New PropertyMetadata(Nothing))
    End Class
End Namespace
