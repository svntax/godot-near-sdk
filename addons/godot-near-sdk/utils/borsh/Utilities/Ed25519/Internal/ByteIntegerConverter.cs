namespace NearClientUnity.Utilities.Ed25519.Internal
{
    // Loops? Arrays? Never heard of that stuff Library avoids unnecessary heap allocations and
    // unsafe code so this ugly code becomes necessary :(
    internal static class ByteIntegerConverter
    {
        public static void Array16LoadBigEndian64(out Array16<ulong> output, byte[] input, int inputOffset)
        {
            output.X0 = LoadBigEndian64(input, inputOffset + 0);
            output.X1 = LoadBigEndian64(input, inputOffset + 8);
            output.X2 = LoadBigEndian64(input, inputOffset + 16);
            output.X3 = LoadBigEndian64(input, inputOffset + 24);
            output.X4 = LoadBigEndian64(input, inputOffset + 32);
            output.X5 = LoadBigEndian64(input, inputOffset + 40);
            output.X6 = LoadBigEndian64(input, inputOffset + 48);
            output.X7 = LoadBigEndian64(input, inputOffset + 56);
            output.X8 = LoadBigEndian64(input, inputOffset + 64);
            output.X9 = LoadBigEndian64(input, inputOffset + 72);
            output.X10 = LoadBigEndian64(input, inputOffset + 80);
            output.X11 = LoadBigEndian64(input, inputOffset + 88);
            output.X12 = LoadBigEndian64(input, inputOffset + 96);
            output.X13 = LoadBigEndian64(input, inputOffset + 104);
            output.X14 = LoadBigEndian64(input, inputOffset + 112);
            output.X15 = LoadBigEndian64(input, inputOffset + 120);
        }

        public static void StoreBigEndian64(byte[] buf, int offset, ulong value)
        {
            buf[offset + 7] = unchecked((byte)value);
            buf[offset + 6] = unchecked((byte)(value >> 8));
            buf[offset + 5] = unchecked((byte)(value >> 16));
            buf[offset + 4] = unchecked((byte)(value >> 24));
            buf[offset + 3] = unchecked((byte)(value >> 32));
            buf[offset + 2] = unchecked((byte)(value >> 40));
            buf[offset + 1] = unchecked((byte)(value >> 48));
            buf[offset + 0] = unchecked((byte)(value >> 56));
        }

        private static ulong LoadBigEndian64(byte[] buf, int offset)
        {
            return
                buf[offset + 7]
                | ((ulong)buf[offset + 6] << 8)
                | ((ulong)buf[offset + 5] << 16)
                | ((ulong)buf[offset + 4] << 24)
                | ((ulong)buf[offset + 3] << 32)
                | ((ulong)buf[offset + 2] << 40)
                | ((ulong)buf[offset + 1] << 48)
                | ((ulong)buf[offset + 0] << 56);
        }
    }
}