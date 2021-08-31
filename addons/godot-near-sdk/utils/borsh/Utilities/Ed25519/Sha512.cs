﻿using NearClientUnity.Utilities.Ed25519.Internal;
using System;

namespace NearClientUnity.Utilities.Ed25519
{
    public class Sha512
    {
        public const int BlockSize = 128;
        private static readonly byte[] Padding = { 0x80 };
        private readonly byte[] _buffer;
        private Array8<ulong> _state;
        private ulong _totalBytes;

        /// <summary>
        /// Allocation and initialization of the new SHA-512 object.
        /// </summary>
        public Sha512()
        {
            _buffer = new byte[BlockSize];
            Init();
        }

        /// <summary>
        /// Calculates SHA-512 hash value for the given bytes array.
        /// </summary>
        /// <param name="data">
        /// Data bytes array
        /// </param>
        /// <returns>
        /// Hash bytes
        /// </returns>
        public static byte[] Hash(byte[] data)
        {
            return Hash(data, 0, data.Length);
        }

        /// <summary>
        /// Calculates SHA-512 hash value for the given bytes array.
        /// </summary>
        /// <param name="data">
        /// Data bytes array
        /// </param>
        /// <param name="index">
        /// Offset of byte sequence
        /// </param>
        /// <param name="length">
        /// Sequence length
        /// </param>
        /// <returns>
        /// Hash bytes
        /// </returns>
        public static byte[] Hash(byte[] data, int index, int length)
        {
            var hasher = new Sha512();
            hasher.Update(data, index, length);
            return hasher.Finalize();
        }

        /// <summary>
        /// Finalizes SHA-512 hashing
        /// </summary>
        /// <param name="output">
        /// Output buffer
        /// </param>
        public void Finalize(ArraySegment<byte> output)
        {
            Update(Padding, 0, Padding.Length);
            Array16<ulong> block;
            ByteIntegerConverter.Array16LoadBigEndian64(out block, _buffer, 0);
            CryptoBytes.InternalWipe(_buffer, 0, _buffer.Length);
            var bytesInBuffer = (int)_totalBytes & (BlockSize - 1);
            if (bytesInBuffer > BlockSize - 16)
            {
                Sha512Internal.Core(out _state, ref _state, ref block);
                block = default;
            }

            block.X15 = (_totalBytes - 1) * 8;
            Sha512Internal.Core(out _state, ref _state, ref block);

            ByteIntegerConverter.StoreBigEndian64(output.Array, output.Offset + 0, _state.X0);
            ByteIntegerConverter.StoreBigEndian64(output.Array, output.Offset + 8, _state.X1);
            ByteIntegerConverter.StoreBigEndian64(output.Array, output.Offset + 16, _state.X2);
            ByteIntegerConverter.StoreBigEndian64(output.Array, output.Offset + 24, _state.X3);
            ByteIntegerConverter.StoreBigEndian64(output.Array, output.Offset + 32, _state.X4);
            ByteIntegerConverter.StoreBigEndian64(output.Array, output.Offset + 40, _state.X5);
            ByteIntegerConverter.StoreBigEndian64(output.Array, output.Offset + 48, _state.X6);
            ByteIntegerConverter.StoreBigEndian64(output.Array, output.Offset + 56, _state.X7);
            _state = default;
        }

        /// <summary>
        /// Finalizes SHA-512 hashing.
        /// </summary>
        /// <returns>
        /// Hash bytes
        /// </returns>
        public byte[] Finalize()
        {
            var result = new byte[64];
            Finalize(new ArraySegment<byte>(result));
            return result;
        }

        /// <summary>
        /// Performs an initialization of internal SHA-512 state.
        /// </summary>
        public void Init()
        {
            Sha512Internal.Sha512Init(out _state);
            _totalBytes = 0;
        }

        /// <summary>
        /// Updates internal state with data from the provided array segment.
        /// </summary>
        /// <param name="data">
        /// Array segment
        /// </param>
        public void Update(ArraySegment<byte> data)
        {
            Update(data.Array, data.Offset, data.Count);
        }

        /// <summary>
        /// Updates internal state with data from the provided array.
        /// </summary>
        /// <param name="data">
        /// Array of bytes
        /// </param>
        /// <param name="index">
        /// Offset of byte sequence
        /// </param>
        /// <param name="length">
        /// Sequence length
        /// </param>
        public void Update(byte[] data, int index, int length)
        {
            Array16<ulong> block;
            var bytesInBuffer = (int)_totalBytes & (BlockSize - 1);
            _totalBytes += (uint)length;

            if (_totalBytes >= ulong.MaxValue / 8)
                throw new InvalidOperationException("Too much data");
            // Fill existing buffer
            if (bytesInBuffer != 0)
            {
                var toCopy = Math.Min(BlockSize - bytesInBuffer, length);
                Buffer.BlockCopy(data, index, _buffer, bytesInBuffer, toCopy);
                index += toCopy;
                length -= toCopy;
                bytesInBuffer += toCopy;
                if (bytesInBuffer == BlockSize)
                {
                    ByteIntegerConverter.Array16LoadBigEndian64(out block, _buffer, 0);
                    Sha512Internal.Core(out _state, ref _state, ref block);
                    CryptoBytes.InternalWipe(_buffer, 0, _buffer.Length);
                    bytesInBuffer = 0;
                }
            }

            // Hash complete blocks without copying
            while (length >= BlockSize)
            {
                ByteIntegerConverter.Array16LoadBigEndian64(out block, data, index);
                Sha512Internal.Core(out _state, ref _state, ref block);
                index += BlockSize;
                length -= BlockSize;
            }

            // Copy remainder into buffer
            if (length > 0) Buffer.BlockCopy(data, index, _buffer, bytesInBuffer, length);
        }
    }
}