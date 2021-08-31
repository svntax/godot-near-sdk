namespace NearClientUnity.Utilities.Ed25519.Internal
{
    // Array16<UInt32> Salsa20 state Array16<UInt64> SHA-512 block
    internal struct Array16<T>
    {
        public T X0;
        public T X1;
        public T X10;
        public T X11;
        public T X12;
        public T X13;
        public T X14;
        public T X15;
        public T X2;
        public T X3;
        public T X4;
        public T X5;
        public T X6;
        public T X7;
        public T X8;
        public T X9;
    }
}