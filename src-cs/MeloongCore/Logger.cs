using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace MeloongCore
{
    [NullableContext(1)]
    [Nullable(0)]
    public static class Logger
    {
        public static BaseLogger Instance { get; set; } = new BaseLogger();

        public static void Trace(string message, LogBehavior behavior = LogBehavior.None, [CallerFilePath] string filePath = "")
        {
            Logger.Log(null, message, LogLevel.Trace, behavior, filePath);
        }

        public static void Info(string message, LogBehavior behavior = LogBehavior.None, [CallerFilePath] string filePath = "")
        {
            Logger.Log(null, message, LogLevel.Info, behavior, filePath);
        }

        public static void Warn(string message, LogBehavior behavior = LogBehavior.ToastIfDebug, [CallerFilePath] string filePath = "")
        {
            Logger.Log(null, message, LogLevel.Warn, behavior, filePath);
        }

        public static void Error(string message, LogBehavior behavior = LogBehavior.AlertThenFeedback, [CallerFilePath] string filePath = "")
        {
            Logger.Log(null, message, LogLevel.Error, behavior, filePath);
        }

        public static void Log(string message, LogLevel level = LogLevel.Info, LogBehavior behavior = LogBehavior.None, [CallerFilePath] string filePath = "")
        {
            Logger.Log(null, message, level, behavior, filePath);
        }

        public static void Trace(Exception ex, [Nullable(2)] string message = null, LogBehavior behavior = LogBehavior.None, [CallerFilePath] string filePath = "")
        {
            Logger.Log(ex, message, LogLevel.Trace, behavior, filePath);
        }

        public static void Info(Exception ex, [Nullable(2)] string message = null, LogBehavior behavior = LogBehavior.None, [CallerFilePath] string filePath = "")
        {
            Logger.Log(ex, message, LogLevel.Info, behavior, filePath);
        }

        public static void Warn(Exception ex, [Nullable(2)] string message = null, LogBehavior behavior = LogBehavior.ToastIfDebug, [CallerFilePath] string filePath = "")
        {
            Logger.Log(ex, message, LogLevel.Warn, behavior, filePath);
        }

        public static void Error(Exception ex, [Nullable(2)] string message = null, LogBehavior behavior = LogBehavior.AlertThenFeedback, [CallerFilePath] string filePath = "")
        {
            Logger.Log(ex, message, LogLevel.Error, behavior, filePath);
        }

        [NullableContext(2)]
        public static void Log(Exception ex, string message = null, LogLevel level = LogLevel.Warn, LogBehavior behavior = LogBehavior.ToastIfDebug, [Nullable(1)] [CallerFilePath] string filePath = "")
        {
            Logger.Instance.Log(ex, message, level, behavior, filePath);
        }

        [Conditional("DEBUG")]
        public static void SendToDebug(string message, LogLevel level = LogLevel.Error, [CallerFilePath] string filePath = "")
        {
        }

        [Conditional("DEBUG")]
        public static void SendToDebug(Exception ex, [Nullable(2)] string message = null, LogLevel level = LogLevel.Error, [CallerFilePath] string filePath = "")
        {
        }

        public static void StackTrace(LogLevel level = LogLevel.Trace, [CallerFilePath] string filePath = "")
        {
            Logger.Instance.Log(null, new StackTrace(1, true).ToString(), level, LogBehavior.None, filePath);
        }

        public static void Log([Nullable(new byte[] { 1, 2 })] Func<string> messageFactory, LogLevel level = LogLevel.Info, LogBehavior behavior = LogBehavior.None, [CallerFilePath] string filePath = "")
        {
            if (Logger.Instance.MinLevel > level)
            {
                return;
            }
            Logger.Instance.Log(null, messageFactory() ?? string.Empty, level, behavior, filePath);
        }

        public static void Trace([Nullable(new byte[] { 1, 2 })] Func<string> messageFactory, LogBehavior behavior = LogBehavior.None, [CallerFilePath] string filePath = "")
        {
            Logger.Log(messageFactory, LogLevel.Trace, behavior, filePath);
        }

        public static void Info([Nullable(new byte[] { 1, 2 })] Func<string> messageFactory, LogBehavior behavior = LogBehavior.None, [CallerFilePath] string filePath = "")
        {
            Logger.Log(messageFactory, LogLevel.Info, behavior, filePath);
        }

        public static void Warn([Nullable(new byte[] { 1, 2 })] Func<string> messageFactory, LogBehavior behavior = LogBehavior.ToastIfDebug, [CallerFilePath] string filePath = "")
        {
            Logger.Log(messageFactory, LogLevel.Warn, behavior, filePath);
        }

        public static void Error([Nullable(new byte[] { 1, 2 })] Func<string> messageFactory, LogBehavior behavior = LogBehavior.AlertThenFeedback, [CallerFilePath] string filePath = "")
        {
            Logger.Log(messageFactory, LogLevel.Error, behavior, filePath);
        }

        public static void Log(LogLevel level, [InterpolatedStringHandlerArgument("level")] ref Logger.LogInterpolatedStringHandler handler, LogBehavior behavior = LogBehavior.None, [CallerFilePath] string filePath = "")
        {
            if (handler.isEnabled)
            {
                Logger.Instance.Log(null, handler.ToStringAndClear(), level, behavior, filePath);
            }
        }

        [NullableContext(0)]
        public static void Trace(ref Logger.LogInterpolatedStringHandler<Logger.TraceLogLevel> handler, LogBehavior behavior = LogBehavior.None, [Nullable(1)] [CallerFilePath] string filePath = "")
        {
            if (handler.IsEnabled)
            {
                Logger.Instance.Log(null, handler.ToStringAndClear(), LogLevel.Trace, behavior, filePath);
            }
        }

        [NullableContext(0)]
        public static void Info(ref Logger.LogInterpolatedStringHandler<Logger.InfoLogLevel> handler, LogBehavior behavior = LogBehavior.None, [Nullable(1)] [CallerFilePath] string filePath = "")
        {
            if (handler.IsEnabled)
            {
                Logger.Instance.Log(null, handler.ToStringAndClear(), LogLevel.Info, behavior, filePath);
            }
        }

        [NullableContext(0)]
        public static void Warn(ref Logger.LogInterpolatedStringHandler<Logger.WarnLogLevel> handler, LogBehavior behavior = LogBehavior.ToastIfDebug, [Nullable(1)] [CallerFilePath] string filePath = "")
        {
            if (handler.IsEnabled)
            {
                Logger.Instance.Log(null, handler.ToStringAndClear(), LogLevel.Warn, behavior, filePath);
            }
        }

        [NullableContext(0)]
        public static void Error(ref Logger.LogInterpolatedStringHandler<Logger.ErrorLogLevel> handler, LogBehavior behavior = LogBehavior.AlertThenFeedback, [Nullable(1)] [CallerFilePath] string filePath = "")
        {
            if (handler.IsEnabled)
            {
                Logger.Instance.Log(null, handler.ToStringAndClear(), LogLevel.Error, behavior, filePath);
            }
        }

        [NullableContext(2)]
        [Nullable(0)]
        [CompilerFeatureRequired("RefStructs")]
        [InterpolatedStringHandler]
        public ref struct LogInterpolatedStringHandler
        {
            public LogInterpolatedStringHandler(int literalLength, LogLevel level, out bool isEnabled)
            {
                this.isEnabled = (isEnabled = Logger.Instance.MinLevel <= level);
                this._sb = (this.isEnabled ? new StringBuilder(literalLength) : null);
            }

            [NullableContext(1)]
            public void AppendLiteral(string value)
            {
                StringBuilder sb = this._sb;
                if (sb == null)
                {
                    return;
                }
                sb.Append(value);
            }

            [NullableContext(1)]
            public void AppendFormatted<[Nullable(2)] T>(T value)
            {
                StringBuilder sb = this._sb;
                if (sb == null)
                {
                    return;
                }
                sb.Append(value);
            }

            public void AppendFormatted<T>([Nullable(1)] T value, string format)
            {
                StringBuilder sb = this._sb;
                if (sb == null)
                {
                    return;
                }
                IFormattable formattable = value as IFormattable;
                string text;
                if (formattable == null)
                {
                    ref T ptr = ref value;
                    T t = default(T);
                    if (t == null)
                    {
                        t = value;
                        ptr = ref t;
                        if (t == null)
                        {
                            text = null;
                            goto IL_0055;
                        }
                    }
                    text = ptr.ToString();
                }
                else
                {
                    text = formattable.ToString(format, null);
                }
                IL_0055:
                sb.Append(text);
            }

            [NullableContext(1)]
            public void AppendFormatted<[Nullable(2)] T>(T value, int alignment)
            {
                if (this._sb == null)
                {
                    return;
                }
                ref T ptr = ref value;
                T t = default(T);
                string text;
                if (t == null)
                {
                    t = value;
                    ptr = ref t;
                    if (t == null)
                    {
                        text = null;
                        goto IL_003A;
                    }
                }
                text = ptr.ToString();
                IL_003A:
                string text2 = text ?? string.Empty;
                this._sb.Append((alignment > 0) ? text2.PadLeft(alignment) : ((alignment < 0) ? text2.PadRight(-alignment) : text2));
            }

            public void AppendFormatted<T>([Nullable(1)] T value, int alignment, string format)
            {
                if (this._sb == null)
                {
                    return;
                }
                IFormattable formattable = value as IFormattable;
                string text2;
                if (formattable == null)
                {
                    ref T ptr = ref value;
                    T t = default(T);
                    string text;
                    if (t == null)
                    {
                        t = value;
                        ptr = ref t;
                        if (t == null)
                        {
                            text = null;
                            goto IL_0049;
                        }
                    }
                    text = ptr.ToString();
                    IL_0049:
                    text2 = text ?? string.Empty;
                }
                else
                {
                    text2 = formattable.ToString(format, null);
                }
                string text3 = text2;
                this._sb.Append((alignment > 0) ? text3.PadLeft(alignment) : ((alignment < 0) ? text3.PadRight(-alignment) : text3));
            }

            public void AppendFormatted(string value)
            {
                StringBuilder sb = this._sb;
                if (sb == null)
                {
                    return;
                }
                sb.Append(value);
            }

            public void AppendFormatted(object value, int alignment = 0, string format = null)
            {
                if (this._sb == null)
                {
                    return;
                }
                IFormattable formattable = value as IFormattable;
                string text = ((formattable != null) ? formattable.ToString(format, null) : (((value != null) ? value.ToString() : null) ?? string.Empty));
                this._sb.Append((alignment > 0) ? text.PadLeft(alignment) : ((alignment < 0) ? text.PadRight(-alignment) : text));
            }

            [NullableContext(1)]
            internal string ToStringAndClear()
            {
                StringBuilder sb = this._sb;
                string text = ((sb != null) ? sb.ToString() : null) ?? string.Empty;
                this._sb = null;
                return text;
            }

            private StringBuilder _sb;

            public readonly bool isEnabled;
        }

        [Nullable(0)]
        [CompilerFeatureRequired("RefStructs")]
        [InterpolatedStringHandler]
        public ref struct LogInterpolatedStringHandler<[Nullable(0)] TLevel> where TLevel : struct, Logger.ILogLevelProvider
        {
            public bool IsEnabled
            {
                get
                {
                    return this._inner.isEnabled;
                }
            }

            public LogInterpolatedStringHandler(int literalLength, int formattedCount, out bool isEnabled)
            {
                TLevel tlevel = default(TLevel);
                this._inner = new Logger.LogInterpolatedStringHandler(literalLength, tlevel.Level, out isEnabled);
            }

            public void AppendLiteral(string value)
            {
                this._inner.AppendLiteral(value);
            }

            public void AppendFormatted<[Nullable(2)] T>(T value)
            {
                this._inner.AppendFormatted<T>(value);
            }

            [NullableContext(2)]
            public void AppendFormatted<T>([Nullable(1)] T value, string format)
            {
                this._inner.AppendFormatted<T>(value, format);
            }

            public void AppendFormatted<[Nullable(2)] T>(T value, int alignment)
            {
                this._inner.AppendFormatted<T>(value, alignment);
            }

            [NullableContext(2)]
            public void AppendFormatted<T>([Nullable(1)] T value, int alignment, string format)
            {
                this._inner.AppendFormatted<T>(value, alignment, format);
            }

            [NullableContext(2)]
            public void AppendFormatted(string value)
            {
                this._inner.AppendFormatted(value);
            }

            [NullableContext(2)]
            public void AppendFormatted(object value, int alignment = 0, string format = null)
            {
                this._inner.AppendFormatted(value, alignment, format);
            }

            internal string ToStringAndClear()
            {
                return this._inner.ToStringAndClear();
            }

            private Logger.LogInterpolatedStringHandler _inner;
        }

        [NullableContext(0)]
        public readonly struct TraceLogLevel : Logger.ILogLevelProvider
        {
            public LogLevel Level
            {
                get
                {
                    return LogLevel.Trace;
                }
            }
        }

        [NullableContext(0)]
        public readonly struct InfoLogLevel : Logger.ILogLevelProvider
        {
            public LogLevel Level
            {
                get
                {
                    return LogLevel.Info;
                }
            }
        }

        [NullableContext(0)]
        public readonly struct WarnLogLevel : Logger.ILogLevelProvider
        {
            public LogLevel Level
            {
                get
                {
                    return LogLevel.Warn;
                }
            }
        }

        [NullableContext(0)]
        public readonly struct ErrorLogLevel : Logger.ILogLevelProvider
        {
            public LogLevel Level
            {
                get
                {
                    return LogLevel.Error;
                }
            }
        }

        public interface ILogLevelProvider
        {
            LogLevel Level { get; }
        }
    }
}
