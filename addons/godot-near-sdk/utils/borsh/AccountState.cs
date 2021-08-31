namespace NearClientUnity
{
    public class AccountState
    {
        public string AccountId { get; set; }
        public string Amount { get; set; }
        public string CodeHash { get; set; }
        public string Locked { get; set; }
        public string Staked { get; set; }
        public uint StoragePaidAt { get; set; }
        public uint StorageUsage { get; set; }
    }
}