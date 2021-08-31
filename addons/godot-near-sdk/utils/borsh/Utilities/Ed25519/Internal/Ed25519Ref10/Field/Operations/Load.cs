namespace NearClientUnity.Utilities.Ed25519.Internal.Ed25519Ref10.Field.Operations
{
    internal static partial class FieldOperations
    {
        private static long Load_3(byte[] data, int offset)
        {
            uint result = data[offset + 0];
            result |= (uint)data[offset + 1] << 8;
            result |= (uint)data[offset + 2] << 16;
            return result;
        }

        private static long Load_4(byte[] data, int offset)
        {
            uint result = data[offset + 0];
            result |= (uint)data[offset + 1] << 8;
            result |= (uint)data[offset + 2] << 16;
            result |= (uint)data[offset + 3] << 24;
            return result;
        }
    }
}