namespace NearClientUnity.Providers
{
    public abstract class BlockResult
    {
        public abstract BlockHeader Header { get; set; }
        public abstract Transaction[] Transactions { get; set; }
    }
}