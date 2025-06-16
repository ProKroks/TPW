using System.Collections.Concurrent;
using Newtonsoft.Json;
using System.Text;

namespace Data
{
    internal class DataLogger
    {
        private readonly BlockingCollection<LogBall> _queue;
        private readonly string _pathToFile;
        private readonly int _queueSize = 100;
        private bool _queueOverflow;
        private readonly object _locker = new object();
        private static readonly object _singletonLocker = new object();
        private static DataLogger? _dataLogger = null;

        public static DataLogger GetInstance()
        {
            lock (_singletonLocker)
            {
                _dataLogger ??= new DataLogger();
                return _dataLogger;
            }
        }

        private DataLogger()
        {
            _queue = new BlockingCollection<LogBall>(new ConcurrentQueue<LogBall>(), _queueSize);
            string tempPath = Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.Parent?.FullName ?? string.Empty;
            string loggersDir = Path.Combine(tempPath, "Loggers");
            Directory.CreateDirectory(loggersDir);
            _pathToFile = Path.Combine(loggersDir, "logs.json");
            WriteToFile();
        }

        public void AddBall(LogBall logBall)
        {
            if (!_queue.TryAdd(logBall))
            {
                lock (_locker)
                {
                    _queueOverflow = true;
                }
            }
        }

        private void WriteToFile()
        {
            Task.Run(async () =>
            {
                using (StreamWriter streamWriter = new StreamWriter(_pathToFile, false, Encoding.UTF8))
                {
                    while (!_queue.IsCompleted)
                    {
                        if (_queueOverflow)
                        {
                            await streamWriter.WriteLineAsync("Queue overflow");
                            _queueOverflow = false;
                        }

                        LogBall ball = _queue.Take(); // Czeka na element
                        string jsonString = JsonConvert.SerializeObject(ball);
                        await streamWriter.WriteLineAsync(jsonString);
                        await streamWriter.FlushAsync();
                    }
                }
            });
        }
    }
}