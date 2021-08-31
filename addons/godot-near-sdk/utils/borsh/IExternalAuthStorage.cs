namespace NearClientUnity
{
    public interface IExternalAuthStorage
    {
        bool HasKey(string key);
        void Add(string key, string value);
        string GetValue(string key);
        void DeleteKey(string key);
    }
}