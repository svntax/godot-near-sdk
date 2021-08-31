using NearClientUnity.Utilities.Ed25519.Internal.Ed25519Ref10.Field.Operations;
using NearClientUnity.Utilities.Ed25519.Internal.Ed25519Ref10.Lookup;

namespace NearClientUnity.Utilities.Ed25519.Internal.Ed25519Ref10.Group.Operations
{
    internal static partial class GroupOperations
    {
        public static void ScalarMultBase(out GroupElementP3 h, byte[] a, int offset)
        {
            var e = new sbyte[64];

            GroupElementP1P1 r;
            GroupElementP2 s;
            GroupElementPreComp t;

            for (var i = 0; i < 32; ++i)
            {
                e[2 * i + 0] = (sbyte)(a[offset + i] & 15);
                e[2 * i + 1] = (sbyte)((a[offset + i] >> 4) & 15);
            }
            /* each e[i] is between 0 and 15 */
            /* e[63] is between 0 and 7 */

            sbyte carry = 0;
            for (var i = 0; i < 63; ++i)
            {
                e[i] += carry;
                carry = (sbyte)(e[i] + 8);
                carry >>= 4;
                e[i] -= (sbyte)(carry << 4);
            }

            e[63] += carry;
            /* each e[i] is between -8 and 8 */

            GetP3(out h);
            for (var i = 1; i < 64; i += 2)
            {
                Select(out t, i / 2, e[i]);
                Madd(out r, ref h, ref t);
                P1P1ConvertToP3(out h, ref r);
            }

            GetP3Dbl(out r, ref h);
            P1P1ConvertToP2(out s, ref r);
            GetP2Dbl(out r, ref s);
            P1P1ConvertToP2(out s, ref r);
            GetP2Dbl(out r, ref s);
            P1P1ConvertToP2(out s, ref r);
            GetP2Dbl(out r, ref s);
            P1P1ConvertToP3(out h, ref r);

            for (var i = 0; i < 64; i += 2)
            {
                Select(out t, i / 2, e[i]);
                Madd(out r, ref h, ref t);
                P1P1ConvertToP3(out h, ref r);
            }
        }

        private static void Cmov(ref GroupElementPreComp t, ref GroupElementPreComp u, byte b)
        {
            FieldOperations.ConditionalMove(ref t.yplusx, ref u.yplusx, b);
            FieldOperations.ConditionalMove(ref t.yminusx, ref u.yminusx, b);
            FieldOperations.ConditionalMove(ref t.xy2d, ref u.xy2d, b);
        }

        private static byte Equal(byte b, byte c)
        {
            var ub = b;
            var uc = c;
            var x = (byte)(ub ^ uc); /* 0: yes; 1..255: no */
            uint y = x; /* 0: yes; 1..255: no */
            unchecked
            {
                y -= 1;
            } /* 4294967295: yes; 0..254: no */

            y >>= 31; /* 1: yes; 0: no */
            return (byte)y;
        }

        private static byte Negative(sbyte b)
        {
            var x = unchecked((ulong)b); /* 18446744073709551361..18446744073709551615: yes; 0..255: no */
            x >>= 63; /* 1: yes; 0: no */
            return (byte)x;
        }

        private static void Select(out GroupElementPreComp t, int pos, sbyte b)
        {
            GroupElementPreComp minust;
            var bnegative = Negative(b);
            var babs = (byte)(b - ((-bnegative & b) << 1));

            GetPreComp(out t);
            var table = LookupTables.Base[pos];
            Cmov(ref t, ref table[0], Equal(babs, 1));
            Cmov(ref t, ref table[1], Equal(babs, 2));
            Cmov(ref t, ref table[2], Equal(babs, 3));
            Cmov(ref t, ref table[3], Equal(babs, 4));
            Cmov(ref t, ref table[4], Equal(babs, 5));
            Cmov(ref t, ref table[5], Equal(babs, 6));
            Cmov(ref t, ref table[6], Equal(babs, 7));
            Cmov(ref t, ref table[7], Equal(babs, 8));
            minust.yplusx = t.yminusx;
            minust.yminusx = t.yplusx;
            FieldOperations.Negative(out minust.xy2d, ref t.xy2d);
            Cmov(ref t, ref minust, bnegative);
        }

        /*
        h = a * B
        where a = a[0]+256*a[1]+...+256^31 a[31]
        B is the Ed25519 base point (x,4/5) with x positive.

        Preconditions:
          a[31] <= 127
        */
    }
}