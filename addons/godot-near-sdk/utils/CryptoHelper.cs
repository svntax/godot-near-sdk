using Godot;
using System;
using Rebex.Security.Cryptography;
using SimpleBase;

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
        this.publicKey = Base58.Bitcoin.Encode(publicKeyBytes);
        byte[] privateKeyBytes = ed.GetPrivateKey();
        this.privateKey = Base58.Bitcoin.Encode(privateKeyBytes);

        GD.Print("Public key: " + this.publicKey);
        GD.Print("Private key: " + this.privateKey);
    }

}
