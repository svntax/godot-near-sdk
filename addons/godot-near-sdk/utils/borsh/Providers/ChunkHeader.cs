namespace NearClientUnity.Providers
{
    public abstract class ChunkHeader
    {
        public abstract string BalanceBurnt { get; set; }
        public abstract string ChunkHash { get; set; }
        public abstract int EncodedLength { get; set; }
        public abstract string EncodedMerkleRoot { get; set; }
        public abstract int GasLimit { get; set; }
        public abstract int GasUsed { get; set; }
        public abstract int HeightCreated { get; set; }
        public abstract int HeightIncluded { get; set; }
        public abstract string OutgoingReceiptsRoot { get; set; }
        public abstract string PrevBlockHash { get; set; }
        public abstract int PrevStateNumParts { get; set; }
        public abstract string PrevStateRootHash { get; set; }
        public abstract string RentPaid { get; set; }
        public abstract int ShardId { get; set; }
        public abstract string Signature { get; set; }
        public abstract string TxRoot { get; set; }
        public abstract dynamic[] ValidatorProposals { get; set; }
        public abstract string ValidatorReward { get; set; }
    }
}