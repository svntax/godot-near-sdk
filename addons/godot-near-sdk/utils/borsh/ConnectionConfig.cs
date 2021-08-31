namespace NearClientUnity
{
    public class ConnectionConfig
    {
        public string NetworkId { get; set; }
        public ProviderConfig Provider { get; set; }
        public SignerConfig Signer { get; set; }
    }
}