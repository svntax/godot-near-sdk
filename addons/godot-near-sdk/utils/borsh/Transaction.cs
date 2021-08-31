using NearClientUnity.Utilities;
using System.Collections.Generic;
using System.IO;

namespace NearClientUnity
{
    public class Transaction
    {
        public Action[] Actions { get; set; }
        public ByteArray32 BlockHash { get; set; }
        public ulong Nonce { get; set; }
        //public PublicKey PublicKey { get; set; }
        public string ReceiverId { get; set; }
        public string SignerId { get; set; }

        // public static Transaction FromByteArray(byte[] rawBytes)
        // {
        //     using (var ms = new MemoryStream(rawBytes))
        //     {
        //         return FromStream(ms);
        //     }
        // }

        // public static Transaction FromStream(MemoryStream stream)
        // {
        //     return FromRawDataStream(stream);
        // }

        // public static Transaction FromStream(ref MemoryStream stream)
        // {
        //     return FromRawDataStream(stream);
        // }

        public byte[] ToByteArray()
        {
            using (var ms = new MemoryStream())
            {
                using (var writer = new NearBinaryWriter(ms))
                {
                    writer.Write(SignerId);
                    //writer.Write(PublicKey.ToByteArray());
                    writer.Write(Nonce);
                    writer.Write(ReceiverId);
                    writer.Write(BlockHash.Buffer);

                    writer.Write((uint)Actions.Length);
                    foreach (var action in Actions)
                    {
                        //writer.Write(action.ToByteArray());
                    }

                    return ms.ToArray();
                }
            }
        }

        // private static Transaction FromRawDataStream(MemoryStream stream)
        // {
        //     using (var reader = new NearBinaryReader(stream, true))
        //     {
        //         var signerId = reader.ReadString();
        //         var publicKey = PublicKey.FromStream(ref stream);
        //         var nonce = reader.ReadULong();
        //         var receiverId = reader.ReadString();
        //         var blockHash = new ByteArray32() { Buffer = reader.ReadBytes(32) };
        //         var actionsCount = reader.ReadUInt();
        //         var actions = new List<Action>();

        //         for (var i = 0; i < actionsCount; i++)
        //         {
        //             actions.Add(Action.FromStream(ref stream));
        //         }

        //         return new Transaction()
        //         {
        //             SignerId = signerId,
        //             PublicKey = publicKey,
        //             Nonce = nonce,
        //             ReceiverId = receiverId,
        //             BlockHash = blockHash,
        //             Actions = actions.ToArray()
        //         };
        //     }
        // }
    }
}