using System;
using System.IO;
using System.Text;

namespace Dev.Tests.Helpers;

internal class MemoryTextStream : IDisposable
{
    private readonly MemoryStream _stream;
    private readonly StreamWriter _writer;
    private bool _disposed = false;

    private MemoryTextStream()
    {
        _stream = new MemoryStream();
        _writer = new StreamWriter(_stream);
    }

    public static MemoryTextStream Create()
    {
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

    public void Clear()
    {
        _writer.Flush();
        _stream.SetLength(0);
        _stream.Position = 0;
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _writer.Dispose();
            _stream.Dispose();
            _disposed = true;
        }
    }
}
