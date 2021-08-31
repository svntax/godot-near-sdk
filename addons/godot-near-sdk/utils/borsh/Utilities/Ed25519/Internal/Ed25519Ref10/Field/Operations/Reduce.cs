namespace NearClientUnity.Utilities.Ed25519.Internal.Ed25519Ref10.Field.Operations
{
    internal static partial class FieldOperations
    {
        private static void Reduce(out FieldElement hr, ref FieldElement h)
        {
            var h0 = h.X0;
            var h1 = h.X1;
            var h2 = h.X2;
            var h3 = h.X3;
            var h4 = h.X4;
            var h5 = h.X5;
            var h6 = h.X6;
            var h7 = h.X7;
            var h8 = h.X8;
            var h9 = h.X9;

            var q = (19 * h9 + (1 << 24)) >> 25;
            q = (h0 + q) >> 26;
            q = (h1 + q) >> 25;
            q = (h2 + q) >> 26;
            q = (h3 + q) >> 25;
            q = (h4 + q) >> 26;
            q = (h5 + q) >> 25;
            q = (h6 + q) >> 26;
            q = (h7 + q) >> 25;
            q = (h8 + q) >> 26;
            q = (h9 + q) >> 25;

            /* Goal: Output h-(2^255-19)q, which is between 0 and 2^255-20. */
            h0 += 19 * q;
            /* Goal: Output h-2^255 q, which is between 0 and 2^255-20. */

            var carry0 = h0 >> 26;
            h1 += carry0;
            h0 -= carry0 << 26;
            var carry1 = h1 >> 25;
            h2 += carry1;
            h1 -= carry1 << 25;
            var carry2 = h2 >> 26;
            h3 += carry2;
            h2 -= carry2 << 26;
            var carry3 = h3 >> 25;
            h4 += carry3;
            h3 -= carry3 << 25;
            var carry4 = h4 >> 26;
            h5 += carry4;
            h4 -= carry4 << 26;
            var carry5 = h5 >> 25;
            h6 += carry5;
            h5 -= carry5 << 25;
            var carry6 = h6 >> 26;
            h7 += carry6;
            h6 -= carry6 << 26;
            var carry7 = h7 >> 25;
            h8 += carry7;
            h7 -= carry7 << 25;
            var carry8 = h8 >> 26;
            h9 += carry8;
            h8 -= carry8 << 26;
            var carry9 = h9 >> 25;
            h9 -= carry9 << 25;
            /* h10 = carry9 */

            hr.X0 = h0;
            hr.X1 = h1;
            hr.X2 = h2;
            hr.X3 = h3;
            hr.X4 = h4;
            hr.X5 = h5;
            hr.X6 = h6;
            hr.X7 = h7;
            hr.X8 = h8;
            hr.X9 = h9;
        }
    }
}