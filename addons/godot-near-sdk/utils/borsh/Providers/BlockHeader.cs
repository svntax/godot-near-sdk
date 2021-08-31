namespace NearClientUnity.Providers
{
    public abstract class BlockHeader
    {
        public abstract string ApprovalMask { get; set; }
        public abstract string ApprovalSigs { get; set; }
        public abstract string Hash { get; set; }
        public abstract int Height { get; set; }
        public abstract string PrevHash { get; set; }
        public abstract string PrevStateRoot { get; set; }
        public abstract int TimeStamp { get; set; }
        public abstract TotalWeight TotalWeight { get; set; }
        public abstract string TxRoot { get; set; }
    }
}