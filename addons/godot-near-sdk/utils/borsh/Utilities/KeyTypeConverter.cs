using System;

namespace NearClientUnity.Utilities
{
    public static class KeyTypeConverter
    {
        public static string KeyTypeToString(KeyType keyType)
        {
            switch (keyType)
            {
                case KeyType.Ed25519:
                    {
                        return "ed25519";
                    }
                default:
                    throw new NotSupportedException($"Unknown key type {keyType}");
            }
        }

        public static KeyType StringToKeyType(string keyType)
        {
            switch (keyType.ToLower())
            {
                case "ed25519":
                    {
                        return KeyType.Ed25519;
                    }
                default:
                    throw new NotSupportedException($"Unknown key type {keyType}");
            }
        }
    }
}