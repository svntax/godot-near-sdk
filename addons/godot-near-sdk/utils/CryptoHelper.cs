using Godot;
using System;
using Rebex.Security.Cryptography;

public class CryptoHelper : Node {

    public byte[] publicKey;
    public byte[] privateKey;

    public override void _Ready(){
        GD.Print("Hello from C#");
    }

    public void CreateKeyPair(){
        byte[][] keypair = new byte[2][];

        var ed = new Ed25519();
        this.publicKey = ed.GetPublicKey();
        this.privateKey = ed.GetPrivateKey();

        String output = "";
        for(int i = 0; i < publicKey.Length; i++){
            output += publicKey[i] + ",";
        }
        GD.Print("Public key: " + output);
        output = "";
        for(int i = 0; i < privateKey.Length; i++){
            output += privateKey[i] + ",";
        }
        GD.Print("Private key: " + output);
    }

}
