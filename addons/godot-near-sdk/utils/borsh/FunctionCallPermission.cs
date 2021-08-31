using NearClientUnity.Utilities;
using System.Collections.Generic;
using System.IO;

namespace NearClientUnity
{
    public class FunctionCallPermission
    {
        public UInt128? Allowance { get; set; }
        public string[] MethodNames { get; set; }
        public string ReceiverId { get; set; }

        public static FunctionCallPermission FromByteArray(byte[] rawBytes)
        {
            using (var ms = new MemoryStream(rawBytes))
            {
                return FromStream(ms);
            }
        }

        public static FunctionCallPermission FromStream(MemoryStream stream)
        {
            return FromRawDataStream(stream);
        }

        public static FunctionCallPermission FromStream(ref MemoryStream stream)
        {
            return FromRawDataStream(stream);
        }

        public byte[] ToByteArray()
        {
            using (var ms = new MemoryStream())
            {
                using (var writer = new NearBinaryWriter(ms))
                {
                    if (Allowance == null)
                    {
                        writer.Write((byte)0);
                    }
                    else
                    {
                        writer.Write((byte)1);
                        writer.Write((UInt128)Allowance);
                    }

                    writer.Write(ReceiverId);

                    writer.Write((uint)MethodNames.Length);

                    foreach (var mn in MethodNames)
                    {
                        writer.Write(mn);
                    }

                    return ms.ToArray();
                }
            }
        }

        private static FunctionCallPermission FromRawDataStream(MemoryStream stream)
        {
            using (var reader = new NearBinaryReader(stream, true))
            {
                UInt128? allowance = null;

                var allowanceFlag = reader.ReadByte();

                if ((int)allowanceFlag == 1) allowance = reader.ReadUInt128();

                var receiverId = reader.ReadString();

                var methodNamesCount = reader.ReadUInt();

                var methodNames = new List<string>();

                for (var i = 0; i < methodNamesCount; i++)
                {
                    methodNames.Add(reader.ReadString());
                }

                return new FunctionCallPermission()
                {
                    Allowance = allowance,
                    ReceiverId = receiverId,
                    MethodNames = methodNames.ToArray()
                };
            }
        }
    }
}