using Godot;
using System;
using System.IO;
using System.Security.Cryptography;
using Rebex.Security.Cryptography;
using SimpleBase;

using NearClient;
using NearClient.Utilities;

public class CryptoHelper : Node {

    public string publicKey;
    public string privateKey;

    public override void _Ready(){
        // Empty
    }

    public void CreateKeyPair(){
        var ed = new Ed25519();
        byte[] publicKeyBytes = ed.GetPublicKey();
        this.publicKey = SimpleBase.Base58.Bitcoin.Encode(publicKeyBytes);
        byte[] privateKeyBytes = ed.GetPrivateKey();
        this.privateKey = SimpleBase.Base58.Bitcoin.Encode(privateKeyBytes);
    }

    public string CreateTransaction(string accountId, string receiverId, string methodName, byte[] methodArgs, string pubKey, string blockHash, ulong nonce, ulong gas, ulong deposit){
        byte[] serializedAction = NearClient.Action.FunctionCallByteArray(methodName, methodArgs, gas, deposit);
        byte[] publicKeyBytes = SimpleBase.Base58.Bitcoin.Decode(pubKey).ToArray();
        byte[] blockHashBytes = SimpleBase.Base58.Bitcoin.Decode(blockHash).ToArray();

        byte[] serializedTx = Transaction.ToByteArray(accountId, receiverId, publicKeyBytes, nonce, blockHashBytes, serializedAction);

        string base64EncodedTx = Convert.ToBase64String(serializedTx);

        return base64EncodedTx;
    }

    public string CreateSignedTransaction(string accountId, string receiverId, string methodName, byte[] methodArgs, string privKey, string pubKey, string blockHash, ulong nonce, ulong gas, ulong deposit){
        // First construct and serialize the transaction
        byte[] serializedAction = NearClient.Action.FunctionCallByteArray(methodName, methodArgs, gas, deposit);
        byte[] publicKeyBytes = SimpleBase.Base58.Bitcoin.Decode(pubKey).ToArray();
        byte[] blockHashBytes = SimpleBase.Base58.Bitcoin.Decode(blockHash).ToArray();

        byte[] serializedTx = Transaction.ToByteArray(accountId, receiverId, publicKeyBytes, nonce, blockHashBytes, serializedAction);

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
                writer.Write(serializedTx);

                // Serialized NEAR signature
                writer.Write((byte)KeyType.Ed25519);
                writer.Write(signatureData);

                signedSerializedTx = ms.ToArray();
            }
        }

        string base64EncodedTx = Convert.ToBase64String(signedSerializedTx);

        return base64EncodedTx;
    }

}
