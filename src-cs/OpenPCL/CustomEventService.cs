using System;
using System.Windows;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL
{
    public class CustomEventService
    {
        // Note: this type is marked as 'beforefieldinit'.
        static CustomEventService()
        {
        }

        public CustomEventService()
        {
        }

        [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
        public static void SetEvents(DependencyObject d, CustomEventCollection value)
        {
            d.SetValue(CustomEventService.EventsProperty, value);
        }

        [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
        public static CustomEventCollection GetEvents(DependencyObject d)
        {
            if (d.GetValue(CustomEventService.EventsProperty) == null)
            {
                d.SetValue(CustomEventService.EventsProperty, new CustomEventCollection());
            }
            return (CustomEventCollection)d.GetValue(CustomEventService.EventsProperty);
        }

        [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
        public static void SetEventType(DependencyObject d, CustomEvent.EventType value)
        {
            d.SetValue(CustomEventService.EventTypeProperty, value);
        }

        [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
        public static CustomEvent.EventType GetEventType(DependencyObject d)
        {
            return (CustomEvent.EventType)Conversions.ToInteger(d.GetValue(CustomEventService.EventTypeProperty));
        }

        [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
        public static void SetEventData(DependencyObject d, string value)
        {
            d.SetValue(CustomEventService.EventDataProperty, value);
        }

        [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
        public static string GetEventData(DependencyObject d)
        {
            return Conversions.ToString(d.GetValue(CustomEventService.EventDataProperty));
        }

        public static readonly DependencyProperty EventsProperty = DependencyProperty.RegisterAttached("Events", typeof(CustomEventCollection), typeof(CustomEventService), new PropertyMetadata(null));

        public static readonly DependencyProperty EventTypeProperty = DependencyProperty.RegisterAttached("EventType", typeof(CustomEvent.EventType), typeof(CustomEventService), new PropertyMetadata(null));

        public static readonly DependencyProperty EventDataProperty = DependencyProperty.RegisterAttached("EventData", typeof(string), typeof(CustomEventService), new PropertyMetadata(null));
    }
}
