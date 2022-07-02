// Type: Polynomial.Polynomial 
// Assembly: Polynomial, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D0DF8EE7-67C0-4E14-A3F6-9D6A748687B8
// Location: /home/lanlp/Projects/Maths/Polynomial/bin/Release/net6.0/Polynomial.dll
// Sequence point data from /home/lanlp/Projects/Maths/Polynomial/bin/Release/net6.0/Polynomial.pdb

.class public auto ansi beforefieldinit
  Polynomial.Polynomial
    extends [System.Runtime]System.Object
{

  .method public hidebysig static int32
    Calc2(
      float64 a,
      float64 b,
      float64 c,
      [out] valuetype [System.Runtime]System.Nullable`1<float64>& result1,
      [out] valuetype [System.Runtime]System.Nullable`1<float64>& result2
    ) cil managed
  {
    .maxstack 4
    .locals init (
      [0] float64 delta
    )

    // [7 9 - 7 20]
    IL_0000: ldarg.0      // a
    IL_0001: ldc.r8       0.0
    IL_000a: bne.un.s     IL_0017

    // [8 13 - 8 58]
    IL_000c: ldarg.1      // b
    IL_000d: ldarg.2      // c
    IL_000e: ldarg.3      // result1
    IL_000f: ldarg.s      result2
    IL_0011: call         int32 Polynomial.Polynomial::Calc2(float64, float64, valuetype [System.Runtime]System.Nullable`1<float64>&, valuetype [System.Runtime]System.Nullable`1<float64>&)
    IL_0016: ret

    // [10 9 - 10 39]
    IL_0017: ldarg.1      // b
    IL_0018: ldarg.1      // b
    IL_0019: mul
    IL_001a: ldc.r8       4
    IL_0023: ldarg.0      // a
    IL_0024: mul
    IL_0025: ldarg.2      // c
    IL_0026: mul
    IL_0027: sub
    IL_0028: stloc.0      // delta

    IL_0029: ldloc.0      // delta
    IL_002a: ldc.r8       0.0
    IL_0033: blt.s        IL_0043
    IL_0035: ldloc.0      // delta
    IL_0036: ldc.r8       0.0
    IL_003f: beq.s        IL_0054
    IL_0041: br.s         IL_0077

    // [14 17 - 14 32]
    IL_0043: ldarg.3      // result1
    IL_0044: initobj      valuetype [System.Runtime]System.Nullable`1<float64>

    // [15 17 - 15 32]
    IL_004a: ldarg.s      result2
    IL_004c: initobj      valuetype [System.Runtime]System.Nullable`1<float64>

    // [16 17 - 16 26]
    IL_0052: ldc.i4.0
    IL_0053: ret

    // [18 17 - 18 40]
    IL_0054: ldarg.3      // result1
    IL_0055: ldarg.1      // b
    IL_0056: neg
    IL_0057: ldc.r8       2
    IL_0060: ldarg.0      // a
    IL_0061: mul
    IL_0062: div
    IL_0063: newobj       instance void valuetype [System.Runtime]System.Nullable`1<float64>::.ctor(!0/*float64*/)
    IL_0068: stobj        valuetype [System.Runtime]System.Nullable`1<float64>

    // [19 17 - 19 32]
    IL_006d: ldarg.s      result2
    IL_006f: initobj      valuetype [System.Runtime]System.Nullable`1<float64>

    // [20 17 - 20 26]
    IL_0075: ldc.i4.1
    IL_0076: ret

    // [22 17 - 22 42]
    IL_0077: ldloc.0      // delta
    IL_0078: call         float64 [System.Runtime]System.Math::Sqrt(float64)
    IL_007d: stloc.0      // delta

    // [23 17 - 23 50]
    IL_007e: ldarg.3      // result1
    IL_007f: ldarg.1      // b
    IL_0080: neg
    IL_0081: ldloc.0      // delta
    IL_0082: add
    IL_0083: ldc.r8       2
    IL_008c: ldarg.0      // a
    IL_008d: mul
    IL_008e: div
    IL_008f: newobj       instance void valuetype [System.Runtime]System.Nullable`1<float64>::.ctor(!0/*float64*/)
    IL_0094: stobj        valuetype [System.Runtime]System.Nullable`1<float64>

    // [24 17 - 24 50]
    IL_0099: ldarg.s      result2
    IL_009b: ldarg.1      // b
    IL_009c: neg
    IL_009d: ldloc.0      // delta
    IL_009e: sub
    IL_009f: ldc.r8       2
    IL_00a8: ldarg.0      // a
    IL_00a9: mul
    IL_00aa: div
    IL_00ab: newobj       instance void valuetype [System.Runtime]System.Nullable`1<float64>::.ctor(!0/*float64*/)
    IL_00b0: stobj        valuetype [System.Runtime]System.Nullable`1<float64>

    // [25 17 - 25 26]
    IL_00b5: ldc.i4.2
    IL_00b6: ret

  } // end of method Polynomial::Calc2

  .method private hidebysig static int32
    Calc2(
      float64 b,
      float64 c,
      [out] valuetype [System.Runtime]System.Nullable`1<float64>& result1,
      [out] valuetype [System.Runtime]System.Nullable`1<float64>& result2
    ) cil managed
  {
    .maxstack 3

    // [31 9 - 31 20]
    IL_0000: ldarg.0      // b
    IL_0001: ldc.r8       0.0
    IL_000a: bne.un.s     IL_002a

    // [33 13 - 33 28]
    IL_000c: ldarg.2      // result1
    IL_000d: initobj      valuetype [System.Runtime]System.Nullable`1<float64>

    // [34 13 - 34 28]
    IL_0013: ldarg.3      // result2
    IL_0014: initobj      valuetype [System.Runtime]System.Nullable`1<float64>

    // [36 13 - 36 24]
    IL_001a: ldarg.1      // c
    IL_001b: ldc.r8       0.0
    IL_0024: bne.un.s     IL_0028

    // [37 17 - 37 27]
    IL_0026: ldc.i4.m1
    IL_0027: ret

    // [39 13 - 39 22]
    IL_0028: ldc.i4.0
    IL_0029: ret

    // [42 9 - 42 26]
    IL_002a: ldarg.2      // result1
    IL_002b: ldarg.1      // c
    IL_002c: neg
    IL_002d: ldarg.0      // b
    IL_002e: div
    IL_002f: newobj       instance void valuetype [System.Runtime]System.Nullable`1<float64>::.ctor(!0/*float64*/)
    IL_0034: stobj        valuetype [System.Runtime]System.Nullable`1<float64>

    // [43 9 - 43 24]
    IL_0039: ldarg.3      // result2
    IL_003a: initobj      valuetype [System.Runtime]System.Nullable`1<float64>

    // [44 9 - 44 18]
    IL_0040: ldc.i4.1
    IL_0041: ret

  } // end of method Polynomial::Calc2

  .method public hidebysig specialname rtspecialname instance void
    .ctor() cil managed
  {
    .maxstack 8

    IL_0000: ldarg.0      // this
    IL_0001: call         instance void [System.Runtime]System.Object::.ctor()
    IL_0006: ret

  } // end of method Polynomial::.ctor
} // end of class Polynomial.Polynomial
