using NearClientUnity.Utilities.Ed25519.Internal.Ed25519Ref10.Field.Operations;

namespace NearClientUnity.Utilities.Ed25519.Internal.Ed25519Ref10.Group.Operations
{
    internal static partial class GroupOperations
    {
        /*
		r = p
		*/

        private static void P1P1ConvertToP2(out GroupElementP2 r, ref GroupElementP1P1 p)
        {
            FieldOperations.Multiplication(out r.X, ref p.X, ref p.T);
            FieldOperations.Multiplication(out r.Y, ref p.Y, ref p.Z);
            FieldOperations.Multiplication(out r.Z, ref p.Z, ref p.T);
        }
    }
}