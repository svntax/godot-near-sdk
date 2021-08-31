namespace NearClientUnity.Providers
{
    public class SyncInfo
    {
        public string LatestBlockHash { get; set; }
        public int LatestBlockHeight { get; set; }
        public string LatestBlockTime { get; set; }
        public string LatestStateRoot { get; set; }
        public bool Syncing { get; set; }
    }
}