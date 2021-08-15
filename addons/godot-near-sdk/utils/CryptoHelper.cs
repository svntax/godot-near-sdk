using Godot;
using System;
using NSec.Cryptography;

public class CryptoHelper : Node {

    public override void _Ready(){
        // TODO: placeholder, taken from https://nsec.rocks/

        // select the Ed25519 signature algorithm
        var algorithm = SignatureAlgorithm.Ed25519;

        // create a new key pair
        using var key = Key.Create(algorithm);

        // generate some data to be signed
        var data = System.Text.Encoding.UTF8.GetBytes("Use the Force, Luke!");

        // sign the data using the private key
        var signature = algorithm.Sign(key, data);

        // verify the data using the signature and the public key
        if(algorithm.Verify(key.PublicKey, data, signature)){
            GD.Print("Verified data using Ed25519.");
        }
    }

    public byte[][] CreateKeyPair(){
        var algorithm = SignatureAlgorithm.Ed25519;
        var creationParameters = new KeyCreationParameters
        {
            ExportPolicy = KeyExportPolicies.AllowPlaintextArchiving
        };
        using Key key = Key.Create(algorithm, creationParameters);
        byte[][] keypair = new byte[2][];
        byte[] privateKey = key.Export(KeyBlobFormat.RawPrivateKey);
        byte[] publicKey = key.Export(KeyBlobFormat.RawPublicKey);
        keypair[0] = publicKey;
        keypair[1] = privateKey;
        for(int i = 0; i < 2; i++){
            String output = "";
            for(int j = 0; j < keypair[i].Length; j++){
                output += keypair[i][j] + ",";
            }
            GD.Print(output);
        }
        return keypair;
    }

}
