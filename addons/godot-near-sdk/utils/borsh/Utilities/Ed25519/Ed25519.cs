﻿using NearClientUnity.Utilities.Ed25519.Internal.Ed25519Ref10.Ed25519Ops;
using System;

namespace NearClientUnity.Utilities.Ed25519
{
    public static class Ed25519
    {
        /// <summary>
        /// A 64 byte expanded form of private key. This form is used internally to improve performance
        /// </summary>
        public const int ExpandedPrivateKeySize = 32 * 2;

        /// <summary>
        /// Private key seeds are 32 byte arbitrary values. This is the form that should be
        /// generated and stored.
        /// </summary>
        public const int PrivateKeySeedSize = 32;

        /// <summary>
        /// Public Keys are 32 byte values. All possible values of this size a valid.
        /// </summary>
        public const int PublicKeySize = 32;

        /// <summary>
        /// Signatures are 64 byte values
        /// </summary>
        public const int SignatureSize = 64;

        /// <summary>
        /// Calculate expanded form of private key from the key seed.
        /// </summary>
        /// <param name="privateKeySeed">
        /// Private key seed value
        /// </param>
        /// <returns>
        /// Expanded form of the private key
        /// </returns>
        public static byte[] ExpandedPrivateKeyFromSeed(byte[] privateKeySeed)
        {
            byte[] privateKey;
            byte[] publicKey;
            KeyPairFromSeed(out publicKey, out privateKey, privateKeySeed);
            CryptoBytes.Wipe(publicKey);
            return privateKey;
        }

        /// <summary>
        /// Calculate key pair from the key seed.
        /// </summary>
        /// <param name="publicKey">
        /// Public key
        /// </param>
        /// <param name="expandedPrivateKey">
        /// Expanded form of the private key
        /// </param>
        /// <param name="privateKeySeed">
        /// Private key seed value
        /// </param>
        public static void KeyPairFromSeed(out byte[] publicKey, out byte[] expandedPrivateKey, byte[] privateKeySeed)
        {
            var pk = new byte[PublicKeySize];
            var sk = new byte[ExpandedPrivateKeySize];

            Ed25519Operations.CryptoSignKeyPair(pk, 0, sk, 0, privateKeySeed, 0);
            publicKey = pk;
            expandedPrivateKey = sk;
        }

        /// <summary>
        /// Calculate key pair from the key seed.
        /// </summary>
        /// <param name="publicKey">
        /// Public key
        /// </param>
        /// <param name="expandedPrivateKey">
        /// Expanded form of the private key
        /// </param>
        /// <param name="privateKeySeed">
        /// Private key seed value
        /// </param>
        public static void KeyPairFromSeed(ArraySegment<byte> publicKey, ArraySegment<byte> expandedPrivateKey,
            ArraySegment<byte> privateKeySeed)
        {
            Ed25519Operations.CryptoSignKeyPair(
                publicKey.Array, publicKey.Offset,
                expandedPrivateKey.Array, expandedPrivateKey.Offset,
                privateKeySeed.Array, privateKeySeed.Offset);
        }

        /// <summary>
        /// Calculate public key from private key seed
        /// </summary>
        /// <param name="privateKeySeed">
        /// Private key seed value
        /// </param>
        /// <returns>
        /// </returns>
        public static byte[] PublicKeyFromSeed(byte[] privateKeySeed)
        {
            byte[] privateKey;
            byte[] publicKey;
            KeyPairFromSeed(out publicKey, out privateKey, privateKeySeed);
            CryptoBytes.Wipe(privateKey);
            return publicKey;
        }

        /// <summary>
        /// Create new Ed25519 signature
        /// </summary>
        /// <param name="signature">
        /// Buffer for signature
        /// </param>
        /// <param name="message">
        /// Message bytes
        /// </param>
        /// <param name="expandedPrivateKey">
        /// Expanded form of private key
        /// </param>
        public static void Sign(ArraySegment<byte> signature, ArraySegment<byte> message,
            ArraySegment<byte> expandedPrivateKey)
        {
            Ed25519Operations.CryptoSign(signature.Array, signature.Offset, message.Array, message.Offset,
                message.Count, expandedPrivateKey.Array, expandedPrivateKey.Offset);
        }

        /// <summary>
        /// Create new Ed25519 signature
        /// </summary>
        /// <param name="message">
        /// Message bytes
        /// </param>
        /// <param name="expandedPrivateKey">
        /// Expanded form of private key
        /// </param>
        public static byte[] Sign(byte[] message, byte[] expandedPrivateKey)
        {
            var signature = new byte[SignatureSize];
            Sign(new ArraySegment<byte>(signature), new ArraySegment<byte>(message),
                new ArraySegment<byte>(expandedPrivateKey));
            return signature;
        }

        /// <summary>
        /// Verify Ed25519 signature
        /// </summary>
        /// <param name="signature">
        /// Signature bytes
        /// </param>
        /// <param name="message">
        /// Message
        /// </param>
        /// <param name="publicKey">
        /// Public key
        /// </param>
        /// <returns>
        /// True if signature is valid, false if it's not
        /// </returns>
        public static bool Verify(ArraySegment<byte> signature, ArraySegment<byte> message,
            ArraySegment<byte> publicKey)
        {
            return Ed25519Operations.CryptoSignVerify(signature.Array, signature.Offset, message.Array,
                message.Offset, message.Count, publicKey.Array, publicKey.Offset);
        }

        /// <summary>
        /// Verify Ed25519 signature
        /// </summary>
        /// <param name="signature">
        /// Signature bytes
        /// </param>
        /// <param name="message">
        /// Message
        /// </param>
        /// <param name="publicKey">
        /// Public key
        /// </param>
        /// <returns>
        /// True if signature is valid, false if it's not
        /// </returns>
        public static bool Verify(byte[] signature, byte[] message, byte[] publicKey)
        {
            return Ed25519Operations.CryptoSignVerify(signature, 0, message, 0, message.Length, publicKey, 0);
        }
    }
}