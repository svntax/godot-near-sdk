namespace NearClientUnity.Utilities
{
    public interface INetwork
    {
        string ChainId { get; set; }
        string Name { get; set; }

        dynamic DefaultProvider(dynamic providers);
    }
}