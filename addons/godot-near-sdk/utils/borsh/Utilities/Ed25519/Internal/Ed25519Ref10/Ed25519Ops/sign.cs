using NearClientUnity.Utilities.Ed25519.Internal.Ed25519Ref10.Group;
using NearClientUnity.Utilities.Ed25519.Internal.Ed25519Ref10.Group.Operations;
using NearClientUnity.Utilities.Ed25519.Internal.Ed25519Ref10.ScalarOps;
using System;

namespace NearClientUnity.Utilities.Ed25519.Internal.Ed25519Ref10.Ed25519Ops
{
    internal static partial class Ed25519Operations
    {
        public static void CryptoSign(
            byte[] sig, int sigoffset,
            byte[] m, int moffset, int mlen,
            byte[] sk, int skoffset)
        {
            var hasher = new Sha512();
            {
                hasher.Update(sk, skoffset, 32);
                var az = hasher.Finalize();
                ScalarOperations.Clamp(az, 0);

                hasher.Init();
                hasher.Update(az, 32, 32);
                hasher.Update(m, moffset, mlen);
                var r = hasher.Finalize();

                ScalarOperations.Reduce(r);
                GroupElementP3 R;
                GroupOperations.ScalarMultBase(out R, r, 0);
                GroupOperations.P3ToBytes(sig, sigoffset, ref R);

                hasher.Init();
                hasher.Update(sig, sigoffset, 32);
                hasher.Update(sk, skoffset + 32, 32);
                hasher.Update(m, moffset, mlen);
                var hram = hasher.Finalize();

                ScalarOperations.Reduce(hram);
                var s = new byte[32];
                Array.Copy(sig, sigoffset + 32, s, 0, 32);
                ScalarOperations.MulAdd(s, hram, az, r);
                Array.Copy(s, 0, sig, sigoffset + 32, 32);
                CryptoBytes.Wipe(s);
            }
        }
    }
}