namespace NearClientUnity.Utilities.Ed25519.Internal.Ed25519Ref10.Group.Operations
{
    internal static partial class GroupOperations
    {
        /*
		r = p
		*/

        private static void P3ConvertToP2(out GroupElementP2 r, ref GroupElementP3 p)
        {
            r.X = p.X;
            r.Y = p.Y;
            r.Z = p.Z;
        }
    }
}