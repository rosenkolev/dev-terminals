using System;
using System.IO;
using System.Text;
using System.Threading;

namespace DevOps.Tests.Helpers
{
    internal class MemoryTextStream : IDisposable
    {
        private readonly MemoryStream _stream;
        private readonly StreamWriter _writer;
        private bool _disposed = false;
        private static readonly object _lock = new object();

        private MemoryTextStream()
        {
            _stream = new MemoryStream();
            _writer = new StreamWriter(_stream);
        }

        public static MemoryTextStream Create()
        {
            Monitor.Enter(_lock);
            return new MemoryTextStream();
        }

        public StreamWriter Writer => _writer;

        public string GetText()
        {
            _writer.Flush();
            _stream.Flush();

            var value = Encoding.ASCII.GetString(_stream.ToArray());

            return value;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _writer.Dispose();
                _stream.Dispose();
                _disposed = true;
                Monitor.Exit(_lock);
            }
        }
    }
}
