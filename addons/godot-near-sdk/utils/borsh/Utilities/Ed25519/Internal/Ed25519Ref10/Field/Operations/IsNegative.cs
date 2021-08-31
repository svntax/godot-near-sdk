namespace NearClientUnity.Utilities.Ed25519.Internal.Ed25519Ref10.Field.Operations
{
    internal static partial class FieldOperations
    {
        /*
        return 1 if f is in {1,3,5,...,q-2}
        return 0 if f is in {0,2,4,...,q-1}

        Preconditions:
        |f| bounded by 1.1*2^26,1.1*2^25,1.1*2^26,1.1*2^25,etc.
        */

        //int IsNegative(const fe f)
        public static int IsNegative(ref FieldElement f)
        {
            Reduce(out var fr, ref f);
            return fr.X0 & 1;
        }
    }
}