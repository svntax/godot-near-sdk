using System;
using System.Linq;
using System.Numerics;

namespace NearClientUnity.Utilities
{
    public static class Base58
    {
        private const string Digits = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";

        public static byte[] Decode(string data)
        {
            BigInteger intData = 0;

            for (var i = 0; i < data.Length; i++)
            {
                var digit = Digits.IndexOf(data[i]);

                if (digit < 0)
                {
                    throw new FormatException($"Invalid Base58 character `{data[i]}` at position {i}");
                }

                intData = intData * 58 + digit;
            }

            var leadingZeroCount = data.TakeWhile(c => c == '1').Count();
            var leadingZeros = Enumerable.Repeat((byte)0, leadingZeroCount);
            var bytesWithoutLeadingZeros =
                intData.ToByteArray()
                    .Reverse()
                    .SkipWhile(b => b == 0);

            var result = leadingZeros.Concat(bytesWithoutLeadingZeros).ToArray();

            return result;
        }

        public static string Encode(byte[] data)
        {
            var intData = data.Aggregate<byte, BigInteger>(0, (current, t) => current * 256 + t);
            var result = string.Empty;

            while (intData > 0)
            {
                var remainder = (int)(intData % 58);
                intData /= 58;
                result = Digits[remainder] + result;
            }

            for (var i = 0; i < data.Length && data[i] == 0; i++)
            {
                result = '1' + result;
            }

            return result;
        }
    }
}