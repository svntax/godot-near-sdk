using Godot;
using System;
using NSec.Cryptography; // Note: using version 19.5.0 due to a libsodium versioning issue with 20.2.0

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

}
