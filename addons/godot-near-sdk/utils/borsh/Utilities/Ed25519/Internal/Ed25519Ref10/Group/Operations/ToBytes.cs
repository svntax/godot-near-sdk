using NearClientUnity.Utilities.Ed25519.Internal.Ed25519Ref10.Field.Operations;

namespace NearClientUnity.Utilities.Ed25519.Internal.Ed25519Ref10.Group.Operations
{
    internal static partial class GroupOperations
    {
        public static void ToBytes(byte[] s, int offset, ref GroupElementP2 h)
        {
            FieldOperations.Invert(out var recip, ref h.Z);
            FieldOperations.Multiplication(out var x, ref h.X, ref recip);
            FieldOperations.Multiplication(out var y, ref h.Y, ref recip);
            FieldOperations.ToBytes(s, offset, ref y);
            s[offset + 31] ^= (byte)(FieldOperations.IsNegative(ref x) << 7);
        }
    }
}