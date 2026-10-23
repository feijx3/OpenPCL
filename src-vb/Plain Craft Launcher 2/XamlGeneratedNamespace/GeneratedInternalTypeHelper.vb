Imports System
Imports System.CodeDom.Compiler
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Globalization
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Windows.Markup

Namespace PCL.XamlGeneratedNamespace
    <DebuggerNonUserCode()>
    <GeneratedCode("PresentationBuildTasks", "4.0.0.0")>
    <EditorBrowsable(EditorBrowsableState.Never)>
    Public NotInheritable Class GeneratedInternalTypeHelper
        Inherits InternalTypeHelper

        Protected Overrides Function CreateInstance(type As Type, culture As CultureInfo) As Object
            Return Activator.CreateInstance(type, BindingFlags.Instance Or BindingFlags.[Public] Or BindingFlags.NonPublic Or BindingFlags.CreateInstance, Nothing, Nothing, culture)
        End Function

        Protected Overrides Function GetPropertyValue(propertyInfo As PropertyInfo, target As Object, culture As CultureInfo) As Object
            Return propertyInfo.GetValue(RuntimeHelpers.GetObjectValue(target), BindingFlags.[Default], Nothing, Nothing, culture)
        End Function

        Protected Overrides Sub SetPropertyValue(propertyInfo As PropertyInfo, target As Object, value As Object, culture As CultureInfo)
            propertyInfo.SetValue(RuntimeHelpers.GetObjectValue(target), RuntimeHelpers.GetObjectValue(value), BindingFlags.[Default], Nothing, Nothing, culture)
        End Sub

        Protected Overrides Function CreateDelegate(delegateType As Type, target As Object, handler As String) As [Delegate]
            Return CType(target.[GetType]().InvokeMember("_CreateDelegate", BindingFlags.Instance Or BindingFlags.NonPublic Or BindingFlags.InvokeMethod, Nothing, RuntimeHelpers.GetObjectValue(target), New Object() { delegateType, handler }, Nothing), [Delegate])
        End Function

        Protected Overrides Sub AddEventHandler(eventInfo As EventInfo, target As Object, handler As [Delegate])
            eventInfo.AddEventHandler(RuntimeHelpers.GetObjectValue(target), handler)
        End Sub
    End Class
End Namespace
