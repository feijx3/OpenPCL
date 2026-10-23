using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using MeloongCore.Extensions;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;

namespace MeloongCore.Wpf
{
    [NullableContext(1)]
    [Nullable(0)]
    public static class Dialogs
    {
        [return: Nullable(2)]
        public static string SaveFile(string title, string defaultFileName = "", [Nullable(2)] string defaultDirectory = null, [TupleElementNames(new string[] { "Extension", "Display" })] [Nullable(new byte[] { 2, 0, 1, 1 })] IEnumerable<ValueTuple<string, string>> filter = null)
        {
            List<ValueTuple<string, string>> list = ((filter != null) ? filter.ToList<ValueTuple<string, string>>() : null);
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                AddExtension = true,
                Title = title,
                FileName = defaultFileName
            };
            if (!string.IsNullOrEmpty(defaultDirectory) && DirectoryUtils.Exists(defaultDirectory))
            {
                saveFileDialog.InitialDirectory = PathUtils.ToShortPath(defaultDirectory);
            }
            if (list != null)
            {
                saveFileDialog.Filter = list.Select<ValueTuple<string, string>, string>(([TupleElementNames(new string[] { "Extension", "Display" })] ValueTuple<string, string> f) => string.Concat(new string[] { f.Item2, "(*.", f.Item1, ")|*.", f.Item1 })).Join("|");
            }
            bool flag;
            Logger.LogInterpolatedStringHandler<Logger.InfoLogLevel> logInterpolatedStringHandler = new Logger.LogInterpolatedStringHandler<Logger.InfoLogLevel>(44, 4, out flag);
            if (flag)
            {
                logInterpolatedStringHandler.AppendLiteral("保存弹窗：");
                logInterpolatedStringHandler.AppendFormatted(saveFileDialog.Title);
                logInterpolatedStringHandler.AppendLiteral("（FileName=");
                logInterpolatedStringHandler.AppendFormatted(saveFileDialog.FileName);
                logInterpolatedStringHandler.AppendLiteral(", InitialDirectory=");
                logInterpolatedStringHandler.AppendFormatted(saveFileDialog.InitialDirectory);
                logInterpolatedStringHandler.AppendLiteral(", Filter=");
                logInterpolatedStringHandler.AppendFormatted(saveFileDialog.Filter);
                logInterpolatedStringHandler.AppendLiteral("）");
            }
            Logger.Info(ref logInterpolatedStringHandler, LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\MeloongCore\\Wpf\\General\\Dialogs.cs");
            if (!saveFileDialog.ShowDialog().GetValueOrDefault())
            {
                return null;
            }
            string result = saveFileDialog.FileName;
            Logger.LogInterpolatedStringHandler<Logger.InfoLogLevel> logInterpolatedStringHandler2 = new Logger.LogInterpolatedStringHandler<Logger.InfoLogLevel>(7, 1, out flag);
            if (flag)
            {
                logInterpolatedStringHandler2.AppendLiteral("保存弹窗返回：");
                logInterpolatedStringHandler2.AppendFormatted(result);
            }
            Logger.Info(ref logInterpolatedStringHandler2, LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\MeloongCore\\Wpf\\General\\Dialogs.cs");
            if (!result.Contains(Path.DirectorySeparatorChar))
            {
                return null;
            }
            if (list != null && !string.IsNullOrEmpty(result) && !list.Any<ValueTuple<string, string>>(([TupleElementNames(new string[] { "Extension", "Display" })] ValueTuple<string, string> f) => result.EndsWithF("." + f.Item1, true)))
            {
                result = result + "." + list[saveFileDialog.FilterIndex - 1].Item1;
                Logger.LogInterpolatedStringHandler<Logger.WarnLogLevel> logInterpolatedStringHandler3 = new Logger.LogInterpolatedStringHandler<Logger.WarnLogLevel>(24, 1, out flag);
                if (flag)
                {
                    logInterpolatedStringHandler3.AppendLiteral("选择文件的返回无扩展名，将会手动添加，修改后为：");
                    logInterpolatedStringHandler3.AppendFormatted(result);
                }
                Logger.Warn(ref logInterpolatedStringHandler3, LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\MeloongCore\\Wpf\\General\\Dialogs.cs");
            }
            return result;
        }

        public static List<string> SelectFile(string title, bool multiselect, [Nullable(2)] string defaultDirectory = null, [TupleElementNames(new string[] { "Extensions", "Display" })] [Nullable(new byte[] { 0, 1, 0, 1, 1, 1, 1 })] OneOf<IEnumerable<ValueTuple<string[], string>>, string>? filter = null)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                AddExtension = true,
                CheckFileExists = true,
                Multiselect = multiselect,
                Title = title,
                ValidateNames = true
            };
            if (!string.IsNullOrEmpty(defaultDirectory) && DirectoryUtils.Exists(defaultDirectory))
            {
                openFileDialog.InitialDirectory = PathUtils.ToShortPath(defaultDirectory);
            }
            if (filter != null)
            {
                if (filter.Value.Is<string>())
                {
                    openFileDialog.Filter = filter.Value.As<string>();
                }
                else
                {
                    openFileDialog.Filter = filter.Value.As<IEnumerable<ValueTuple<string[], string>>>().Select<ValueTuple<string[], string>, string>(delegate([TupleElementNames(new string[] { "Extensions", "Display" })] ValueTuple<string[], string> f)
                    {
                        string text = string.Join(";", f.Item1.Select<string, string>((string e) => "*." + e));
                        return string.Concat(new string[] { f.Item2, "(", text, ")|", text });
                    }).Join("|");
                }
            }
            bool flag;
            Logger.LogInterpolatedStringHandler<Logger.InfoLogLevel> logInterpolatedStringHandler = new Logger.LogInterpolatedStringHandler<Logger.InfoLogLevel>(49, 4, out flag);
            if (flag)
            {
                logInterpolatedStringHandler.AppendLiteral("选择文件弹窗：");
                logInterpolatedStringHandler.AppendFormatted(openFileDialog.Title);
                logInterpolatedStringHandler.AppendLiteral("（Multiselect=");
                logInterpolatedStringHandler.AppendFormatted<bool>(openFileDialog.Multiselect);
                logInterpolatedStringHandler.AppendLiteral(", InitialDirectory=");
                logInterpolatedStringHandler.AppendFormatted(openFileDialog.InitialDirectory);
                logInterpolatedStringHandler.AppendLiteral(", Filter=");
                logInterpolatedStringHandler.AppendFormatted(openFileDialog.Filter);
                logInterpolatedStringHandler.AppendLiteral("）");
            }
            Logger.Info(ref logInterpolatedStringHandler, LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\MeloongCore\\Wpf\\General\\Dialogs.cs");
            if (!openFileDialog.ShowDialog().GetValueOrDefault())
            {
                return new List<string>();
            }
            List<string> list = openFileDialog.FileNames.Where<string>((string f) => f.Contains(Path.DirectorySeparatorChar)).ToList<string>();
            Logger.LogInterpolatedStringHandler<Logger.InfoLogLevel> logInterpolatedStringHandler2 = new Logger.LogInterpolatedStringHandler<Logger.InfoLogLevel>(9, 1, out flag);
            if (flag)
            {
                logInterpolatedStringHandler2.AppendLiteral("选择文件弹窗返回：");
                logInterpolatedStringHandler2.AppendFormatted(list.Join(","));
            }
            Logger.Info(ref logInterpolatedStringHandler2, LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\MeloongCore\\Wpf\\General\\Dialogs.cs");
            return list;
        }

        public static List<string> SelectFolder(string title, bool multiselect, [Nullable(2)] string defaultDirectory = null)
        {
            VistaFolderBrowserDialog vistaFolderBrowserDialog = new VistaFolderBrowserDialog
            {
                ShowNewFolderButton = true,
                Description = title,
                Multiselect = multiselect,
                UseDescriptionForTitle = true
            };
            if (!string.IsNullOrEmpty(defaultDirectory) && DirectoryUtils.Exists(defaultDirectory))
            {
                vistaFolderBrowserDialog.SelectedPath = defaultDirectory;
            }
            bool flag;
            Logger.LogInterpolatedStringHandler<Logger.InfoLogLevel> logInterpolatedStringHandler = new Logger.LogInterpolatedStringHandler<Logger.InfoLogLevel>(37, 3, out flag);
            if (flag)
            {
                logInterpolatedStringHandler.AppendLiteral("选择文件夹弹窗：");
                logInterpolatedStringHandler.AppendFormatted(vistaFolderBrowserDialog.Description);
                logInterpolatedStringHandler.AppendLiteral("（Multiselect=");
                logInterpolatedStringHandler.AppendFormatted<bool>(vistaFolderBrowserDialog.Multiselect);
                logInterpolatedStringHandler.AppendLiteral(", SelectedPath=");
                logInterpolatedStringHandler.AppendFormatted(vistaFolderBrowserDialog.SelectedPath);
                logInterpolatedStringHandler.AppendLiteral("）");
            }
            Logger.Info(ref logInterpolatedStringHandler, LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\MeloongCore\\Wpf\\General\\Dialogs.cs");
            if (!vistaFolderBrowserDialog.ShowDialog().GetValueOrDefault())
            {
                return new List<string>();
            }
            IEnumerable<string> enumerable = vistaFolderBrowserDialog.SelectedPaths.Where<string>((string f) => f.Contains(Path.DirectorySeparatorChar));
            Func<string, string> func;
            if ((func = Dialogs.<>O.<0>__AddSlashSuffix) == null)
            {
                func = (Dialogs.<>O.<0>__AddSlashSuffix = new Func<string, string>(PathUtils.AddSlashSuffix));
            }
            List<string> list = enumerable.Select<string, string>(func).ToList<string>();
            Logger.LogInterpolatedStringHandler<Logger.InfoLogLevel> logInterpolatedStringHandler2 = new Logger.LogInterpolatedStringHandler<Logger.InfoLogLevel>(10, 1, out flag);
            if (flag)
            {
                logInterpolatedStringHandler2.AppendLiteral("选择文件夹弹窗返回：");
                logInterpolatedStringHandler2.AppendFormatted(list.Join(","));
            }
            Logger.Info(ref logInterpolatedStringHandler2, LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\MeloongCore\\Wpf\\General\\Dialogs.cs");
            return list;
        }

        [CompilerGenerated]
        private static class <>O
        {
            [Nullable(0)]
            public static Func<string, string> <0>__AddSlashSuffix;
        }
    }
}
