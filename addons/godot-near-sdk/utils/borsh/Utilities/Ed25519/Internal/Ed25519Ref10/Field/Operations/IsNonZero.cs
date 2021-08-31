namespace NearClientUnity.Utilities.Ed25519.Internal.Ed25519Ref10.Field.Operations
{
    internal static partial class FieldOperations
    {
        /*
        return 1 if f == 0
        return 0 if f != 0

        Preconditions:
           |f| bounded by 1.1*2^26,1.1*2^25,1.1*2^26,1.1*2^25,etc.
        */

        // Todo: Discuss this with upstream Above comment is from the original code. But I believe
        // the original code returned 0 if f == 0
        // -1 if f != 0 This code actually returns 0 if f==0 and 1 if f != 0
        internal static int IsNonZero(ref FieldElement f)
        {
            Reduce(out var fieldElement, ref f);
            var differentBits = 0;
            differentBits |= fieldElement.X0;
            differentBits |= fieldElement.X1;
            differentBits |= fieldElement.X2;
            differentBits |= fieldElement.X3;
            differentBits |= fieldElement.X4;
            differentBits |= fieldElement.X5;
            differentBits |= fieldElement.X6;
            differentBits |= fieldElement.X7;
            differentBits |= fieldElement.X8;
            differentBits |= fieldElement.X9;
            return (int)((unchecked((uint)differentBits - 1) >> 31) ^ 1);
        }
    }
}