using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;

namespace Data
{
    internal class Logger
    {
        private readonly ConcurrentQueue<string> _logQueue = new();
        private readonly string _filePath = "diagnostic_log.json";
        private bool _isLogging = true;
        private readonly Task _logTask;

        public Logger()
        {
            if (File.Exists(_filePath)) File.Delete(_filePath);
            _logTask = Task.Run(LogLoop);
        }

        public void Log(string message)
        {
            _logQueue.Enqueue(message);
        }

        private async Task LogLoop()
        {
            using StreamWriter sw = new StreamWriter(_filePath, append: true, System.Text.Encoding.ASCII);
            while (_isLogging || !_logQueue.IsEmpty)
            {
                if (_logQueue.TryDequeue(out string log))
                {
                    await sw.WriteLineAsync(log);
                }
                else
                {
                    await Task.Delay(10);
                }
            }
        }

        public void Stop()
        {
            _isLogging = false;
            _logTask.Wait();
        }
    }
}