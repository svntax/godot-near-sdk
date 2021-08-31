namespace NearClientUnity.Utilities.Ed25519.Internal.Ed25519Ref10.Field.Operations
{
    internal static partial class FieldOperations
    {
        /*
		h = f - g
		Can overlap h with f or g.

		Preconditions:
		   |f| bounded by 1.1*2^25,1.1*2^24,1.1*2^25,1.1*2^24,etc.
		   |g| bounded by 1.1*2^25,1.1*2^24,1.1*2^25,1.1*2^24,etc.

		Postconditions:
		   |h| bounded by 1.1*2^26,1.1*2^25,1.1*2^26,1.1*2^25,etc.
		*/

        internal static void Subtract(out FieldElement h, ref FieldElement f, ref FieldElement g)
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

            var g0 = g.X0;
            var g1 = g.X1;
            var g2 = g.X2;
            var g3 = g.X3;
            var g4 = g.X4;
            var g5 = g.X5;
            var g6 = g.X6;
            var g7 = g.X7;
            var g8 = g.X8;
            var g9 = g.X9;

            var h0 = f0 - g0;
            var h1 = f1 - g1;
            var h2 = f2 - g2;
            var h3 = f3 - g3;
            var h4 = f4 - g4;
            var h5 = f5 - g5;
            var h6 = f6 - g6;
            var h7 = f7 - g7;
            var h8 = f8 - g8;
            var h9 = f9 - g9;

            h.X0 = h0;
            h.X1 = h1;
            h.X2 = h2;
            h.X3 = h3;
            h.X4 = h4;
            h.X5 = h5;
            h.X6 = h6;
            h.X7 = h7;
            h.X8 = h8;
            h.X9 = h9;
        }
    }
}