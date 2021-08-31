namespace NearClientUnity.Utilities.Ed25519.Internal.Ed25519Ref10.Field.Operations
{
    internal static partial class FieldOperations
    {
        /*
		h = -f

		Preconditions:
		   |f| bounded by 1.1*2^25,1.1*2^24,1.1*2^25,1.1*2^24,etc.

		Postconditions:
		   |h| bounded by 1.1*2^25,1.1*2^24,1.1*2^25,1.1*2^24,etc.
		*/

        internal static void Negative(out FieldElement h, ref FieldElement f)
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
            var h0 = -f0;
            var h1 = -f1;
            var h2 = -f2;
            var h3 = -f3;
            var h4 = -f4;
            var h5 = -f5;
            var h6 = -f6;
            var h7 = -f7;
            var h8 = -f8;
            var h9 = -f9;

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