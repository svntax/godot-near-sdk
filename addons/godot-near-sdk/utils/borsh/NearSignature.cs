using NearClientUnity.Utilities;
using System;
using System.IO;

namespace NearClientUnity
{
    public class NearSignature
    {
        private ByteArray64 _data;
        private readonly KeyType _keyType;

        public NearSignature(byte[] signature)
        {
            _keyType = KeyType.Ed25519;
            _data = new ByteArray64() { Buffer = signature };
        }

        public NearSignature(KeyType keyType, ByteArray64 data)
        {
            _keyType = KeyType.Ed25519;
            _data = data;
        }

        public ByteArray64 Data => _data;

        public KeyType KeyType => _keyType;

        public static NearSignature FromByteArray(byte[] rawBytes)
        {
            if (rawBytes.Length != 65) throw new ArgumentException("Invalid raw bytes for near signature");
            using (var ms = new MemoryStream(rawBytes))
            {
                return FromStream(ms);
            }
        }

        public static NearSignature FromStream(MemoryStream stream)
        {
            return FromRawDataStream(stream);
        }

        public static NearSignature FromStream(ref MemoryStream stream)
        {
            return FromRawDataStream(stream);
        }

        public byte[] ToByteArray()
        {
            using (var ms = new MemoryStream())
            {
                using (var writer = new NearBinaryWriter(ms))
                {
                    writer.Write((byte)_keyType);
                    writer.Write(_data.Buffer);
                    return ms.ToArray();
                }
            }
        }

        private static NearSignature FromRawDataStream(MemoryStream stream)
        {
            using (var reader = new NearBinaryReader(stream, true))
            {
                KeyType keyType;
                switch ((int)reader.ReadByte())
                {
                    case 0:
                        {
                            keyType = KeyType.Ed25519;
                            break;
                        }
                    default:
                        {
                            throw new NotSupportedException("Invalid key type in raw bytes for public key");
                        }
                }

                var data = new ByteArray64
                {
                    Buffer = reader.ReadBytes(64)
                };

                return new NearSignature(keyType, data);
            }
        }
    }
}