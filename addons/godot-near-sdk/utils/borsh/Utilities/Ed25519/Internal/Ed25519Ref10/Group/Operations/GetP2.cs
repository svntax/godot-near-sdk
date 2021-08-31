using NearClientUnity.Utilities.Ed25519.Internal.Ed25519Ref10.Field.Operations;

namespace NearClientUnity.Utilities.Ed25519.Internal.Ed25519Ref10.Group.Operations
{
    internal static partial class GroupOperations
    {
        private static void GetP2(out GroupElementP2 h)
        {
            FieldOperations.FieldOperations_0(out h.X);
            FieldOperations.FieldOperations_1(out h.Y);
            FieldOperations.FieldOperations_1(out h.Z);
        }
    }
}