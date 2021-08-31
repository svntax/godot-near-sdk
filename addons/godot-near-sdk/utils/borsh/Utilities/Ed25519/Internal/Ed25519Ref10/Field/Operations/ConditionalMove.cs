namespace NearClientUnity.Utilities.Ed25519.Internal.Ed25519Ref10.Field.Operations
{
    internal static partial class FieldOperations
    {
        /*
		Replace (f,g) with (g,g) if b == 1;
		replace (f,g) with (f,g) if b == 0.

		Preconditions: b in {0,1}.
		*/

        //void ConditionalMove(fe f,const fe g,unsigned int b)
        internal static void ConditionalMove(ref FieldElement f, ref FieldElement g, int b)
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
            var x0 = f0 ^ g0;
            var x1 = f1 ^ g1;
            var x2 = f2 ^ g2;
            var x3 = f3 ^ g3;
            var x4 = f4 ^ g4;
            var x5 = f5 ^ g5;
            var x6 = f6 ^ g6;
            var x7 = f7 ^ g7;
            var x8 = f8 ^ g8;
            var x9 = f9 ^ g9;

            b = -b;
            x0 &= b;
            x1 &= b;
            x2 &= b;
            x3 &= b;
            x4 &= b;
            x5 &= b;
            x6 &= b;
            x7 &= b;
            x8 &= b;
            x9 &= b;
            f.X0 = f0 ^ x0;
            f.X1 = f1 ^ x1;
            f.X2 = f2 ^ x2;
            f.X3 = f3 ^ x3;
            f.X4 = f4 ^ x4;
            f.X5 = f5 ^ x5;
            f.X6 = f6 ^ x6;
            f.X7 = f7 ^ x7;
            f.X8 = f8 ^ x8;
            f.X9 = f9 ^ x9;
        }
    }
}