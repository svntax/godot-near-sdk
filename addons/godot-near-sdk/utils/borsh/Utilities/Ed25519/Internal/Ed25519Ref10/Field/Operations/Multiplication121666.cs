namespace NearClientUnity.Utilities.Ed25519.Internal.Ed25519Ref10.Field.Operations
{
    internal static partial class FieldOperations
    {
        /*
		h = f * 121666
		Can overlap h with f.

		Preconditions:
		   |f| bounded by 1.1*2^26,1.1*2^25,1.1*2^26,1.1*2^25,etc.

		Postconditions:
		   |h| bounded by 1.1*2^25,1.1*2^24,1.1*2^25,1.1*2^24,etc.
		*/

        public static void Multiplication121666(out FieldElement h, ref FieldElement f)
        {
            var f0 = f.X0;
            var f1 = f.X1;
            var f2 = f.X2;
            var f3 = f.X3;
            var f4 = f.X4;
            var f5 = f.X5;
            var f6 = f.X6;
            var f7 = f.X7;
            var f8 = f.X8;
            var f9 = f.X9;

            var h0 = f0 * 121666L;
            var h1 = f1 * 121666L;
            var h2 = f2 * 121666L;
            var h3 = f3 * 121666L;
            var h4 = f4 * 121666L;
            var h5 = f5 * 121666L;
            var h6 = f6 * 121666L;
            var h7 = f7 * 121666L;
            var h8 = f8 * 121666L;
            var h9 = f9 * 121666L;

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