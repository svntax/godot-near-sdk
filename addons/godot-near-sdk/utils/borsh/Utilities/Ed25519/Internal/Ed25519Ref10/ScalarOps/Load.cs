namespace NearClientUnity.Utilities.Ed25519.Internal.Ed25519Ref10.ScalarOps
{
    internal static partial class ScalarOperations
    {
        private static long Load_3(byte[] input, int offset)
        {
            long result = input[offset + 0];
            result |= (long)input[offset + 1] << 8;
            result |= (long)input[offset + 2] << 16;
            return result;
        }

        private static long Load_4(byte[] input, int offset)
        {
            long result = input[offset + 0];
            result |= (long)input[offset + 1] << 8;
            result |= (long)input[offset + 2] << 16;
            result |= (long)input[offset + 3] << 24;
            return result;
        }
    }
}