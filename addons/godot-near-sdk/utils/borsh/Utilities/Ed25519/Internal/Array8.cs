namespace NearClientUnity.Utilities.Ed25519.Internal
{
    // Array8<UInt32> Poly1305 key Array8<UInt64> SHA-512 state/output
    internal struct Array8<T>
    {
        public T X0;
        public T X1;
        public T X2;
        public T X3;
        public T X4;
        public T X5;
        public T X6;
        public T X7;
    }
}