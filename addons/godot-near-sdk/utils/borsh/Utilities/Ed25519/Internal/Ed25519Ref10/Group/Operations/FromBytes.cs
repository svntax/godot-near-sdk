using NearClientUnity.Utilities.Ed25519.Internal.Ed25519Ref10.Field.Operations;
using NearClientUnity.Utilities.Ed25519.Internal.Ed25519Ref10.Lookup;

namespace NearClientUnity.Utilities.Ed25519.Internal.Ed25519Ref10.Group.Operations
{
    internal static partial class GroupOperations
    {
        public static int FromBytes(out GroupElementP3 h, byte[] data, int offset)
        {
            FieldOperations.FromBytes(out h.Y, data, offset);
            FieldOperations.FieldOperations_1(out h.Z);
            FieldOperations.Square(out var u, ref h.Y);
            FieldOperations.Multiplication(out var v, ref u, ref LookupTables.D);
            FieldOperations.Subtract(out u, ref u, ref h.Z); /* u = y^2-1 */
            FieldOperations.Add(out v, ref v, ref h.Z); /* v = dy^2+1 */

            FieldOperations.Square(out var v3, ref v);
            FieldOperations.Multiplication(out v3, ref v3, ref v); /* v3 = v^3 */
            FieldOperations.Square(out h.X, ref v3);
            FieldOperations.Multiplication(out h.X, ref h.X, ref v);
            FieldOperations.Multiplication(out h.X, ref h.X, ref u); /* x = uv^7 */

            FieldOperations.Pow22523(out h.X, ref h.X); /* x = (uv^7)^((q-5)/8) */
            FieldOperations.Multiplication(out h.X, ref h.X, ref v3);
            FieldOperations.Multiplication(out h.X, ref h.X, ref u); /* x = uv^3(uv^7)^((q-5)/8) */

            FieldOperations.Square(out var vxx, ref h.X);
            FieldOperations.Multiplication(out vxx, ref vxx, ref v);
            FieldOperations.Subtract(out var check, ref vxx, ref u); /* vx^2-u */
            if (FieldOperations.IsNonZero(ref check) != 0)
            {
                FieldOperations.Add(out check, ref vxx, ref u); /* vx^2+u */
                if (FieldOperations.IsNonZero(ref check) != 0)
                {
                    h = default;
                    return -1;
                }

                FieldOperations.Multiplication(out h.X, ref h.X, ref LookupTables.Sqrtm1);
            }

            if (FieldOperations.IsNegative(ref h.X) == data[offset + 31] >> 7)
                FieldOperations.Negative(out h.X, ref h.X);

            FieldOperations.Multiplication(out h.T, ref h.X, ref h.Y);
            return 0;
        }
    }
}