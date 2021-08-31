using NearClientUnity.Utilities.Ed25519.Internal.Ed25519Ref10.Group.Operations;
using NearClientUnity.Utilities.Ed25519.Internal.Ed25519Ref10.ScalarOps;
using System;

namespace NearClientUnity.Utilities.Ed25519.Internal.Ed25519Ref10.Ed25519Ops
{
    internal static partial class Ed25519Operations
    {
        public static void CryptoSignKeyPair(byte[] pk, int pkoffset, byte[] sk, int skoffset, byte[] seed,
            int seedoffset)
        {
            int i;

            Array.Copy(seed, seedoffset, sk, skoffset, 32);
            var h = Sha512.Hash(sk, skoffset, 32);
            ScalarOperations.Clamp(h, 0);

            GroupOperations.ScalarMultBase(out var A, h, 0);
            GroupOperations.P3ToBytes(pk, pkoffset, ref A);

            for (i = 0; i < 32; ++i) sk[skoffset + 32 + i] = pk[pkoffset + i];
            CryptoBytes.Wipe(h);
        }
    }
}