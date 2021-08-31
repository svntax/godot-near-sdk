using NearClientUnity.Utilities.Ed25519.Internal.Ed25519Ref10.Group;
using NearClientUnity.Utilities.Ed25519.Internal.Ed25519Ref10.Group.Operations;
using NearClientUnity.Utilities.Ed25519.Internal.Ed25519Ref10.ScalarOps;
using System;

namespace NearClientUnity.Utilities.Ed25519.Internal.Ed25519Ref10.Ed25519Ops
{
    internal static partial class Ed25519Operations
    {
        public static bool CryptoSignVerify(
            byte[] sig, int sigoffset,
            byte[] m, int moffset, int mlen,
            byte[] pk, int pkoffset)
        {
            var checker = new byte[32];
            GroupElementP3 A;
            GroupElementP2 R;

            if ((sig[sigoffset + 63] & 224) != 0) return false;
            if (GroupOperations.FromBytes(out A, pk, pkoffset) != 0)
                return false;

            var hasher = new Sha512();
            hasher.Update(sig, sigoffset, 32);
            hasher.Update(pk, pkoffset, 32);
            hasher.Update(m, moffset, mlen);
            var h = hasher.Finalize();

            ScalarOperations.Reduce(h);

            var sm32 = new byte[32];
            Array.Copy(sig, sigoffset + 32, sm32, 0, 32);
            GroupOperations.DoubleScalarMult(out R, h, ref A, sm32);
            GroupOperations.ToBytes(checker, 0, ref R);
            var result = CryptoBytes.ConstantTimeEquals(checker, 0, sig, sigoffset, 32);
            CryptoBytes.Wipe(h);
            CryptoBytes.Wipe(checker);
            return result;
        }
    }
}