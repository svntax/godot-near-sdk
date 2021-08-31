using NearClientUnity.Utilities.Ed25519.Internal.Ed25519Ref10.Field.Operations;

namespace NearClientUnity.Utilities.Ed25519.Internal.Ed25519Ref10.Group.Operations
{
    internal static partial class GroupOperations
    {
        /*
		r = p + q
		*/

        private static void Madd(out GroupElementP1P1 r, ref GroupElementP3 p, ref GroupElementPreComp q)
        {
            /* qhasm: YpX1 = Y1+X1 */
            /* asm 1: Add(>YpX1=fe#1,<Y1=fe#12,<X1=fe#11); */
            /* asm 2: Add(>YpX1=r.X,<Y1=p.Y,<X1=p.X); */
            FieldOperations.Add(out r.X, ref p.Y, ref p.X);

            /* qhasm: YmX1 = Y1-X1 */
            /* asm 1: Subtract(>YmX1=fe#2,<Y1=fe#12,<X1=fe#11); */
            /* asm 2: Subtract(>YmX1=r.Y,<Y1=p.Y,<X1=p.X); */
            FieldOperations.Subtract(out r.Y, ref p.Y, ref p.X);

            /* qhasm: A = YpX1*ypx2 */
            /* asm 1: Multiplication(>A=fe#3,<YpX1=fe#1,<ypx2=fe#15); */
            /* asm 2: Multiplication(>A=r.Z,<YpX1=r.X,<ypx2=q.yplusx); */
            FieldOperations.Multiplication(out r.Z, ref r.X, ref q.yplusx);

            /* qhasm: B = YmX1*ymx2 */
            /* asm 1: Multiplication(>B=fe#2,<YmX1=fe#2,<ymx2=fe#16); */
            /* asm 2: Multiplication(>B=r.Y,<YmX1=r.Y,<ymx2=q.yminusx); */
            FieldOperations.Multiplication(out r.Y, ref r.Y, ref q.yminusx);

            /* qhasm: C = xy2d2*T1 */
            /* asm 1: Multiplication(>C=fe#4,<xy2d2=fe#17,<T1=fe#14); */
            /* asm 2: Multiplication(>C=r.T,<xy2d2=q.xy2d,<T1=p.T); */
            FieldOperations.Multiplication(out r.T, ref q.xy2d, ref p.T);

            /* qhasm: D = 2*Z1 */
            /* asm 1: Add(>D=fe#5,<Z1=fe#13,<Z1=fe#13); */
            /* asm 2: Add(>D=t0,<Z1=p.Z,<Z1=p.Z); */
            FieldOperations.Add(out var t0, ref p.Z, ref p.Z);

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