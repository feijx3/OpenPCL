using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Markup;

namespace PCL.XamlGeneratedNamespace
{
    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class GeneratedInternalTypeHelper : InternalTypeHelper
    {
        public GeneratedInternalTypeHelper()
        {
        }

        protected override object CreateInstance(Type type, CultureInfo culture)
        {
            return Activator.CreateInstance(type, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance, null, null, culture);
        }

        protected override object GetPropertyValue(PropertyInfo propertyInfo, object target, CultureInfo culture)
        {
            return propertyInfo.GetValue(RuntimeHelpers.GetObjectValue(target), BindingFlags.Default, null, null, culture);
        }

        protected override void SetPropertyValue(PropertyInfo propertyInfo, object target, object value, CultureInfo culture)
        {
            propertyInfo.SetValue(RuntimeHelpers.GetObjectValue(target), RuntimeHelpers.GetObjectValue(value), BindingFlags.Default, null, null, culture);
        }

        protected override Delegate CreateDelegate(Type delegateType, object target, string handler)
        {
            return (Delegate)target.GetType().InvokeMember("_CreateDelegate", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod, null, RuntimeHelpers.GetObjectValue(target), new object[] { delegateType, handler }, null);
        }

        protected override void AddEventHandler(EventInfo eventInfo, object target, Delegate handler)
        {
            eventInfo.AddEventHandler(RuntimeHelpers.GetObjectValue(target), handler);
        }
    }
}
