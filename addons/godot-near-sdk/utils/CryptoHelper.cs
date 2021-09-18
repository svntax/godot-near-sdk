using Godot;
using System;
using System.IO;
using System.Security.Cryptography;
using Rebex.Security.Cryptography;
using SimpleBase;

using NearClient;
using NearClient.Utilities;

public class CryptoHelper : Node {

    public const int NEAR_NOMINATION_EXP = 24;
    public const float MINIMUM_REQUIRED_ALLOWANCE = 0.05F;

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

    // Converts a given amount of NEAR to yoctoNEAR units
    public UInt128 nearToYocto(float amount){
        string amountAsString = amount.ToString();
		string[] split = amountAsString.Split(".");
		string wholePart = split[0];
		string fracPart = "";
        // TODO: check if fractional part is too long?
        if(split.Length == 2){
            fracPart = split[1];
        }
		string yoctoString = wholePart + fracPart.PadRight(NEAR_NOMINATION_EXP, '0');
		return UInt128.Parse(yoctoString);
    }

    public bool CheckEnoughAllowance(string allowance){
        UInt128 allowanceValue = UInt128.Parse(allowance);
        UInt128 minimumRequiredAllowance = nearToYocto(MINIMUM_REQUIRED_ALLOWANCE);
        return allowanceValue >= minimumRequiredAllowance;
    }

    public string CreateTransaction(string accountId, string receiverId, string methodName, byte[] methodArgs, string pubKey, string blockHash, ulong nonce, ulong gas, float deposit){
        UInt128 depositInYoctoNear = nearToYocto(deposit);
        byte[] serializedAction = NearClient.Action.FunctionCallByteArray(methodName, methodArgs, gas, depositInYoctoNear);
        byte[] publicKeyBytes = SimpleBase.Base58.Bitcoin.Decode(pubKey).ToArray();
        byte[] blockHashBytes = SimpleBase.Base58.Bitcoin.Decode(blockHash).ToArray();

        byte[] serializedTx = Transaction.ToByteArray(accountId, receiverId, publicKeyBytes, nonce, blockHashBytes, serializedAction);

        string base64EncodedTx = Convert.ToBase64String(serializedTx);

        return base64EncodedTx;
    }

    public string CreateSignedTransaction(string accountId, string receiverId, string methodName, byte[] methodArgs, string privKey, string pubKey, string blockHash, ulong nonce, ulong gas, float deposit){
        // First construct and serialize the transaction
        UInt128 depositInYoctoNear = nearToYocto(deposit);
        byte[] serializedAction = NearClient.Action.FunctionCallByteArray(methodName, methodArgs, gas, depositInYoctoNear);
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
