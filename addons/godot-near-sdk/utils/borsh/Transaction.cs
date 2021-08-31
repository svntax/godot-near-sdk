using NearClient.Utilities;
using System.Collections.Generic;
using System.IO;

namespace NearClient {

    public class Transaction {

        public static byte[] ToByteArray(string accountId, string receiverId, byte[] publicKey, ulong nonce, byte[] blockHash, byte[] serializedAction){
            using (var ms = new MemoryStream()){
                using (var writer = new NearBinaryWriter(ms)){
                    writer.Write(accountId);

                    // Public key is prepended by key type
                    writer.Write((byte)KeyType.Ed25519);
                    writer.Write(publicKey);

                    writer.Write(nonce);
                    writer.Write(receiverId);
                    writer.Write(blockHash);

                    writer.Write((uint)1); // Only 1 action bundled in this transaction
                    writer.Write(serializedAction);

                    return ms.ToArray();
                }
            }
        }
    }
}