namespace NearClientUnity.Providers
{
    public abstract class Transaction
    {
        public abstract dynamic Body { get; set; }
        public abstract string Hash { get; set; }
        public abstract string PublicKey { get; set; }
        public abstract string Signature { get; set; }
    }
}