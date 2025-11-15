using System;
using System.IO;
using System.Text;
using System.Threading;

using PTD.Core.Config;

namespace PTD.Core.Logging
{
    public sealed class FileLogger : IDisposable
    {
        private readonly string _filePath;

        private StreamWriter? writer;
        private readonly object _lock = new object();
        private bool disposed = false;

        public enum LogLevel
        {
            None,
            Log,
            Error
        }

        public FileLogger(string fileName)
        {
            if (!Directory.Exists(GlobalConfig.Files.OutputDirectory))
            {
                Directory.CreateDirectory(GlobalConfig.Files.OutputDirectory);
            }

            _filePath = Path.Combine(GlobalConfig.Files.OutputDirectory, fileName);
            writer = CreateWriter();
        }

        public void Log(string message)
        {
            Write(LogLevel.Log, message);
        }

        public void Error(string message)
        {
            Write(LogLevel.Error, message);
        }

        public void Write(LogLevel level, string message)
        {
            if (disposed) { return; }

            lock (_lock)
            {
                while (true)
                {
                    try
                    {
                        if (writer == null) { writer = CreateWriter(); }

                        if (level == LogLevel.None)
                        {
                            writer.WriteLine($"{message}");
                        }
                        else
                        {
                            writer.WriteLine($"[{level.ToString().ToUpper()}] {message}");
                        }

                        return;
                    }
                    catch (IOException)
                    {
                        writer?.Dispose();
                        writer = null;
                        Thread.Sleep(GlobalConfig.Performance.RetryDelayMilliseconds);
                    }
                }
            }
        }

        public void Dispose()
        {
            lock (_lock)
            {
                if (disposed) { return; }

                writer?.Dispose();
                disposed = true;
            }
        }

        private StreamWriter CreateWriter()
        {
            while (true)
            {
                try
                {
                    return new StreamWriter(_filePath, true, Encoding.UTF8)
                    {
                        AutoFlush = true
                    };
                }
                catch (IOException)
                {
                    Thread.Sleep(GlobalConfig.Performance.RetryDelayMilliseconds);
                }
            }
        }
        

    }
}
