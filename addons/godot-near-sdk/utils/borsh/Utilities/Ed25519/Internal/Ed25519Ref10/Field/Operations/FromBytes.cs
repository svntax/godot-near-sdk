namespace NearClientUnity.Utilities.Ed25519.Internal.Ed25519Ref10.Field.Operations
{
    internal static partial class FieldOperations
    {
        // Ignores top bit of h.
        internal static void FromBytes(out FieldElement h, byte[] data, int offset)
        {
            var h0 = Load_4(data, offset);
            var h1 = Load_3(data, offset + 4) << 6;
            var h2 = Load_3(data, offset + 7) << 5;
            var h3 = Load_3(data, offset + 10) << 3;
            var h4 = Load_3(data, offset + 13) << 2;
            var h5 = Load_4(data, offset + 16);
            var h6 = Load_3(data, offset + 20) << 7;
            var h7 = Load_3(data, offset + 23) << 5;
            var h8 = Load_3(data, offset + 26) << 4;
            var h9 = (Load_3(data, offset + 29) & 8388607) << 2;

            var carry9 = (h9 + (1 << 24)) >> 25;
            h0 += carry9 * 19;
            h9 -= carry9 << 25;
            var carry1 = (h1 + (1 << 24)) >> 25;
            h2 += carry1;
            h1 -= carry1 << 25;
            var carry3 = (h3 + (1 << 24)) >> 25;
            h4 += carry3;
            h3 -= carry3 << 25;
            var carry5 = (h5 + (1 << 24)) >> 25;
            h6 += carry5;
            h5 -= carry5 << 25;
            var carry7 = (h7 + (1 << 24)) >> 25;
            h8 += carry7;
            h7 -= carry7 << 25;

            var carry0 = (h0 + (1 << 25)) >> 26;
            h1 += carry0;
            h0 -= carry0 << 26;
            var carry2 = (h2 + (1 << 25)) >> 26;
            h3 += carry2;
            h2 -= carry2 << 26;
            var carry4 = (h4 + (1 << 25)) >> 26;
            h5 += carry4;
            h4 -= carry4 << 26;
            var carry6 = (h6 + (1 << 25)) >> 26;
            h7 += carry6;
            h6 -= carry6 << 26;
            var carry8 = (h8 + (1 << 25)) >> 26;
            h9 += carry8;
            h8 -= carry8 << 26;

            h.X0 = (int)h0;
            h.X1 = (int)h1;
            h.X2 = (int)h2;
            h.X3 = (int)h3;
            h.X4 = (int)h4;
            h.X5 = (int)h5;
            h.X6 = (int)h6;
            h.X7 = (int)h7;
            h.X8 = (int)h8;
            h.X9 = (int)h9;
        }
    }
}