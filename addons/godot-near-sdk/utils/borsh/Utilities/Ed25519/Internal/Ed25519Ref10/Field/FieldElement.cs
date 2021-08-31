namespace NearClientUnity.Utilities.Ed25519.Internal.Ed25519Ref10.Field
{
    internal struct FieldElement
    {
        internal int X0, X1, X2, X3, X4, X5, X6, X7, X8, X9;

        internal FieldElement(params int[] elements)
        {
            X0 = elements[0];
            X1 = elements[1];
            X2 = elements[2];
            X3 = elements[3];
            X4 = elements[4];
            X5 = elements[5];
            X6 = elements[6];
            X7 = elements[7];
            X8 = elements[8];
            X9 = elements[9];
        }
    }
}