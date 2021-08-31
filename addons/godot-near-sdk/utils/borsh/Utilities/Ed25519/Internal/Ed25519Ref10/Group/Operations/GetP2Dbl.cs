using NearClientUnity.Utilities.Ed25519.Internal.Ed25519Ref10.Field;
using NearClientUnity.Utilities.Ed25519.Internal.Ed25519Ref10.Field.Operations;

namespace NearClientUnity.Utilities.Ed25519.Internal.Ed25519Ref10.Group.Operations
{
    internal static partial class GroupOperations
    {
        /*
		r = 2 * p
		*/

        private static void GetP2Dbl(out GroupElementP1P1 r, ref GroupElementP2 p)
        {
            FieldElement t0;

            /* qhasm: XX=X1^2 */
            /* asm 1: Square(>XX=fe#1,<X1=fe#11); */
            /* asm 2: Square(>XX=r.X,<X1=p.X); */
            FieldOperations.Square(out r.X, ref p.X);

            /* qhasm: YY=Y1^2 */
            /* asm 1: Square(>YY=fe#3,<Y1=fe#12); */
            /* asm 2: Square(>YY=r.Z,<Y1=p.Y); */
            FieldOperations.Square(out r.Z, ref p.Y);

            /* qhasm: B=2*Z1^2 */
            /* asm 1: Square2(>B=fe#4,<Z1=fe#13); */
            /* asm 2: Square2(>B=r.T,<Z1=p.Z); */
            FieldOperations.Square2(out r.T, ref p.Z);

            /* qhasm: A=X1+Y1 */
            /* asm 1: Add(>A=fe#2,<X1=fe#11,<Y1=fe#12); */
            /* asm 2: Add(>A=r.Y,<X1=p.X,<Y1=p.Y); */
            FieldOperations.Add(out r.Y, ref p.X, ref p.Y);

            /* qhasm: AA=A^2 */
            /* asm 1: Square(>AA=fe#5,<A=fe#2); */
            /* asm 2: Square(>AA=t0,<A=r.Y); */
            FieldOperations.Square(out t0, ref r.Y);

            /* qhasm: Y3=YY+XX */
            /* asm 1: Add(>Y3=fe#2,<YY=fe#3,<XX=fe#1); */
            /* asm 2: Add(>Y3=r.Y,<YY=r.Z,<XX=r.X); */
            FieldOperations.Add(out r.Y, ref r.Z, ref r.X);

            /* qhasm: Z3=YY-XX */
            /* asm 1: Subtract(>Z3=fe#3,<YY=fe#3,<XX=fe#1); */
            /* asm 2: Subtract(>Z3=r.Z,<YY=r.Z,<XX=r.X); */
            FieldOperations.Subtract(out r.Z, ref r.Z, ref r.X);

            /* qhasm: X3=AA-Y3 */
            /* asm 1: Subtract(>X3=fe#1,<AA=fe#5,<Y3=fe#2); */
            /* asm 2: Subtract(>X3=r.X,<AA=t0,<Y3=r.Y); */
            FieldOperations.Subtract(out r.X, ref t0, ref r.Y);

            /* qhasm: T3=B-Z3 */
            /* asm 1: Subtract(>T3=fe#4,<B=fe#4,<Z3=fe#3); */
            /* asm 2: Subtract(>T3=r.T,<B=r.T,<Z3=r.Z); */
            FieldOperations.Subtract(out r.T, ref r.T, ref r.Z);

            /* qhasm: return */
        }
    }
}