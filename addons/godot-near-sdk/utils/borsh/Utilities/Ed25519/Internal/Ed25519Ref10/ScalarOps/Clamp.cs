namespace NearClientUnity.Utilities.Ed25519.Internal.Ed25519Ref10.ScalarOps
{
    internal static partial class ScalarOperations
    {
        public static void Clamp(byte[] s, int offset)
        {
            s[offset + 0] &= 248;
            s[offset + 31] &= 127;
            s[offset + 31] |= 64;
        }
    }
}