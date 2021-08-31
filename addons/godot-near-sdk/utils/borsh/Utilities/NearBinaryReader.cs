using System;
using System.IO;
using System.Text;

namespace NearClientUnity.Utilities
{
    public class NearBinaryReader : IDisposable
    {
        private const int MaxCharBytesSize = 128;

        private readonly bool _leaveOpen;
        private readonly int _maxCharsSize;
        private byte[] _buffer;
        private char[] _charBuffer;
        private byte[] _charBytes;
        private Decoder _decoder;
        private Stream _stream;

        // From MaxCharBytesSize & Encoding
        public NearBinaryReader(Stream input) : this(input, new UTF8Encoding())
        {
        }

        public NearBinaryReader(Stream input, bool leaveOpen) : this(input, new UTF8Encoding(), leaveOpen)
        {
        }

        public NearBinaryReader(Stream input, Encoding encoding, bool leaveOpen = false)
        {
            _stream = input;
            _decoder = encoding.GetDecoder();
            _maxCharsSize = encoding.GetMaxCharCount(MaxCharBytesSize);
            var minBufferSize = encoding.GetMaxByteCount(1); // max bytes per one char

            if (minBufferSize < 16)
                minBufferSize = 16;

            _buffer = new byte[minBufferSize];
            _leaveOpen = leaveOpen;
        }

        public virtual Stream BaseStream => _stream;

        public virtual void Close()
        {
            Dispose(true);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public virtual byte ReadByte()
        {
            var b = _stream.ReadByte();
            return (byte)b;
        }

        public virtual byte[] ReadBytes(int count)
        {
            if (count < 0) throw new ArgumentOutOfRangeException("count", "ArgumentOutOfRange_NeedNonNegNum");

            if (count == 0)
            {
                var emptyArray = new byte[0];
                return emptyArray;
            }

            var result = new byte[count];

            var numRead = 0;
            do
            {
                var n = _stream.Read(result, numRead, count);
                if (n == 0)
                    break;
                numRead += n;
                count -= n;
            } while (count > 0);

            return result;
        }

        public virtual string ReadString()
        {
            var currentPosition = 0;

            var stringLength = (int)ReadUInt();

            if (stringLength == 0)
            {
                return string.Empty;
            }

            if (_charBytes == null)
            {
                _charBytes = new byte[MaxCharBytesSize];
            }

            if (_charBuffer == null)
            {
                _charBuffer = new char[_maxCharsSize];
            }

            StringBuilder sb = null;
            do
            {
                var readLength = ((stringLength - currentPosition) > MaxCharBytesSize)
                    ? MaxCharBytesSize
                    : (stringLength - currentPosition);

                var n = _stream.Read(_charBytes, 0, readLength);

                var charsRead = _decoder.GetChars(_charBytes, 0, n, _charBuffer, 0);

                if (currentPosition == 0 && n == stringLength)
                    return new string(_charBuffer, 0, charsRead);

                if (sb == null)
                    sb = StringBuilderCache.Acquire(stringLength); // Actual string length in chars may be smaller.
                sb.Append(_charBuffer, 0, charsRead);
                currentPosition += n;
            } while (currentPosition < stringLength);

            return StringBuilderCache.GetStringAndRelease(sb);
        }

        public virtual uint ReadUInt()
        {
            FillBuffer(4);
            return (uint)(_buffer[0] | _buffer[1] << 8 | _buffer[2] << 16 | _buffer[3] << 24);
        }

        public virtual UInt128 ReadUInt128()
        {
            FillBuffer(16);

            var result = new UInt128(_buffer);

            return result;
        }

        public virtual ulong ReadULong()
        {
            FillBuffer(8);
            var lo = (uint)(_buffer[0] | _buffer[1] << 8 |
                             _buffer[2] << 16 | _buffer[3] << 24);
            var hi = (uint)(_buffer[4] | _buffer[5] << 8 |
                             _buffer[6] << 16 | _buffer[7] << 24);
            return ((ulong)hi) << 32 | lo;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                var copyOfStream = _stream;
                _stream = null;
                if (copyOfStream != null && !_leaveOpen)
                    copyOfStream.Close();
            }

            _stream = null;
            _buffer = null;
            _decoder = null;
            _charBytes = null;
            _charBuffer = null;
        }

        protected virtual void FillBuffer(int numBytes)
        {
            if (_buffer != null && (numBytes < 0 || numBytes > _buffer.Length))
            {
                throw new ArgumentOutOfRangeException("numBytes", "ArgumentOutOfRange_BinaryReaderFillBuffer");
            }

            var bytesRead = 0;
            int n;

            if (numBytes == 1)
            {
                n = _stream.ReadByte();
                _buffer[0] = (byte)n;
                return;
            }

            do
            {
                n = _stream.Read(_buffer, bytesRead, numBytes - bytesRead);
                bytesRead += n;
            } while (bytesRead < numBytes);
        }
    }

    internal static class StringBuilderCache
    {
        private const int MaxBuilderSize = 256;
        [ThreadStatic] private static StringBuilder _cachedInstance;

        public static StringBuilder Acquire(int capacity = 16)
        {
            if (capacity > MaxBuilderSize) return new StringBuilder(capacity);
            var cachedInstance = _cachedInstance;
            if (cachedInstance == null || capacity > cachedInstance.Capacity) return new StringBuilder(capacity);
            _cachedInstance = null;
            cachedInstance.Clear();
            return cachedInstance;
        }

        public static string GetStringAndRelease(StringBuilder sb)
        {
            var result = sb.ToString();
            Release(sb);
            return result;
        }

        private static void Release(StringBuilder sb)
        {
            if (sb.Capacity <= MaxBuilderSize)
            {
                _cachedInstance = sb;
            }
        }
    }
}