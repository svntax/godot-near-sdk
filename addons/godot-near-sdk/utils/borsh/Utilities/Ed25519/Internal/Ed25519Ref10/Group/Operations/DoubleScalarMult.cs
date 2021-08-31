using NearClientUnity.Utilities.Ed25519.Internal.Ed25519Ref10.Lookup;

namespace NearClientUnity.Utilities.Ed25519.Internal.Ed25519Ref10.Group.Operations
{
    internal static partial class GroupOperations
    {
        public static void DoubleScalarMult(out GroupElementP2 r, byte[] a, ref GroupElementP3 A, byte[] b)
        {
            var Bi = LookupTables.Base2;
            // todo: Perhaps remove these allocations?
            var aslide = new sbyte[256];
            var bslide = new sbyte[256];
            var Ai = new GroupElementCached[8]; /* A,3A,5A,7A,9A,11A,13A,15A */
            GroupElementP1P1 t;
            GroupElementP3 u;
            GroupElementP3 A2;
            int i;

            Slide(aslide, a);
            Slide(bslide, b);

            P3ToCached(out Ai[0], ref A);
            GetP3Dbl(out t, ref A);
            P1P1ConvertToP3(out A2, ref t);
            Add(out t, ref A2, ref Ai[0]);
            P1P1ConvertToP3(out u, ref t);
            P3ToCached(out Ai[1], ref u);
            Add(out t, ref A2, ref Ai[1]);
            P1P1ConvertToP3(out u, ref t);
            P3ToCached(out Ai[2], ref u);
            Add(out t, ref A2, ref Ai[2]);
            P1P1ConvertToP3(out u, ref t);
            P3ToCached(out Ai[3], ref u);
            Add(out t, ref A2, ref Ai[3]);
            P1P1ConvertToP3(out u, ref t);
            P3ToCached(out Ai[4], ref u);
            Add(out t, ref A2, ref Ai[4]);
            P1P1ConvertToP3(out u, ref t);
            P3ToCached(out Ai[5], ref u);
            Add(out t, ref A2, ref Ai[5]);
            P1P1ConvertToP3(out u, ref t);
            P3ToCached(out Ai[6], ref u);
            Add(out t, ref A2, ref Ai[6]);
            P1P1ConvertToP3(out u, ref t);
            P3ToCached(out Ai[7], ref u);

            GetP2(out r);

            for (i = 255; i >= 0; --i)
                if (aslide[i] != 0 || bslide[i] != 0)
                    break;

            for (; i >= 0; --i)
            {
                GetP2Dbl(out t, ref r);

                if (aslide[i] > 0)
                {
                    P1P1ConvertToP3(out u, ref t);
                    Add(out t, ref u, ref Ai[aslide[i] / 2]);
                }
                else if (aslide[i] < 0)
                {
                    P1P1ConvertToP3(out u, ref t);
                    Subtract(out t, ref u, ref Ai[-aslide[i] / 2]);
                }

                if (bslide[i] > 0)
                {
                    P1P1ConvertToP3(out u, ref t);
                    Madd(out t, ref u, ref Bi[bslide[i] / 2]);
                }
                else if (bslide[i] < 0)
                {
                    P1P1ConvertToP3(out u, ref t);
                    Msub(out t, ref u, ref Bi[-bslide[i] / 2]);
                }

                P1P1ConvertToP2(out r, ref t);
            }
        }

        private static void Slide(sbyte[] r, byte[] a)
        {
            for (var i = 0; i < 256; ++i)
                r[i] = (sbyte)(1 & (a[i >> 3] >> (i & 7)));

            for (var i = 0; i < 256; ++i)
                if (r[i] != 0)
                    for (var b = 1; b <= 6 && i + b < 256; ++b)
                        if (r[i + b] != 0)
                        {
                            if (r[i] + (r[i + b] << b) <= 15)
                            {
                                r[i] += (sbyte)(r[i + b] << b);
                                r[i + b] = 0;
                            }
                            else if (r[i] - (r[i + b] << b) >= -15)
                            {
                                r[i] -= (sbyte)(r[i + b] << b);
                                for (var k = i + b; k < 256; ++k)
                                {
                                    if (r[k] == 0)
                                    {
                                        r[k] = 1;
                                        break;
                                    }

                                    r[k] = 0;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
        }

        /*
		r = a * A + b * B
		where a = a[0]+256*a[1]+...+256^31 a[31].
		and b = b[0]+256*b[1]+...+256^31 b[31].
		B is the Ed25519 base point (x,4/5) with x positive.
		*/
    }
}