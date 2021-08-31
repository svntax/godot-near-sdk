using NearClientUnity.Utilities;
using System.IO;

namespace NearClientUnity
{
    public class FullAccessPermission
    {
        public static FullAccessPermission FromByteArray(byte[] rawBytes)
        {
            using (var ms = new MemoryStream(rawBytes))
            {
                return FromStream(ms);
            }
        }

        public static FullAccessPermission FromStream(MemoryStream stream)
        {
            return FromRawDataStream(stream);
        }

        public static FullAccessPermission FromStream(ref MemoryStream stream)
        {
            return FromRawDataStream(stream);
        }

        public byte[] ToByteArray()
        {
            using (var ms = new MemoryStream())
            {
                using (var writer = new NearBinaryWriter(ms))
                {
                    return ms.ToArray();
                }
            }
        }

        private static FullAccessPermission FromRawDataStream(MemoryStream stream)
        {
            using (var reader = new NearBinaryReader(stream, true))
            {
                return new FullAccessPermission();
            }
        }
    }
}