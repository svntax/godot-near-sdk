using NearClientUnity.Utilities.Ed25519.Internal.Ed25519Ref10.Field;
using NearClientUnity.Utilities.Ed25519.Internal.Ed25519Ref10.Field.Operations;
using NearClientUnity.Utilities.Ed25519.Internal.Ed25519Ref10.ScalarOps;

namespace NearClientUnity.Utilities.Ed25519.Internal.Ed25519Ref10.MontgomeryOps
{
    public static class MontgomeryOperations
    {
        public static void ScalarMult(
            byte[] q, int qoffset,
            byte[] n, int noffset,
            byte[] p, int poffset)
        {
            FieldElement p0, q0;
            FieldOperations.FromBytes2(out p0, p, poffset);
            ScalarMult(out q0, n, noffset, ref p0);
            FieldOperations.ToBytes(q, qoffset, ref q0);
        }

        private static void ScalarMult(
            out FieldElement q,
            byte[] n, int noffset,
            ref FieldElement p)
        {
            var e = new byte[32]; //ToDo: remove allocation
            FieldElement tmp1;

            for (var i = 0; i < 32; ++i)
                e[i] = n[noffset + i];
            ScalarOperations.Clamp(e, 0);
            var x1 = p;
            FieldOperations.FieldOperations_1(out var x2);
            FieldOperations.FieldOperations_0(out var z2);
            var x3 = x1;
            FieldOperations.FieldOperations_1(out var z3);

            uint swap = 0;
            for (var pos = 254; pos >= 0; --pos)
            {
                var b = (uint)(e[pos / 8] >> (pos & 7));
                b &= 1;
                swap ^= b;
                FieldOperations.ControlledSwap(ref x2, ref x3, swap);
                FieldOperations.ControlledSwap(ref z2, ref z3, swap);
                swap = b;

                /* qhasm: enter ladder */

                /* qhasm: D = X3-Z3 */
                /* asm 1: Subtract(>D=fe#5,<X3=fe#3,<Z3=fe#4); */
                /* asm 2: Subtract(>D=tmp0,<X3=x3,<Z3=z3); */
                FieldOperations.Subtract(out var tmp0, ref x3, ref z3);

                /* qhasm: B = X2-Z2 */
                /* asm 1: Subtract(>B=fe#6,<X2=fe#1,<Z2=fe#2); */
                /* asm 2: Subtract(>B=tmp1,<X2=x2,<Z2=z2); */
                FieldOperations.Subtract(out tmp1, ref x2, ref z2);

                /* qhasm: A = X2+Z2 */
                /* asm 1: Add(>A=fe#1,<X2=fe#1,<Z2=fe#2); */
                /* asm 2: Add(>A=x2,<X2=x2,<Z2=z2); */
                FieldOperations.Add(out x2, ref x2, ref z2);

                /* qhasm: C = X3+Z3 */
                /* asm 1: Add(>C=fe#2,<X3=fe#3,<Z3=fe#4); */
                /* asm 2: Add(>C=z2,<X3=x3,<Z3=z3); */
                FieldOperations.Add(out z2, ref x3, ref z3);

                /* qhasm: DA = D*A */
                /* asm 1: Multiplication(>DA=fe#4,<D=fe#5,<A=fe#1); */
                /* asm 2: Multiplication(>DA=z3,<D=tmp0,<A=x2); */
                FieldOperations.Multiplication(out z3, ref tmp0, ref x2);

                /* qhasm: CB = C*B */
                /* asm 1: Multiplication(>CB=fe#2,<C=fe#2,<B=fe#6); */
                /* asm 2: Multiplication(>CB=z2,<C=z2,<B=tmp1); */
                FieldOperations.Multiplication(out z2, ref z2, ref tmp1);

                /* qhasm: BB = B^2 */
                /* asm 1: Square(>BB=fe#5,<B=fe#6); */
                /* asm 2: Square(>BB=tmp0,<B=tmp1); */
                FieldOperations.Square(out tmp0, ref tmp1);

                /* qhasm: AA = A^2 */
                /* asm 1: Square(>AA=fe#6,<A=fe#1); */
                /* asm 2: Square(>AA=tmp1,<A=x2); */
                FieldOperations.Square(out tmp1, ref x2);

                /* qhasm: t0 = DA+CB */
                /* asm 1: Add(>t0=fe#3,<DA=fe#4,<CB=fe#2); */
                /* asm 2: Add(>t0=x3,<DA=z3,<CB=z2); */
                FieldOperations.Add(out x3, ref z3, ref z2);

                /* qhasm: assign x3 to t0 */

                /* qhasm: t1 = DA-CB */
                /* asm 1: Subtract(>t1=fe#2,<DA=fe#4,<CB=fe#2); */
                /* asm 2: Subtract(>t1=z2,<DA=z3,<CB=z2); */
                FieldOperations.Subtract(out z2, ref z3, ref z2);

                /* qhasm: X4 = AA*BB */
                /* asm 1: Multiplication(>X4=fe#1,<AA=fe#6,<BB=fe#5); */
                /* asm 2: Multiplication(>X4=x2,<AA=tmp1,<BB=tmp0); */
                FieldOperations.Multiplication(out x2, ref tmp1, ref tmp0);

                /* qhasm: E = AA-BB */
                /* asm 1: Subtract(>E=fe#6,<AA=fe#6,<BB=fe#5); */
                /* asm 2: Subtract(>E=tmp1,<AA=tmp1,<BB=tmp0); */
                FieldOperations.Subtract(out tmp1, ref tmp1, ref tmp0);

                /* qhasm: t2 = t1^2 */
                /* asm 1: Square(>t2=fe#2,<t1=fe#2); */
                /* asm 2: Square(>t2=z2,<t1=z2); */
                FieldOperations.Square(out z2, ref z2);

                /* qhasm: t3 = a24*E */
                /* asm 1: Multiplication121666(>t3=fe#4,<E=fe#6); */
                /* asm 2: Multiplication121666(>t3=z3,<E=tmp1); */
                FieldOperations.Multiplication121666(out z3, ref tmp1);

                /* qhasm: X5 = t0^2 */
                /* asm 1: Square(>X5=fe#3,<t0=fe#3); */
                /* asm 2: Square(>X5=x3,<t0=x3); */
                FieldOperations.Square(out x3, ref x3);

                /* qhasm: t4 = BB+t3 */
                /* asm 1: Add(>t4=fe#5,<BB=fe#5,<t3=fe#4); */
                /* asm 2: Add(>t4=tmp0,<BB=tmp0,<t3=z3); */
                FieldOperations.Add(out tmp0, ref tmp0, ref z3);

                /* qhasm: Z5 = X1*t2 */
                /* asm 1: Multiplication(>Z5=fe#4,x1,<t2=fe#2); */
                /* asm 2: Multiplication(>Z5=z3,x1,<t2=z2); */
                FieldOperations.Multiplication(out z3, ref x1, ref z2);

                /* qhasm: Z4 = E*t4 */
                /* asm 1: Multiplication(>Z4=fe#2,<E=fe#6,<t4=fe#5); */
                /* asm 2: Multiplication(>Z4=z2,<E=tmp1,<t4=tmp0); */
                FieldOperations.Multiplication(out z2, ref tmp1, ref tmp0);

                /* qhasm: return */
            }

            FieldOperations.ControlledSwap(ref x2, ref x3, swap);
            FieldOperations.ControlledSwap(ref z2, ref z3, swap);

            FieldOperations.Invert(out z2, ref z2);
            FieldOperations.Multiplication(out x2, ref x2, ref z2);
            q = x2;
            CryptoBytes.Wipe(e);
        }
    }
}