using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace MeloongCore
{
    [NullableContext(1)]
    [Nullable(0)]
    public class FileLogger : BaseLogger, IDisposable
    {
        public override void Init()
        {
            Logger.Info("开始轮转日志文件", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\MeloongCore\\Shared\\General\\Logger.cs");
            try
            {
                for (int i = 4; i >= 1; i--)
                {
                    string text = Path.Combine(this.logFolder, string.Format("{0}{1}{2}", this.fileNamePrefix, i + 1, this.fileNameSuffix));
                    string text2 = Path.Combine(this.logFolder, string.Format("{0}{1}{2}", this.fileNamePrefix, i, this.fileNameSuffix));
                    if (FileUtils.Exists(text2))
                    {
                        FileUtils.Copy(text2, text);
                        FileUtils.Delete(text2, false);
                    }
                }
            }
            catch (IOException ex)
            {
                Logger.Warn(ex, "可能同时开启了多个程序，这或许会导致未知问题", LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\MeloongCore\\Shared\\General\\Logger.cs");
            }
            catch (Exception ex2)
            {
                Logger.Warn(ex2, "整理日志文件失败", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\MeloongCore\\Shared\\General\\Logger.cs");
            }
            string text3 = PathUtils.ForApi(Path.Combine(this.logFolder, this.fileNamePrefix + "1" + this.fileNameSuffix));
            DirectoryUtils.Create(this.logFolder);
            this.writer = new StreamWriter(text3, true)
            {
                AutoFlush = true
            };
        }

        public override void Output(BaseLogger.LogEntry entry)
        {
            base.Output(entry);
            StreamWriter streamWriter = this.writer;
            if (streamWriter == null)
            {
                return;
            }
            streamWriter.WriteLine(entry.message);
        }

        public void Dispose()
        {
            this.available = false;
            if (this.writer != null)
            {
                ((IDisposable)this.writer).Dispose();
            }
            this.queuedEvent.Dispose();
            GC.SuppressFinalize(this);
        }

        public string logFolder = Path.Combine(Paths.Base, "logs");

        public string fileNamePrefix = "Log";

        public string fileNameSuffix = ".txt";

        [Nullable(2)]
        private StreamWriter writer;
    }
}
