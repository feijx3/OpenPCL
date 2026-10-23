using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using MeloongCore.Extensions;

namespace MeloongCore
{
    [NullableContext(1)]
    [Nullable(0)]
    public class BaseLogger
    {
        public LogLevel MinLevel { get; set; }

        [NullableContext(2)]
        public virtual void Log(Exception ex, string message, LogLevel level, LogBehavior behavior, [Nullable(1)] string filePath)
        {
            if (ex != null && ex.IsCanceled())
            {
                this.Log(null, "操作已取消（" + message + "）", LogLevel.Info, LogBehavior.None, filePath);
                return;
            }
            if (this.MinLevel > level)
            {
                return;
            }
            try
            {
                string text = ((ex == null) ? (message ?? "") : (((message == null) ? "" : (message + "：")) + ex.GetDisplay(true)));
                text = this.Format(text, level, filePath, ex);
                if (this.available || this.queue.Count < 100)
                {
                    this.queue.Enqueue(new BaseLogger.LogEntry(level, text));
                    this.queuedEvent.Set();
                }
                this.HandleBehavior(message, text, behavior, ex);
            }
            catch (Exception)
            {
            }
        }

        public virtual string Format(string text, LogLevel level, string filePath, [Nullable(2)] Exception ex)
        {
            string prefix = BaseLogger.GetLogPrefix(level, filePath);
            if (string.IsNullOrEmpty(text))
            {
                return prefix;
            }
            return (from t in text.ReplaceLineEndings("\n", true).Split('\n', true)
                select prefix + t).Join("\r\n");
        }

        public static string GetLogPrefix(LogLevel level, string filePath)
        {
            return string.Format("{0:HH':'mm':'ss'.'fff} {1} {2}[{3}] ", new object[]
            {
                DateTime.Now,
                level.ToString().First<char>(),
                (Thread.CurrentThread.Name != null) ? string.Format("<{0} · {1}> ", Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.Name) : (Thread.CurrentThread.IsThreadPoolThread ? "<Async> " : ""),
                PathUtils.GetLastPart(filePath).BeforeFirst(".", false)
            });
        }

        [NullableContext(2)]
        public virtual void HandleBehavior(string rawMessage, [Nullable(1)] string formattedMessage, LogBehavior behavior, Exception ex)
        {
        }

        public virtual void Output(BaseLogger.LogEntry entry)
        {
        }

        public virtual void Init()
        {
        }

        public BaseLogger()
        {
            new Thread(delegate
            {
                try
                {
                    this.Init();
                    this.available = true;
                    bool flag;
                    Logger.LogInterpolatedStringHandler<Logger.InfoLogLevel> logInterpolatedStringHandler = new Logger.LogInterpolatedStringHandler<Logger.InfoLogLevel>(8, 1, out flag);
                    if (flag)
                    {
                        logInterpolatedStringHandler.AppendFormatted(base.GetType().Name);
                        logInterpolatedStringHandler.AppendLiteral(" 日志初始化结束");
                    }
                    Logger.Info(ref logInterpolatedStringHandler, LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\MeloongCore\\Shared\\General\\Logger.cs");
                    while (this.available)
                    {
                        this.queuedEvent.WaitOne();
                        this.Flush();
                    }
                }
                catch (Exception)
                {
                    this.available = false;
                }
            })
            {
                Name = "Logger",
                Priority = ThreadPriority.Lowest,
                IsBackground = true
            }.Start();
        }

        public void Flush()
        {
            try
            {
                if (this.available)
                {
                    object obj = this.flushLock;
                    lock (obj)
                    {
                        BaseLogger.LogEntry logEntry;
                        while (this.queue.TryDequeue(out logEntry))
                        {
                            this.Output(logEntry);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        internal volatile bool available;

        internal readonly ConcurrentQueue<BaseLogger.LogEntry> queue = new ConcurrentQueue<BaseLogger.LogEntry>();

        internal readonly AutoResetEvent queuedEvent = new AutoResetEvent(false);

        private readonly object flushLock = new object();

        [Nullable(0)]
        public struct LogEntry : IEquatable<BaseLogger.LogEntry>
        {
            public LogEntry(LogLevel Level, string message)
            {
                this.Level = Level;
                this.message = message;
            }

            public LogLevel Level { readonly get; set; }

            public string message { readonly get; set; }

            [NullableContext(0)]
            [CompilerGenerated]
            public override readonly string ToString()
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("LogEntry");
                stringBuilder.Append(" { ");
                if (this.PrintMembers(stringBuilder))
                {
                    stringBuilder.Append(' ');
                }
                stringBuilder.Append('}');
                return stringBuilder.ToString();
            }

            [NullableContext(0)]
            [CompilerGenerated]
            private readonly bool PrintMembers(StringBuilder builder)
            {
                builder.Append("Level = ");
                builder.Append(this.Level.ToString());
                builder.Append(", message = ");
                builder.Append(this.message);
                return true;
            }

            [CompilerGenerated]
            public static bool operator !=(BaseLogger.LogEntry left, BaseLogger.LogEntry right)
            {
                return !(left == right);
            }

            [CompilerGenerated]
            public static bool operator ==(BaseLogger.LogEntry left, BaseLogger.LogEntry right)
            {
                return left.Equals(right);
            }

            [CompilerGenerated]
            public override readonly int GetHashCode()
            {
                return EqualityComparer<LogLevel>.Default.GetHashCode(this.<Level>k__BackingField) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.<message>k__BackingField);
            }

            [NullableContext(0)]
            [CompilerGenerated]
            public override readonly bool Equals(object obj)
            {
                return obj is BaseLogger.LogEntry && this.Equals((BaseLogger.LogEntry)obj);
            }

            [CompilerGenerated]
            public readonly bool Equals(BaseLogger.LogEntry other)
            {
                return EqualityComparer<LogLevel>.Default.Equals(this.<Level>k__BackingField, other.<Level>k__BackingField) && EqualityComparer<string>.Default.Equals(this.<message>k__BackingField, other.<message>k__BackingField);
            }

            [CompilerGenerated]
            public readonly void Deconstruct(out LogLevel Level, out string message)
            {
                Level = this.Level;
                message = this.message;
            }
        }
    }
}
