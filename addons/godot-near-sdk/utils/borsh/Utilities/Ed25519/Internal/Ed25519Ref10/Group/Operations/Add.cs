using NearClientUnity.Utilities.Ed25519.Internal.Ed25519Ref10.Field.Operations;

namespace NearClientUnity.Utilities.Ed25519.Internal.Ed25519Ref10.Group.Operations
{
    internal static partial class GroupOperations
    {
        /*
		r = p + q
		*/

        private static void Add(out GroupElementP1P1 r, ref GroupElementP3 p, ref GroupElementCached q)
        {
            /* qhasm: YpX1 = Y1+X1 */
            /* asm 1: Add(>YpX1=fe#1,<Y1=fe#12,<X1=fe#11); */
            /* asm 2: Add(>YpX1=r.X,<Y1=p.Y,<X1=p.X); */
            FieldOperations.Add(out r.X, ref p.Y, ref p.X);

            /* qhasm: YmX1 = Y1-X1 */
            /* asm 1: Subtract(>YmX1=fe#2,<Y1=fe#12,<X1=fe#11); */
            /* asm 2: Subtract(>YmX1=r.Y,<Y1=p.Y,<X1=p.X); */
            FieldOperations.Subtract(out r.Y, ref p.Y, ref p.X);

            /* qhasm: A = YpX1*YpX2 */
            /* asm 1: Multiplication(>A=fe#3,<YpX1=fe#1,<YpX2=fe#15); */
            /* asm 2: Multiplication(>A=r.Z,<YpX1=r.X,<YpX2=q.YplusX); */
            FieldOperations.Multiplication(out r.Z, ref r.X, ref q.YplusX);

            /* qhasm: B = YmX1*YmX2 */
            /* asm 1: Multiplication(>B=fe#2,<YmX1=fe#2,<YmX2=fe#16); */
            /* asm 2: Multiplication(>B=r.Y,<YmX1=r.Y,<YmX2=q.YminusX); */
            FieldOperations.Multiplication(out r.Y, ref r.Y, ref q.YminusX);

            /* qhasm: C = T2d2*T1 */
            /* asm 1: Multiplication(>C=fe#4,<T2d2=fe#18,<T1=fe#14); */
            /* asm 2: Multiplication(>C=r.T,<T2d2=q.T2d,<T1=p.T); */
            FieldOperations.Multiplication(out r.T, ref q.T2d, ref p.T);

            /* qhasm: ZZ = Z1*Z2 */
            /* asm 1: Multiplication(>ZZ=fe#1,<Z1=fe#13,<Z2=fe#17); */
            /* asm 2: Multiplication(>ZZ=r.X,<Z1=p.Z,<Z2=q.Z); */
            FieldOperations.Multiplication(out r.X, ref p.Z, ref q.Z);

            /* qhasm: D = 2*ZZ */
            /* asm 1: Add(>D=fe#5,<ZZ=fe#1,<ZZ=fe#1); */
            /* asm 2: Add(>D=t0,<ZZ=r.X,<ZZ=r.X); */
            FieldOperations.Add(out var t0, ref r.X, ref r.X);

            /* qhasm: X3 = A-B */
            /* asm 1: Subtract(>X3=fe#1,<A=fe#3,<B=fe#2); */
            /* asm 2: Subtract(>X3=r.X,<A=r.Z,<B=r.Y); */
            FieldOperations.Subtract(out r.X, ref r.Z, ref r.Y);

            /* qhasm: Y3 = A+B */
            /* asm 1: Add(>Y3=fe#2,<A=fe#3,<B=fe#2); */
            /* asm 2: Add(>Y3=r.Y,<A=r.Z,<B=r.Y); */
            FieldOperations.Add(out r.Y, ref r.Z, ref r.Y);

            /* qhasm: Z3 = D+C */
            /* asm 1: Add(>Z3=fe#3,<D=fe#5,<C=fe#4); */
            /* asm 2: Add(>Z3=r.Z,<D=t0,<C=r.T); */
            FieldOperations.Add(out r.Z, ref t0, ref r.T);

            /* qhasm: T3 = D-C */
            /* asm 1: Subtract(>T3=fe#4,<D=fe#5,<C=fe#4); */
            /* asm 2: Subtract(>T3=r.T,<D=t0,<C=r.T); */
            FieldOperations.Subtract(out r.T, ref t0, ref r.T);

            /* qhasm: return */
        }
    }
}