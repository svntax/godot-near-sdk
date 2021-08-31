using NearClientUnity.Utilities;
using System;
using System.IO;

namespace NearClientUnity
{
    public class AccessKeyPermission
    {
        public FullAccessPermission FullAccess { get; set; }
        public FunctionCallPermission FunctionCall { get; set; }
        public AccessKeyPermissionType PermissionType { get; set; }

        public static AccessKeyPermission FromByteArray(byte[] rawBytes)
        {
            using (var ms = new MemoryStream(rawBytes))
            {
                return FromStream(ms);
            }
        }

        public static AccessKeyPermission FromStream(MemoryStream stream)
        {
            return FromRawDataStream(stream);
        }

        public static AccessKeyPermission FromStream(ref MemoryStream stream)
        {
            return FromRawDataStream(stream);
        }

        public byte[] ToByteArray()
        {
            using (var ms = new MemoryStream())
            {
                using (var writer = new NearBinaryWriter(ms))
                {
                    switch (PermissionType)
                    {
                        case AccessKeyPermissionType.FullAccessPermission:
                            {
                                writer.Write((byte)AccessKeyPermissionType.FullAccessPermission);
                                writer.Write(FullAccess.ToByteArray());
                                return ms.ToArray();
                            }
                        case AccessKeyPermissionType.FunctionCallPermission:
                            {
                                writer.Write((byte)AccessKeyPermissionType.FunctionCallPermission);
                                writer.Write(FunctionCall.ToByteArray());
                                return ms.ToArray();
                            }
                        default:
                            throw new NotSupportedException("Unsupported access key permission type");
                    }
                }
            }
        }

        private static AccessKeyPermission FromRawDataStream(MemoryStream stream)
        {
            using (var reader = new NearBinaryReader(stream, true))
            {
                var permissionType = (AccessKeyPermissionType)reader.ReadByte();

                switch (permissionType)
                {
                    case AccessKeyPermissionType.FullAccessPermission:
                        {
                            var fullAccess = FullAccessPermission.FromStream(ref stream);

                            return new AccessKeyPermission()
                            {
                                PermissionType = permissionType,
                                FullAccess = fullAccess,
                            };
                        }
                    case AccessKeyPermissionType.FunctionCallPermission:
                        {
                            var functionCall = FunctionCallPermission.FromStream(ref stream);

                            return new AccessKeyPermission()
                            {
                                PermissionType = permissionType,
                                FunctionCall = functionCall,
                            };
                        }
                    default:
                        throw new NotSupportedException("Unsupported access key permission type");
                }
            }
        }
    }
}