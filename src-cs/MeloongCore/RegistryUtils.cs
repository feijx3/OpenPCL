using System;
using System.Runtime.CompilerServices;
using MeloongCore.Extensions;
using Microsoft.Win32;

namespace MeloongCore
{
    [NullableContext(1)]
    [Nullable(0)]
    public static class RegistryUtils
    {
        [return: TupleElementNames(new string[] { "result", "exists" })]
        [return: Nullable(new byte[] { 0, 2 })]
        public static ValueTuple<object, bool> Read(string folder, string entry)
        {
            ValueTuple<object, bool> valueTuple;
            using (RegistryKey registryKey = RegistryUtils.GetRegistryKey(folder, false, false))
            {
                object obj = ((registryKey != null) ? registryKey.GetValue(entry) : null);
                if (obj == null)
                {
                    valueTuple = new ValueTuple<object, bool>(null, false);
                }
                else
                {
                    string text = obj as string;
                    valueTuple = new ValueTuple<object, bool>((text != null && text == "__NULL__") ? null : obj, true);
                }
            }
            return valueTuple;
        }

        [return: Nullable(2)]
        public static object TryRead(string folder, string entry, [Nullable(2)] object defaultValue = null)
        {
            object obj;
            try
            {
                ValueTuple<object, bool> valueTuple = RegistryUtils.Read(folder, entry);
                object item = valueTuple.Item1;
                obj = (valueTuple.Item2 ? item : defaultValue);
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, string.Concat(new string[] { "读取注册表值失败（", folder, "，", entry, "）" }), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\MeloongCore\\Shared\\Utils\\RegistryUtils.cs");
                obj = defaultValue;
            }
            return obj;
        }

        public static void Write(string folder, string entry, [Nullable(2)] object value)
        {
            using (RegistryKey registryKey = RegistryUtils.GetRegistryKey(folder, false, true))
            {
                registryKey.SetValue(entry, value ?? "__NULL__");
            }
        }

        public static bool Has(string folder, string entry)
        {
            bool flag;
            using (RegistryKey registryKey = RegistryUtils.GetRegistryKey(folder, false, false))
            {
                if (registryKey == null)
                {
                    flag = false;
                }
                else
                {
                    object obj = new object();
                    flag = registryKey.GetValue(entry, obj) != obj;
                }
            }
            return flag;
        }

        public static void Delete(string folder, string entry)
        {
            using (RegistryKey registryKey = RegistryUtils.GetRegistryKey(folder, true, false))
            {
                if (registryKey != null)
                {
                    registryKey.DeleteValue(entry, false);
                }
            }
        }

        [return: Nullable(2)]
        private static RegistryKey GetRegistryKey(string fullPath, bool writable = false, bool create = false)
        {
            string text = fullPath.BeforeFirst("\\", false).ToUpperInvariant();
            RegistryKey registryKey;
            if (!(text == "HKEY_CLASSES_ROOT"))
            {
                if (!(text == "HKEY_CURRENT_USER"))
                {
                    if (!(text == "HKEY_LOCAL_MACHINE"))
                    {
                        if (!(text == "HKEY_USERS"))
                        {
                            if (!(text == "HKEY_CURRENT_CONFIG"))
                            {
                                throw new ArgumentException("注册表路径有误（" + fullPath + "）", "fullPath");
                            }
                            registryKey = Registry.CurrentConfig;
                        }
                        else
                        {
                            registryKey = Registry.Users;
                        }
                    }
                    else
                    {
                        registryKey = Registry.LocalMachine;
                    }
                }
                else
                {
                    registryKey = Registry.CurrentUser;
                }
            }
            else
            {
                registryKey = Registry.ClassesRoot;
            }
            using (RegistryKey registryKey2 = registryKey)
            {
                string text2 = fullPath.AfterFirst("\\", false);
                registryKey = (create ? registryKey2.CreateSubKey(text2) : registryKey2.OpenSubKey(text2, writable));
            }
            return registryKey;
        }

        public const string NULL_STRING = "__NULL__";
    }
}
