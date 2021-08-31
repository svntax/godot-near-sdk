using Godot;
using System;
using System.IO;
using System.Security.Cryptography;
using Rebex.Security.Cryptography;
using SimpleBase;

using NearClientUnity.Utilities;

public class CryptoHelper : Node {

    public string publicKey;
    public string privateKey;

    public override void _Ready(){
        // Empty
    }

    public void CreateKeyPair(){
        byte[][] keypair = new byte[2][];

        var ed = new Ed25519();
        byte[] publicKeyBytes = ed.GetPublicKey();
        this.publicKey = SimpleBase.Base58.Bitcoin.Encode(publicKeyBytes);
        byte[] privateKeyBytes = ed.GetPrivateKey();
        this.privateKey = SimpleBase.Base58.Bitcoin.Encode(privateKeyBytes);
    }

    public string CreateSignedTransaction(string accountId, string receiverId, string methodName, byte[] methodArgs, string privKey, string pubKey, string blockHash, ulong nonce){
        // First construct and serialize the transaction
        // TODO: gas and deposit handling
        byte[] serializedAction = NearClientUnity.Action.FunctionCallByteArray(methodName, methodArgs, 20000000, 0);
        byte[] publicKeyBytes = SimpleBase.Base58.Bitcoin.Decode(pubKey).ToArray();
        byte[] blockHashBytes = SimpleBase.Base58.Bitcoin.Decode(blockHash).ToArray();

        byte[] serializedTx;
        using (var ms = new MemoryStream()){
            using (var writer = new NearBinaryWriter(ms)){
                writer.Write(accountId);

                // Public key
                writer.Write((byte)0); // Ed25519 key type
                writer.Write(publicKeyBytes);

                writer.Write(nonce);
                writer.Write(receiverId);
                writer.Write(blockHashBytes);

                writer.Write((uint)1); // Only 1 action bundled in this transaction
                writer.Write(serializedAction);

                serializedTx = ms.ToArray();
            }
        }

        // Hash the serialized transaction using sha256
        byte[] serializedTxHash;
        using (var sha256 = SHA256.Create()){
            serializedTxHash = sha256.ComputeHash(serializedTx);
        }

        // Create a signature using the hashed transaction
        byte[] privateKeyBytes = SimpleBase.Base58.Bitcoin.Decode(privKey).ToArray();
        var ed = new Ed25519();
        ed.FromPrivateKey(privateKeyBytes);
        byte[] signatureData = ed.SignMessage(serializedTxHash);

        // Encode signed transaction to serialized Borsh
        byte[] signedSerializedTx;
        using (var ms = new MemoryStream()){
            using (var writer = new NearBinaryWriter(ms)){
                // Serialized transaction
                //writer.Write(Transaction.ToByteArray());
                writer.Write(serializedTx);

                // Serialized NEAR signature
                //writer.Write(Signature.ToByteArray());
                writer.Write((byte)0); // Ed25519 key type
                writer.Write(signatureData);

                signedSerializedTx = ms.ToArray();
            }
        }

        string base64EncodedTx = Convert.ToBase64String(signedSerializedTx);

        return base64EncodedTx;
    }

}
