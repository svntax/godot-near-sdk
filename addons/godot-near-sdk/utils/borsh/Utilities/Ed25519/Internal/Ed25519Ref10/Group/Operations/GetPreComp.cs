using NearClientUnity.Utilities.Ed25519.Internal.Ed25519Ref10.Field.Operations;

namespace NearClientUnity.Utilities.Ed25519.Internal.Ed25519Ref10.Group.Operations
{
    internal static partial class GroupOperations
    {
        private static void GetPreComp(out GroupElementPreComp h)
        {
            FieldOperations.FieldOperations_1(out h.yplusx);
            FieldOperations.FieldOperations_1(out h.yminusx);
            FieldOperations.FieldOperations_0(out h.xy2d);
        }
    }
}