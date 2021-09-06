using System;
using System.IO;
using System.Text;

namespace NearClient.Utilities
{
    public class NearBinaryWriter : IDisposable
    {
        public static readonly NearBinaryWriter Null = new NearBinaryWriter();

        protected Stream OutStream;
        private const int LargeByteBufferSize = 256;
        private byte[] _buffer;
        private readonly Encoder _encoder;
        private readonly Encoding _encoding;
        private byte[] _largeByteBuffer;
        private readonly bool _leaveOpen;
        private int _maxChars;        

        public NearBinaryWriter(Stream output) : this(output, new UTF8Encoding(false, true))
        {
        }

        public NearBinaryWriter(Stream output, Encoding encoding, bool leaveOpen = false)
        {
            if (output == null)
                throw new ArgumentNullException(nameof(output));
            if (!output.CanWrite)
                throw new ArgumentException("Argument_StreamNotWritable");

            OutStream = output;
            _buffer = new byte[16];
            _encoding = encoding ?? throw new ArgumentNullException(nameof(encoding));
            _encoder = _encoding.GetEncoder();
            _leaveOpen = leaveOpen;
        }

        protected NearBinaryWriter()
        {
            OutStream = Stream.Null;
            _buffer = new byte[16];
            _encoding = new UTF8Encoding(false, true);
            _encoder = _encoding.GetEncoder();
        }

        public virtual Stream BaseStream
        {
            get
            {
                Flush();
                return OutStream;
            }
        }

        public virtual void Close()
        {
            Dispose(true);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public virtual void Flush()
        {
            OutStream.Flush();
        }

        public virtual long Seek(int offset, SeekOrigin origin)
        {
            return OutStream.Seek(offset, origin);
        }

        public virtual void Write(byte value)
        {
            OutStream.WriteByte(value);
        }

        public virtual void Write(byte[] buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer");

            OutStream.Write(buffer, 0, buffer.Length);
        }

        public virtual void Write(uint value)
        {
            _buffer[0] = (byte)value;
            _buffer[1] = (byte)(value >> 8);
            _buffer[2] = (byte)(value >> 16);
            _buffer[3] = (byte)(value >> 24);
            OutStream.Write(_buffer, 0, 4);
        }

        public virtual void Write(ulong value)
        {
            _buffer[0] = (byte)value;
            _buffer[1] = (byte)(value >> 8);
            _buffer[2] = (byte)(value >> 16);
            _buffer[3] = (byte)(value >> 24);
            _buffer[4] = (byte)(value >> 32);
            _buffer[5] = (byte)(value >> 40);
            _buffer[6] = (byte)(value >> 48);
            _buffer[7] = (byte)(value >> 56);

            OutStream.Write(_buffer, 0, 8);
        }

        public virtual void Write(UInt128 value)
        {
            _buffer = value.GetBytes();
            OutStream.Write(_buffer, 0, 16);
        }

        [System.Security.SecuritySafeCritical]
        public unsafe virtual void Write(String value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            int len = _encoding.GetByteCount(value);
            Write((uint)len);

            if (_largeByteBuffer == null)
            {
                _largeByteBuffer = new byte[LargeByteBufferSize];
                _maxChars = _largeByteBuffer.Length / _encoding.GetMaxByteCount(1);
            }

            if (len <= _largeByteBuffer.Length)
            {
                _encoding.GetBytes(value, 0, value.Length, _largeByteBuffer, 0);

                OutStream.Write(_largeByteBuffer, 0, len);
            }
            else
            {
                int charStart = 0;
                int numLeft = value.Length;

                while (numLeft > 0)
                {
                    int charCount = (numLeft > _maxChars) ? _maxChars : numLeft;
                    int byteLen;

                    checked
                    {
                        if (charStart < 0 || charCount < 0 || charStart + charCount > value.Length)
                        {
                            throw new ArgumentOutOfRangeException("charCount");
                        }

                        fixed (char* pChars = value)
                        {
                            fixed (byte* pBytes = _largeByteBuffer)
                            {
                                byteLen = _encoder.GetBytes(pChars + charStart, charCount, pBytes,
                                    _largeByteBuffer.Length, charCount == numLeft);
                            }
                        }
                    }

                    OutStream.Write(_largeByteBuffer, 0, byteLen);

                    charStart += charCount;
                    numLeft -= charCount;
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;
            if (_leaveOpen)
                OutStream.Flush();
            else
                OutStream.Close();
        }
    }
}