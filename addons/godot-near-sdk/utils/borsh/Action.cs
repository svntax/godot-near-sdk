using NearClient.Utilities;
using System;
using System.Collections.Generic;
using System.IO;

namespace NearClient {
    
    public class Action {

        public static byte[] FunctionCallByteArray(string methodName, byte[] methodArgs, ulong gas, UInt128 deposit){
            using (var ms = new MemoryStream()){
                using (var writer = new NearBinaryWriter(ms)){
                    writer.Write((byte)ActionType.FunctionCall);

                    writer.Write(methodName);
                    writer.Write((uint)methodArgs.Length);
                    writer.Write(methodArgs);
                    writer.Write(gas);
                    writer.Write(deposit);
                    return ms.ToArray();
                }
            }
        }

        public static byte[] TransferByteArray(UInt128 deposit){
            using (var ms = new MemoryStream()){
                using (var writer = new NearBinaryWriter(ms)){
                    writer.Write((byte)ActionType.Transfer);

                    writer.Write(deposit);
                    return ms.ToArray();
                }
            }
        }
    }
}