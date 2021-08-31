namespace NearClientUnity.Providers
{
    public abstract class ChunkResult
    {
        public abstract ChunkHeader Header { get; set; }
        public abstract dynamic[] Receipts { get; set; }
        public abstract Transaction[] Transactions { get; set; }
    }
}