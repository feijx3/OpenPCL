using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using MeloongCore;
using MeloongCore.Extensions;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL
{
    public class Settings
    {
        public static void Set(string Key, object Value, ModMinecraft.McInstance Instance = null)
        {
            Settings.Setting setting = null;
            if (!Settings.Entries.TryGetValue(Key, out setting))
            {
                throw new KeyNotFoundException("未找到设置项：" + Key);
            }
            try
            {
                Value = RuntimeHelpers.GetObjectValue(Conversion.CTypeDynamic(RuntimeHelpers.GetObjectValue(Value), setting.Type));
                if (setting.GetCache(Instance) == null)
                {
                    Settings.Get(Key, Instance);
                }
                if (!Operators.ConditionalCompareObjectEqual(setting.GetCache(Instance), Value, false))
                {
                    setting.Set(RuntimeHelpers.GetObjectValue(Value), Instance);
                    setting.Save(Instance);
                    if (setting.OnChanged != null)
                    {
                        setting.OnChanged(RuntimeHelpers.GetObjectValue(Value));
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, string.Format("设置设置项时出错（{0}, {1}）", Key, RuntimeHelpers.GetObjectValue(Value)), LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageSetup\\Settings.vb");
            }
        }

        public static void SetSafe(string Key, object Value, ModMinecraft.McInstance Instance = null)
        {
            Settings.Setting setting = null;
            if (!Settings.Entries.TryGetValue(Key, out setting))
            {
                throw new KeyNotFoundException("未找到设置项：" + Key);
            }
            if (setting.Encrypted)
            {
                throw new InvalidOperationException("禁止写入加密设置项：" + Key);
            }
            Settings.Set(Key, RuntimeHelpers.GetObjectValue(Value), Instance);
        }

        public static object Get(string Key, ModMinecraft.McInstance Instance = null)
        {
            Settings.Setting setting = null;
            if (!Settings.Entries.TryGetValue(Key, out setting))
            {
                throw new KeyNotFoundException("未找到设置项：" + Key);
            }
            object objectValue = RuntimeHelpers.GetObjectValue(setting.GetCache(Instance));
            object obj;
            if (objectValue != null)
            {
                obj = objectValue;
            }
            else
            {
                if (Settings.$STATIC$Get$021CE1287E8$ForcedSettings$Init == null)
                {
                    Interlocked.CompareExchange<StaticLocalInitFlag>(ref Settings.$STATIC$Get$021CE1287E8$ForcedSettings$Init, new StaticLocalInitFlag(), null);
                }
                bool flag = false;
                try
                {
                    Monitor.Enter(Settings.$STATIC$Get$021CE1287E8$ForcedSettings$Init, ref flag);
                    if (Settings.$STATIC$Get$021CE1287E8$ForcedSettings$Init.State == 0)
                    {
                        Settings.$STATIC$Get$021CE1287E8$ForcedSettings$Init.State = 2;
                        Settings.$STATIC$Get$021CE1287E8$ForcedSettings = new Dictionary<string, object>
                        {
                            { "UiHiddenPageLink", true },
                            { "UiHiddenSetupLink", true }
                        };
                    }
                    else if (Settings.$STATIC$Get$021CE1287E8$ForcedSettings$Init.State == 2)
                    {
                        throw new IncompleteInitialization();
                    }
                }
                finally
                {
                    Settings.$STATIC$Get$021CE1287E8$ForcedSettings$Init.State = 1;
                    if (flag)
                    {
                        Monitor.Exit(Settings.$STATIC$Get$021CE1287E8$ForcedSettings$Init);
                    }
                }
                object obj2 = null;
                if (Settings.$STATIC$Get$021CE1287E8$ForcedSettings.TryGetValue(Key, out obj2))
                {
                    setting.Set(RuntimeHelpers.GetObjectValue(obj2), Instance);
                    obj = obj2;
                }
                else
                {
                    try
                    {
                        string text = null;
                        string text2 = Conversions.ToString(setting.Encrypted ? Settings.DesEncrypt(Conversions.ToString(setting.DefaultValue), "PCL" + ModBase.Identify) : setting.DefaultValue);
                        switch (setting.Source)
                        {
                        case Settings.Sources.Normal:
                            text = ModBase.ReadIni("Setup", Key, text2);
                            break;
                        case Settings.Sources.Registry:
                            text = Conversions.ToString(RegistryUtils.TryRead(string.Format("HKEY_CURRENT_USER\\Software\\{0}", "PCL"), Key, text2));
                            break;
                        case Settings.Sources.Instance:
                            if (Instance == null)
                            {
                                throw new Exception(string.Format("读取版本设置 {0} 时未提供目标版本", Key));
                            }
                            text = ModBase.ReadIni(Instance.PathVersion + "PCL\\Setup.ini", Key, text2);
                            break;
                        }
                        if (setting.Encrypted)
                        {
                            if (Operators.CompareString(text, text2, false) == 0)
                            {
                                text = Conversions.ToString(setting.DefaultValue);
                            }
                            else
                            {
                                try
                                {
                                    text = Settings.DesDecrypt(text, "PCL" + ModBase.Identify);
                                }
                                catch (Exception ex)
                                {
                                    Logger.Warn(ex, string.Format("解密设置失败：{0}", Key), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageSetup\\Settings.vb");
                                    text = Conversions.ToString(setting.DefaultValue);
                                    setting.Set(RuntimeHelpers.GetObjectValue(setting.DefaultValue), Instance);
                                    setting.Save(Instance);
                                }
                            }
                        }
                        setting.Set(RuntimeHelpers.GetObjectValue(Conversion.CTypeDynamic(text, setting.Type)), Instance);
                    }
                    catch (Exception ex2)
                    {
                        Logger.Error(ex2, string.Format("读取设置失败：{0}", Key), LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageSetup\\Settings.vb");
                        setting.Set(RuntimeHelpers.GetObjectValue(Conversion.CTypeDynamic(RuntimeHelpers.GetObjectValue(setting.DefaultValue), setting.Type)), Instance);
                    }
                    obj = setting.GetCache(Instance);
                }
            }
            return obj;
        }

        public static T Get<T>(string Key, ModMinecraft.McInstance Instance = null)
        {
            return Conversions.ToGenericParameter<T>(Settings.Get(Key, Instance));
        }

        public static object GetSafe(string Key, ModMinecraft.McInstance Instance = null)
        {
            Settings.Setting setting = null;
            if (!Settings.Entries.TryGetValue(Key, out setting))
            {
                throw new KeyNotFoundException("未找到设置项：" + Key);
            }
            if (setting.Encrypted)
            {
                throw new InvalidOperationException("禁止读取加密设置项：" + Key);
            }
            return Settings.Get(Key, Instance);
        }

        public static void Reset(string Key, ModMinecraft.McInstance Instance = null)
        {
            Settings.Setting setting = null;
            if (!Settings.Entries.TryGetValue(Key, out setting))
            {
                throw new KeyNotFoundException("未找到设置项：" + Key);
            }
            try
            {
                if (setting.GetCache(Instance) == null)
                {
                    Settings.Get(Key, Instance);
                }
                if (!Operators.ConditionalCompareObjectEqual(setting.GetCache(Instance), setting.DefaultValue, false))
                {
                    setting.Set(RuntimeHelpers.GetObjectValue(setting.DefaultValue), Instance);
                    switch (setting.Source)
                    {
                    case Settings.Sources.Normal:
                        ModBase.DeleteIniKey("Setup", Key);
                        goto IL_00EF;
                    case Settings.Sources.Registry:
                        try
                        {
                            RegistryUtils.Delete(string.Format("HKEY_CURRENT_USER\\Software\\{0}", "PCL"), Key);
                            goto IL_00EF;
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn(ex, string.Format("删除注册表失败（{0}）", Key), LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageSetup\\Settings.vb");
                            goto IL_00EF;
                        }
                        break;
                    case Settings.Sources.Instance:
                        break;
                    default:
                        goto IL_00EF;
                    }
                    if (Instance == null)
                    {
                        throw new Exception(string.Format("重置版本设置 {0} 时未提供目标版本", Key));
                    }
                    ModBase.DeleteIniKey(Instance.PathVersion + "PCL\\Setup.ini", Key);
                    IL_00EF:
                    if (setting.OnChanged != null)
                    {
                        setting.OnChanged(RuntimeHelpers.GetObjectValue(setting.DefaultValue));
                    }
                }
            }
            catch (Exception ex2)
            {
                Logger.Error(ex2, string.Format("重置设置项时出错（{0}）", Key), LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageSetup\\Settings.vb");
            }
        }

        public static string GetDefault(string Key)
        {
            return Conversions.ToString(Settings.Entries[Key].DefaultValue);
        }

        public static bool HasSaved(string Key, ModMinecraft.McInstance Instance = null)
        {
            Settings.Sources source = Settings.Entries[Key].Source;
            bool flag;
            if (source != Settings.Sources.Normal)
            {
                if (source == Settings.Sources.Registry)
                {
                    try
                    {
                        return RegistryUtils.Has(string.Format("HKEY_CURRENT_USER\\Software\\{0}", "PCL"), Key);
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn(ex, string.Format("判断注册表 {0} 是否存在时出错", Key), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageSetup\\Settings.vb");
                        return false;
                    }
                }
                if (Instance == null)
                {
                    throw new Exception(string.Format("判断版本设置 {0} 是否存在时未提供目标版本", Key));
                }
                flag = ModBase.HasIniKey(Instance.PathVersion + "PCL\\Setup.ini", Key);
            }
            else
            {
                flag = ModBase.HasIniKey("Setup", Key);
            }
            return flag;
        }

        private static string DesEncrypt(string sourceString, string key = null)
        {
            key = ((key == null) ? "@;$ Abv2" : key.GetStableHashCode().ToString().EnsureLength('X', 8)
                .Substring(0, 8));
            byte[] bytes = Encoding.UTF8.GetBytes(key);
            string text;
            using (DESCryptoServiceProvider descryptoServiceProvider = new DESCryptoServiceProvider())
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, descryptoServiceProvider.CreateEncryptor(bytes, Settings.desInitialVector), CryptoStreamMode.Write))
                    {
                        byte[] bytes2 = Encoding.UTF8.GetBytes(sourceString);
                        cryptoStream.Write(bytes2, 0, bytes2.Length);
                        cryptoStream.FlushFinalBlock();
                        text = Convert.ToBase64String(memoryStream.ToArray());
                    }
                }
            }
            return text;
        }

        private static string DesDecrypt(string encryptedString, string key = null)
        {
            key = ((key == null) ? "@;$ Abv2" : key.GetStableHashCode().ToString().EnsureLength('X', 8)
                .Substring(0, 8));
            byte[] bytes = Encoding.UTF8.GetBytes(key);
            string @string;
            using (DESCryptoServiceProvider descryptoServiceProvider = new DESCryptoServiceProvider())
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, descryptoServiceProvider.CreateDecryptor(bytes, Settings.desInitialVector), CryptoStreamMode.Write))
                    {
                        byte[] array = Convert.FromBase64String(encryptedString);
                        cryptoStream.Write(array, 0, array.Length);
                        cryptoStream.FlushFinalBlock();
                        @string = Encoding.UTF8.GetString(memoryStream.ToArray());
                    }
                }
            }
            return @string;
        }

        public static readonly Dictionary<string, Settings.Setting> Entries = new List<Settings.Setting>
        {
            new Settings.Setting("Identify", "", Settings.Sources.Registry, false, null),
            new Settings.Setting("WindowHeight", 550, Settings.Sources.Normal, false, null),
            new Settings.Setting("WindowWidth", 900, Settings.Sources.Normal, false, null),
            new Settings.Setting("AprilYear", 0, Settings.Sources.Registry, true, null),
            new Settings.Setting("Potatoes", "", Settings.Sources.Registry, true, null),
            new Settings.Setting("HintDownloadThread", false, Settings.Sources.Registry, false, null),
            new Settings.Setting("HintNotice", 0, Settings.Sources.Registry, false, null),
            new Settings.Setting("HintDownload", 0, Settings.Sources.Registry, false, null),
            new Settings.Setting("HintHide", false, Settings.Sources.Registry, false, null),
            new Settings.Setting("HintHandInstall", false, Settings.Sources.Registry, false, null),
            new Settings.Setting("HintBuy", false, Settings.Sources.Registry, false, null),
            new Settings.Setting("HintClearRubbish", 0, Settings.Sources.Registry, false, null),
            new Settings.Setting("HintUpdateMod", false, Settings.Sources.Registry, false, null),
            new Settings.Setting("HintCustomCommand", false, Settings.Sources.Registry, false, null),
            new Settings.Setting("HintCustomWarn", false, Settings.Sources.Registry, false, null),
            new Settings.Setting("HintMoreAdvancedSetup", false, Settings.Sources.Registry, false, null),
            new Settings.Setting("HintIndieSetup", false, Settings.Sources.Registry, false, null),
            new Settings.Setting("HintExportConfig", false, Settings.Sources.Registry, false, null),
            new Settings.Setting("HintSnapshot", false, Settings.Sources.Registry, false, null),
            new Settings.Setting("SystemEulaVersion", 0, Settings.Sources.Registry, false, null),
            new Settings.Setting("SystemCount", 0, Settings.Sources.Registry, true, null),
            new Settings.Setting("SystemLaunchCount", 0, Settings.Sources.Registry, true, null),
            new Settings.Setting("SystemLastVersionReg", 0, Settings.Sources.Registry, true, null),
            new Settings.Setting("SystemHighestSavedBetaVersionReg", 0, Settings.Sources.Registry, true, null),
            new Settings.Setting("SystemHighestBetaVersionReg", 0, Settings.Sources.Registry, true, null),
            new Settings.Setting("SystemHighestAlphaVersionReg", 0, Settings.Sources.Registry, true, null),
            new Settings.Setting("SystemHelpVersion", 0, Settings.Sources.Registry, false, null),
            new Settings.Setting("SystemDebugMode", false, Settings.Sources.Registry, false, delegate(object a0)
            {
                ((Settings._Closure$__.$I0-0 == null) ? (Settings._Closure$__.$I0-0 = delegate
                {
                    Logger.Instance.MinLevel = (ModBase.ModeDebug ? LogLevel.Trace : LogLevel.Info);
                }) : Settings._Closure$__.$I0-0)();
            }),
            new Settings.Setting("SystemDebugAnim", 9, Settings.Sources.Registry, false, null),
            new Settings.Setting("SystemDebugDelay", false, Settings.Sources.Registry, false, null),
            new Settings.Setting("SystemDebugSkipCopy", false, Settings.Sources.Registry, false, null),
            new Settings.Setting("SystemSystemCache", "", Settings.Sources.Registry, false, null),
            new Settings.Setting("SystemSystemUpdate", 0, Settings.Sources.Normal, false, null),
            new Settings.Setting("SystemSystemActivity", 0, Settings.Sources.Normal, false, null),
            new Settings.Setting("SystemSystemTelemetry", true, Settings.Sources.Registry, false, null),
            new Settings.Setting("CacheDrops", "", Settings.Sources.Registry, false, null),
            new Settings.Setting("CacheConfig", 0, Settings.Sources.Registry, false, null),
            new Settings.Setting("CacheExportConfig", "", Settings.Sources.Registry, false, null),
            new Settings.Setting("CacheSavedPageUrl", "", Settings.Sources.Registry, false, null),
            new Settings.Setting("CacheSavedPageVersion", "", Settings.Sources.Registry, false, null),
            new Settings.Setting("CacheMsOAuthRefresh", "", Settings.Sources.Registry, true, null),
            new Settings.Setting("CacheMsAccess", "", Settings.Sources.Registry, true, null),
            new Settings.Setting("CacheMsProfileJson", "", Settings.Sources.Registry, true, null),
            new Settings.Setting("CacheMsUuid", "", Settings.Sources.Registry, true, null),
            new Settings.Setting("CacheMsName", "", Settings.Sources.Registry, true, null),
            new Settings.Setting("CacheMsV2Migrated", false, Settings.Sources.Registry, false, null),
            new Settings.Setting("CacheMsV2OAuthRefresh", "", Settings.Sources.Registry, true, null),
            new Settings.Setting("CacheMsV2Access", "", Settings.Sources.Registry, true, null),
            new Settings.Setting("CacheMsV2ProfileJson", "", Settings.Sources.Registry, true, null),
            new Settings.Setting("CacheMsV2Uuid", "", Settings.Sources.Registry, true, null),
            new Settings.Setting("CacheMsV2Name", "", Settings.Sources.Registry, true, null),
            new Settings.Setting("CacheMsV2Expires", 0L, Settings.Sources.Registry, true, null),
            new Settings.Setting("CacheNideAccess", "", Settings.Sources.Registry, true, null),
            new Settings.Setting("CacheNideClient", "", Settings.Sources.Registry, true, null),
            new Settings.Setting("CacheNideUuid", "", Settings.Sources.Registry, true, null),
            new Settings.Setting("CacheNideName", "", Settings.Sources.Registry, true, null),
            new Settings.Setting("CacheNideUsername", "", Settings.Sources.Registry, true, null),
            new Settings.Setting("CacheNidePass", "", Settings.Sources.Registry, true, null),
            new Settings.Setting("CacheNideServer", "", Settings.Sources.Registry, true, null),
            new Settings.Setting("CacheAuthAccess", "", Settings.Sources.Registry, true, null),
            new Settings.Setting("CacheAuthClient", "", Settings.Sources.Registry, true, null),
            new Settings.Setting("CacheAuthUuid", "", Settings.Sources.Registry, true, null),
            new Settings.Setting("CacheAuthName", "", Settings.Sources.Registry, true, null),
            new Settings.Setting("CacheAuthUsername", "", Settings.Sources.Registry, true, null),
            new Settings.Setting("CacheAuthPass", "", Settings.Sources.Registry, true, null),
            new Settings.Setting("CacheAuthServerServer", "", Settings.Sources.Registry, true, null),
            new Settings.Setting("CacheAuthServerName", "", Settings.Sources.Registry, true, null),
            new Settings.Setting("CacheAuthServerRegister", "", Settings.Sources.Registry, true, null),
            new Settings.Setting("CacheDownloadFolder", "", Settings.Sources.Registry, false, null),
            new Settings.Setting("CacheJavaListVersion", 0, Settings.Sources.Registry, false, null),
            new Settings.Setting("LoginRemember", true, Settings.Sources.Registry, true, null),
            new Settings.Setting("LoginLegacyName", "", Settings.Sources.Registry, true, null),
            new Settings.Setting("LoginMsJson", "{}", Settings.Sources.Registry, true, null),
            new Settings.Setting("LoginNideEmail", "", Settings.Sources.Registry, true, null),
            new Settings.Setting("LoginNidePass", "", Settings.Sources.Registry, true, null),
            new Settings.Setting("LoginAuthEmail", "", Settings.Sources.Registry, true, null),
            new Settings.Setting("LoginAuthPass", "", Settings.Sources.Registry, true, null),
            new Settings.Setting("LoginType", ModLaunch.McLoginType.Legacy, Settings.Sources.Registry, false, null),
            new Settings.Setting("LoginPageType", 0, Settings.Sources.Normal, false, null),
            new Settings.Setting("LaunchSkinID", "", Settings.Sources.Registry, false, delegate(object a0)
            {
                ((Settings._Closure$__.$I0-1 == null) ? (Settings._Closure$__.$I0-1 = delegate
                {
                    PageLaunchLeft.SkinLegacy.Start(null, false);
                }) : Settings._Closure$__.$I0-1)();
            }),
            new Settings.Setting("LaunchSkinType", 0, Settings.Sources.Registry, false, delegate(object a0)
            {
                PageSetupLaunch.UpdateSkinType();
            }),
            new Settings.Setting("LaunchSkinSlim", false, Settings.Sources.Registry, false, null),
            new Settings.Setting("LaunchFolderSelect", "", Settings.Sources.Normal, false, null),
            new Settings.Setting("LaunchFolders", "", Settings.Sources.Registry, false, null),
            new Settings.Setting("LaunchArgumentTitle", "", Settings.Sources.Normal, false, null),
            new Settings.Setting("LaunchArgumentInfo", "PCL", Settings.Sources.Normal, false, null),
            new Settings.Setting("LaunchArgumentJavaSelect", "", Settings.Sources.Registry, false, null),
            new Settings.Setting("LaunchArgumentJavaAll", "[]", Settings.Sources.Registry, false, null),
            new Settings.Setting("LaunchArgumentIndie", 0, Settings.Sources.Normal, false, null),
            new Settings.Setting("LaunchArgumentIndieV2", 4, Settings.Sources.Normal, false, null),
            new Settings.Setting("LaunchArgumentVisible", 5, Settings.Sources.Registry, false, null),
            new Settings.Setting("LaunchArgumentPriority", 1, Settings.Sources.Registry, false, null),
            new Settings.Setting("LaunchArgumentWindowWidth", 854, Settings.Sources.Normal, false, null),
            new Settings.Setting("LaunchArgumentWindowHeight", 480, Settings.Sources.Normal, false, null),
            new Settings.Setting("LaunchArgumentWindowType", 1, Settings.Sources.Normal, false, null),
            new Settings.Setting("LaunchArgumentRam", false, Settings.Sources.Registry, false, null),
            new Settings.Setting("LaunchAdvanceJvm", "-XX:-OmitStackTraceInFastThrow -Djdk.lang.Process.allowAmbiguousCommands=True -Dfml.ignoreInvalidMinecraftCertificates=True -Dfml.ignorePatchDiscrepancies=True", Settings.Sources.Normal, false, null),
            new Settings.Setting("LaunchAdvanceGame", "", Settings.Sources.Normal, false, null),
            new Settings.Setting("LaunchAdvanceRun", "", Settings.Sources.Normal, false, null),
            new Settings.Setting("LaunchAdvanceRunWait", true, Settings.Sources.Normal, false, null),
            new Settings.Setting("LaunchAdvanceDisableJLW", false, Settings.Sources.Normal, false, null),
            new Settings.Setting("LaunchAdvanceDisableLUA", false, Settings.Sources.Normal, false, null),
            new Settings.Setting("LaunchAdvanceGraphicCard", true, Settings.Sources.Registry, false, null),
            new Settings.Setting("LaunchAdvanceGC", 4, Settings.Sources.Normal, false, null),
            new Settings.Setting("LaunchRamType", 0, Settings.Sources.Normal, false, delegate(object a0)
            {
                PageSetupLaunch.UpdateRamType();
            }),
            new Settings.Setting("LaunchRamCustom", 15, Settings.Sources.Normal, false, null),
            new Settings.Setting("LinkLastAutoJoinInviteCode", "", Settings.Sources.Registry, false, null),
            new Settings.Setting("LinkLatencyMode", 0, Settings.Sources.Registry, false, null),
            new Settings.Setting("LinkCustomPeer", "", Settings.Sources.Normal, false, null),
            new Settings.Setting("LinkEasyTierVersion", -1, Settings.Sources.Registry, false, null),
            new Settings.Setting("ToolHelpChinese", true, Settings.Sources.Registry, false, null),
            new Settings.Setting("ToolDownloadThread", 63, Settings.Sources.Registry, false, null),
            new Settings.Setting("ToolDownloadSpeed", 42, Settings.Sources.Registry, false, delegate(object a0)
            {
                ModNet.UpdateNetTaskSpeedLimitHigh(Conversions.ToInteger(a0));
            }),
            new Settings.Setting("ToolDownloadSource", 1, Settings.Sources.Registry, false, null),
            new Settings.Setting("ToolDownloadVersion", 1, Settings.Sources.Registry, false, null),
            new Settings.Setting("ToolDownloadTranslate", 0, Settings.Sources.Registry, false, null),
            new Settings.Setting("ToolDownloadTranslateV2", 1, Settings.Sources.Registry, false, null),
            new Settings.Setting("ToolDownloadCert", false, Settings.Sources.Registry, false, delegate(object a0)
            {
                ModNet.ShouldValidateSslCertificateOnLogin(Conversions.ToBoolean(a0));
            }),
            new Settings.Setting("ToolDownloadMod", 2, Settings.Sources.Registry, false, null),
            new Settings.Setting("ToolModLocalNameStyle", 0, Settings.Sources.Registry, false, null),
            new Settings.Setting("ToolUpdateRelease", false, Settings.Sources.Registry, false, null),
            new Settings.Setting("ToolUpdateSnapshot", false, Settings.Sources.Registry, false, null),
            new Settings.Setting("ToolUpdateReleaseLast", "", Settings.Sources.Registry, false, null),
            new Settings.Setting("ToolUpdateSnapshotLast", "", Settings.Sources.Registry, false, null),
            new Settings.Setting("UiLauncherTransparent", 600, Settings.Sources.Normal, false, delegate(object a0)
            {
                ((Settings._Closure$__.$I0-2 == null) ? (Settings._Closure$__.$I0-2 = delegate(int Value)
                {
                    if (ModMain.FrmMain != null)
                    {
                        ModMain.FrmMain.Opacity = (double)Value / 1000.0 + 0.4;
                    }
                }) : Settings._Closure$__.$I0-2)(Conversions.ToInteger(a0));
            }),
            new Settings.Setting("UiLauncherHue", 180, Settings.Sources.Normal, false, null),
            new Settings.Setting("UiLauncherSat", 80, Settings.Sources.Normal, false, null),
            new Settings.Setting("UiLauncherDelta", 90, Settings.Sources.Normal, false, null),
            new Settings.Setting("UiLauncherLight", 20, Settings.Sources.Normal, false, null),
            new Settings.Setting("UiLauncherTheme", 0, Settings.Sources.Normal, false, delegate(object a0)
            {
                ModSecret.ThemeRefresh(Conversions.ToInteger(a0));
            }),
            new Settings.Setting("UiLauncherThemeHide", "0|1|2|3|4", Settings.Sources.Registry, true, null),
            new Settings.Setting("UiLauncherThemeHide2", "0|1|2|3|4", Settings.Sources.Registry, true, null),
            new Settings.Setting("UiLauncherLogo", true, Settings.Sources.Normal, false, null),
            new Settings.Setting("UiLauncherEmail", false, Settings.Sources.Normal, false, null),
            new Settings.Setting("UiBackgroundColorful", true, Settings.Sources.Normal, false, delegate(object a0)
            {
                ((Settings._Closure$__.$I0-3 == null) ? (Settings._Closure$__.$I0-3 = delegate
                {
                    ModSecret.ThemeRefresh(-1);
                }) : Settings._Closure$__.$I0-3)();
            }),
            new Settings.Setting("UiBackgroundOpacity", 1000, Settings.Sources.Normal, false, new Action<object>(FormMain.UpdateBackgroundAndTitleBar)),
            new Settings.Setting("UiBackgroundBlur", 0, Settings.Sources.Normal, false, new Action<object>(FormMain.UpdateBackgroundAndTitleBar)),
            new Settings.Setting("UiBackgroundSuit", 0, Settings.Sources.Normal, false, new Action<object>(FormMain.UpdateBackgroundAndTitleBar)),
            new Settings.Setting("UiCustomType", 0, Settings.Sources.Normal, false, delegate(object a0)
            {
                PageSetupUI.OnMainPageTypeChanged();
            }),
            new Settings.Setting("UiCustomPreset", 0, Settings.Sources.Normal, false, null),
            new Settings.Setting("UiCustomNet", "", Settings.Sources.Normal, false, null),
            new Settings.Setting("UiLogoType", 1, Settings.Sources.Normal, false, new Action<object>(FormMain.UpdateBackgroundAndTitleBar)),
            new Settings.Setting("UiLogoText", "", Settings.Sources.Normal, false, new Action<object>(FormMain.UpdateBackgroundAndTitleBar)),
            new Settings.Setting("UiLogoLeft", false, Settings.Sources.Normal, false, new Action<object>(FormMain.UpdateBackgroundAndTitleBar)),
            new Settings.Setting("UiMusicVolume", 500, Settings.Sources.Normal, false, null),
            new Settings.Setting("UiMusicStop", false, Settings.Sources.Normal, false, null),
            new Settings.Setting("UiMusicStart", false, Settings.Sources.Normal, false, null),
            new Settings.Setting("UiMusicRandom", true, Settings.Sources.Normal, false, null),
            new Settings.Setting("UiMusicAuto", true, Settings.Sources.Normal, false, null),
            new Settings.Setting("UiHiddenPageDownload", false, Settings.Sources.Normal, false, delegate(object a0)
            {
                PageSetupUI.HiddenRefresh();
            }),
            new Settings.Setting("UiHiddenPageLink", false, Settings.Sources.Normal, false, delegate(object a0)
            {
                PageSetupUI.HiddenRefresh();
            }),
            new Settings.Setting("UiHiddenPageSetup", false, Settings.Sources.Normal, false, delegate(object a0)
            {
                PageSetupUI.HiddenRefresh();
            }),
            new Settings.Setting("UiHiddenPageOther", false, Settings.Sources.Normal, false, delegate(object a0)
            {
                PageSetupUI.HiddenRefresh();
            }),
            new Settings.Setting("UiHiddenFunctionSelect", false, Settings.Sources.Normal, false, delegate(object a0)
            {
                PageSetupUI.HiddenRefresh();
            }),
            new Settings.Setting("UiHiddenFunctionModUpdate", false, Settings.Sources.Normal, false, delegate(object a0)
            {
                PageSetupUI.HiddenRefresh();
            }),
            new Settings.Setting("UiHiddenFunctionInstanceModify", false, Settings.Sources.Normal, false, delegate(object a0)
            {
                PageSetupUI.HiddenRefresh();
            }),
            new Settings.Setting("UiHiddenFunctionHidden", false, Settings.Sources.Normal, false, delegate(object a0)
            {
                PageSetupUI.HiddenRefresh();
            }),
            new Settings.Setting("UiHiddenSetupLaunch", false, Settings.Sources.Normal, false, delegate(object a0)
            {
                PageSetupUI.HiddenRefresh();
            }),
            new Settings.Setting("UiHiddenSetupUi", false, Settings.Sources.Normal, false, delegate(object a0)
            {
                PageSetupUI.HiddenRefresh();
            }),
            new Settings.Setting("UiHiddenSetupLink", false, Settings.Sources.Normal, false, delegate(object a0)
            {
                PageSetupUI.HiddenRefresh();
            }),
            new Settings.Setting("UiHiddenSetupSystem", false, Settings.Sources.Normal, false, delegate(object a0)
            {
                PageSetupUI.HiddenRefresh();
            }),
            new Settings.Setting("UiHiddenOtherHelp", false, Settings.Sources.Normal, false, delegate(object a0)
            {
                PageSetupUI.HiddenRefresh();
            }),
            new Settings.Setting("UiHiddenOtherFeedback", false, Settings.Sources.Normal, false, delegate(object a0)
            {
                PageSetupUI.HiddenRefresh();
            }),
            new Settings.Setting("UiHiddenOtherVote", false, Settings.Sources.Normal, false, delegate(object a0)
            {
                PageSetupUI.HiddenRefresh();
            }),
            new Settings.Setting("UiHiddenOtherAbout", false, Settings.Sources.Normal, false, delegate(object a0)
            {
                PageSetupUI.HiddenRefresh();
            }),
            new Settings.Setting("UiHiddenOtherTest", false, Settings.Sources.Normal, false, delegate(object a0)
            {
                PageSetupUI.HiddenRefresh();
            }),
            new Settings.Setting("VersionAdvanceJvm", "", Settings.Sources.Instance, false, null),
            new Settings.Setting("VersionAdvanceGame", "", Settings.Sources.Instance, false, null),
            new Settings.Setting("VersionAdvanceAssets", 0, Settings.Sources.Instance, false, null),
            new Settings.Setting("VersionAdvanceAssetsV2", false, Settings.Sources.Instance, false, null),
            new Settings.Setting("VersionAdvanceRun", "", Settings.Sources.Instance, false, null),
            new Settings.Setting("VersionAdvanceRunWait", true, Settings.Sources.Instance, false, null),
            new Settings.Setting("VersionAdvanceDisableJLW", false, Settings.Sources.Instance, false, null),
            new Settings.Setting("VersionAdvanceDisableLUA", false, Settings.Sources.Instance, false, null),
            new Settings.Setting("VersionAdvanceDisableModUpdate", false, Settings.Sources.Instance, false, null),
            new Settings.Setting("VersionAdvanceGC", 0, Settings.Sources.Instance, false, null),
            new Settings.Setting("VersionRamType", 2, Settings.Sources.Instance, false, delegate(object a0)
            {
                PageInstanceSetup.OnVersionRamTypeChanged(Conversions.ToInteger(a0));
            }),
            new Settings.Setting("VersionRamCustom", 15, Settings.Sources.Instance, false, null),
            new Settings.Setting("VersionRamOptimize", 0, Settings.Sources.Instance, false, null),
            new Settings.Setting("VersionArgumentTitle", "", Settings.Sources.Instance, false, null),
            new Settings.Setting("VersionArgumentInfo", "", Settings.Sources.Instance, false, null),
            new Settings.Setting("VersionArgumentIndie", -1, Settings.Sources.Instance, false, null),
            new Settings.Setting("VersionArgumentIndieV2", false, Settings.Sources.Instance, false, null),
            new Settings.Setting("VersionArgumentJavaSelect", "使用全局设置", Settings.Sources.Instance, false, null),
            new Settings.Setting("VersionArgumentJavaV2", 0, Settings.Sources.Instance, false, null),
            new Settings.Setting("VersionArgumentJavaRange", "", Settings.Sources.Instance, false, null),
            new Settings.Setting("VersionServerEnter", "", Settings.Sources.Instance, false, null),
            new Settings.Setting("VersionServerLogin", 0, Settings.Sources.Instance, false, delegate(object a0)
            {
                PageInstanceSetup.OnVersionServerLoginChanged(Conversions.ToInteger(a0));
            }),
            new Settings.Setting("VersionServerNide", "", Settings.Sources.Instance, false, null),
            new Settings.Setting("VersionServerAuthRegister", "", Settings.Sources.Instance, false, null),
            new Settings.Setting("VersionServerAuthName", "", Settings.Sources.Instance, false, null),
            new Settings.Setting("VersionServerAuthServer", "", Settings.Sources.Instance, false, null)
        }.ToDictionary<Settings.Setting, string>((Settings.Setting e) => e.Key);

        private static readonly byte[] desInitialVector = Encoding.UTF8.GetBytes("95168702");

        private static Dictionary<string, object> $STATIC$Get$021CE1287E8$ForcedSettings;

        private static StaticLocalInitFlag $STATIC$Get$021CE1287E8$ForcedSettings$Init;

        public enum Sources
        {
            Normal,
            Registry,
            Instance
        }

        public class Setting
        {
            public Setting(string Key, object Value, Settings.Sources Source = Settings.Sources.Normal, bool Encrypted = false, Action<object> OnChanged = null)
            {
                this.ValueCache = new ConcurrentDictionary<string, object>();
                try
                {
                    this.Key = Key;
                    this.DefaultValue = RuntimeHelpers.GetObjectValue(Value);
                    this.Encrypted = Encrypted;
                    this.Source = Source;
                    this.Type = (Value ?? "").GetType();
                    this.OnChanged = OnChanged;
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "初始化设置项失败", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageSetup\\Settings.vb");
                }
            }

            public object GetCache(ModMinecraft.McInstance Instance)
            {
                string text = ((this.Source == Settings.Sources.Instance) ? Instance.PathVersion : "");
                object obj = null;
                if (!this.ValueCache.TryGetValue(text, out obj))
                {
                    return null;
                }
                return obj;
            }

            public void Set(object Value, ModMinecraft.McInstance Instance)
            {
                string text = ((this.Source == Settings.Sources.Instance) ? Instance.PathVersion : "");
                this.ValueCache[text] = RuntimeHelpers.GetObjectValue(Value);
            }

            public void Save(ModMinecraft.McInstance Instance = null)
            {
                string text = Conversions.ToString(this.GetCache(Instance));
                Logger.Trace(string.Format("保存设置：{0} {1}{2}", this.Key, this.Encrypted ? "" : ("= " + text), (Instance == null) ? "" : string.Format("（实例：{0}）", (Instance != null) ? Instance.PathVersion : null)), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageSetup\\Settings.vb");
                if (this.Encrypted)
                {
                    try
                    {
                        if (text == null)
                        {
                            text = "";
                        }
                        text = Settings.DesEncrypt(text, "PCL" + ModBase.Identify);
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn(ex, string.Format("加密设置失败：{0}", this.Key), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageSetup\\Settings.vb");
                    }
                }
                switch (this.Source)
                {
                case Settings.Sources.Normal:
                    ModBase.WriteIni("Setup", this.Key, text);
                    return;
                case Settings.Sources.Registry:
                    try
                    {
                        RegistryUtils.Write(string.Format("HKEY_CURRENT_USER\\Software\\{0}", "PCL"), this.Key, text);
                        return;
                    }
                    catch (Exception ex2)
                    {
                        Logger.Warn(ex2, string.Format("写入注册表失败（{0} → {1}）", this.Key, text), LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageSetup\\Settings.vb");
                        return;
                    }
                    break;
                case Settings.Sources.Instance:
                    break;
                default:
                    return;
                }
                if (Instance == null)
                {
                    throw new Exception(string.Format("保存版本独立设置 {0} 时未提供目标版本", this.Key));
                }
                ModBase.WriteIni(Instance.PathVersion + "PCL\\Setup.ini", this.Key, text);
            }

            public string Key;

            public bool Encrypted;

            public object DefaultValue;

            public Settings.Sources Source;

            public Action<object> OnChanged;

            public Type Type;

            public ConcurrentDictionary<string, object> ValueCache;
        }
    }
}
