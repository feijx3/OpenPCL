using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using MeloongCore.Extensions;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL
{
    public class SettingService
    {
        public static string GetKey(DependencyObject obj)
        {
            string text;
            if (obj == null)
            {
                text = null;
            }
            else
            {
                text = Conversions.ToString(obj.GetValue(SettingService.KeyProperty));
            }
            return text;
        }

        public static void SetKey(DependencyObject obj, string value)
        {
            obj.SetValue(SettingService.KeyProperty, value);
        }

        public static string GetValue(DependencyObject obj)
        {
            string text;
            if (obj == null)
            {
                text = null;
            }
            else
            {
                text = Conversions.ToString(obj.GetValue(SettingService.ValueProperty));
            }
            return text;
        }

        public static void SetValue(DependencyObject obj, string value)
        {
            obj.SetValue(SettingService.ValueProperty, value);
        }

        public static void ResetSettings(DependencyObject Target)
        {
            LogicalTreeHelper.GetChildren(Target).OfType<DependencyObject>().ForAll<DependencyObject>((SettingService._Closure$__.$I8-0 == null) ? (SettingService._Closure$__.$I8-0 = delegate(DependencyObject c)
            {
                SettingService.ResetSettings(c);
            }) : SettingService._Closure$__.$I8-0);
            string key = SettingService.GetKey(Target);
            if (key != null)
            {
                Settings.Reset(key, PageInstanceLeft.Instance);
            }
        }

        public static void RefreshSettings(DependencyObject Target)
        {
            LogicalTreeHelper.GetChildren(Target).OfType<DependencyObject>().ForAll<DependencyObject>((SettingService._Closure$__.$I9-0 == null) ? (SettingService._Closure$__.$I9-0 = delegate(DependencyObject c)
            {
                SettingService.RefreshSettings(c);
            }) : SettingService._Closure$__.$I9-0);
            string key = SettingService.GetKey(Target);
            if (key != null)
            {
                object objectValue = RuntimeHelpers.GetObjectValue(Settings.Get(key, PageInstanceLeft.Instance));
                if (Target is ISettingControl)
                {
                    ((ISettingControl)Target).RefreshSetting(Conversions.ToString(objectValue));
                }
            }
        }

        public static void SaveSetting(object sender)
        {
            if (ModAnimation.AniControlEnabled == 0 && sender is ISettingControl)
            {
                string key = SettingService.GetKey((DependencyObject)sender);
                if (key != null)
                {
                    string currentSetting = ((ISettingControl)sender).GetCurrentSetting();
                    if (currentSetting != null)
                    {
                        Settings.Set(key, currentSetting, PageInstanceLeft.Instance);
                    }
                }
            }
        }

        public static readonly DependencyProperty KeyProperty = DependencyProperty.RegisterAttached("Key", typeof(string), typeof(SettingService), new PropertyMetadata(null));

        public static readonly DependencyProperty ValueProperty = DependencyProperty.RegisterAttached("Value", typeof(string), typeof(SettingService), new PropertyMetadata(null));
    }
}
