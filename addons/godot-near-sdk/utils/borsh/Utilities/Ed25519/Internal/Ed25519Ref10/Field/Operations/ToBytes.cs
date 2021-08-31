namespace NearClientUnity.Utilities.Ed25519.Internal.Ed25519Ref10.Field.Operations
{
    internal static partial class FieldOperations
    {
        /*
        Preconditions:
          |h| bounded by 1.1*2^26,1.1*2^25,1.1*2^26,1.1*2^25,etc.

        Write p=2^255-19; q=floor(h/p).
        Basic claim: q = floor(2^(-255)(h + 19 2^(-25)h9 + 2^(-1))).

        Proof:
          Have |h|<=p so |q|<=1 so |19^2 2^(-255) q|<1/4.
          Also have |h-2^230 h9|<2^231 so |19 2^(-255)(h-2^230 h9)|<1/4.

          Write y=2^(-1)-19^2 2^(-255)q-19 2^(-255)(h-2^230 h9).
          Then 0<y<1.

          Write r=h-pq.
          Have 0<=r<=p-1=2^255-20.
          Thus 0<=r+19(2^-255)r<r+19(2^-255)2^255<=2^255-1.

          Write x=r+19(2^-255)r+y.
          Then 0<x<2^255 so floor(2^(-255)x) = 0 so floor(q+2^(-255)x) = q.

          Have q+2^(-255)x = 2^(-255)(h + 19 2^(-25) h9 + 2^(-1))
          so floor(2^(-255)(h + 19 2^(-25) h9 + 2^(-1))) = q.
        */

        internal static void ToBytes(byte[] s, int offset, ref FieldElement h)
        {
            FieldElement hr;
            Reduce(out hr, ref h);

            var h0 = hr.X0;
            var h1 = hr.X1;
            var h2 = hr.X2;
            var h3 = hr.X3;
            var h4 = hr.X4;
            var h5 = hr.X5;
            var h6 = hr.X6;
            var h7 = hr.X7;
            var h8 = hr.X8;
            var h9 = hr.X9;

            /*
            Goal: Output h0+...+2^255 h10-2^255 q, which is between 0 and 2^255-20.
            Have h0+...+2^230 h9 between 0 and 2^255-1;
            evidently 2^255 h10-2^255 q = 0.
            Goal: Output h0+...+2^230 h9.
            */
            unchecked
            {
                s[offset + 0] = (byte)(h0);
                s[offset + 1] = (byte)(h0 >> 8);
                s[offset + 2] = (byte)(h0 >> 16);
                s[offset + 3] = (byte)((h0 >> 24) | (h1 << 2));
                s[offset + 4] = (byte)(h1 >> 6);
                s[offset + 5] = (byte)(h1 >> 14);
                s[offset + 6] = (byte)((h1 >> 22) | (h2 << 3));
                s[offset + 7] = (byte)(h2 >> 5);
                s[offset + 8] = (byte)(h2 >> 13);
                s[offset + 9] = (byte)((h2 >> 21) | (h3 << 5));
                s[offset + 10] = (byte)(h3 >> 3);
                s[offset + 11] = (byte)(h3 >> 11);
                s[offset + 12] = (byte)((h3 >> 19) | (h4 << 6));
                s[offset + 13] = (byte)(h4 >> 2);
                s[offset + 14] = (byte)(h4 >> 10);
                s[offset + 15] = (byte)(h4 >> 18);
                s[offset + 16] = (byte)(h5);
                s[offset + 17] = (byte)(h5 >> 8);
                s[offset + 18] = (byte)(h5 >> 16);
                s[offset + 19] = (byte)((h5 >> 24) | (h6 << 1));
                s[offset + 20] = (byte)(h6 >> 7);
                s[offset + 21] = (byte)(h6 >> 15);
                s[offset + 22] = (byte)((h6 >> 23) | (h7 << 3));
                s[offset + 23] = (byte)(h7 >> 5);
                s[offset + 24] = (byte)(h7 >> 13);
                s[offset + 25] = (byte)((h7 >> 21) | (h8 << 4));
                s[offset + 26] = (byte)(h8 >> 4);
                s[offset + 27] = (byte)(h8 >> 12);
                s[offset + 28] = (byte)((h8 >> 20) | (h9 << 6));
                s[offset + 29] = (byte)(h9 >> 2);
                s[offset + 30] = (byte)(h9 >> 10);
                s[offset + 31] = (byte)(h9 >> 18);
            }
        }
    }
}