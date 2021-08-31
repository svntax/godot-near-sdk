namespace NearClientUnity.Utilities.Ed25519.Internal.Ed25519Ref10.Group.Operations
{
    internal static partial class GroupOperations
    {
        /*
		r = 2 * p
		*/

        private static void GetP3Dbl(out GroupElementP1P1 r, ref GroupElementP3 p)
        {
            GroupElementP2 q;
            P3ConvertToP2(out q, ref p);
            GetP2Dbl(out r, ref q);
        }
    }
}