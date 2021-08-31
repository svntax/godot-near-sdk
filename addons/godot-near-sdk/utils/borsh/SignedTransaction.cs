using NearClientUnity.Utilities;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace NearClientUnity
{
    public class SignedTransaction
    {
        public NearSignature Signature { get; set; }
        public Transaction Transaction { get; set; }

        // public static SignedTransaction FromByteArray(byte[] rawBytes)
        // {
        //     using (var ms = new MemoryStream(rawBytes))
        //     {
        //         return FromStream(ms);
        //     }
        // }

        // public static SignedTransaction FromStream(MemoryStream stream)
        // {
        //     return FromRawDataStream(stream);
        // }

        // public static SignedTransaction FromStream(ref MemoryStream stream)
        // {
        //     return FromRawDataStream(stream);
        // }

        public static async Task<Tuple<byte[], SignedTransaction>> SignTransactionAsync(string receiverId, ulong nonce, Action[] actions, ByteArray32 blockHash, Signer signer, string accountId, string networkId)
        {
            //var publicKey = await signer.GetPublicKeyAsync(accountId, networkId);
            var transaction = new Transaction
            {
                SignerId = accountId,
                //PublicKey = publicKey,
                Nonce = nonce,
                ReceiverId = receiverId,
                Actions = actions,
                BlockHash = blockHash
            };
            var message = transaction.ToByteArray();

            byte[] hash;
            using (var sha256 = SHA256.Create())
            {
                hash = sha256.ComputeHash(message);
            }

            var signature = await signer.SignMessageAsync(message, accountId, networkId);

            var signedTx = new SignedTransaction
            {
                Transaction = transaction,
                Signature = new NearSignature(signature.SignatureBytes)
            };
            var result = new Tuple<byte[], SignedTransaction>(hash, signedTx);
            return result;
        }

        public static async Task<Tuple<byte[], SignedTransaction>> SignTransactionAsync(string receiverId, ulong nonce, Action[] actions, ByteArray32 blockHash, Signer signer, string accountId)
        {
            //var publicKey = await signer.GetPublicKeyAsync(accountId);
            var transaction = new Transaction
            {
                SignerId = accountId,
                //PublicKey = publicKey,
                Nonce = nonce,
                ReceiverId = receiverId,
                Actions = actions,
                BlockHash = blockHash
            };
            var message = transaction.ToByteArray();

            byte[] hash;
            using (var sha256 = SHA256.Create())
            {
                hash = sha256.ComputeHash(message);
            }

            var signature = await signer.SignMessageAsync(message, accountId);

            var signedTx = new SignedTransaction
            {
                Transaction = transaction,
                Signature = new NearSignature(signature.SignatureBytes)
            };

            var result = new Tuple<byte[], SignedTransaction>(message, signedTx);
            return result;
        }

        public static async Task<Tuple<byte[], SignedTransaction>> SignTransactionAsync(string receiverId, ulong nonce, Action[] actions, ByteArray32 blockHash, Signer signer)
        {
            //var publicKey = await signer.GetPublicKeyAsync();
            var transaction = new Transaction
            {
                //PublicKey = publicKey,
                Nonce = nonce,
                ReceiverId = receiverId,
                Actions = actions,
                BlockHash = blockHash
            };
            var message = transaction.ToByteArray();

            byte[] hash;
            using (var sha256 = SHA256.Create())
            {
                hash = sha256.ComputeHash(message);
            }

            var signature = await signer.SignMessageAsync(message);

            var signedTx = new SignedTransaction
            {
                Transaction = transaction,
                Signature = new NearSignature(signature.SignatureBytes)
            };

            var result = new Tuple<byte[], SignedTransaction>(hash, signedTx);
            return result;
        }

        public byte[] ToByteArray()
        {
            using (var ms = new MemoryStream())
            {
                using (var writer = new NearBinaryWriter(ms))
                {
                    writer.Write(Transaction.ToByteArray());
                    writer.Write(Signature.ToByteArray());
                    return ms.ToArray();
                }
            }
        }

        // private static SignedTransaction FromRawDataStream(MemoryStream stream)
        // {
        //     using (var reader = new NearBinaryReader(stream, true))
        //     {
        //         var transaction = Transaction.FromStream(ref stream);
        //         var signature = NearSignature.FromStream(ref stream);

        //         return new SignedTransaction()
        //         {
        //             Transaction = transaction,
        //             Signature = signature,
        //         };
        //     }
        // }
    }
}