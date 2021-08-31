using NearClientUnity.Utilities.Ed25519.Internal.Ed25519Ref10.Field.Operations;
using NearClientUnity.Utilities.Ed25519.Internal.Ed25519Ref10.Lookup;

namespace NearClientUnity.Utilities.Ed25519.Internal.Ed25519Ref10.Group.Operations
{
    internal static partial class GroupOperations
    {
        /*
		r = p
		*/

        private static void P3ToCached(out GroupElementCached r, ref GroupElementP3 p)
        {
            FieldOperations.Add(out r.YplusX, ref p.Y, ref p.X);
            FieldOperations.Subtract(out r.YminusX, ref p.Y, ref p.X);
            r.Z = p.Z;
            FieldOperations.Multiplication(out r.T2d, ref p.T, ref LookupTables.D2);
        }
    }
}