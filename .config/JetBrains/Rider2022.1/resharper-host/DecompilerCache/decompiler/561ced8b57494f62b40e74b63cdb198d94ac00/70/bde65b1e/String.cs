// Decompiled with JetBrains decompiler
// Type: System.String
// Assembly: System.Private.CoreLib, Version=6.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e
// MVID: 561CED8B-5749-4F62-B40E-74B63CDB198D
// Assembly location: /usr/share/dotnet/shared/Microsoft.NETCore.App/6.0.2/System.Private.CoreLib.dll
// XML documentation location: /usr/share/dotnet/packs/Microsoft.NETCore.App.Ref/6.0.2/ref/net6.0/System.Runtime.xml

using Internal.Runtime.CompilerServices;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Runtime.Versioning;
using System.Text;
using System.Text.Unicode;


#nullable enable
namespace System
{
  /// <summary>Represents text as a sequence of UTF-16 code units.</summary>
  [TypeForwardedFrom("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
  [Serializable]
  public sealed class String : 
    IComparable,
    IEnumerable,
    IConvertible,
    IEnumerable<char>,
    IComparable<string?>,
    IEquatable<string?>,
    ICloneable
  {
    /// <summary>Represents the empty string. This field is read-only.</summary>
    [Intrinsic]
    public static readonly string Empty;
    [NonSerialized]
    private readonly int _stringLength;
    [NonSerialized]
    private char _firstChar;


    #nullable disable
    [MethodImpl(MethodImplOptions.InternalCall)]
    internal static extern string FastAllocateString(int length);

    [MethodImpl(MethodImplOptions.InternalCall)]
    internal extern void SetTrailByte(byte data);

    [MethodImpl(MethodImplOptions.InternalCall)]
    internal extern bool TryGetTrailByte(out byte data);

    [MethodImpl(MethodImplOptions.InternalCall)]
    private extern string Intern();

    [MethodImpl(MethodImplOptions.InternalCall)]
    private extern string IsInterned();


    #nullable enable
    /// <summary>Retrieves the system's reference to the specified <see cref="T:System.String" />.</summary>
    /// <param name="str">A string to search for in the intern pool.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="str" /> is <see langword="null" />.</exception>
    /// <returns>The system's reference to <paramref name="str" />, if it is interned; otherwise, a new reference to a string with the value of <paramref name="str" />.</returns>
    public static string Intern(string str) => str != null ? str.Intern() : throw new ArgumentNullException(nameof (str));

    /// <summary>Retrieves a reference to a specified <see cref="T:System.String" />.</summary>
    /// <param name="str">The string to search for in the intern pool.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="str" /> is <see langword="null" />.</exception>
    /// <returns>A reference to <paramref name="str" /> if it is in the common language runtime intern pool; otherwise, <see langword="null" />.</returns>
    public static string? IsInterned(string str) => str != null ? str.IsInterned() : throw new ArgumentNullException(nameof (str));


    #nullable disable
    internal static unsafe void InternalCopy(string src, IntPtr dest, int len)
    {
      if (len == 0)
        return;
      Buffer.Memmove(ref *(byte*) (void*) dest, ref Unsafe.As<char, byte>(ref src.GetRawStringData()), (UIntPtr) len);
    }

    internal unsafe int GetBytesFromEncoding(
      byte* pbNativeBuffer,
      int cbNativeBuffer,
      Encoding encoding)
    {
      fixed (char* chars = &this._firstChar)
        return encoding.GetBytes(chars, this.Length, pbNativeBuffer, cbNativeBuffer);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool EqualsHelper(string strA, string strB) => SpanHelpers.SequenceEqual(ref Unsafe.As<char, byte>(ref strA.GetRawStringData()), ref Unsafe.As<char, byte>(ref strB.GetRawStringData()), (UIntPtr) (uint) (strA.Length * 2));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int CompareOrdinalHelper(
      string strA,
      int indexA,
      int countA,
      string strB,
      int indexB,
      int countB)
    {
      return SpanHelpers.SequenceCompareTo(ref Unsafe.Add<char>(ref strA.GetRawStringData(), (IntPtr) (uint) indexA), countA, ref Unsafe.Add<char>(ref strB.GetRawStringData(), (IntPtr) (uint) indexB), countB);
    }

    internal static bool EqualsOrdinalIgnoreCase(string strA, string strB)
    {
      if ((object) strA == (object) strB)
        return true;
      return strA != null && strB != null && strA.Length == strB.Length && string.EqualsOrdinalIgnoreCaseNoLengthCheck(strA, strB);
    }

    private static bool EqualsOrdinalIgnoreCaseNoLengthCheck(string strA, string strB) => Ordinal.EqualsIgnoreCase(ref strA.GetRawStringData(), ref strB.GetRawStringData(), strB.Length);

    private static unsafe int CompareOrdinalHelper(string strA, string strB)
    {
      int num1 = Math.Min(strA.Length, strB.Length);
      fixed (char* chPtr1 = &strA._firstChar)
        fixed (char* chPtr2 = &strB._firstChar)
        {
          char* chPtr3 = chPtr1;
          char* chPtr4 = chPtr2;
          if ((int) chPtr3[1] == (int) chPtr4[1])
          {
            int num2 = num1 - 2;
            chPtr3 += 2;
            chPtr4 += 2;
            while (num2 >= 12)
            {
              if (*(long*) chPtr3 == *(long*) chPtr4)
              {
                if (*(long*) (chPtr3 + 4) == *(long*) (chPtr4 + 4))
                {
                  if (*(long*) (chPtr3 + 8) == *(long*) (chPtr4 + 8))
                  {
                    num2 -= 12;
                    chPtr3 += 12;
                    chPtr4 += 12;
                    continue;
                  }
                  chPtr3 += 4;
                  chPtr4 += 4;
                }
                chPtr3 += 4;
                chPtr4 += 4;
              }
              if (*(int*) chPtr3 == *(int*) chPtr4)
              {
                chPtr3 += 2;
                chPtr4 += 2;
                goto label_15;
              }
              else
                goto label_15;
            }
            while (num2 > 0)
            {
              if (*(int*) chPtr3 == *(int*) chPtr4)
              {
                num2 -= 2;
                chPtr3 += 2;
                chPtr4 += 2;
              }
              else
                goto label_15;
            }
            return strA.Length - strB.Length;
label_15:
            if ((int) *chPtr3 != (int) *chPtr4)
              return (int) *chPtr3 - (int) *chPtr4;
          }
          return (int) chPtr3[1] - (int) chPtr4[1];
        }
    }


    #nullable enable
    /// <summary>Compares two specified <see cref="T:System.String" /> objects and returns an integer that indicates their relative position in the sort order.</summary>
    /// <param name="strA">The first string to compare.</param>
    /// <param name="strB">The second string to compare.</param>
    /// <returns>A 32-bit signed integer that indicates the lexical relationship between the two comparands.
    /// 
    /// <list type="table"><listheader><term> Value</term><description> Condition</description></listheader><item><term> Less than zero</term><description><paramref name="strA" /> precedes <paramref name="strB" /> in the sort order.</description></item><item><term> Zero</term><description><paramref name="strA" /> occurs in the same position as <paramref name="strB" /> in the sort order.</description></item><item><term> Greater than zero</term><description><paramref name="strA" /> follows <paramref name="strB" /> in the sort order.</description></item></list></returns>
    public static int Compare(string? strA, string? strB) => string.Compare(strA, strB, StringComparison.CurrentCulture);

    /// <summary>Compares two specified <see cref="T:System.String" /> objects, ignoring or honoring their case, and returns an integer that indicates their relative position in the sort order.</summary>
    /// <param name="strA">The first string to compare.</param>
    /// <param name="strB">The second string to compare.</param>
    /// <param name="ignoreCase">
    /// <see langword="true" /> to ignore case during the comparison; otherwise, <see langword="false" />.</param>
    /// <returns>A 32-bit signed integer that indicates the lexical relationship between the two comparands.
    /// 
    /// <list type="table"><listheader><term> Value</term><description> Condition</description></listheader><item><term> Less than zero</term><description><paramref name="strA" /> precedes <paramref name="strB" /> in the sort order.</description></item><item><term> Zero</term><description><paramref name="strA" /> occurs in the same position as <paramref name="strB" /> in the sort order.</description></item><item><term> Greater than zero</term><description><paramref name="strA" /> follows <paramref name="strB" /> in the sort order.</description></item></list></returns>
    public static int Compare(string? strA, string? strB, bool ignoreCase)
    {
      StringComparison comparisonType = ignoreCase ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture;
      return string.Compare(strA, strB, comparisonType);
    }

    /// <summary>Compares two specified <see cref="T:System.String" /> objects using the specified rules, and returns an integer that indicates their relative position in the sort order.</summary>
    /// <param name="strA">The first string to compare.</param>
    /// <param name="strB">The second string to compare.</param>
    /// <param name="comparisonType">One of the enumeration values that specifies the rules to use in the comparison.</param>
    /// <exception cref="T:System.ArgumentException">
    /// <paramref name="comparisonType" /> is not a <see cref="T:System.StringComparison" /> value.</exception>
    /// <exception cref="T:System.NotSupportedException">
    /// <see cref="T:System.StringComparison" /> is not supported.</exception>
    /// <returns>A 32-bit signed integer that indicates the lexical relationship between the two comparands.
    /// 
    /// <list type="table"><listheader><term> Value</term><description> Condition</description></listheader><item><term> Less than zero</term><description><paramref name="strA" /> precedes <paramref name="strB" /> in the sort order.</description></item><item><term> Zero</term><description><paramref name="strA" /> is in the same position as <paramref name="strB" /> in the sort order.</description></item><item><term> Greater than zero</term><description><paramref name="strA" /> follows <paramref name="strB" /> in the sort order.</description></item></list></returns>
    public static int Compare(string? strA, string? strB, StringComparison comparisonType)
    {
      if ((object) strA == (object) strB)
      {
        string.CheckStringComparison(comparisonType);
        return 0;
      }
      if (strA == null)
      {
        string.CheckStringComparison(comparisonType);
        return -1;
      }
      if (strB == null)
      {
        string.CheckStringComparison(comparisonType);
        return 1;
      }
      switch (comparisonType)
      {
        case StringComparison.CurrentCulture:
        case StringComparison.CurrentCultureIgnoreCase:
          return CultureInfo.CurrentCulture.CompareInfo.Compare(strA, strB, string.GetCaseCompareOfComparisonCulture(comparisonType));
        case StringComparison.InvariantCulture:
        case StringComparison.InvariantCultureIgnoreCase:
          return CompareInfo.Invariant.Compare(strA, strB, string.GetCaseCompareOfComparisonCulture(comparisonType));
        case StringComparison.Ordinal:
          return (int) strA._firstChar != (int) strB._firstChar ? (int) strA._firstChar - (int) strB._firstChar : string.CompareOrdinalHelper(strA, strB);
        case StringComparison.OrdinalIgnoreCase:
          return Ordinal.CompareStringIgnoreCase(ref strA.GetRawStringData(), strA.Length, ref strB.GetRawStringData(), strB.Length);
        default:
          throw new ArgumentException(SR.NotSupported_StringComparison, nameof (comparisonType));
      }
    }

    /// <summary>Compares two specified <see cref="T:System.String" /> objects using the specified comparison options and culture-specific information to influence the comparison, and returns an integer that indicates the relationship of the two strings to each other in the sort order.</summary>
    /// <param name="strA">The first string to compare.</param>
    /// <param name="strB">The second string to compare.</param>
    /// <param name="culture">The culture that supplies culture-specific comparison information. If <paramref name="culture" /> is <see langword="null" />, the current culture is used.</param>
    /// <param name="options">Options to use when performing the comparison (such as ignoring case or symbols).</param>
    /// <exception cref="T:System.ArgumentException">
    /// <paramref name="options" /> is not a <see cref="T:System.Globalization.CompareOptions" /> value.</exception>
    /// <returns>A 32-bit signed integer that indicates the lexical relationship between <paramref name="strA" /> and <paramref name="strB" />, as shown in the following table
    /// 
    /// <list type="table"><listheader><term> Value</term><description> Condition</description></listheader><item><term> Less than zero</term><description><paramref name="strA" /> precedes <paramref name="strB" /> in the sort order.</description></item><item><term> Zero</term><description><paramref name="strA" /> occurs in the same position as <paramref name="strB" /> in the sort order.</description></item><item><term> Greater than zero</term><description><paramref name="strA" /> follows <paramref name="strB" /> in the sort order.</description></item></list></returns>
    public static int Compare(
      string? strA,
      string? strB,
      CultureInfo? culture,
      CompareOptions options)
    {
      return (culture ?? CultureInfo.CurrentCulture).CompareInfo.Compare(strA, strB, options);
    }

    /// <summary>Compares two specified <see cref="T:System.String" /> objects, ignoring or honoring their case, and using culture-specific information to influence the comparison, and returns an integer that indicates their relative position in the sort order.</summary>
    /// <param name="strA">The first string to compare.</param>
    /// <param name="strB">The second string to compare.</param>
    /// <param name="ignoreCase">
    /// <see langword="true" /> to ignore case during the comparison; otherwise, <see langword="false" />.</param>
    /// <param name="culture">An object that supplies culture-specific comparison information. If <paramref name="culture" /> is <see langword="null" />, the current culture is used.</param>
    /// <returns>A 32-bit signed integer that indicates the lexical relationship between the two comparands.
    /// 
    /// <list type="table"><listheader><term> Value</term><description> Condition</description></listheader><item><term> Less than zero</term><description><paramref name="strA" /> precedes <paramref name="strB" /> in the sort order.</description></item><item><term> Zero</term><description><paramref name="strA" /> occurs in the same position as <paramref name="strB" /> in the sort order.</description></item><item><term> Greater than zero</term><description><paramref name="strA" /> follows <paramref name="strB" /> in the sort order.</description></item></list></returns>
    public static int Compare(string? strA, string? strB, bool ignoreCase, CultureInfo? culture)
    {
      CompareOptions options = ignoreCase ? CompareOptions.IgnoreCase : CompareOptions.None;
      return string.Compare(strA, strB, culture, options);
    }

    /// <summary>Compares substrings of two specified <see cref="T:System.String" /> objects and returns an integer that indicates their relative position in the sort order.</summary>
    /// <param name="strA">The first string to use in the comparison.</param>
    /// <param name="indexA">The position of the substring within <paramref name="strA" />.</param>
    /// <param name="strB">The second string to use in the comparison.</param>
    /// <param name="indexB">The position of the substring within <paramref name="strB" />.</param>
    /// <param name="length">The maximum number of characters in the substrings to compare.</param>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    ///        <paramref name="indexA" /> is greater than <paramref name="strA" />.<see cref="P:System.String.Length" />.
    /// 
    /// -or-
    /// 
    /// <paramref name="indexB" /> is greater than <paramref name="strB" />.<see cref="P:System.String.Length" />.
    /// 
    /// -or-
    /// 
    /// <paramref name="indexA" />, <paramref name="indexB" />, or <paramref name="length" /> is negative.
    /// 
    /// -or-
    /// 
    /// Either <paramref name="indexA" /> or <paramref name="indexB" /> is <see langword="null" />, and <paramref name="length" /> is greater than zero.</exception>
    /// <returns>A 32-bit signed integer indicating the lexical relationship between the two comparands.
    /// 
    /// <list type="table"><listheader><term> Value</term><description> Condition</description></listheader><item><term> Less than zero</term><description> The substring in <paramref name="strA" /> precedes the substring in <paramref name="strB" /> in the sort order.</description></item><item><term> Zero</term><description> The substrings occur in the same position in the sort order, or <paramref name="length" /> is zero.</description></item><item><term> Greater than zero</term><description> The substring in <paramref name="strA" /> follows the substring in <paramref name="strB" /> in the sort order.</description></item></list></returns>
    public static int Compare(string? strA, int indexA, string? strB, int indexB, int length) => string.Compare(strA, indexA, strB, indexB, length, false);

    /// <summary>Compares substrings of two specified <see cref="T:System.String" /> objects, ignoring or honoring their case, and returns an integer that indicates their relative position in the sort order.</summary>
    /// <param name="strA">The first string to use in the comparison.</param>
    /// <param name="indexA">The position of the substring within <paramref name="strA" />.</param>
    /// <param name="strB">The second string to use in the comparison.</param>
    /// <param name="indexB">The position of the substring within <paramref name="strB" />.</param>
    /// <param name="length">The maximum number of characters in the substrings to compare.</param>
    /// <param name="ignoreCase">
    /// <see langword="true" /> to ignore case during the comparison; otherwise, <see langword="false" />.</param>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    ///        <paramref name="indexA" /> is greater than <paramref name="strA" />.<see cref="P:System.String.Length" />.
    /// 
    /// -or-
    /// 
    /// <paramref name="indexB" /> is greater than <paramref name="strB" />.<see cref="P:System.String.Length" />.
    /// 
    /// -or-
    /// 
    /// <paramref name="indexA" />, <paramref name="indexB" />, or <paramref name="length" /> is negative.
    /// 
    /// -or-
    /// 
    /// Either <paramref name="indexA" /> or <paramref name="indexB" /> is <see langword="null" />, and <paramref name="length" /> is greater than zero.</exception>
    /// <returns>A 32-bit signed integer that indicates the lexical relationship between the two comparands.
    /// 
    /// <list type="table"><listheader><term> Value</term><description> Condition</description></listheader><item><term> Less than zero</term><description> The substring in <paramref name="strA" /> precedes the substring in <paramref name="strB" /> in the sort order.</description></item><item><term> Zero</term><description> The substrings occur in the same position in the sort order, or <paramref name="length" /> is zero.</description></item><item><term> Greater than zero</term><description> The substring in <paramref name="strA" /> follows the substring in <paramref name="strB" /> in the sort order.</description></item></list></returns>
    public static int Compare(
      string? strA,
      int indexA,
      string? strB,
      int indexB,
      int length,
      bool ignoreCase)
    {
      int num1 = length;
      int num2 = length;
      if (strA != null)
        num1 = Math.Min(num1, strA.Length - indexA);
      if (strB != null)
        num2 = Math.Min(num2, strB.Length - indexB);
      CompareOptions options = ignoreCase ? CompareOptions.IgnoreCase : CompareOptions.None;
      return CultureInfo.CurrentCulture.CompareInfo.Compare(strA, indexA, num1, strB, indexB, num2, options);
    }

    /// <summary>Compares substrings of two specified <see cref="T:System.String" /> objects, ignoring or honoring their case and using culture-specific information to influence the comparison, and returns an integer that indicates their relative position in the sort order.</summary>
    /// <param name="strA">The first string to use in the comparison.</param>
    /// <param name="indexA">The position of the substring within <paramref name="strA" />.</param>
    /// <param name="strB">The second string to use in the comparison.</param>
    /// <param name="indexB">The position of the substring within <paramref name="strB" />.</param>
    /// <param name="length">The maximum number of characters in the substrings to compare.</param>
    /// <param name="ignoreCase">
    /// <see langword="true" /> to ignore case during the comparison; otherwise, <see langword="false" />.</param>
    /// <param name="culture">An object that supplies culture-specific comparison information. If <paramref name="culture" /> is <see langword="null" />, the current culture is used.</param>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    ///        <paramref name="indexA" /> is greater than <paramref name="strA" />.<see cref="P:System.String.Length" />.
    /// 
    /// -or-
    /// 
    /// <paramref name="indexB" /> is greater than <paramref name="strB" />.<see cref="P:System.String.Length" />.
    /// 
    /// -or-
    /// 
    /// <paramref name="indexA" />, <paramref name="indexB" />, or <paramref name="length" /> is negative.
    /// 
    /// -or-
    /// 
    /// Either <paramref name="strA" /> or <paramref name="strB" /> is <see langword="null" />, and <paramref name="length" /> is greater than zero.</exception>
    /// <returns>An integer that indicates the lexical relationship between the two comparands.
    /// 
    /// <list type="table"><listheader><term> Value</term><description> Condition</description></listheader><item><term> Less than zero</term><description> The substring in <paramref name="strA" /> precedes the substring in <paramref name="strB" /> in the sort order.</description></item><item><term> Zero</term><description> The substrings occur in the same position in the sort order, or <paramref name="length" /> is zero.</description></item><item><term> Greater than zero</term><description> The substring in <paramref name="strA" /> follows the substring in <paramref name="strB" /> in the sort order.</description></item></list></returns>
    public static int Compare(
      string? strA,
      int indexA,
      string? strB,
      int indexB,
      int length,
      bool ignoreCase,
      CultureInfo? culture)
    {
      CompareOptions options = ignoreCase ? CompareOptions.IgnoreCase : CompareOptions.None;
      return string.Compare(strA, indexA, strB, indexB, length, culture, options);
    }

    /// <summary>Compares substrings of two specified <see cref="T:System.String" /> objects using the specified comparison options and culture-specific information to influence the comparison, and returns an integer that indicates the relationship of the two substrings to each other in the sort order.</summary>
    /// <param name="strA">The first string to use in the comparison.</param>
    /// <param name="indexA">The starting position of the substring within <paramref name="strA" />.</param>
    /// <param name="strB">The second string to use in the comparison.</param>
    /// <param name="indexB">The starting position of the substring within <paramref name="strB" />.</param>
    /// <param name="length">The maximum number of characters in the substrings to compare.</param>
    /// <param name="culture">An object that supplies culture-specific comparison information. If <paramref name="culture" /> is <see langword="null" />, the current culture is used.</param>
    /// <param name="options">Options to use when performing the comparison (such as ignoring case or symbols).</param>
    /// <exception cref="T:System.ArgumentException">
    /// <paramref name="options" /> is not a <see cref="T:System.Globalization.CompareOptions" /> value.</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    ///        <paramref name="indexA" /> is greater than <paramref name="strA" /><see langword=".Length" />.
    /// 
    /// -or-
    /// 
    /// <paramref name="indexB" /> is greater than <paramref name="strB" /><see langword=".Length" />.
    /// 
    /// -or-
    /// 
    /// <paramref name="indexA" />, <paramref name="indexB" />, or <paramref name="length" /> is negative.
    /// 
    /// -or-
    /// 
    /// Either <paramref name="strA" /> or <paramref name="strB" /> is <see langword="null" />, and <paramref name="length" /> is greater than zero.</exception>
    /// <returns>An integer that indicates the lexical relationship between the two substrings, as shown in the following table.
    /// 
    /// <list type="table"><listheader><term> Value</term><description> Condition</description></listheader><item><term> Less than zero</term><description> The substring in <paramref name="strA" /> precedes the substring in <paramref name="strB" /> in the sort order.</description></item><item><term> Zero</term><description> The substrings occur in the same position in the sort order, or <paramref name="length" /> is zero.</description></item><item><term> Greater than zero</term><description> The substring in <paramref name="strA" /> follows the substring in <paramref name="strB" /> in the sort order.</description></item></list></returns>
    public static int Compare(
      string? strA,
      int indexA,
      string? strB,
      int indexB,
      int length,
      CultureInfo? culture,
      CompareOptions options)
    {
      CultureInfo cultureInfo = culture ?? CultureInfo.CurrentCulture;
      int num1 = length;
      int num2 = length;
      if (strA != null)
        num1 = Math.Min(num1, strA.Length - indexA);
      if (strB != null)
        num2 = Math.Min(num2, strB.Length - indexB);
      return cultureInfo.CompareInfo.Compare(strA, indexA, num1, strB, indexB, num2, options);
    }

    /// <summary>Compares substrings of two specified <see cref="T:System.String" /> objects using the specified rules, and returns an integer that indicates their relative position in the sort order.</summary>
    /// <param name="strA">The first string to use in the comparison.</param>
    /// <param name="indexA">The position of the substring within <paramref name="strA" />.</param>
    /// <param name="strB">The second string to use in the comparison.</param>
    /// <param name="indexB">The position of the substring within <paramref name="strB" />.</param>
    /// <param name="length">The maximum number of characters in the substrings to compare.</param>
    /// <param name="comparisonType">One of the enumeration values that specifies the rules to use in the comparison.</param>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    ///        <paramref name="indexA" /> is greater than <paramref name="strA" />.<see cref="P:System.String.Length" />.
    /// 
    /// -or-
    /// 
    /// <paramref name="indexB" /> is greater than <paramref name="strB" />.<see cref="P:System.String.Length" />.
    /// 
    /// -or-
    /// 
    /// <paramref name="indexA" />, <paramref name="indexB" />, or <paramref name="length" /> is negative.
    /// 
    /// -or-
    /// 
    /// Either <paramref name="indexA" /> or <paramref name="indexB" /> is <see langword="null" />, and <paramref name="length" /> is greater than zero.</exception>
    /// <exception cref="T:System.ArgumentException">
    /// <paramref name="comparisonType" /> is not a <see cref="T:System.StringComparison" /> value.</exception>
    /// <returns>A 32-bit signed integer that indicates the lexical relationship between the two comparands.
    /// 
    /// <list type="table"><listheader><term> Value</term><description> Condition</description></listheader><item><term> Less than zero</term><description> The substring in <paramref name="strA" /> precedes the substring in <paramref name="strB" /> in the sort order.</description></item><item><term> Zero</term><description> The substrings occur in the same position in the sort order, or the <paramref name="length" /> parameter is zero.</description></item><item><term> Greater than zero</term><description> The substring in <paramref name="strA" /> follows the substring in <paramref name="strB" /> in the sort order.</description></item></list></returns>
    public static int Compare(
      string? strA,
      int indexA,
      string? strB,
      int indexB,
      int length,
      StringComparison comparisonType)
    {
      string.CheckStringComparison(comparisonType);
      if (strA == null || strB == null)
      {
        if ((object) strA == (object) strB)
          return 0;
        return strA != null ? 1 : -1;
      }
      if (length < 0)
        throw new ArgumentOutOfRangeException(nameof (length), SR.ArgumentOutOfRange_NegativeLength);
      if (indexA < 0 || indexB < 0)
        throw new ArgumentOutOfRangeException(indexA < 0 ? nameof (indexA) : nameof (indexB), SR.ArgumentOutOfRange_Index);
      if (strA.Length - indexA < 0 || strB.Length - indexB < 0)
        throw new ArgumentOutOfRangeException(strA.Length - indexA < 0 ? nameof (indexA) : nameof (indexB), SR.ArgumentOutOfRange_Index);
      if (length == 0 || (object) strA == (object) strB && indexA == indexB)
        return 0;
      int num1 = Math.Min(length, strA.Length - indexA);
      int num2 = Math.Min(length, strB.Length - indexB);
      switch (comparisonType)
      {
        case StringComparison.CurrentCulture:
        case StringComparison.CurrentCultureIgnoreCase:
          return CultureInfo.CurrentCulture.CompareInfo.Compare(strA, indexA, num1, strB, indexB, num2, string.GetCaseCompareOfComparisonCulture(comparisonType));
        case StringComparison.InvariantCulture:
        case StringComparison.InvariantCultureIgnoreCase:
          return CompareInfo.Invariant.Compare(strA, indexA, num1, strB, indexB, num2, string.GetCaseCompareOfComparisonCulture(comparisonType));
        case StringComparison.Ordinal:
          return string.CompareOrdinalHelper(strA, indexA, num1, strB, indexB, num2);
        default:
          return Ordinal.CompareStringIgnoreCase(ref Unsafe.Add<char>(ref strA.GetRawStringData(), indexA), num1, ref Unsafe.Add<char>(ref strB.GetRawStringData(), indexB), num2);
      }
    }

    /// <summary>Compares two specified <see cref="T:System.String" /> objects by evaluating the numeric values of the corresponding <see cref="T:System.Char" /> objects in each string.</summary>
    /// <param name="strA">The first string to compare.</param>
    /// <param name="strB">The second string to compare.</param>
    /// <returns>An integer that indicates the lexical relationship between the two comparands.
    /// 
    /// <list type="table"><listheader><term> Value</term><description> Condition</description></listheader><item><term> Less than zero</term><description><paramref name="strA" /> is less than <paramref name="strB" />.</description></item><item><term> Zero</term><description><paramref name="strA" /> and <paramref name="strB" /> are equal.</description></item><item><term> Greater than zero</term><description><paramref name="strA" /> is greater than <paramref name="strB" />.</description></item></list></returns>
    public static int CompareOrdinal(string? strA, string? strB)
    {
      if ((object) strA == (object) strB)
        return 0;
      if (strA == null)
        return -1;
      if (strB == null)
        return 1;
      return (int) strA._firstChar != (int) strB._firstChar ? (int) strA._firstChar - (int) strB._firstChar : string.CompareOrdinalHelper(strA, strB);
    }


    #nullable disable
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int CompareOrdinal(ReadOnlySpan<char> strA, ReadOnlySpan<char> strB) => SpanHelpers.SequenceCompareTo(ref MemoryMarshal.GetReference<char>(strA), strA.Length, ref MemoryMarshal.GetReference<char>(strB), strB.Length);


    #nullable enable
    /// <summary>Compares substrings of two specified <see cref="T:System.String" /> objects by evaluating the numeric values of the corresponding <see cref="T:System.Char" /> objects in each substring.</summary>
    /// <param name="strA">The first string to use in the comparison.</param>
    /// <param name="indexA">The starting index of the substring in <paramref name="strA" />.</param>
    /// <param name="strB">The second string to use in the comparison.</param>
    /// <param name="indexB">The starting index of the substring in <paramref name="strB" />.</param>
    /// <param name="length">The maximum number of characters in the substrings to compare.</param>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    ///        <paramref name="strA" /> is not <see langword="null" /> and <paramref name="indexA" /> is greater than <paramref name="strA" />.<see cref="P:System.String.Length" />.
    /// 
    /// -or-
    /// 
    /// <paramref name="strB" /> is not <see langword="null" /> and <paramref name="indexB" /> is greater than <paramref name="strB" />.<see cref="P:System.String.Length" />.
    /// 
    /// -or-
    /// 
    /// <paramref name="indexA" />, <paramref name="indexB" />, or <paramref name="length" /> is negative.</exception>
    /// <returns>A 32-bit signed integer that indicates the lexical relationship between the two comparands.
    /// 
    /// <list type="table"><listheader><term> Value</term><description> Condition</description></listheader><item><term> Less than zero</term><description> The substring in <paramref name="strA" /> is less than the substring in <paramref name="strB" />.</description></item><item><term> Zero</term><description> The substrings are equal, or <paramref name="length" /> is zero.</description></item><item><term> Greater than zero</term><description> The substring in <paramref name="strA" /> is greater than the substring in <paramref name="strB" />.</description></item></list></returns>
    public static int CompareOrdinal(
      string? strA,
      int indexA,
      string? strB,
      int indexB,
      int length)
    {
      if (strA == null || strB == null)
      {
        if ((object) strA == (object) strB)
          return 0;
        return strA != null ? 1 : -1;
      }
      if (length < 0)
        throw new ArgumentOutOfRangeException(nameof (length), SR.ArgumentOutOfRange_NegativeCount);
      if (indexA < 0 || indexB < 0)
        throw new ArgumentOutOfRangeException(indexA < 0 ? nameof (indexA) : nameof (indexB), SR.ArgumentOutOfRange_Index);
      int countA = Math.Min(length, strA.Length - indexA);
      int countB = Math.Min(length, strB.Length - indexB);
      if (countA < 0 || countB < 0)
        throw new ArgumentOutOfRangeException(countA < 0 ? nameof (indexA) : nameof (indexB), SR.ArgumentOutOfRange_Index);
      return length == 0 || (object) strA == (object) strB && indexA == indexB ? 0 : string.CompareOrdinalHelper(strA, indexA, countA, strB, indexB, countB);
    }

    /// <summary>Compares this instance with a specified <see cref="T:System.Object" /> and indicates whether this instance precedes, follows, or appears in the same position in the sort order as the specified <see cref="T:System.Object" />.</summary>
    /// <param name="value">An object that evaluates to a <see cref="T:System.String" />.</param>
    /// <exception cref="T:System.ArgumentException">
    /// <paramref name="value" /> is not a <see cref="T:System.String" />.</exception>
    /// <returns>A 32-bit signed integer that indicates whether this instance precedes, follows, or appears in the same position in the sort order as the <paramref name="value" /> parameter.
    /// 
    /// <list type="table"><listheader><term> Value</term><description> Condition</description></listheader><item><term> Less than zero</term><description> This instance precedes <paramref name="value" />.</description></item><item><term> Zero</term><description> This instance has the same position in the sort order as <paramref name="value" />.</description></item><item><term> Greater than zero</term><description> This instance follows <paramref name="value" />.
    /// 
    /// -or-
    /// 
    /// <paramref name="value" /> is <see langword="null" />.</description></item></list></returns>
    public int CompareTo(object? value)
    {
      if (value == null)
        return 1;
      return value is string strB ? this.CompareTo(strB) : throw new ArgumentException(SR.Arg_MustBeString);
    }

    /// <summary>Compares this instance with a specified <see cref="T:System.String" /> object and indicates whether this instance precedes, follows, or appears in the same position in the sort order as the specified string.</summary>
    /// <param name="strB">The string to compare with this instance.</param>
    /// <returns>A 32-bit signed integer that indicates whether this instance precedes, follows, or appears in the same position in the sort order as the <paramref name="strB" /> parameter.
    /// 
    /// <list type="table"><listheader><term> Value</term><description> Condition</description></listheader><item><term> Less than zero</term><description> This instance precedes <paramref name="strB" />.</description></item><item><term> Zero</term><description> This instance has the same position in the sort order as <paramref name="strB" />.</description></item><item><term> Greater than zero</term><description> This instance follows <paramref name="strB" />.
    /// 
    /// -or-
    /// 
    /// <paramref name="strB" /> is <see langword="null" />.</description></item></list></returns>
    public int CompareTo(string? strB) => string.Compare(this, strB, StringComparison.CurrentCulture);

    /// <summary>Determines whether the end of this string instance matches the specified string.</summary>
    /// <param name="value">The string to compare to the substring at the end of this instance.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="value" /> is <see langword="null" />.</exception>
    /// <returns>
    /// <see langword="true" /> if <paramref name="value" /> matches the end of this instance; otherwise, <see langword="false" />.</returns>
    public bool EndsWith(string value) => this.EndsWith(value, StringComparison.CurrentCulture);

    /// <summary>Determines whether the end of this string instance matches the specified string when compared using the specified comparison option.</summary>
    /// <param name="value">The string to compare to the substring at the end of this instance.</param>
    /// <param name="comparisonType">One of the enumeration values that determines how this string and <paramref name="value" /> are compared.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="value" /> is <see langword="null" />.</exception>
    /// <exception cref="T:System.ArgumentException">
    /// <paramref name="comparisonType" /> is not a <see cref="T:System.StringComparison" /> value.</exception>
    /// <returns>
    /// <see langword="true" /> if the <paramref name="value" /> parameter matches the end of this string; otherwise, <see langword="false" />.</returns>
    public bool EndsWith(string value, StringComparison comparisonType)
    {
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      if ((object) this == (object) value)
      {
        string.CheckStringComparison(comparisonType);
        return true;
      }
      if (value.Length == 0)
      {
        string.CheckStringComparison(comparisonType);
        return true;
      }
      switch (comparisonType)
      {
        case StringComparison.CurrentCulture:
        case StringComparison.CurrentCultureIgnoreCase:
          return CultureInfo.CurrentCulture.CompareInfo.IsSuffix(this, value, string.GetCaseCompareOfComparisonCulture(comparisonType));
        case StringComparison.InvariantCulture:
        case StringComparison.InvariantCultureIgnoreCase:
          return CompareInfo.Invariant.IsSuffix(this, value, string.GetCaseCompareOfComparisonCulture(comparisonType));
        case StringComparison.Ordinal:
          int start = this.Length - value.Length;
          return (uint) start <= (uint) this.Length && this.AsSpan(start).SequenceEqual<char>((ReadOnlySpan<char>) value);
        case StringComparison.OrdinalIgnoreCase:
          return this.Length >= value.Length && Ordinal.CompareStringIgnoreCase(ref Unsafe.Add<char>(ref this.GetRawStringData(), this.Length - value.Length), value.Length, ref value.GetRawStringData(), value.Length) == 0;
        default:
          throw new ArgumentException(SR.NotSupported_StringComparison, nameof (comparisonType));
      }
    }

    /// <summary>Determines whether the end of this string instance matches the specified string when compared using the specified culture.</summary>
    /// <param name="value">The string to compare to the substring at the end of this instance.</param>
    /// <param name="ignoreCase">
    /// <see langword="true" /> to ignore case during the comparison; otherwise, <see langword="false" />.</param>
    /// <param name="culture">Cultural information that determines how this instance and <paramref name="value" /> are compared. If <paramref name="culture" /> is <see langword="null" />, the current culture is used.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="value" /> is <see langword="null" />.</exception>
    /// <returns>
    /// <see langword="true" /> if the <paramref name="value" /> parameter matches the end of this string; otherwise, <see langword="false" />.</returns>
    public bool EndsWith(string value, bool ignoreCase, CultureInfo? culture)
    {
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      return (object) this == (object) value || (culture ?? CultureInfo.CurrentCulture).CompareInfo.IsSuffix(this, value, ignoreCase ? CompareOptions.IgnoreCase : CompareOptions.None);
    }

    /// <summary>Determines whether the end of this string instance matches the specified character.</summary>
    /// <param name="value">The character to compare to the character at the end of this instance.</param>
    /// <returns>
    /// <see langword="true" /> if <paramref name="value" /> matches the end of this instance; otherwise, <see langword="false" />.</returns>
    public bool EndsWith(char value)
    {
      int index = this.Length - 1;
      return (uint) index < (uint) this.Length && (int) this[index] == (int) value;
    }

    /// <summary>Determines whether this instance and a specified object, which must also be a <see cref="T:System.String" /> object, have the same value.</summary>
    /// <param name="obj">The string to compare to this instance.</param>
    /// <returns>
    /// <see langword="true" /> if <paramref name="obj" /> is a <see cref="T:System.String" /> and its value is the same as this instance; otherwise, <see langword="false" />.  If <paramref name="obj" /> is <see langword="null" />, the method returns <see langword="false" />.</returns>
    public override bool Equals([NotNullWhen(true)] object? obj)
    {
      if ((object) this == obj)
        return true;
      return obj is string strB && this.Length == strB.Length && string.EqualsHelper(this, strB);
    }

    /// <summary>Determines whether this instance and another specified <see cref="T:System.String" /> object have the same value.</summary>
    /// <param name="value">The string to compare to this instance.</param>
    /// <returns>
    /// <see langword="true" /> if the value of the <paramref name="value" /> parameter is the same as the value of this instance; otherwise, <see langword="false" />. If <paramref name="value" /> is <see langword="null" />, the method returns <see langword="false" />.</returns>
    public bool Equals([NotNullWhen(true)] string? value)
    {
      if ((object) this == (object) value)
        return true;
      return value != null && this.Length == value.Length && string.EqualsHelper(this, value);
    }

    /// <summary>Determines whether this string and a specified <see cref="T:System.String" /> object have the same value. A parameter specifies the culture, case, and sort rules used in the comparison.</summary>
    /// <param name="value">The string to compare to this instance.</param>
    /// <param name="comparisonType">One of the enumeration values that specifies how the strings will be compared.</param>
    /// <exception cref="T:System.ArgumentException">
    /// <paramref name="comparisonType" /> is not a <see cref="T:System.StringComparison" /> value.</exception>
    /// <returns>
    /// <see langword="true" /> if the value of the <paramref name="value" /> parameter is the same as this string; otherwise, <see langword="false" />.</returns>
    public bool Equals([NotNullWhen(true)] string? value, StringComparison comparisonType)
    {
      if ((object) this == (object) value)
      {
        string.CheckStringComparison(comparisonType);
        return true;
      }
      if (value == null)
      {
        string.CheckStringComparison(comparisonType);
        return false;
      }
      switch (comparisonType)
      {
        case StringComparison.CurrentCulture:
        case StringComparison.CurrentCultureIgnoreCase:
          return CultureInfo.CurrentCulture.CompareInfo.Compare(this, value, string.GetCaseCompareOfComparisonCulture(comparisonType)) == 0;
        case StringComparison.InvariantCulture:
        case StringComparison.InvariantCultureIgnoreCase:
          return CompareInfo.Invariant.Compare(this, value, string.GetCaseCompareOfComparisonCulture(comparisonType)) == 0;
        case StringComparison.Ordinal:
          return this.Length == value.Length && string.EqualsHelper(this, value);
        case StringComparison.OrdinalIgnoreCase:
          return this.Length == value.Length && string.EqualsOrdinalIgnoreCaseNoLengthCheck(this, value);
        default:
          throw new ArgumentException(SR.NotSupported_StringComparison, nameof (comparisonType));
      }
    }

    /// <summary>Determines whether two specified <see cref="T:System.String" /> objects have the same value.</summary>
    /// <param name="a">The first string to compare, or <see langword="null" />.</param>
    /// <param name="b">The second string to compare, or <see langword="null" />.</param>
    /// <returns>
    /// <see langword="true" /> if the value of <paramref name="a" /> is the same as the value of <paramref name="b" />; otherwise, <see langword="false" />. If both <paramref name="a" /> and <paramref name="b" /> are <see langword="null" />, the method returns <see langword="true" />.</returns>
    public static bool Equals(string? a, string? b)
    {
      if ((object) a == (object) b)
        return true;
      return a != null && b != null && a.Length == b.Length && string.EqualsHelper(a, b);
    }

    /// <summary>Determines whether two specified <see cref="T:System.String" /> objects have the same value. A parameter specifies the culture, case, and sort rules used in the comparison.</summary>
    /// <param name="a">The first string to compare, or <see langword="null" />.</param>
    /// <param name="b">The second string to compare, or <see langword="null" />.</param>
    /// <param name="comparisonType">One of the enumeration values that specifies the rules for the comparison.</param>
    /// <exception cref="T:System.ArgumentException">
    /// <paramref name="comparisonType" /> is not a <see cref="T:System.StringComparison" /> value.</exception>
    /// <returns>
    /// <see langword="true" /> if the value of the <paramref name="a" /> parameter is equal to the value of the <paramref name="b" /> parameter; otherwise, <see langword="false" />.</returns>
    public static bool Equals(string? a, string? b, StringComparison comparisonType)
    {
      if ((object) a == (object) b)
      {
        string.CheckStringComparison(comparisonType);
        return true;
      }
      if (a == null || b == null)
      {
        string.CheckStringComparison(comparisonType);
        return false;
      }
      switch (comparisonType)
      {
        case StringComparison.CurrentCulture:
        case StringComparison.CurrentCultureIgnoreCase:
          return CultureInfo.CurrentCulture.CompareInfo.Compare(a, b, string.GetCaseCompareOfComparisonCulture(comparisonType)) == 0;
        case StringComparison.InvariantCulture:
        case StringComparison.InvariantCultureIgnoreCase:
          return CompareInfo.Invariant.Compare(a, b, string.GetCaseCompareOfComparisonCulture(comparisonType)) == 0;
        case StringComparison.Ordinal:
          return a.Length == b.Length && string.EqualsHelper(a, b);
        case StringComparison.OrdinalIgnoreCase:
          return a.Length == b.Length && string.EqualsOrdinalIgnoreCaseNoLengthCheck(a, b);
        default:
          throw new ArgumentException(SR.NotSupported_StringComparison, nameof (comparisonType));
      }
    }

    /// <summary>Determines whether two specified strings have the same value.</summary>
    /// <param name="a">The first string to compare, or <see langword="null" />.</param>
    /// <param name="b">The second string to compare, or <see langword="null" />.</param>
    /// <returns>
    /// <see langword="true" /> if the value of <paramref name="a" /> is the same as the value of <paramref name="b" />; otherwise, <see langword="false" />.</returns>
    public static bool operator ==(string? a, string? b) => string.Equals(a, b);

    /// <summary>Determines whether two specified strings have different values.</summary>
    /// <param name="a">The first string to compare, or <see langword="null" />.</param>
    /// <param name="b">The second string to compare, or <see langword="null" />.</param>
    /// <returns>
    /// <see langword="true" /> if the value of <paramref name="a" /> is different from the value of <paramref name="b" />; otherwise, <see langword="false" />.</returns>
    public static bool operator !=(string? a, string? b) => !string.Equals(a, b);

    /// <summary>Returns the hash code for this string.</summary>
    /// <returns>A 32-bit signed integer hash code.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode()
    {
      ulong defaultSeed = Marvin.DefaultSeed;
      return Marvin.ComputeHash32(ref Unsafe.As<char, byte>(ref this._firstChar), (uint) (this._stringLength * 2), (uint) defaultSeed, (uint) (defaultSeed >> 32));
    }

    /// <summary>Returns the hash code for this string using the specified rules.</summary>
    /// <param name="comparisonType">One of the enumeration values that specifies the rules to use in the comparison.</param>
    /// <returns>A 32-bit signed integer hash code.</returns>
    public int GetHashCode(StringComparison comparisonType) => StringComparer.FromComparison(comparisonType).GetHashCode(this);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal int GetHashCodeOrdinalIgnoreCase()
    {
      ulong defaultSeed = Marvin.DefaultSeed;
      return Marvin.ComputeHash32OrdinalIgnoreCase(ref this._firstChar, this._stringLength, (uint) defaultSeed, (uint) (defaultSeed >> 32));
    }

    /// <summary>Returns the hash code for the provided read-only character span.</summary>
    /// <param name="value">A read-only character span.</param>
    /// <returns>A 32-bit signed integer hash code.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetHashCode(ReadOnlySpan<char> value)
    {
      ulong defaultSeed = Marvin.DefaultSeed;
      return Marvin.ComputeHash32(ref Unsafe.As<char, byte>(ref MemoryMarshal.GetReference<char>(value)), (uint) (value.Length * 2), (uint) defaultSeed, (uint) (defaultSeed >> 32));
    }

    /// <summary>Returns the hash code for the provided read-only character span using the specified rules.</summary>
    /// <param name="value">A read-only character span.</param>
    /// <param name="comparisonType">One of the enumeration values that specifies the rules to use in the comparison.</param>
    /// <returns>A 32-bit signed integer hash code.</returns>
    public static int GetHashCode(ReadOnlySpan<char> value, StringComparison comparisonType)
    {
      switch (comparisonType)
      {
        case StringComparison.CurrentCulture:
        case StringComparison.CurrentCultureIgnoreCase:
          return CultureInfo.CurrentCulture.CompareInfo.GetHashCode(value, string.GetCaseCompareOfComparisonCulture(comparisonType));
        case StringComparison.InvariantCulture:
        case StringComparison.InvariantCultureIgnoreCase:
          return CompareInfo.Invariant.GetHashCode(value, string.GetCaseCompareOfComparisonCulture(comparisonType));
        case StringComparison.Ordinal:
          return string.GetHashCode(value);
        case StringComparison.OrdinalIgnoreCase:
          return string.GetHashCodeOrdinalIgnoreCase(value);
        default:
          ThrowHelper.ThrowArgumentException(ExceptionResource.NotSupported_StringComparison, ExceptionArgument.comparisonType);
          return 0;
      }
    }


    #nullable disable
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int GetHashCodeOrdinalIgnoreCase(ReadOnlySpan<char> value)
    {
      ulong defaultSeed = Marvin.DefaultSeed;
      return Marvin.ComputeHash32OrdinalIgnoreCase(ref MemoryMarshal.GetReference<char>(value), value.Length, (uint) defaultSeed, (uint) (defaultSeed >> 32));
    }

    internal unsafe int GetNonRandomizedHashCode()
    {
      fixed (char* chPtr = &this._firstChar)
      {
        uint num1 = 352654597;
        uint num2 = num1;
        uint* numPtr = (uint*) chPtr;
        int length = this.Length;
        while (length > 2)
        {
          length -= 4;
          num1 = BitOperations.RotateLeft(num1, 5) + num1 ^ *numPtr;
          num2 = BitOperations.RotateLeft(num2, 5) + num2 ^ numPtr[1];
          numPtr += 2;
        }
        if (length > 0)
          num2 = BitOperations.RotateLeft(num2, 5) + num2 ^ *numPtr;
        return (int) num1 + (int) num2 * 1566083941;
      }
    }

    internal unsafe int GetNonRandomizedHashCodeOrdinalIgnoreCase()
    {
      uint num1 = 352654597;
      uint num2 = num1;
      fixed (char* chPtr = &this._firstChar)
      {
        uint* numPtr = (uint*) chPtr;
        int length = this.Length;
        while (length > 2)
        {
          uint num3 = *numPtr;
          uint num4 = numPtr[1];
          if (Utf16Utility.AllCharsInUInt32AreAscii(num3 | num4))
          {
            length -= 4;
            num1 = (uint) ((int) BitOperations.RotateLeft(num1, 5) + (int) num1 ^ ((int) num3 | 2097184));
            num2 = (uint) ((int) BitOperations.RotateLeft(num2, 5) + (int) num2 ^ ((int) num4 | 2097184));
            numPtr += 2;
          }
          else
            goto label_10;
        }
        if (length > 0)
        {
          uint num5 = *numPtr;
          if (Utf16Utility.AllCharsInUInt32AreAscii(num5))
            num2 = (uint) ((int) BitOperations.RotateLeft(num2, 5) + (int) num2 ^ ((int) num5 | 2097184));
          else
            goto label_10;
        }
      }
      return (int) num1 + (int) num2 * 1566083941;
label_10:
      return GetNonRandomizedHashCodeOrdinalIgnoreCaseSlow(this);

      static unsafe int GetNonRandomizedHashCodeOrdinalIgnoreCaseSlow(string str)
      {
        int length = str.Length;
        char[] array = (char[]) null;
        // ISSUE: untyped stack allocation
        Span<char> destination = (uint) length >= 64U ? (Span<char>) (array = ArrayPool<char>.Shared.Rent(length + 1)) : new Span<char>((void*) __untypedstackalloc(new IntPtr(128)), 64);
        Ordinal.ToUpperOrdinal((ReadOnlySpan<char>) str, destination);
        destination[length] = char.MinValue;
        uint num1 = 352654597;
        uint num2 = num1;
        fixed (char* chPtr = &destination.GetPinnableReference())
        {
          uint* numPtr = (uint*) chPtr;
          while (length > 2)
          {
            length -= 4;
            num1 = (uint) ((int) BitOperations.RotateLeft(num1, 5) + (int) num1 ^ ((int) *numPtr | 2097184));
            num2 = (uint) ((int) BitOperations.RotateLeft(num2, 5) + (int) num2 ^ ((int) numPtr[1] | 2097184));
            numPtr += 2;
          }
          if (length > 0)
            num2 = (uint) ((int) BitOperations.RotateLeft(num2, 5) + (int) num2 ^ ((int) *numPtr | 2097184));
        }
        if (array != null)
          ArrayPool<char>.Shared.Return(array);
        return (int) num1 + (int) num2 * 1566083941;
      }
    }


    #nullable enable
    /// <summary>Determines whether the beginning of this string instance matches the specified string.</summary>
    /// <param name="value">The string to compare.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="value" /> is <see langword="null" />.</exception>
    /// <returns>
    /// <see langword="true" /> if <paramref name="value" /> matches the beginning of this string; otherwise, <see langword="false" />.</returns>
    public bool StartsWith(string value) => value != null ? this.StartsWith(value, StringComparison.CurrentCulture) : throw new ArgumentNullException(nameof (value));

    /// <summary>Determines whether the beginning of this string instance matches the specified string when compared using the specified comparison option.</summary>
    /// <param name="value">The string to compare.</param>
    /// <param name="comparisonType">One of the enumeration values that determines how this string and <paramref name="value" /> are compared.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="value" /> is <see langword="null" />.</exception>
    /// <exception cref="T:System.ArgumentException">
    /// <paramref name="comparisonType" /> is not a <see cref="T:System.StringComparison" /> value.</exception>
    /// <returns>
    /// <see langword="true" /> if this instance begins with <paramref name="value" />; otherwise, <see langword="false" />.</returns>
    public bool StartsWith(string value, StringComparison comparisonType)
    {
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      if ((object) this == (object) value)
      {
        string.CheckStringComparison(comparisonType);
        return true;
      }
      if (value.Length == 0)
      {
        string.CheckStringComparison(comparisonType);
        return true;
      }
      switch (comparisonType)
      {
        case StringComparison.CurrentCulture:
        case StringComparison.CurrentCultureIgnoreCase:
          return CultureInfo.CurrentCulture.CompareInfo.IsPrefix(this, value, string.GetCaseCompareOfComparisonCulture(comparisonType));
        case StringComparison.InvariantCulture:
        case StringComparison.InvariantCultureIgnoreCase:
          return CompareInfo.Invariant.IsPrefix(this, value, string.GetCaseCompareOfComparisonCulture(comparisonType));
        case StringComparison.Ordinal:
          if (this.Length < value.Length || (int) this._firstChar != (int) value._firstChar)
            return false;
          return value.Length == 1 || SpanHelpers.SequenceEqual(ref Unsafe.As<char, byte>(ref this.GetRawStringData()), ref Unsafe.As<char, byte>(ref value.GetRawStringData()), (UIntPtr) value.Length * new UIntPtr(2));
        case StringComparison.OrdinalIgnoreCase:
          return this.Length >= value.Length && Ordinal.EqualsIgnoreCase(ref this.GetRawStringData(), ref value.GetRawStringData(), value.Length);
        default:
          throw new ArgumentException(SR.NotSupported_StringComparison, nameof (comparisonType));
      }
    }

    /// <summary>Determines whether the beginning of this string instance matches the specified string when compared using the specified culture.</summary>
    /// <param name="value">The string to compare.</param>
    /// <param name="ignoreCase">
    /// <see langword="true" /> to ignore case during the comparison; otherwise, <see langword="false" />.</param>
    /// <param name="culture">Cultural information that determines how this string and <paramref name="value" /> are compared. If <paramref name="culture" /> is <see langword="null" />, the current culture is used.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="value" /> is <see langword="null" />.</exception>
    /// <returns>
    /// <see langword="true" /> if the <paramref name="value" /> parameter matches the beginning of this string; otherwise, <see langword="false" />.</returns>
    public bool StartsWith(string value, bool ignoreCase, CultureInfo? culture)
    {
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      return (object) this == (object) value || (culture ?? CultureInfo.CurrentCulture).CompareInfo.IsPrefix(this, value, ignoreCase ? CompareOptions.IgnoreCase : CompareOptions.None);
    }

    /// <summary>Determines whether this string instance starts with the specified character.</summary>
    /// <param name="value">The character to compare.</param>
    /// <returns>
    /// <see langword="true" /> if <paramref name="value" /> matches the beginning of this string; otherwise, <see langword="false" />.</returns>
    public bool StartsWith(char value) => this.Length != 0 && (int) this._firstChar == (int) value;

    internal static void CheckStringComparison(StringComparison comparisonType)
    {
      switch (comparisonType)
      {
        case StringComparison.CurrentCulture:
        case StringComparison.CurrentCultureIgnoreCase:
        case StringComparison.InvariantCulture:
        case StringComparison.InvariantCultureIgnoreCase:
        case StringComparison.Ordinal:
        case StringComparison.OrdinalIgnoreCase:
          break;
        default:
          ThrowHelper.ThrowArgumentException(ExceptionResource.NotSupported_StringComparison, ExceptionArgument.comparisonType);
          break;
      }
    }

    internal static CompareOptions GetCaseCompareOfComparisonCulture(
      StringComparison comparisonType)
    {
      return (CompareOptions) (comparisonType & StringComparison.CurrentCultureIgnoreCase);
    }

    private static CompareOptions GetCompareOptionsFromOrdinalStringComparison(
      StringComparison comparisonType)
    {
      int num = (int) comparisonType;
      return (CompareOptions) ((num & -num) << 28);
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.String" /> class to the Unicode characters indicated in the specified character array.</summary>
    /// <param name="value">An array of Unicode characters.</param>
    [DynamicDependency("Ctor(System.Char[])")]
    [MethodImpl(MethodImplOptions.InternalCall)]
    public extern String(char[]? value);


    #nullable disable
    private string Ctor(char[] value)
    {
      if (value == null || value.Length == 0)
        return string.Empty;
      string str = string.FastAllocateString(value.Length);
      UIntPtr length = (UIntPtr) (uint) str.Length;
      Buffer.Memmove<char>(ref str._firstChar, ref MemoryMarshal.GetArrayDataReference<char>(value), length);
      return str;
    }


    #nullable enable
    /// <summary>Initializes a new instance of the <see cref="T:System.String" /> class to the value indicated by an array of Unicode characters, a starting character position within that array, and a length.</summary>
    /// <param name="value">An array of Unicode characters.</param>
    /// <param name="startIndex">The starting position within <paramref name="value" />.</param>
    /// <param name="length">The number of characters within <paramref name="value" /> to use.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="value" /> is <see langword="null" />.</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    ///        <paramref name="startIndex" /> or <paramref name="length" /> is less than zero.
    /// 
    /// -or-
    /// 
    /// The sum of <paramref name="startIndex" /> and <paramref name="length" /> is greater than the number of elements in <paramref name="value" />.</exception>
    [DynamicDependency("Ctor(System.Char[],System.Int32,System.Int32)")]
    [MethodImpl(MethodImplOptions.InternalCall)]
    public extern String(char[] value, int startIndex, int length);


    #nullable disable
    private string Ctor(char[] value, int startIndex, int length)
    {
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      if (startIndex < 0)
        throw new ArgumentOutOfRangeException(nameof (startIndex), SR.ArgumentOutOfRange_StartIndex);
      if (length < 0)
        throw new ArgumentOutOfRangeException(nameof (length), SR.ArgumentOutOfRange_NegativeLength);
      if (startIndex > value.Length - length)
        throw new ArgumentOutOfRangeException(nameof (startIndex), SR.ArgumentOutOfRange_Index);
      if (length == 0)
        return string.Empty;
      string str = string.FastAllocateString(length);
      UIntPtr length1 = (UIntPtr) (uint) str.Length;
      Buffer.Memmove<char>(ref str._firstChar, ref Unsafe.Add<char>(ref MemoryMarshal.GetArrayDataReference<char>(value), startIndex), length1);
      return str;
    }


    #nullable enable
    /// <summary>Initializes a new instance of the <see cref="T:System.String" /> class to the value indicated by a specified pointer to an array of Unicode characters.</summary>
    /// <param name="value">A pointer to a null-terminated array of Unicode characters.</param>
    /// <exception cref="T:System.ArgumentOutOfRangeException">The current process does not have read access to all the addressed characters.</exception>
    /// <exception cref="T:System.ArgumentException">
    /// <paramref name="value" /> specifies an array that contains an invalid Unicode character, or <paramref name="value" /> specifies an address less than 64000.</exception>
    [CLSCompliant(false)]
    [DynamicDependency("Ctor(System.Char*)")]
    [MethodImpl(MethodImplOptions.InternalCall)]
    public extern unsafe String(char* value);


    #nullable disable
    private unsafe string Ctor(char* ptr)
    {
      if ((IntPtr) ptr == IntPtr.Zero)
        return string.Empty;
      int length1 = string.wcslen(ptr);
      if (length1 == 0)
        return string.Empty;
      string str = string.FastAllocateString(length1);
      UIntPtr length2 = (UIntPtr) (uint) str.Length;
      Buffer.Memmove<char>(ref str._firstChar, ref *ptr, length2);
      return str;
    }


    #nullable enable
    /// <summary>Initializes a new instance of the <see cref="T:System.String" /> class to the value indicated by a specified pointer to an array of Unicode characters, a starting character position within that array, and a length.</summary>
    /// <param name="value">A pointer to an array of Unicode characters.</param>
    /// <param name="startIndex">The starting position within <paramref name="value" />.</param>
    /// <param name="length">The number of characters within <paramref name="value" /> to use.</param>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    /// <paramref name="startIndex" /> or <paramref name="length" /> is less than zero, <paramref name="value" /> + <paramref name="startIndex" /> cause a pointer overflow, or the current process does not have read access to all the addressed characters.</exception>
    /// <exception cref="T:System.ArgumentException">
    /// <paramref name="value" /> specifies an array that contains an invalid Unicode character, or <paramref name="value" /> + <paramref name="startIndex" /> specifies an address less than 64000.</exception>
    [CLSCompliant(false)]
    [DynamicDependency("Ctor(System.Char*,System.Int32,System.Int32)")]
    [MethodImpl(MethodImplOptions.InternalCall)]
    public extern unsafe String(char* value, int startIndex, int length);


    #nullable disable
    private unsafe string Ctor(char* ptr, int startIndex, int length)
    {
      if (length < 0)
        throw new ArgumentOutOfRangeException(nameof (length), SR.ArgumentOutOfRange_NegativeLength);
      if (startIndex < 0)
        throw new ArgumentOutOfRangeException(nameof (startIndex), SR.ArgumentOutOfRange_StartIndex);
      char* chPtr = ptr + startIndex;
      if (chPtr < ptr)
        throw new ArgumentOutOfRangeException(nameof (startIndex), SR.ArgumentOutOfRange_PartialWCHAR);
      if (length == 0)
        return string.Empty;
      if ((IntPtr) ptr == IntPtr.Zero)
        throw new ArgumentOutOfRangeException(nameof (ptr), SR.ArgumentOutOfRange_PartialWCHAR);
      string str = string.FastAllocateString(length);
      UIntPtr length1 = (UIntPtr) (uint) str.Length;
      Buffer.Memmove<char>(ref str._firstChar, ref *chPtr, length1);
      return str;
    }


    #nullable enable
    /// <summary>Initializes a new instance of the <see cref="T:System.String" /> class to the value indicated by a pointer to an array of 8-bit signed integers.</summary>
    /// <param name="value">A pointer to a null-terminated array of 8-bit signed integers. The integers are interpreted using the current system code page encoding (that is, the encoding specified by <see cref="P:System.Text.Encoding.Default" />).</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="value" /> is <see langword="null" />.</exception>
    /// <exception cref="T:System.ArgumentException">A new instance of <see cref="T:System.String" /> could not be initialized using <paramref name="value" />, assuming <paramref name="value" /> is encoded in ANSI.</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">The length of the new string to initialize, which is determined by the null termination character of <paramref name="value" />, is too large to allocate.</exception>
    /// <exception cref="T:System.AccessViolationException">
    /// <paramref name="value" /> specifies an invalid address.</exception>
    [CLSCompliant(false)]
    [DynamicDependency("Ctor(System.SByte*)")]
    [MethodImpl(MethodImplOptions.InternalCall)]
    public extern unsafe String(sbyte* value);


    #nullable disable
    private unsafe string Ctor(sbyte* value)
    {
      byte* pb = (byte*) value;
      if ((IntPtr) pb == IntPtr.Zero)
        return string.Empty;
      int numBytes = string.strlen((byte*) value);
      return string.CreateStringForSByteConstructor(pb, numBytes);
    }


    #nullable enable
    /// <summary>Initializes a new instance of the <see cref="T:System.String" /> class to the value indicated by a specified pointer to an array of 8-bit signed integers, a starting position within that array, and a length.</summary>
    /// <param name="value">A pointer to an array of 8-bit signed integers. The integers are interpreted using the current system code page encoding (that is, the encoding specified by <see cref="P:System.Text.Encoding.Default" />).</param>
    /// <param name="startIndex">The starting position within <paramref name="value" />.</param>
    /// <param name="length">The number of characters within <paramref name="value" /> to use.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="value" /> is <see langword="null" />.</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    ///        <paramref name="startIndex" /> or <paramref name="length" /> is less than zero.
    /// 
    /// -or-
    /// 
    /// The address specified by <paramref name="value" /> + <paramref name="startIndex" /> is too large for the current platform; that is, the address calculation overflowed.
    /// 
    /// -or-
    /// 
    /// The length of the new string to initialize is too large to allocate.</exception>
    /// <exception cref="T:System.ArgumentException">The address specified by <paramref name="value" /> + <paramref name="startIndex" /> is less than 64K.
    /// 
    /// -or-
    /// 
    /// A new instance of <see cref="T:System.String" /> could not be initialized using <paramref name="value" />, assuming <paramref name="value" /> is encoded in ANSI.</exception>
    /// <exception cref="T:System.AccessViolationException">
    /// <paramref name="value" />, <paramref name="startIndex" />, and <paramref name="length" /> collectively specify an invalid address.</exception>
    [CLSCompliant(false)]
    [DynamicDependency("Ctor(System.SByte*,System.Int32,System.Int32)")]
    [MethodImpl(MethodImplOptions.InternalCall)]
    public extern unsafe String(sbyte* value, int startIndex, int length);


    #nullable disable
    private unsafe string Ctor(sbyte* value, int startIndex, int length)
    {
      if (startIndex < 0)
        throw new ArgumentOutOfRangeException(nameof (startIndex), SR.ArgumentOutOfRange_StartIndex);
      if (length < 0)
        throw new ArgumentOutOfRangeException(nameof (length), SR.ArgumentOutOfRange_NegativeLength);
      if ((IntPtr) value == IntPtr.Zero)
      {
        if (length == 0)
          return string.Empty;
        throw new ArgumentNullException(nameof (value));
      }
      byte* pb = (byte*) (value + startIndex);
      return pb >= value ? string.CreateStringForSByteConstructor(pb, length) : throw new ArgumentOutOfRangeException(nameof (value), SR.ArgumentOutOfRange_PartialWCHAR);
    }

    private static unsafe string CreateStringForSByteConstructor(byte* pb, int numBytes) => numBytes == 0 ? string.Empty : Encoding.UTF8.GetString(pb, numBytes);


    #nullable enable
    /// <summary>Initializes a new instance of the <see cref="T:System.String" /> class to the value indicated by a specified pointer to an array of 8-bit signed integers, a starting position within that array, a length, and an <see cref="T:System.Text.Encoding" /> object.</summary>
    /// <param name="value">A pointer to an array of 8-bit signed integers.</param>
    /// <param name="startIndex">The starting position within <paramref name="value" />.</param>
    /// <param name="length">The number of characters within <paramref name="value" /> to use.</param>
    /// <param name="enc">An object that specifies how the array referenced by <paramref name="value" /> is encoded. If <paramref name="enc" /> is <see langword="null" />, ANSI encoding is assumed.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="value" /> is <see langword="null" />.</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    ///        <paramref name="startIndex" /> or <paramref name="length" /> is less than zero.
    /// 
    /// -or-
    /// 
    /// The address specified by <paramref name="value" /> + <paramref name="startIndex" /> is too large for the current platform; that is, the address calculation overflowed.
    /// 
    /// -or-
    /// 
    /// The length of the new string to initialize is too large to allocate.</exception>
    /// <exception cref="T:System.ArgumentException">The address specified by <paramref name="value" /> + <paramref name="startIndex" /> is less than 64K.
    /// 
    /// -or-
    /// 
    /// A new instance of <see cref="T:System.String" /> could not be initialized using <paramref name="value" />, assuming <paramref name="value" /> is encoded as specified by <paramref name="enc" />.</exception>
    /// <exception cref="T:System.AccessViolationException">
    /// <paramref name="value" />, <paramref name="startIndex" />, and <paramref name="length" /> collectively specify an invalid address.</exception>
    [CLSCompliant(false)]
    [DynamicDependency("Ctor(System.SByte*,System.Int32,System.Int32,System.Text.Encoding)")]
    [MethodImpl(MethodImplOptions.InternalCall)]
    public extern unsafe String(sbyte* value, int startIndex, int length, Encoding enc);


    #nullable disable
    private unsafe string Ctor(sbyte* value, int startIndex, int length, Encoding enc)
    {
      if (enc == null)
        return new string(value, startIndex, length);
      if (length < 0)
        throw new ArgumentOutOfRangeException(nameof (length), SR.ArgumentOutOfRange_NeedNonNegNum);
      if (startIndex < 0)
        throw new ArgumentOutOfRangeException(nameof (startIndex), SR.ArgumentOutOfRange_StartIndex);
      if ((IntPtr) value == IntPtr.Zero)
      {
        if (length == 0)
          return string.Empty;
        throw new ArgumentNullException(nameof (value));
      }
      byte* pointer = (byte*) (value + startIndex);
      if (pointer < value)
        throw new ArgumentOutOfRangeException(nameof (startIndex), SR.ArgumentOutOfRange_PartialWCHAR);
      return enc.GetString(new ReadOnlySpan<byte>((void*) pointer, length));
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.String" /> class to the value indicated by a specified Unicode character repeated a specified number of times.</summary>
    /// <param name="c">A Unicode character.</param>
    /// <param name="count">The number of times <paramref name="c" /> occurs.</param>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    /// <paramref name="count" /> is less than zero.</exception>
    [DynamicDependency("Ctor(System.Char,System.Int32)")]
    [MethodImpl(MethodImplOptions.InternalCall)]
    public extern String(char c, int count);

    private unsafe string Ctor(char c, int count)
    {
      if (count <= 0)
      {
        if (count == 0)
          return string.Empty;
        throw new ArgumentOutOfRangeException(nameof (count), SR.ArgumentOutOfRange_NegativeCount);
      }
      string str = string.FastAllocateString(count);
      if (c != char.MinValue)
      {
        fixed (char* chPtr = &str._firstChar)
        {
          uint num = (uint) c << 16 | (uint) c;
          uint* numPtr = (uint*) chPtr;
          if (count >= 4)
          {
            count -= 4;
            do
            {
              *numPtr = num;
              numPtr[1] = num;
              numPtr += 2;
              count -= 4;
            }
            while (count >= 0);
          }
          if ((count & 2) != 0)
          {
            *numPtr = num;
            ++numPtr;
          }
          if ((count & 1) != 0)
            *(short*) numPtr = (short) c;
        }
      }
      return str;
    }


    #nullable enable
    /// <summary>Initializes a new instance of the <see cref="T:System.String" /> class to the Unicode characters indicated in the specified read-only span.</summary>
    /// <param name="value">A read-only span of Unicode characters.</param>
    [DynamicDependency("Ctor(System.ReadOnlySpan{System.Char})")]
    [MethodImpl(MethodImplOptions.InternalCall)]
    public extern String(ReadOnlySpan<char> value);


    #nullable disable
    private string Ctor(ReadOnlySpan<char> value)
    {
      if (value.Length == 0)
        return string.Empty;
      string str = string.FastAllocateString(value.Length);
      Buffer.Memmove<char>(ref str._firstChar, ref MemoryMarshal.GetReference<char>(value), (UIntPtr) (uint) value.Length);
      return str;
    }


    #nullable enable
    /// <summary>Creates a new string with a specific length and initializes it after creation by using the specified callback.</summary>
    /// <param name="length">The length of the string to create.</param>
    /// <param name="state">The element to pass to <paramref name="action" />.</param>
    /// <param name="action">A callback to initialize the string.</param>
    /// <typeparam name="TState">The type of the element to pass to <paramref name="action" />.</typeparam>
    /// <returns>The created string.</returns>
    public static string Create<TState>(int length, TState state, SpanAction<char, TState> action)
    {
      if (action == null)
        throw new ArgumentNullException(nameof (action));
      if (length <= 0)
      {
        if (length == 0)
          return string.Empty;
        throw new ArgumentOutOfRangeException(nameof (length));
      }
      string str = string.FastAllocateString(length);
      action(new Span<char>(ref str.GetRawStringData(), length), state);
      return str;
    }

    /// <summary>Creates a new string by using the specified provider to control the formatting of the specified interpolated string.</summary>
    /// <param name="provider">An object that supplies culture-specific formatting information.</param>
    /// <param name="handler">The interpolated string, passed by reference.</param>
    /// <returns>The string that results for formatting the interpolated string using the specified format provider.</returns>
    public static string Create(
      IFormatProvider? provider,
      [InterpolatedStringHandlerArgument("provider")] ref DefaultInterpolatedStringHandler handler)
    {
      return handler.ToStringAndClear();
    }

    /// <summary>Creates a new string by using the specified provider to control the formatting of the specified interpolated string.</summary>
    /// <param name="provider">An object that supplies culture-specific formatting information.</param>
    /// <param name="initialBuffer">The initial buffer that may be used as temporary space as part of the formatting operation. The contents of this buffer may be overwritten.</param>
    /// <param name="handler">The interpolated string, passed by reference.</param>
    /// <returns>The string that results for formatting the interpolated string using the specified format provider.</returns>
    public static string Create(
      IFormatProvider? provider,
      Span<char> initialBuffer,
      [InterpolatedStringHandlerArgument(new string[] {"provider", "initialBuffer"})] ref DefaultInterpolatedStringHandler handler)
    {
      return handler.ToStringAndClear();
    }

    /// <summary>Defines an implicit conversion of a given string to a read-only span of characters.</summary>
    /// <param name="value">A string to implicitly convert.</param>
    /// <returns>A new read-only span of characters representing the string.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ReadOnlySpan<char>(string? value) => value == null ? new ReadOnlySpan<char>() : new ReadOnlySpan<char>(ref value.GetRawStringData(), value.Length);


    #nullable disable
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal bool TryGetSpan(int startIndex, int count, out ReadOnlySpan<char> slice)
    {
      if ((ulong) (uint) startIndex + (ulong) (uint) count > (ulong) (uint) this.Length)
      {
        slice = new ReadOnlySpan<char>();
        return false;
      }
      slice = new ReadOnlySpan<char>(ref Unsafe.Add<char>(ref this._firstChar, (IntPtr) (uint) startIndex), count);
      return true;
    }


    #nullable enable
    /// <summary>Returns a reference to this instance of <see cref="T:System.String" />.</summary>
    /// <returns>This instance of <see cref="T:System.String" />.</returns>
    public object Clone() => (object) this;

    /// <summary>Creates a new instance of <see cref="T:System.String" /> with the same value as a specified <see cref="T:System.String" />.</summary>
    /// <param name="str">The string to copy.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="str" /> is <see langword="null" />.</exception>
    /// <returns>A new string with the same value as <paramref name="str" />.</returns>
    public static string Copy(string str)
    {
      string str1 = str != null ? string.FastAllocateString(str.Length) : throw new ArgumentNullException(nameof (str));
      UIntPtr length = (UIntPtr) (uint) str1.Length;
      Buffer.Memmove<char>(ref str1._firstChar, ref str._firstChar, length);
      return str1;
    }

    /// <summary>Copies a specified number of characters from a specified position in this instance to a specified position in an array of Unicode characters.</summary>
    /// <param name="sourceIndex">The index of the first character in this instance to copy.</param>
    /// <param name="destination">An array of Unicode characters to which characters in this instance are copied.</param>
    /// <param name="destinationIndex">The index in <paramref name="destination" /> at which the copy operation begins.</param>
    /// <param name="count">The number of characters in this instance to copy to <paramref name="destination" />.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="destination" /> is <see langword="null" />.</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    ///        <paramref name="sourceIndex" />, <paramref name="destinationIndex" />, or <paramref name="count" /> is negative
    /// 
    /// -or-
    /// 
    /// <paramref name="sourceIndex" /> does not identify a position in the current instance.
    /// 
    /// -or-
    /// 
    /// <paramref name="destinationIndex" /> does not identify a valid index in the <paramref name="destination" /> array.
    /// 
    /// -or-
    /// 
    /// <paramref name="count" /> is greater than the length of the substring from <paramref name="sourceIndex" /> to the end of this instance
    /// 
    /// -or-
    /// 
    /// <paramref name="count" /> is greater than the length of the subarray from <paramref name="destinationIndex" /> to the end of the <paramref name="destination" /> array.</exception>
    public void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count)
    {
      if (destination == null)
        throw new ArgumentNullException(nameof (destination));
      if (count < 0)
        throw new ArgumentOutOfRangeException(nameof (count), SR.ArgumentOutOfRange_NegativeCount);
      if (sourceIndex < 0)
        throw new ArgumentOutOfRangeException(nameof (sourceIndex), SR.ArgumentOutOfRange_Index);
      if (count > this.Length - sourceIndex)
        throw new ArgumentOutOfRangeException(nameof (sourceIndex), SR.ArgumentOutOfRange_IndexCount);
      if (destinationIndex > destination.Length - count || destinationIndex < 0)
        throw new ArgumentOutOfRangeException(nameof (destinationIndex), SR.ArgumentOutOfRange_IndexCount);
      Buffer.Memmove<char>(ref Unsafe.Add<char>(ref MemoryMarshal.GetArrayDataReference<char>(destination), destinationIndex), ref Unsafe.Add<char>(ref this._firstChar, sourceIndex), (UIntPtr) (uint) count);
    }

    /// <summary>Copies the contents of this string into the destination span.</summary>
    /// <param name="destination">The span into which to copy this string's contents.</param>
    /// <exception cref="T:System.ArgumentException">The destination span is shorter than the source string.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CopyTo(Span<char> destination)
    {
      if ((uint) this.Length <= (uint) destination.Length)
        Buffer.Memmove<char>(ref destination._pointer.Value, ref this._firstChar, (UIntPtr) (uint) this.Length);
      else
        ThrowHelper.ThrowArgumentException_DestinationTooShort();
    }

    /// <summary>Copies the contents of this string into the destination span.</summary>
    /// <param name="destination">The span into which to copy this string's contents.</param>
    /// <returns>
    /// <see langword="true" /> if the data was copied; <see langword="false" /> if the destination was too short to fit the contents of the string.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryCopyTo(Span<char> destination)
    {
      bool flag = false;
      if ((uint) this.Length <= (uint) destination.Length)
      {
        Buffer.Memmove<char>(ref destination._pointer.Value, ref this._firstChar, (UIntPtr) (uint) this.Length);
        flag = true;
      }
      return flag;
    }

    /// <summary>Copies the characters in this instance to a Unicode character array.</summary>
    /// <returns>A Unicode character array whose elements are the individual characters of this instance. If this instance is an empty string, the returned array is empty and has a zero length.</returns>
    public char[] ToCharArray()
    {
      if (this.Length == 0)
        return Array.Empty<char>();
      char[] array = new char[this.Length];
      Buffer.Memmove<char>(ref MemoryMarshal.GetArrayDataReference<char>(array), ref this._firstChar, (UIntPtr) (uint) this.Length);
      return array;
    }

    /// <summary>Copies the characters in a specified substring in this instance to a Unicode character array.</summary>
    /// <param name="startIndex">The starting position of a substring in this instance.</param>
    /// <param name="length">The length of the substring in this instance.</param>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    ///        <paramref name="startIndex" /> or <paramref name="length" /> is less than zero.
    /// 
    /// -or-
    /// 
    /// <paramref name="startIndex" /> plus <paramref name="length" /> is greater than the length of this instance.</exception>
    /// <returns>A Unicode character array whose elements are the <paramref name="length" /> number of characters in this instance starting from character position <paramref name="startIndex" />.</returns>
    public char[] ToCharArray(int startIndex, int length)
    {
      if (startIndex < 0 || startIndex > this.Length || startIndex > this.Length - length)
        throw new ArgumentOutOfRangeException(nameof (startIndex), SR.ArgumentOutOfRange_Index);
      if (length <= 0)
      {
        if (length == 0)
          return Array.Empty<char>();
        throw new ArgumentOutOfRangeException(nameof (length), SR.ArgumentOutOfRange_Index);
      }
      char[] array = new char[length];
      Buffer.Memmove<char>(ref MemoryMarshal.GetArrayDataReference<char>(array), ref Unsafe.Add<char>(ref this._firstChar, startIndex), (UIntPtr) (uint) length);
      return array;
    }

    /// <summary>Indicates whether the specified string is <see langword="null" /> or an empty string ("").</summary>
    /// <param name="value">The string to test.</param>
    /// <returns>
    /// <see langword="true" /> if the <paramref name="value" /> parameter is <see langword="null" /> or an empty string (""); otherwise, <see langword="false" />.</returns>
    [NonVersionable]
    public static bool IsNullOrEmpty([NotNullWhen(false)] string? value)
    {
      switch (value)
      {
        case "":
        case null:
          return true;
        default:
          return false;
      }
    }

    /// <summary>Indicates whether a specified string is <see langword="null" />, empty, or consists only of white-space characters.</summary>
    /// <param name="value">The string to test.</param>
    /// <returns>
    /// <see langword="true" /> if the <paramref name="value" /> parameter is <see langword="null" /> or <see cref="F:System.String.Empty" />, or if <paramref name="value" /> consists exclusively of white-space characters.</returns>
    public static bool IsNullOrWhiteSpace([NotNullWhen(false)] string? value)
    {
      if (value == null)
        return true;
      for (int index = 0; index < value.Length; ++index)
      {
        if (!char.IsWhiteSpace(value[index]))
          return false;
      }
      return true;
    }

    /// <summary>Returns a reference to the element of the string at index zero.
    /// 
    /// This method is intended to support .NET compilers and is not intended to be called by user code.</summary>
    /// <exception cref="T:System.NullReferenceException">The string is null.</exception>
    /// <returns>A reference to the first character in the string, or a reference to the string's null terminator if the string is empty.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [NonVersionable]
    public ref readonly char GetPinnableReference() => ref this._firstChar;


    #nullable disable
    internal ref char GetRawStringData() => ref this._firstChar;

    internal static unsafe string CreateStringFromEncoding(
      byte* bytes,
      int byteLength,
      Encoding encoding)
    {
      int charCount = encoding.GetCharCount(bytes, byteLength);
      if (charCount == 0)
        return string.Empty;
      string stringFromEncoding = string.FastAllocateString(charCount);
      fixed (char* chars = &stringFromEncoding._firstChar)
        encoding.GetChars(bytes, byteLength, chars, charCount);
      return stringFromEncoding;
    }

    internal static string CreateFromChar(char c)
    {
      string fromChar = string.FastAllocateString(1);
      fromChar._firstChar = c;
      return fromChar;
    }

    internal static string CreateFromChar(char c1, char c2)
    {
      string fromChar = string.FastAllocateString(2);
      fromChar._firstChar = c1;
      Unsafe.Add<char>(ref fromChar._firstChar, 1) = c2;
      return fromChar;
    }


    #nullable enable
    /// <summary>Returns this instance of <see cref="T:System.String" />; no actual conversion is performed.</summary>
    /// <returns>The current string.</returns>
    public override string ToString() => this;

    /// <summary>Returns this instance of <see cref="T:System.String" />; no actual conversion is performed.</summary>
    /// <param name="provider">(Reserved) An object that supplies culture-specific formatting information.</param>
    /// <returns>The current string.</returns>
    public string ToString(IFormatProvider? provider) => this;

    /// <summary>Retrieves an object that can iterate through the individual characters in this string.</summary>
    /// <returns>An enumerator object.</returns>
    public CharEnumerator GetEnumerator() => new CharEnumerator(this);


    #nullable disable
    IEnumerator<char> IEnumerable<char>.GetEnumerator() => (IEnumerator<char>) new CharEnumerator(this);

    /// <summary>Returns an enumerator that iterates through the current <see cref="T:System.String" /> object.</summary>
    /// <returns>An enumerator that can be used to iterate through the current string.</returns>
    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) new CharEnumerator(this);

    /// <summary>Returns an enumeration of <see cref="T:System.Text.Rune" /> from this string.</summary>
    /// <returns>A string rune enumerator.</returns>
    public StringRuneEnumerator EnumerateRunes() => new StringRuneEnumerator(this);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static unsafe int wcslen(char* ptr)
    {
      int num = SpanHelpers.IndexOf(ref *ptr, char.MinValue, int.MaxValue);
      if (num < 0)
        string.ThrowMustBeNullTerminatedString();
      return num;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static unsafe int strlen(byte* ptr)
    {
      int num = SpanHelpers.IndexOf(ref *ptr, (byte) 0, int.MaxValue);
      if (num < 0)
        string.ThrowMustBeNullTerminatedString();
      return num;
    }

    [DoesNotReturn]
    private static void ThrowMustBeNullTerminatedString() => throw new ArgumentException(SR.Arg_MustBeNullTerminatedString);

    /// <summary>Returns the <see cref="T:System.TypeCode" /> for the <see cref="T:System.String" /> class.</summary>
    /// <returns>The enumerated constant, <see cref="F:System.TypeCode.String" />.</returns>
    public TypeCode GetTypeCode() => TypeCode.String;

    /// <summary>For a description of this member, see <see cref="M:System.IConvertible.ToBoolean(System.IFormatProvider)" />.</summary>
    /// <param name="provider">This parameter is ignored.</param>
    /// <exception cref="T:System.FormatException">The value of the current string is not <see cref="F:System.Boolean.TrueString" /> or <see cref="F:System.Boolean.FalseString" />.</exception>
    /// <returns>
    /// <see langword="true" /> if the value of the current string is <see cref="F:System.Boolean.TrueString" />; <see langword="false" /> if the value of the current string is <see cref="F:System.Boolean.FalseString" />.</returns>
    bool IConvertible.ToBoolean(IFormatProvider provider) => Convert.ToBoolean(this, provider);

    /// <summary>For a description of this member, see <see cref="M:System.IConvertible.ToChar(System.IFormatProvider)" />.</summary>
    /// <param name="provider">An object that provides culture-specific formatting information.</param>
    /// <returns>The character at index 0 in the current <see cref="T:System.String" /> object.</returns>
    char IConvertible.ToChar(IFormatProvider provider) => Convert.ToChar(this, provider);

    /// <summary>For a description of this member, see <see cref="M:System.IConvertible.ToSByte(System.IFormatProvider)" />.</summary>
    /// <param name="provider">An object that provides culture-specific formatting information.</param>
    /// <exception cref="T:System.FormatException">The value of the current <see cref="T:System.String" /> object cannot be parsed.</exception>
    /// <exception cref="T:System.OverflowException">The value of the current <see cref="T:System.String" /> object is a number greater than <see cref="F:System.SByte.MaxValue" /> or less than <see cref="F:System.SByte.MinValue" />.</exception>
    /// <returns>The converted value of the current <see cref="T:System.String" /> object.</returns>
    sbyte IConvertible.ToSByte(IFormatProvider provider) => Convert.ToSByte(this, provider);

    /// <summary>For a description of this member, see <see cref="M:System.IConvertible.ToByte(System.IFormatProvider)" />.</summary>
    /// <param name="provider">An object that provides culture-specific formatting information.</param>
    /// <exception cref="T:System.FormatException">The value of the current <see cref="T:System.String" /> object cannot be parsed.</exception>
    /// <exception cref="T:System.OverflowException">The value of the current <see cref="T:System.String" /> object is a number greater than <see cref="F:System.Byte.MaxValue" /> or less than <see cref="F:System.Byte.MinValue" />.</exception>
    /// <returns>The converted value of the current <see cref="T:System.String" /> object.</returns>
    byte IConvertible.ToByte(IFormatProvider provider) => Convert.ToByte(this, provider);

    /// <summary>For a description of this member, see <see cref="M:System.IConvertible.ToInt16(System.IFormatProvider)" />.</summary>
    /// <param name="provider">An object that provides culture-specific formatting information.</param>
    /// <exception cref="T:System.FormatException">The value of the current <see cref="T:System.String" /> object cannot be parsed.</exception>
    /// <exception cref="T:System.OverflowException">The value of the current <see cref="T:System.String" /> object is a number greater than <see cref="F:System.Int16.MaxValue" /> or less than <see cref="F:System.Int16.MinValue" />.</exception>
    /// <returns>The converted value of the current <see cref="T:System.String" /> object.</returns>
    short IConvertible.ToInt16(IFormatProvider provider) => Convert.ToInt16(this, provider);

    /// <summary>For a description of this member, see <see cref="M:System.IConvertible.ToUInt16(System.IFormatProvider)" />.</summary>
    /// <param name="provider">An object that provides culture-specific formatting information.</param>
    /// <exception cref="T:System.FormatException">The value of the current <see cref="T:System.String" /> object cannot be parsed.</exception>
    /// <exception cref="T:System.OverflowException">The value of the current <see cref="T:System.String" /> object is a number greater than <see cref="F:System.UInt16.MaxValue" /> or less than <see cref="F:System.UInt16.MinValue" />.</exception>
    /// <returns>The converted value of the current <see cref="T:System.String" /> object.</returns>
    ushort IConvertible.ToUInt16(IFormatProvider provider) => Convert.ToUInt16(this, provider);

    /// <summary>For a description of this member, see <see cref="M:System.IConvertible.ToInt32(System.IFormatProvider)" />.</summary>
    /// <param name="provider">An object that provides culture-specific formatting information.</param>
    /// <returns>The converted value of the current <see cref="T:System.String" /> object.</returns>
    int IConvertible.ToInt32(IFormatProvider provider) => Convert.ToInt32(this, provider);

    /// <summary>For a description of this member, see <see cref="M:System.IConvertible.ToUInt32(System.IFormatProvider)" />.</summary>
    /// <param name="provider">An object that provides culture-specific formatting information.</param>
    /// <exception cref="T:System.FormatException">The value of the current <see cref="T:System.String" /> object cannot be parsed.</exception>
    /// <exception cref="T:System.OverflowException">The value of the current <see cref="T:System.String" /> object is a number greater <see cref="F:System.UInt32.MaxValue" /> or less than <see cref="F:System.UInt32.MinValue" /></exception>
    /// <returns>The converted value of the current <see cref="T:System.String" /> object.</returns>
    uint IConvertible.ToUInt32(IFormatProvider provider) => Convert.ToUInt32(this, provider);

    /// <summary>For a description of this member, see <see cref="M:System.IConvertible.ToInt64(System.IFormatProvider)" />.</summary>
    /// <param name="provider">An object that provides culture-specific formatting information.</param>
    /// <returns>The converted value of the current <see cref="T:System.String" /> object.</returns>
    long IConvertible.ToInt64(IFormatProvider provider) => Convert.ToInt64(this, provider);

    /// <summary>For a description of this member, see <see cref="M:System.IConvertible.ToUInt64(System.IFormatProvider)" />.</summary>
    /// <param name="provider">An object that provides culture-specific formatting information.</param>
    /// <returns>The converted value of the current <see cref="T:System.String" /> object.</returns>
    ulong IConvertible.ToUInt64(IFormatProvider provider) => Convert.ToUInt64(this, provider);

    /// <summary>For a description of this member, see <see cref="M:System.IConvertible.ToSingle(System.IFormatProvider)" />.</summary>
    /// <param name="provider">An object that provides culture-specific formatting information.</param>
    /// <returns>The converted value of the current <see cref="T:System.String" /> object.</returns>
    float IConvertible.ToSingle(IFormatProvider provider) => Convert.ToSingle(this, provider);

    /// <summary>For a description of this member, see <see cref="M:System.IConvertible.ToDouble(System.IFormatProvider)" />.</summary>
    /// <param name="provider">An object that provides culture-specific formatting information.</param>
    /// <exception cref="T:System.FormatException">The value of the current <see cref="T:System.String" /> object cannot be parsed.</exception>
    /// <exception cref="T:System.OverflowException">The value of the current <see cref="T:System.String" /> object is a number less than <see cref="F:System.Double.MinValue" /> or greater than <see cref="F:System.Double.MaxValue" />.</exception>
    /// <returns>The converted value of the current <see cref="T:System.String" /> object.</returns>
    double IConvertible.ToDouble(IFormatProvider provider) => Convert.ToDouble(this, provider);

    /// <summary>For a description of this member, see <see cref="M:System.IConvertible.ToDecimal(System.IFormatProvider)" />.</summary>
    /// <param name="provider">An object that provides culture-specific formatting information.</param>
    /// <exception cref="T:System.FormatException">The value of the current <see cref="T:System.String" /> object cannot be parsed.</exception>
    /// <exception cref="T:System.OverflowException">The value of the current <see cref="T:System.String" /> object is a number less than <see cref="F:System.Decimal.MinValue" /> or than <see cref="F:System.Decimal.MaxValue" /> greater.</exception>
    /// <returns>The converted value of the current <see cref="T:System.String" /> object.</returns>
    Decimal IConvertible.ToDecimal(IFormatProvider provider) => Convert.ToDecimal(this, provider);

    /// <summary>For a description of this member, see <see cref="M:System.IConvertible.ToDateTime(System.IFormatProvider)" />.</summary>
    /// <param name="provider">An object that provides culture-specific formatting information.</param>
    /// <returns>The converted value of the current <see cref="T:System.String" /> object.</returns>
    DateTime IConvertible.ToDateTime(IFormatProvider provider) => Convert.ToDateTime(this, provider);

    /// <summary>For a description of this member, see <see cref="M:System.IConvertible.ToType(System.Type,System.IFormatProvider)" />.</summary>
    /// <param name="type">The type of the returned object.</param>
    /// <param name="provider">An object that provides culture-specific formatting information.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="type" /> is <see langword="null" />.</exception>
    /// <exception cref="T:System.InvalidCastException">The value of the current <see cref="T:System.String" /> object cannot be converted to the type specified by the <paramref name="type" /> parameter.</exception>
    /// <returns>The converted value of the current <see cref="T:System.String" /> object.</returns>
    object IConvertible.ToType(Type type, IFormatProvider provider) => Convert.DefaultToType((IConvertible) this, type, provider);

    /// <summary>Indicates whether this string is in Unicode normalization form C.</summary>
    /// <exception cref="T:System.ArgumentException">The current instance contains invalid Unicode characters.</exception>
    /// <returns>
    /// <see langword="true" /> if this string is in normalization form C; otherwise, <see langword="false" />.</returns>
    public bool IsNormalized() => this.IsNormalized(NormalizationForm.FormC);

    /// <summary>Indicates whether this string is in the specified Unicode normalization form.</summary>
    /// <param name="normalizationForm">A Unicode normalization form.</param>
    /// <exception cref="T:System.ArgumentException">The current instance contains invalid Unicode characters.</exception>
    /// <returns>
    /// <see langword="true" /> if this string is in the normalization form specified by the <paramref name="normalizationForm" /> parameter; otherwise, <see langword="false" />.</returns>
    public bool IsNormalized(NormalizationForm normalizationForm) => this.IsAscii() && (normalizationForm == NormalizationForm.FormC || normalizationForm == NormalizationForm.FormKC || normalizationForm == NormalizationForm.FormD || normalizationForm == NormalizationForm.FormKD) || Normalization.IsNormalized(this, normalizationForm);


    #nullable enable
    /// <summary>Returns a new string whose textual value is the same as this string, but whose binary representation is in Unicode normalization form C.</summary>
    /// <exception cref="T:System.ArgumentException">The current instance contains invalid Unicode characters.</exception>
    /// <returns>A new, normalized string whose textual value is the same as this string, but whose binary representation is in normalization form C.</returns>
    public string Normalize() => this.Normalize(NormalizationForm.FormC);

    /// <summary>Returns a new string whose textual value is the same as this string, but whose binary representation is in the specified Unicode normalization form.</summary>
    /// <param name="normalizationForm">A Unicode normalization form.</param>
    /// <exception cref="T:System.ArgumentException">The current instance contains invalid Unicode characters.</exception>
    /// <returns>A new string whose textual value is the same as this string, but whose binary representation is in the normalization form specified by the <paramref name="normalizationForm" /> parameter.</returns>
    public string Normalize(NormalizationForm normalizationForm) => this.IsAscii() && (normalizationForm == NormalizationForm.FormC || normalizationForm == NormalizationForm.FormKC || normalizationForm == NormalizationForm.FormD || normalizationForm == NormalizationForm.FormKD) ? this : Normalization.Normalize(this, normalizationForm);

    private unsafe bool IsAscii()
    {
      fixed (char* pBuffer = &this._firstChar)
        return (IntPtr) ASCIIUtility.GetIndexOfFirstNonAsciiChar(pBuffer, (UIntPtr) (uint) this.Length) == (IntPtr) (uint) this.Length;
    }

    /// <summary>Gets the <see cref="T:System.Char" /> object at a specified position in the current <see cref="T:System.String" /> object.</summary>
    /// <param name="index">A position in the current string.</param>
    /// <exception cref="T:System.IndexOutOfRangeException">
    /// <paramref name="index" /> is greater than or equal to the length of this object or less than zero.</exception>
    /// <returns>The object at position <paramref name="index" />.</returns>
    [IndexerName("Chars")]
    public char this[int index]
    {
      [Intrinsic] get
      {
        if ((uint) index >= (uint) this._stringLength)
          ThrowHelper.ThrowIndexOutOfRangeException();
        return Unsafe.Add<char>(ref this._firstChar, (IntPtr) (uint) index);
      }
    }

    /// <summary>Gets the number of characters in the current <see cref="T:System.String" /> object.</summary>
    /// <returns>The number of characters in the current string.</returns>
    public int Length
    {
      [Intrinsic] get => this._stringLength;
    }


    #nullable disable
    private static void FillStringChecked(string dest, int destPos, string src)
    {
      if (src.Length > dest.Length - destPos)
        throw new IndexOutOfRangeException();
      Buffer.Memmove<char>(ref Unsafe.Add<char>(ref dest._firstChar, destPos), ref src._firstChar, (UIntPtr) (uint) src.Length);
    }


    #nullable enable
    /// <summary>Creates the string  representation of a specified object.</summary>
    /// <param name="arg0">The object to represent, or <see langword="null" />.</param>
    /// <returns>The string representation of the value of <paramref name="arg0" />, or <see cref="F:System.String.Empty" /> if <paramref name="arg0" /> is <see langword="null" />.</returns>
    public static string Concat(object? arg0) => arg0?.ToString() ?? string.Empty;

    /// <summary>Concatenates the string representations of two specified objects.</summary>
    /// <param name="arg0">The first object to concatenate.</param>
    /// <param name="arg1">The second object to concatenate.</param>
    /// <returns>The concatenated string representations of the values of <paramref name="arg0" /> and <paramref name="arg1" />.</returns>
    public static string Concat(object? arg0, object? arg1) => arg0?.ToString() + arg1?.ToString();

    /// <summary>Concatenates the string representations of three specified objects.</summary>
    /// <param name="arg0">The first object to concatenate.</param>
    /// <param name="arg1">The second object to concatenate.</param>
    /// <param name="arg2">The third object to concatenate.</param>
    /// <returns>The concatenated string representations of the values of <paramref name="arg0" />, <paramref name="arg1" />, and <paramref name="arg2" />.</returns>
    public static string Concat(object? arg0, object? arg1, object? arg2) => arg0?.ToString() + arg1?.ToString() + arg2?.ToString();

    /// <summary>Concatenates the string representations of the elements in a specified <see cref="T:System.Object" /> array.</summary>
    /// <param name="args">An object array that contains the elements to concatenate.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="args" /> is <see langword="null" />.</exception>
    /// <exception cref="T:System.OutOfMemoryException">Out of memory.</exception>
    /// <returns>The concatenated string representations of the values of the elements in <paramref name="args" />.</returns>
    public static string Concat(params object?[] args)
    {
      if (args == null)
        throw new ArgumentNullException(nameof (args));
      if (args.Length <= 1)
        return args.Length != 0 ? args[0]?.ToString() ?? string.Empty : string.Empty;
      string[] strArray = new string[args.Length];
      int length = 0;
      for (int index = 0; index < args.Length; ++index)
      {
        string str = args[index]?.ToString() ?? string.Empty;
        strArray[index] = str;
        length += str.Length;
        if (length < 0)
          throw new OutOfMemoryException();
      }
      if (length == 0)
        return string.Empty;
      string dest = string.FastAllocateString(length);
      int destPos = 0;
      for (int index = 0; index < strArray.Length; ++index)
      {
        string src = strArray[index];
        string.FillStringChecked(dest, destPos, src);
        destPos += src.Length;
      }
      return dest;
    }

    /// <summary>Concatenates the members of an <see cref="T:System.Collections.Generic.IEnumerable`1" /> implementation.</summary>
    /// <param name="values">A collection object that implements the <see cref="T:System.Collections.Generic.IEnumerable`1" /> interface.</param>
    /// <typeparam name="T">The type of the members of <paramref name="values" />.</typeparam>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="values" /> is <see langword="null" />.</exception>
    /// <returns>The concatenated members in <paramref name="values" />.</returns>
    public static unsafe string Concat<T>(IEnumerable<T> values)
    {
      if (values == null)
        throw new ArgumentNullException(nameof (values));
      if (typeof (T) == typeof (char))
      {
        using (IEnumerator<char> enumerator = Unsafe.As<IEnumerable<char>>((object) values).GetEnumerator())
        {
          if (!enumerator.MoveNext())
            return string.Empty;
          char current1 = enumerator.Current;
          if (!enumerator.MoveNext())
            return string.CreateFromChar(current1);
          // ISSUE: untyped stack allocation
          ValueStringBuilder valueStringBuilder = new ValueStringBuilder(new Span<char>((void*) __untypedstackalloc(new IntPtr(512)), 256));
          valueStringBuilder.Append(current1);
          do
          {
            char current2 = enumerator.Current;
            valueStringBuilder.Append(current2);
          }
          while (enumerator.MoveNext());
          return valueStringBuilder.ToString();
        }
      }
      else
      {
        using (IEnumerator<T> enumerator = values.GetEnumerator())
        {
          if (!enumerator.MoveNext())
            return string.Empty;
          string s = enumerator.Current?.ToString();
          if (!enumerator.MoveNext())
            return s ?? string.Empty;
          // ISSUE: untyped stack allocation
          ValueStringBuilder valueStringBuilder = new ValueStringBuilder(new Span<char>((void*) __untypedstackalloc(new IntPtr(512)), 256));
          valueStringBuilder.Append(s);
          do
          {
            T current = enumerator.Current;
            if ((object) current != null)
              valueStringBuilder.Append(current.ToString());
          }
          while (enumerator.MoveNext());
          return valueStringBuilder.ToString();
        }
      }
    }

    /// <summary>Concatenates the members of a constructed <see cref="T:System.Collections.Generic.IEnumerable`1" /> collection of type <see cref="T:System.String" />.</summary>
    /// <param name="values">A collection object that implements <see cref="T:System.Collections.Generic.IEnumerable`1" /> and whose generic type argument is <see cref="T:System.String" />.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="values" /> is <see langword="null" />.</exception>
    /// <returns>The concatenated strings in <paramref name="values" />, or <see cref="F:System.String.Empty" /> if <paramref name="values" /> is an empty <see langword="IEnumerable(Of String)" />.</returns>
    public static unsafe string Concat(IEnumerable<string?> values)
    {
      if (values == null)
        throw new ArgumentNullException(nameof (values));
      using (IEnumerator<string> enumerator = values.GetEnumerator())
      {
        if (!enumerator.MoveNext())
          return string.Empty;
        string current = enumerator.Current;
        if (!enumerator.MoveNext())
          return current ?? string.Empty;
        // ISSUE: untyped stack allocation
        ValueStringBuilder valueStringBuilder = new ValueStringBuilder(new Span<char>((void*) __untypedstackalloc(new IntPtr(512)), 256));
        valueStringBuilder.Append(current);
        do
        {
          valueStringBuilder.Append(enumerator.Current);
        }
        while (enumerator.MoveNext());
        return valueStringBuilder.ToString();
      }
    }

    /// <summary>Concatenates two specified instances of <see cref="T:System.String" />.</summary>
    /// <param name="str0">The first string to concatenate.</param>
    /// <param name="str1">The second string to concatenate.</param>
    /// <returns>The concatenation of <paramref name="str0" /> and <paramref name="str1" />.</returns>
    public static string Concat(string? str0, string? str1)
    {
      if (string.IsNullOrEmpty(str0))
        return string.IsNullOrEmpty(str1) ? string.Empty : str1;
      if (string.IsNullOrEmpty(str1))
        return str0;
      int length = str0.Length;
      string dest = string.FastAllocateString(length + str1.Length);
      string.FillStringChecked(dest, 0, str0);
      string.FillStringChecked(dest, length, str1);
      return dest;
    }

    /// <summary>Concatenates three specified instances of <see cref="T:System.String" />.</summary>
    /// <param name="str0">The first string to concatenate.</param>
    /// <param name="str1">The second string to concatenate.</param>
    /// <param name="str2">The third string to concatenate.</param>
    /// <returns>The concatenation of <paramref name="str0" />, <paramref name="str1" />, and <paramref name="str2" />.</returns>
    public static string Concat(string? str0, string? str1, string? str2)
    {
      if (string.IsNullOrEmpty(str0))
        return str1 + str2;
      if (string.IsNullOrEmpty(str1))
        return str0 + str2;
      if (string.IsNullOrEmpty(str2))
        return str0 + str1;
      string dest = string.FastAllocateString(str0.Length + str1.Length + str2.Length);
      string.FillStringChecked(dest, 0, str0);
      string.FillStringChecked(dest, str0.Length, str1);
      string.FillStringChecked(dest, str0.Length + str1.Length, str2);
      return dest;
    }

    /// <summary>Concatenates four specified instances of <see cref="T:System.String" />.</summary>
    /// <param name="str0">The first string to concatenate.</param>
    /// <param name="str1">The second string to concatenate.</param>
    /// <param name="str2">The third string to concatenate.</param>
    /// <param name="str3">The fourth string to concatenate.</param>
    /// <returns>The concatenation of <paramref name="str0" />, <paramref name="str1" />, <paramref name="str2" />, and <paramref name="str3" />.</returns>
    public static string Concat(string? str0, string? str1, string? str2, string? str3)
    {
      if (string.IsNullOrEmpty(str0))
        return str1 + str2 + str3;
      if (string.IsNullOrEmpty(str1))
        return str0 + str2 + str3;
      if (string.IsNullOrEmpty(str2))
        return str0 + str1 + str3;
      if (string.IsNullOrEmpty(str3))
        return str0 + str1 + str2;
      string dest = string.FastAllocateString(str0.Length + str1.Length + str2.Length + str3.Length);
      string.FillStringChecked(dest, 0, str0);
      string.FillStringChecked(dest, str0.Length, str1);
      string.FillStringChecked(dest, str0.Length + str1.Length, str2);
      string.FillStringChecked(dest, str0.Length + str1.Length + str2.Length, str3);
      return dest;
    }

    /// <summary>Concatenates the string representations of two specified read-only character spans.</summary>
    /// <param name="str0">The first read-only character span to concatenate.</param>
    /// <param name="str1">The second read-only character span to concatenate.</param>
    /// <returns>The concatenated string representations of the values of <paramref name="str0" /> and <paramref name="str1" />.</returns>
    public static string Concat(ReadOnlySpan<char> str0, ReadOnlySpan<char> str1)
    {
      int length = checked (str0.Length + str1.Length);
      if (length == 0)
        return string.Empty;
      string str = string.FastAllocateString(length);
      Span<char> destination = new Span<char>(ref str._firstChar, str.Length);
      str0.CopyTo(destination);
      str1.CopyTo(destination.Slice(str0.Length));
      return str;
    }

    /// <summary>Concatenates the string representations of three specified read-only character spans.</summary>
    /// <param name="str0">The first read-only character span to concatenate.</param>
    /// <param name="str1">The second read-only character span to concatenate.</param>
    /// <param name="str2">The third read-only character span to concatenate.</param>
    /// <returns>The concatenated string representations of the values of <paramref name="str0" />, <paramref name="str1" /> and <paramref name="str2" />.</returns>
    public static string Concat(
      ReadOnlySpan<char> str0,
      ReadOnlySpan<char> str1,
      ReadOnlySpan<char> str2)
    {
      int length = checked (str0.Length + str1.Length + str2.Length);
      if (length == 0)
        return string.Empty;
      string str = string.FastAllocateString(length);
      Span<char> destination = new Span<char>(ref str._firstChar, str.Length);
      str0.CopyTo(destination);
      destination = destination.Slice(str0.Length);
      str1.CopyTo(destination);
      destination = destination.Slice(str1.Length);
      str2.CopyTo(destination);
      return str;
    }

    /// <summary>Concatenates the string representations of four specified read-only character spans.</summary>
    /// <param name="str0">The first read-only character span to concatenate.</param>
    /// <param name="str1">The second read-only character span to concatenate.</param>
    /// <param name="str2">The third read-only character span to concatenate.</param>
    /// <param name="str3">The fourth read-only character span to concatenate.</param>
    /// <returns>The concatenated string representations of the values of <paramref name="str0" />, <paramref name="str1" />, <paramref name="str2" /> and <paramref name="str3" />.</returns>
    public static string Concat(
      ReadOnlySpan<char> str0,
      ReadOnlySpan<char> str1,
      ReadOnlySpan<char> str2,
      ReadOnlySpan<char> str3)
    {
      int length = checked (str0.Length + str1.Length + str2.Length + str3.Length);
      if (length == 0)
        return string.Empty;
      string str = string.FastAllocateString(length);
      Span<char> destination1 = new Span<char>(ref str._firstChar, str.Length);
      str0.CopyTo(destination1);
      Span<char> destination2 = destination1.Slice(str0.Length);
      str1.CopyTo(destination2);
      destination2 = destination2.Slice(str1.Length);
      str2.CopyTo(destination2);
      destination2 = destination2.Slice(str2.Length);
      str3.CopyTo(destination2);
      return str;
    }

    /// <summary>Concatenates the elements of a specified <see cref="T:System.String" /> array.</summary>
    /// <param name="values">An array of string instances.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="values" /> is <see langword="null" />.</exception>
    /// <exception cref="T:System.OutOfMemoryException">Out of memory.</exception>
    /// <returns>The concatenated elements of <paramref name="values" />.</returns>
    public static string Concat(params string?[] values)
    {
      if (values == null)
        throw new ArgumentNullException(nameof (values));
      if (values.Length <= 1)
        return values.Length != 0 ? values[0] ?? string.Empty : string.Empty;
      long num = 0;
      for (int index = 0; index < values.Length; ++index)
      {
        string str = values[index];
        if (str != null)
          num += (long) str.Length;
      }
      int length1 = num <= (long) int.MaxValue ? (int) num : throw new OutOfMemoryException();
      if (length1 == 0)
        return string.Empty;
      string dest = string.FastAllocateString(length1);
      int destPos = 0;
      for (int index = 0; index < values.Length; ++index)
      {
        string src = values[index];
        if (!string.IsNullOrEmpty(src))
        {
          int length2 = src.Length;
          if (length2 > length1 - destPos)
          {
            destPos = -1;
            break;
          }
          string.FillStringChecked(dest, destPos, src);
          destPos += length2;
        }
      }
      return destPos != length1 ? string.Concat((string[]) values.Clone()) : dest;
    }

    /// <summary>Replaces one or more format items in a string with the string representation of a specified object.</summary>
    /// <param name="format">A composite format string.</param>
    /// <param name="arg0">The object to format.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="format" /> is <see langword="null" />.</exception>
    /// <exception cref="T:System.FormatException">The format item in <paramref name="format" /> is invalid.
    /// 
    /// -or-
    /// 
    /// The index of a format item is not zero.</exception>
    /// <returns>A copy of <paramref name="format" /> in which any format items are replaced by the string representation of <paramref name="arg0" />.</returns>
    public static string Format(string format, object? arg0) => string.FormatHelper((IFormatProvider) null, format, new ParamsArray(arg0));

    /// <summary>Replaces the format items in a string with the string representation of two specified objects.</summary>
    /// <param name="format">A composite format string.</param>
    /// <param name="arg0">The first object to format.</param>
    /// <param name="arg1">The second object to format.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="format" /> is <see langword="null" />.</exception>
    /// <exception cref="T:System.FormatException">
    ///        <paramref name="format" /> is invalid.
    /// 
    /// -or-
    /// 
    /// The index of a format item is not zero or one.</exception>
    /// <returns>A copy of <paramref name="format" /> in which format items are replaced by the string representations of <paramref name="arg0" /> and <paramref name="arg1" />.</returns>
    public static string Format(string format, object? arg0, object? arg1) => string.FormatHelper((IFormatProvider) null, format, new ParamsArray(arg0, arg1));

    /// <summary>Replaces the format items in a string with the string representation of three specified objects.</summary>
    /// <param name="format">A composite format string.</param>
    /// <param name="arg0">The first object to format.</param>
    /// <param name="arg1">The second object to format.</param>
    /// <param name="arg2">The third object to format.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="format" /> is <see langword="null" />.</exception>
    /// <exception cref="T:System.FormatException">
    ///        <paramref name="format" /> is invalid.
    /// 
    /// -or-
    /// 
    /// The index of a format item is less than zero, or greater than two.</exception>
    /// <returns>A copy of <paramref name="format" /> in which the format items have been replaced by the string representations of <paramref name="arg0" />, <paramref name="arg1" />, and <paramref name="arg2" />.</returns>
    public static string Format(string format, object? arg0, object? arg1, object? arg2) => string.FormatHelper((IFormatProvider) null, format, new ParamsArray(arg0, arg1, arg2));

    /// <summary>Replaces the format item in a specified string with the string representation of a corresponding object in a specified array.</summary>
    /// <param name="format">A composite format string.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="format" /> or <paramref name="args" /> is <see langword="null" />.</exception>
    /// <exception cref="T:System.FormatException">
    ///        <paramref name="format" /> is invalid.
    /// 
    /// -or-
    /// 
    /// The index of a format item is less than zero, or greater than or equal to the length of the <paramref name="args" /> array.</exception>
    /// <returns>A copy of <paramref name="format" /> in which the format items have been replaced by the string representation of the corresponding objects in <paramref name="args" />.</returns>
    public static string Format(string format, params object?[] args) => args != null ? string.FormatHelper((IFormatProvider) null, format, new ParamsArray(args)) : throw new ArgumentNullException(format == null ? nameof (format) : nameof (args));

    /// <summary>Replaces the format item or items in a specified string with the string representation of the corresponding object. A parameter supplies culture-specific formatting information.</summary>
    /// <param name="provider">An object that supplies culture-specific formatting information.</param>
    /// <param name="format">A composite format string.</param>
    /// <param name="arg0">The object to format.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="format" /> is <see langword="null" />.</exception>
    /// <exception cref="T:System.FormatException">
    ///        <paramref name="format" /> is invalid.
    /// 
    /// -or-
    /// 
    /// The index of a format item is not zero.</exception>
    /// <returns>A copy of <paramref name="format" /> in which the format item or items have been replaced by the string representation of <paramref name="arg0" />.</returns>
    public static string Format(IFormatProvider? provider, string format, object? arg0) => string.FormatHelper(provider, format, new ParamsArray(arg0));

    /// <summary>Replaces the format items in a string with the string representation of two specified objects. A parameter supplies culture-specific formatting information.</summary>
    /// <param name="provider">An object that supplies culture-specific formatting information.</param>
    /// <param name="format">A composite format string.</param>
    /// <param name="arg0">The first object to format.</param>
    /// <param name="arg1">The second object to format.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="format" /> is <see langword="null" />.</exception>
    /// <exception cref="T:System.FormatException">
    ///        <paramref name="format" /> is invalid.
    /// 
    /// -or-
    /// 
    /// The index of a format item is not zero or one.</exception>
    /// <returns>A copy of <paramref name="format" /> in which format items are replaced by the string representations of <paramref name="arg0" /> and <paramref name="arg1" />.</returns>
    public static string Format(IFormatProvider? provider, string format, object? arg0, object? arg1) => string.FormatHelper(provider, format, new ParamsArray(arg0, arg1));

    /// <summary>Replaces the format items in a string with the string representation of three specified objects. An parameter supplies culture-specific formatting information.</summary>
    /// <param name="provider">An object that supplies culture-specific formatting information.</param>
    /// <param name="format">A composite format string.</param>
    /// <param name="arg0">The first object to format.</param>
    /// <param name="arg1">The second object to format.</param>
    /// <param name="arg2">The third object to format.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="format" /> is <see langword="null" />.</exception>
    /// <exception cref="T:System.FormatException">
    ///        <paramref name="format" /> is invalid.
    /// 
    /// -or-
    /// 
    /// The index of a format item is less than zero, or greater than two.</exception>
    /// <returns>A copy of <paramref name="format" /> in which the format items have been replaced by the string representations of <paramref name="arg0" />, <paramref name="arg1" />, and <paramref name="arg2" />.</returns>
    public static string Format(
      IFormatProvider? provider,
      string format,
      object? arg0,
      object? arg1,
      object? arg2)
    {
      return string.FormatHelper(provider, format, new ParamsArray(arg0, arg1, arg2));
    }

    /// <summary>Replaces the format items in a string with the string representations of corresponding objects in a specified array. A parameter supplies culture-specific formatting information.</summary>
    /// <param name="provider">An object that supplies culture-specific formatting information.</param>
    /// <param name="format">A composite format string.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="format" /> or <paramref name="args" /> is <see langword="null" />.</exception>
    /// <exception cref="T:System.FormatException">
    ///        <paramref name="format" /> is invalid.
    /// 
    /// -or-
    /// 
    /// The index of a format item is less than zero, or greater than or equal to the length of the <paramref name="args" /> array.</exception>
    /// <returns>A copy of <paramref name="format" /> in which the format items have been replaced by the string representation of the corresponding objects in <paramref name="args" />.</returns>
    public static string Format(IFormatProvider? provider, string format, params object?[] args)
    {
      if (args == null)
        throw new ArgumentNullException(format == null ? nameof (format) : nameof (args));
      return string.FormatHelper(provider, format, new ParamsArray(args));
    }


    #nullable disable
    private static unsafe string FormatHelper(
      IFormatProvider provider,
      string format,
      ParamsArray args)
    {
      if (format == null)
        throw new ArgumentNullException(nameof (format));
      // ISSUE: untyped stack allocation
      ValueStringBuilder valueStringBuilder = new ValueStringBuilder(new Span<char>((void*) __untypedstackalloc(new IntPtr(512)), 256));
      valueStringBuilder.EnsureCapacity(format.Length + args.Length * 8);
      valueStringBuilder.AppendFormatHelper(provider, format, args);
      return valueStringBuilder.ToString();
    }


    #nullable enable
    /// <summary>Returns a new string in which a specified string is inserted at a specified index position in this instance.</summary>
    /// <param name="startIndex">The zero-based index position of the insertion.</param>
    /// <param name="value">The string to insert.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="value" /> is <see langword="null" />.</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    /// <paramref name="startIndex" /> is negative or greater than the length of this instance.</exception>
    /// <returns>A new string that is equivalent to this instance, but with <paramref name="value" /> inserted at position <paramref name="startIndex" />.</returns>
    public string Insert(int startIndex, string value)
    {
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      int num = (long) (uint) startIndex <= (long) this.Length ? this.Length : throw new ArgumentOutOfRangeException(nameof (startIndex));
      int length = value.Length;
      if (num == 0)
        return value;
      if (length == 0)
        return this;
      string str = string.FastAllocateString(num + length);
      Buffer.Memmove<char>(ref str._firstChar, ref this._firstChar, (UIntPtr) startIndex);
      Buffer.Memmove<char>(ref Unsafe.Add<char>(ref str._firstChar, startIndex), ref value._firstChar, (UIntPtr) length);
      Buffer.Memmove<char>(ref Unsafe.Add<char>(ref str._firstChar, startIndex + length), ref Unsafe.Add<char>(ref this._firstChar, startIndex), (UIntPtr) (num - startIndex));
      return str;
    }

    /// <summary>Concatenates an array of strings, using the specified separator between each member.</summary>
    /// <param name="separator">The character to use as a separator. <paramref name="separator" /> is included in the returned string only if <paramref name="value" /> has more than one element.</param>
    /// <param name="value">An array of strings to concatenate.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="value" /> is <see langword="null" />.</exception>
    /// <exception cref="T:System.OutOfMemoryException">The length of the resulting string overflows the maximum allowed length (<see cref="F:System.Int32.MaxValue" />).</exception>
    /// <returns>A string that consists of the elements of <paramref name="value" /> delimited by the <paramref name="separator" /> character.
    /// 
    /// -or-
    /// 
    /// <see cref="F:System.String.Empty" /> if <paramref name="value" /> has zero elements.</returns>
    public static string Join(char separator, params string?[] value)
    {
      if (value == null)
        ThrowHelper.ThrowArgumentNullException(ExceptionArgument.value);
      return string.JoinCore(MemoryMarshal.CreateReadOnlySpan<char>(ref separator, 1), new ReadOnlySpan<string>(value));
    }

    /// <summary>Concatenates all the elements of a string array, using the specified separator between each element.</summary>
    /// <param name="separator">The string to use as a separator. <paramref name="separator" /> is included in the returned string only if <paramref name="value" /> has more than one element.</param>
    /// <param name="value">An array that contains the elements to concatenate.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="value" /> is <see langword="null" />.</exception>
    /// <exception cref="T:System.OutOfMemoryException">The length of the resulting string overflows the maximum allowed length (<see cref="F:System.Int32.MaxValue" />).</exception>
    /// <returns>A string that consists of the elements in <paramref name="value" /> delimited by the <paramref name="separator" /> string.
    /// 
    /// -or-
    /// 
    /// <see cref="F:System.String.Empty" /> if <paramref name="values" /> has zero elements.</returns>
    public static string Join(string? separator, params string?[] value)
    {
      if (value == null)
        ThrowHelper.ThrowArgumentNullException(ExceptionArgument.value);
      return string.JoinCore(separator.AsSpan(), new ReadOnlySpan<string>(value));
    }

    /// <summary>Concatenates an array of strings, using the specified separator between each member, starting with the element in <paramref name="value" /> located at the <paramref name="startIndex" /> position, and concatenating up to <paramref name="count" /> elements.</summary>
    /// <param name="separator">Concatenates an array of strings, using the specified separator between each member, starting with the element located at the specified index and including a specified number of elements.</param>
    /// <param name="value">An array of strings to concatenate.</param>
    /// <param name="startIndex">The first item in <paramref name="value" /> to concatenate.</param>
    /// <param name="count">The number of elements from <paramref name="value" /> to concatenate, starting with the element in the <paramref name="startIndex" /> position.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="value" /> is <see langword="null" />.</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    ///         <paramref name="startIndex" /> or <paramref name="count" /> are negative.
    /// 
    /// -or-
    /// 
    /// <paramref name="startIndex" /> is greater than the length of <paramref name="value" />  - <paramref name="count" />.</exception>
    /// <exception cref="T:System.OutOfMemoryException">The length of the resulting string overflows the maximum allowed length (<see cref="F:System.Int32.MaxValue" />).</exception>
    /// <returns>A string that consists of <paramref name="count" /> elements of <paramref name="value" /> starting at <paramref name="startIndex" /> delimited by the <paramref name="separator" /> character.
    /// 
    /// -or-
    /// 
    /// <see cref="F:System.String.Empty" /> if <paramref name="count" /> is zero.</returns>
    public static string Join(char separator, string?[] value, int startIndex, int count) => string.JoinCore(MemoryMarshal.CreateReadOnlySpan<char>(ref separator, 1), value, startIndex, count);

    /// <summary>Concatenates the specified elements of a string array, using the specified separator between each element.</summary>
    /// <param name="separator">The string to use as a separator. <paramref name="separator" /> is included in the returned string only if <paramref name="value" /> has more than one element.</param>
    /// <param name="value">An array that contains the elements to concatenate.</param>
    /// <param name="startIndex">The first element in <paramref name="value" /> to use.</param>
    /// <param name="count">The number of elements of <paramref name="value" /> to use.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="value" /> is <see langword="null" />.</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    ///         <paramref name="startIndex" /> or <paramref name="count" /> is less than 0.
    /// 
    /// -or-
    /// 
    /// <paramref name="startIndex" /> plus <paramref name="count" /> is greater than the number of elements in <paramref name="value" />.</exception>
    /// <exception cref="T:System.OutOfMemoryException">Out of memory.</exception>
    /// <returns>A string that consists of <paramref name="count" /> elements of <paramref name="value" /> starting at <paramref name="startIndex" /> delimited by the <paramref name="separator" /> character.
    /// 
    /// -or-
    /// 
    /// <see cref="F:System.String.Empty" /> if <paramref name="count" /> is zero.</returns>
    public static string Join(string? separator, string?[] value, int startIndex, int count) => string.JoinCore(separator.AsSpan(), value, startIndex, count);


    #nullable disable
    private static string JoinCore(
      ReadOnlySpan<char> separator,
      string[] value,
      int startIndex,
      int count)
    {
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      if (startIndex < 0)
        throw new ArgumentOutOfRangeException(nameof (startIndex), SR.ArgumentOutOfRange_StartIndex);
      if (count < 0)
        throw new ArgumentOutOfRangeException(nameof (count), SR.ArgumentOutOfRange_NegativeCount);
      if (startIndex > value.Length - count)
        throw new ArgumentOutOfRangeException(nameof (startIndex), SR.ArgumentOutOfRange_IndexCountBuffer);
      return string.JoinCore(separator, new ReadOnlySpan<string>(value, startIndex, count));
    }


    #nullable enable
    /// <summary>Concatenates the members of a constructed <see cref="T:System.Collections.Generic.IEnumerable`1" /> collection of type <see cref="T:System.String" />, using the specified separator between each member.</summary>
    /// <param name="separator">The string to use as a separator.<paramref name="separator" /> is included in the returned string only if <paramref name="values" /> has more than one element.</param>
    /// <param name="values">A collection that contains the strings to concatenate.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="values" /> is <see langword="null" />.</exception>
    /// <exception cref="T:System.OutOfMemoryException">The length of the resulting string overflows the maximum allowed length (<see cref="F:System.Int32.MaxValue" />).</exception>
    /// <returns>A string that consists of the elements of <paramref name="values" /> delimited by the <paramref name="separator" /> string.
    /// 
    /// -or-
    /// 
    /// <see cref="F:System.String.Empty" /> if <paramref name="values" /> has zero elements.</returns>
    public static unsafe string Join(string? separator, IEnumerable<string?> values)
    {
      switch (values)
      {
        case List<string> list:
          return string.JoinCore(separator.AsSpan(), (ReadOnlySpan<string>) CollectionsMarshal.AsSpan<string>(list));
        case string[] array:
          return string.JoinCore(separator.AsSpan(), new ReadOnlySpan<string>(array));
        case null:
          ThrowHelper.ThrowArgumentNullException(ExceptionArgument.values);
          break;
      }
      using (IEnumerator<string> enumerator = values.GetEnumerator())
      {
        if (!enumerator.MoveNext())
          return string.Empty;
        string current = enumerator.Current;
        if (!enumerator.MoveNext())
          return current ?? string.Empty;
        // ISSUE: untyped stack allocation
        ValueStringBuilder valueStringBuilder = new ValueStringBuilder(new Span<char>((void*) __untypedstackalloc(new IntPtr(512)), 256));
        valueStringBuilder.Append(current);
        do
        {
          valueStringBuilder.Append(separator);
          valueStringBuilder.Append(enumerator.Current);
        }
        while (enumerator.MoveNext());
        return valueStringBuilder.ToString();
      }
    }

    /// <summary>Concatenates the string representations of an array of objects, using the specified separator between each member.</summary>
    /// <param name="separator">The character to use as a separator. <paramref name="separator" /> is included in the returned string only if <paramref name="value" /> has more than one element.</param>
    /// <param name="values">An array of objects whose string representations will be concatenated.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="value" /> is <see langword="null" />.</exception>
    /// <exception cref="T:System.OutOfMemoryException">The length of the resulting string overflows the maximum allowed length (<see cref="F:System.Int32.MaxValue" />).</exception>
    /// <returns>A string that consists of the elements of <paramref name="values" /> delimited by the <paramref name="separator" /> character.
    /// 
    /// -or-
    /// 
    /// <see cref="F:System.String.Empty" /> if <paramref name="values" /> has zero elements.</returns>
    public static string Join(char separator, params object?[] values) => string.JoinCore(MemoryMarshal.CreateReadOnlySpan<char>(ref separator, 1), values);

    /// <summary>Concatenates the elements of an object array, using the specified separator between each element.</summary>
    /// <param name="separator">The string to use as a separator. <paramref name="separator" /> is included in the returned string only if <paramref name="values" /> has more than one element.</param>
    /// <param name="values">An array that contains the elements to concatenate.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="values" /> is <see langword="null" />.</exception>
    /// <exception cref="T:System.OutOfMemoryException">The length of the resulting string overflows the maximum allowed length (<see cref="F:System.Int32.MaxValue" />).</exception>
    /// <returns>A string that consists of the elements of <paramref name="values" /> delimited by the <paramref name="separator" /> string.
    /// 
    /// -or-
    /// 
    /// <see cref="F:System.String.Empty" /> if <paramref name="values" /> has zero elements.
    /// 
    /// -or-
    /// 
    /// .NET Framework only: <see cref="F:System.String.Empty" /> if the first element of <paramref name="values" /> is <see langword="null" />.</returns>
    public static string Join(string? separator, params object?[] values) => string.JoinCore(separator.AsSpan(), values);


    #nullable disable
    private static unsafe string JoinCore(ReadOnlySpan<char> separator, object[] values)
    {
      if (values == null)
        ThrowHelper.ThrowArgumentNullException(ExceptionArgument.values);
      if (values.Length == 0)
        return string.Empty;
      string s = values[0]?.ToString();
      if (values.Length == 1)
        return s ?? string.Empty;
      // ISSUE: untyped stack allocation
      ValueStringBuilder valueStringBuilder = new ValueStringBuilder(new Span<char>((void*) __untypedstackalloc(new IntPtr(512)), 256));
      valueStringBuilder.Append(s);
      for (int index = 1; index < values.Length; ++index)
      {
        valueStringBuilder.Append(separator);
        object obj = values[index];
        if (obj != null)
          valueStringBuilder.Append(obj.ToString());
      }
      return valueStringBuilder.ToString();
    }


    #nullable enable
    /// <summary>Concatenates the members of a collection, using the specified separator between each member.</summary>
    /// <param name="separator">The character to use as a separator. <paramref name="separator" /> is included in the returned string only if <paramref name="values" /> has more than one element.</param>
    /// <param name="values">A collection that contains the objects to concatenate.</param>
    /// <typeparam name="T">The type of the members of <paramref name="values" />.</typeparam>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="values" /> is <see langword="null" />.</exception>
    /// <exception cref="T:System.OutOfMemoryException">The length of the resulting string overflows the maximum allowed length (<see cref="F:System.Int32.MaxValue" />).</exception>
    /// <returns>A string that consists of the members of <paramref name="values" /> delimited by the <paramref name="separator" /> character.
    /// 
    /// -or-
    /// 
    /// <see cref="F:System.String.Empty" /> if <paramref name="values" /> has no elements.</returns>
    public static string Join<T>(char separator, IEnumerable<T> values) => string.JoinCore<T>(MemoryMarshal.CreateReadOnlySpan<char>(ref separator, 1), values);

    /// <summary>Concatenates the members of a collection, using the specified separator between each member.</summary>
    /// <param name="separator">The string to use as a separator. <paramref name="separator" /> is included in the returned string only if <paramref name="values" /> has more than one element.</param>
    /// <param name="values">A collection that contains the objects to concatenate.</param>
    /// <typeparam name="T">The type of the members of <paramref name="values" />.</typeparam>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="values" /> is <see langword="null" />.</exception>
    /// <exception cref="T:System.OutOfMemoryException">The length of the resulting string overflows the maximum allowed length (<see cref="F:System.Int32.MaxValue" />).</exception>
    /// <returns>A string that consists of the elements of <paramref name="values" /> delimited by the <paramref name="separator" /> string.
    /// 
    /// -or-
    /// 
    /// <see cref="F:System.String.Empty" /> if <paramref name="values" /> has no elements.</returns>
    public static string Join<T>(string? separator, IEnumerable<T> values) => string.JoinCore<T>(separator.AsSpan(), values);


    #nullable disable
    private static unsafe string JoinCore<T>(ReadOnlySpan<char> separator, IEnumerable<T> values)
    {
      if (typeof (T) == typeof (string))
      {
        if (values is List<string> list)
          return string.JoinCore(separator, (ReadOnlySpan<string>) CollectionsMarshal.AsSpan<string>(list));
        if (values is string[] array)
          return string.JoinCore(separator, new ReadOnlySpan<string>(array));
      }
      if (values == null)
        ThrowHelper.ThrowArgumentNullException(ExceptionArgument.values);
      using (IEnumerator<T> enumerator = values.GetEnumerator())
      {
        if (!enumerator.MoveNext())
          return string.Empty;
        T current = enumerator.Current;
        string s = current?.ToString();
        if (!enumerator.MoveNext())
          return s ?? string.Empty;
        // ISSUE: untyped stack allocation
        ValueStringBuilder valueStringBuilder = new ValueStringBuilder(new Span<char>((void*) __untypedstackalloc(new IntPtr(512)), 256));
        valueStringBuilder.Append(s);
        do
        {
          current = enumerator.Current;
          valueStringBuilder.Append(separator);
          if ((object) current != null)
            valueStringBuilder.Append(current.ToString());
        }
        while (enumerator.MoveNext());
        return valueStringBuilder.ToString();
      }
    }

    private static string JoinCore(ReadOnlySpan<char> separator, ReadOnlySpan<string> values)
    {
      if (values.Length <= 1)
        return !values.IsEmpty ? values[0] ?? string.Empty : string.Empty;
      long num1 = (long) (values.Length - 1) * (long) separator.Length;
      if (num1 > (long) int.MaxValue)
        ThrowHelper.ThrowOutOfMemoryException();
      int length1 = (int) num1;
      ReadOnlySpan<string> readOnlySpan = values;
      for (int index = 0; index < readOnlySpan.Length; ++index)
      {
        string str = readOnlySpan[index];
        if (str != null)
        {
          length1 += str.Length;
          if (length1 < 0)
            ThrowHelper.ThrowOutOfMemoryException();
        }
      }
      string dest = string.FastAllocateString(length1);
      int num2 = 0;
      for (int index = 0; index < values.Length; ++index)
      {
        string src = values[index];
        if (src != null)
        {
          int length2 = src.Length;
          if (length2 > length1 - num2)
          {
            num2 = -1;
            break;
          }
          string.FillStringChecked(dest, num2, src);
          num2 += length2;
        }
        if (index < values.Length - 1)
        {
          ref char local = ref Unsafe.Add<char>(ref dest._firstChar, num2);
          if (separator.Length == 1)
            local = separator[0];
          else
            separator.CopyTo(new Span<char>(ref local, separator.Length));
          num2 += separator.Length;
        }
      }
      return num2 != length1 ? string.JoinCore(separator, (ReadOnlySpan<string>) values.ToArray().AsSpan<string>()) : dest;
    }


    #nullable enable
    /// <summary>Returns a new string that right-aligns the characters in this instance by padding them with spaces on the left, for a specified total length.</summary>
    /// <param name="totalWidth">The number of characters in the resulting string, equal to the number of original characters plus any additional padding characters.</param>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    /// <paramref name="totalWidth" /> is less than zero.</exception>
    /// <returns>A new string that is equivalent to this instance, but right-aligned and padded on the left with as many spaces as needed to create a length of <paramref name="totalWidth" />. However, if <paramref name="totalWidth" /> is less than the length of this instance, the method returns a reference to the existing instance. If <paramref name="totalWidth" /> is equal to the length of this instance, the method returns a new string that is identical to this instance.</returns>
    public string PadLeft(int totalWidth) => this.PadLeft(totalWidth, ' ');

    /// <summary>Returns a new string that right-aligns the characters in this instance by padding them on the left with a specified Unicode character, for a specified total length.</summary>
    /// <param name="totalWidth">The number of characters in the resulting string, equal to the number of original characters plus any additional padding characters.</param>
    /// <param name="paddingChar">A Unicode padding character.</param>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    /// <paramref name="totalWidth" /> is less than zero.</exception>
    /// <returns>A new string that is equivalent to this instance, but right-aligned and padded on the left with as many <paramref name="paddingChar" /> characters as needed to create a length of <paramref name="totalWidth" />. However, if <paramref name="totalWidth" /> is less than the length of this instance, the method returns a reference to the existing instance. If <paramref name="totalWidth" /> is equal to the length of this instance, the method returns a new string that is identical to this instance.</returns>
    public string PadLeft(int totalWidth, char paddingChar)
    {
      if (totalWidth < 0)
        throw new ArgumentOutOfRangeException(nameof (totalWidth), SR.ArgumentOutOfRange_NeedNonNegNum);
      int length = this.Length;
      int num = totalWidth - length;
      if (num <= 0)
        return this;
      string str = string.FastAllocateString(totalWidth);
      new Span<char>(ref str._firstChar, num).Fill(paddingChar);
      Buffer.Memmove<char>(ref Unsafe.Add<char>(ref str._firstChar, num), ref this._firstChar, (UIntPtr) length);
      return str;
    }

    /// <summary>Returns a new string that left-aligns the characters in this string by padding them with spaces on the right, for a specified total length.</summary>
    /// <param name="totalWidth">The number of characters in the resulting string, equal to the number of original characters plus any additional padding characters.</param>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    /// <paramref name="totalWidth" /> is less than zero.</exception>
    /// <returns>A new string that is equivalent to this instance, but left-aligned and padded on the right with as many spaces as needed to create a length of <paramref name="totalWidth" />. However, if <paramref name="totalWidth" /> is less than the length of this instance, the method returns a reference to the existing instance. If <paramref name="totalWidth" /> is equal to the length of this instance, the method returns a new string that is identical to this instance.</returns>
    public string PadRight(int totalWidth) => this.PadRight(totalWidth, ' ');

    /// <summary>Returns a new string that left-aligns the characters in this string by padding them on the right with a specified Unicode character, for a specified total length.</summary>
    /// <param name="totalWidth">The number of characters in the resulting string, equal to the number of original characters plus any additional padding characters.</param>
    /// <param name="paddingChar">A Unicode padding character.</param>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    /// <paramref name="totalWidth" /> is less than zero.</exception>
    /// <returns>A new string that is equivalent to this instance, but left-aligned and padded on the right with as many <paramref name="paddingChar" /> characters as needed to create a length of <paramref name="totalWidth" />. However, if <paramref name="totalWidth" /> is less than the length of this instance, the method returns a reference to the existing instance. If <paramref name="totalWidth" /> is equal to the length of this instance, the method returns a new string that is identical to this instance.</returns>
    public string PadRight(int totalWidth, char paddingChar)
    {
      if (totalWidth < 0)
        throw new ArgumentOutOfRangeException(nameof (totalWidth), SR.ArgumentOutOfRange_NeedNonNegNum);
      int length1 = this.Length;
      int length2 = totalWidth - length1;
      if (length2 <= 0)
        return this;
      string str = string.FastAllocateString(totalWidth);
      Buffer.Memmove<char>(ref str._firstChar, ref this._firstChar, (UIntPtr) length1);
      new Span<char>(ref Unsafe.Add<char>(ref str._firstChar, length1), length2).Fill(paddingChar);
      return str;
    }

    /// <summary>Returns a new string in which a specified number of characters in the current instance beginning at a specified position have been deleted.</summary>
    /// <param name="startIndex">The zero-based position to begin deleting characters.</param>
    /// <param name="count">The number of characters to delete.</param>
    /// <exception cref="T:System.ArgumentOutOfRangeException">Either <paramref name="startIndex" /> or <paramref name="count" /> is less than zero.
    /// 
    /// -or-
    /// 
    /// <paramref name="startIndex" /> plus <paramref name="count" /> specify a position outside this instance.</exception>
    /// <returns>A new string that is equivalent to this instance except for the removed characters.</returns>
    public string Remove(int startIndex, int count)
    {
      if (startIndex < 0)
        throw new ArgumentOutOfRangeException(nameof (startIndex), SR.ArgumentOutOfRange_StartIndex);
      if (count < 0)
        throw new ArgumentOutOfRangeException(nameof (count), SR.ArgumentOutOfRange_NegativeCount);
      int length1 = this.Length;
      if (count > length1 - startIndex)
        throw new ArgumentOutOfRangeException(nameof (count), SR.ArgumentOutOfRange_IndexCount);
      if (count == 0)
        return this;
      int length2 = length1 - count;
      if (length2 == 0)
        return string.Empty;
      string str = string.FastAllocateString(length2);
      Buffer.Memmove<char>(ref str._firstChar, ref this._firstChar, (UIntPtr) startIndex);
      Buffer.Memmove<char>(ref Unsafe.Add<char>(ref str._firstChar, startIndex), ref Unsafe.Add<char>(ref this._firstChar, startIndex + count), (UIntPtr) (length2 - startIndex));
      return str;
    }

    /// <summary>Returns a new string in which all the characters in the current instance, beginning at a specified position and continuing through the last position, have been deleted.</summary>
    /// <param name="startIndex">The zero-based position to begin deleting characters.</param>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    ///        <paramref name="startIndex" /> is less than zero.
    /// 
    /// -or-
    /// 
    /// <paramref name="startIndex" /> specifies a position that is not within this string.</exception>
    /// <returns>A new string that is equivalent to this string except for the removed characters.</returns>
    public string Remove(int startIndex) => (long) (uint) startIndex <= (long) this.Length ? this.Substring(0, startIndex) : throw new ArgumentOutOfRangeException(nameof (startIndex), startIndex < 0 ? SR.ArgumentOutOfRange_StartIndex : SR.ArgumentOutOfRange_StartIndexLargerThanLength);

    /// <summary>Returns a new string in which all occurrences of a specified string in the current instance are replaced with another specified string, using the provided culture and case sensitivity.</summary>
    /// <param name="oldValue">The string to be replaced.</param>
    /// <param name="newValue">The string to replace all occurrences of <paramref name="oldValue" />.</param>
    /// <param name="ignoreCase">
    /// <see langword="true" /> to ignore casing when comparing; <see langword="false" /> otherwise.</param>
    /// <param name="culture">The culture to use when comparing. If <paramref name="culture" /> is <see langword="null" />, the current culture is used.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="oldValue" /> is <see langword="null" />.</exception>
    /// <exception cref="T:System.ArgumentException">
    /// <paramref name="oldValue" /> is the empty string ("").</exception>
    /// <returns>A string that is equivalent to the current string except that all instances of <paramref name="oldValue" /> are replaced with <paramref name="newValue" />. If <paramref name="oldValue" /> is not found in the current instance, the method returns the current instance unchanged.</returns>
    public string Replace(string oldValue, string? newValue, bool ignoreCase, CultureInfo? culture) => this.ReplaceCore(oldValue, newValue, culture?.CompareInfo, ignoreCase ? CompareOptions.IgnoreCase : CompareOptions.None);

    /// <summary>Returns a new string in which all occurrences of a specified string in the current instance are replaced with another specified string, using the provided comparison type.</summary>
    /// <param name="oldValue">The string to be replaced.</param>
    /// <param name="newValue">The string to replace all occurrences of <paramref name="oldValue" />.</param>
    /// <param name="comparisonType">One of the enumeration values that determines how <paramref name="oldValue" /> is searched within this instance.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="oldValue" /> is <see langword="null" />.</exception>
    /// <exception cref="T:System.ArgumentException">
    /// <paramref name="oldValue" /> is the empty string ("").</exception>
    /// <returns>A string that is equivalent to the current string except that all instances of <paramref name="oldValue" /> are replaced with <paramref name="newValue" />. If <paramref name="oldValue" /> is not found in the current instance, the method returns the current instance unchanged.</returns>
    public string Replace(string oldValue, string? newValue, StringComparison comparisonType)
    {
      switch (comparisonType)
      {
        case StringComparison.CurrentCulture:
        case StringComparison.CurrentCultureIgnoreCase:
          return this.ReplaceCore(oldValue, newValue, CultureInfo.CurrentCulture.CompareInfo, string.GetCaseCompareOfComparisonCulture(comparisonType));
        case StringComparison.InvariantCulture:
        case StringComparison.InvariantCultureIgnoreCase:
          return this.ReplaceCore(oldValue, newValue, CompareInfo.Invariant, string.GetCaseCompareOfComparisonCulture(comparisonType));
        case StringComparison.Ordinal:
          return this.Replace(oldValue, newValue);
        case StringComparison.OrdinalIgnoreCase:
          return this.ReplaceCore(oldValue, newValue, CompareInfo.Invariant, CompareOptions.OrdinalIgnoreCase);
        default:
          throw new ArgumentException(SR.NotSupported_StringComparison, nameof (comparisonType));
      }
    }


    #nullable disable
    private string ReplaceCore(
      string oldValue,
      string newValue,
      CompareInfo ci,
      CompareOptions options)
    {
      switch (oldValue)
      {
        case "":
          throw new ArgumentException(SR.Argument_StringZeroLength, nameof (oldValue));
        case null:
          throw new ArgumentNullException(nameof (oldValue));
        default:
          return string.ReplaceCore((ReadOnlySpan<char>) this, oldValue.AsSpan(), newValue.AsSpan(), ci ?? CultureInfo.CurrentCulture.CompareInfo, options) ?? this;
      }
    }

    private static unsafe string ReplaceCore(
      ReadOnlySpan<char> searchSpace,
      ReadOnlySpan<char> oldValue,
      ReadOnlySpan<char> newValue,
      CompareInfo compareInfo,
      CompareOptions options)
    {
      // ISSUE: untyped stack allocation
      ValueStringBuilder valueStringBuilder = new ValueStringBuilder(new Span<char>((void*) __untypedstackalloc(new IntPtr(512)), 256));
      valueStringBuilder.EnsureCapacity(searchSpace.Length);
      bool flag = false;
      while (true)
      {
        int matchLength;
        int length = compareInfo.IndexOf(searchSpace, oldValue, options, out matchLength);
        if (length >= 0 && matchLength != 0)
        {
          valueStringBuilder.Append(searchSpace.Slice(0, length));
          valueStringBuilder.Append(newValue);
          searchSpace = searchSpace.Slice(length + matchLength);
          flag = true;
        }
        else
          break;
      }
      if (!flag)
      {
        valueStringBuilder.Dispose();
        return (string) null;
      }
      valueStringBuilder.Append(searchSpace);
      return valueStringBuilder.ToString();
    }


    #nullable enable
    /// <summary>Returns a new string in which all occurrences of a specified Unicode character in this instance are replaced with another specified Unicode character.</summary>
    /// <param name="oldChar">The Unicode character to be replaced.</param>
    /// <param name="newChar">The Unicode character to replace all occurrences of <paramref name="oldChar" />.</param>
    /// <returns>A string that is equivalent to this instance except that all instances of <paramref name="oldChar" /> are replaced with <paramref name="newChar" />. If <paramref name="oldChar" /> is not found in the current instance, the method returns the current instance unchanged.</returns>
    public string Replace(char oldChar, char newChar)
    {
      if ((int) oldChar == (int) newChar)
        return this;
      int num1 = this.IndexOf(oldChar);
      if (num1 < 0)
        return this;
      int num2 = this.Length - num1;
      string str = string.FastAllocateString(this.Length);
      int num3 = num1;
      if (num3 > 0)
        Buffer.Memmove<char>(ref str._firstChar, ref this._firstChar, (UIntPtr) (uint) num3);
      ref ushort local1 = ref Unsafe.Add<ushort>(ref Unsafe.As<char, ushort>(ref this._firstChar), num3);
      ref ushort local2 = ref Unsafe.Add<ushort>(ref Unsafe.As<char, ushort>(ref str._firstChar), num3);
      if (Vector.IsHardwareAccelerated && num2 >= Vector<ushort>.Count)
      {
        Vector<ushort> right = new Vector<ushort>((ushort) oldChar);
        Vector<ushort> left = new Vector<ushort>((ushort) newChar);
        do
        {
          Vector<ushort> vector1 = Unsafe.ReadUnaligned<Vector<ushort>>(ref Unsafe.As<ushort, byte>(ref local1));
          Vector<ushort> vector2 = Vector.ConditionalSelect<ushort>(Vector.Equals<ushort>(vector1, right), left, vector1);
          Unsafe.WriteUnaligned<Vector<ushort>>(ref Unsafe.As<ushort, byte>(ref local2), vector2);
          local1 = ref Unsafe.Add<ushort>(ref local1, Vector<ushort>.Count);
          local2 = ref Unsafe.Add<ushort>(ref local2, Vector<ushort>.Count);
          num2 -= Vector<ushort>.Count;
        }
        while (num2 >= Vector<ushort>.Count);
      }
      for (; num2 > 0; --num2)
      {
        ushort num4 = local1;
        local2 = (int) num4 == (int) oldChar ? (ushort) newChar : num4;
        local1 = ref Unsafe.Add<ushort>(ref local1, 1);
        local2 = ref Unsafe.Add<ushort>(ref local2, 1);
      }
      return str;
    }

    /// <summary>Returns a new string in which all occurrences of a specified string in the current instance are replaced with another specified string.</summary>
    /// <param name="oldValue">The string to be replaced.</param>
    /// <param name="newValue">The string to replace all occurrences of <paramref name="oldValue" />.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="oldValue" /> is <see langword="null" />.</exception>
    /// <exception cref="T:System.ArgumentException">
    /// <paramref name="oldValue" /> is the empty string ("").</exception>
    /// <returns>A string that is equivalent to the current string except that all instances of <paramref name="oldValue" /> are replaced with <paramref name="newValue" />. If <paramref name="oldValue" /> is not found in the current instance, the method returns the current instance unchanged.</returns>
    public unsafe string Replace(string oldValue, string? newValue)
    {
      switch (oldValue)
      {
        case "":
          throw new ArgumentException(SR.Argument_StringZeroLength, nameof (oldValue));
        case null:
          throw new ArgumentNullException(nameof (oldValue));
        default:
          if (newValue == null)
            newValue = string.Empty;
          // ISSUE: untyped stack allocation
          ValueListBuilder<int> valueListBuilder = new ValueListBuilder<int>(new Span<int>((void*) __untypedstackalloc(new IntPtr(512)), 128));
          if (oldValue.Length == 1)
          {
            if (newValue.Length == 1)
              return this.Replace(oldValue[0], newValue[0]);
            char ch = oldValue[0];
            int elementOffset = 0;
            while (true)
            {
              int num = SpanHelpers.IndexOf(ref Unsafe.Add<char>(ref this._firstChar, elementOffset), ch, this.Length - elementOffset);
              if (num != -1)
              {
                valueListBuilder.Append(elementOffset + num);
                elementOffset += num + 1;
              }
              else
                break;
            }
          }
          else
          {
            int elementOffset = 0;
            while (true)
            {
              int num = SpanHelpers.IndexOf(ref Unsafe.Add<char>(ref this._firstChar, elementOffset), this.Length - elementOffset, ref oldValue._firstChar, oldValue.Length);
              if (num != -1)
              {
                valueListBuilder.Append(elementOffset + num);
                elementOffset += num + oldValue.Length;
              }
              else
                break;
            }
          }
          if (valueListBuilder.Length == 0)
            return this;
          string str = this.ReplaceHelper(oldValue.Length, newValue, valueListBuilder.AsSpan());
          valueListBuilder.Dispose();
          return str;
      }
    }


    #nullable disable
    private string ReplaceHelper(int oldValueLength, string newValue, ReadOnlySpan<int> indices)
    {
      long length1 = (long) this.Length + (long) (newValue.Length - oldValueLength) * (long) indices.Length;
      string str = length1 <= (long) int.MaxValue ? string.FastAllocateString((int) length1) : throw new OutOfMemoryException();
      Span<char> span = new Span<char>(ref str._firstChar, str.Length);
      int start1 = 0;
      int start2 = 0;
      for (int index = 0; index < indices.Length; ++index)
      {
        int num = indices[index];
        int length2 = num - start1;
        if (length2 != 0)
        {
          this.AsSpan(start1, length2).CopyTo(span.Slice(start2));
          start2 += length2;
        }
        start1 = num + oldValueLength;
        newValue.CopyTo(span.Slice(start2));
        start2 += newValue.Length;
      }
      this.AsSpan(start1).CopyTo(span.Slice(start2));
      return str;
    }


    #nullable enable
    /// <summary>Replaces all newline sequences in the current string with <see cref="P:System.Environment.NewLine" />.</summary>
    /// <returns>A string whose contents match the current string, but with all newline sequences replaced with <see cref="P:System.Environment.NewLine" />.</returns>
    public string ReplaceLineEndings() => this.ReplaceLineEndings("\n");

    /// <summary>Replaces all newline sequences in the current string with <paramref name="replacementText" />.</summary>
    /// <param name="replacementText">The text to use as replacement.</param>
    /// <returns>A string whose contents match the current string, but with all newline sequences replaced with <paramref name="replacementText" />.</returns>
    public unsafe string ReplaceLineEndings(string replacementText)
    {
      if (replacementText == null)
        throw new ArgumentNullException(nameof (replacementText));
      int stride;
      int length1 = string.IndexOfNewlineChar((ReadOnlySpan<char>) this, out stride);
      if (length1 < 0)
        return this;
      ReadOnlySpan<char> readOnlySpan = this.AsSpan(0, length1);
      ReadOnlySpan<char> text = this.AsSpan(length1 + stride);
      // ISSUE: untyped stack allocation
      ValueStringBuilder valueStringBuilder = new ValueStringBuilder(new Span<char>((void*) __untypedstackalloc(new IntPtr(512)), 256));
      while (true)
      {
        int length2 = string.IndexOfNewlineChar(text, out stride);
        if (length2 >= 0)
        {
          valueStringBuilder.Append(replacementText);
          valueStringBuilder.Append(text.Slice(0, length2));
          text = text.Slice(length2 + stride);
        }
        else
          break;
      }
      string str = readOnlySpan.ToString() + valueStringBuilder.AsSpan() + (ReadOnlySpan<char>) replacementText + text;
      valueStringBuilder.Dispose();
      return str;
    }


    #nullable disable
    internal static int IndexOfNewlineChar(ReadOnlySpan<char> text, out int stride)
    {
      stride = 0;
      int index1 = text.IndexOfAny<char>((ReadOnlySpan<char>) "\r\n\f\u0085\u2028\u2029");
      if ((uint) index1 < (uint) text.Length)
      {
        stride = 1;
        if (text[index1] == '\r')
        {
          int index2 = index1 + 1;
          if ((uint) index2 < (uint) text.Length && text[index2] == '\n')
            stride = 2;
        }
      }
      return index1;
    }


    #nullable enable
    /// <summary>Splits a string into substrings based on a specified delimiting character and, optionally, options.</summary>
    /// <param name="separator">A character that delimits the substrings in this string.</param>
    /// <param name="options">A bitwise combination of the enumeration values that specifies whether to trim substrings and include empty substrings.</param>
    /// <returns>An array whose elements contain the substrings from this instance that are delimited by <paramref name="separator" />.</returns>
    public string[] Split(char separator, StringSplitOptions options = StringSplitOptions.None) => this.SplitInternal(new ReadOnlySpan<char>(ref separator, 1), int.MaxValue, options);

    /// <summary>Splits a string into a maximum number of substrings based on a specified delimiting character and, optionally, options.
    /// Splits a string into a maximum number of substrings based on the provided character separator, optionally omitting empty substrings from the result.</summary>
    /// <param name="separator">A character that delimits the substrings in this instance.</param>
    /// <param name="count">The maximum number of elements expected in the array.</param>
    /// <param name="options">A bitwise combination of the enumeration values that specifies whether to trim substrings and include empty substrings.</param>
    /// <returns>An array that contains at most <paramref name="count" /> substrings from this instance that are delimited by <paramref name="separator" />.</returns>
    public string[] Split(char separator, int count, StringSplitOptions options = StringSplitOptions.None) => this.SplitInternal(new ReadOnlySpan<char>(ref separator, 1), count, options);

    /// <summary>Splits a string into substrings based on specified delimiting characters.</summary>
    /// <param name="separator">An array of delimiting characters, an empty array that contains no delimiters, or <see langword="null" />.</param>
    /// <returns>An array whose elements contain the substrings from this instance that are delimited by one or more characters in <paramref name="separator" />. For more information, see the Remarks section.</returns>
    public string[] Split(params char[]? separator) => this.SplitInternal((ReadOnlySpan<char>) separator, int.MaxValue, StringSplitOptions.None);

    /// <summary>Splits a string into a maximum number of substrings based on specified delimiting characters.</summary>
    /// <param name="separator">An array of characters that delimit the substrings in this string, an empty array that contains no delimiters, or <see langword="null" />.</param>
    /// <param name="count">The maximum number of substrings to return.</param>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    /// <paramref name="count" /> is negative.</exception>
    /// <returns>An array whose elements contain the substrings in this instance that are delimited by one or more characters in <paramref name="separator" />. For more information, see the Remarks section.</returns>
    public string[] Split(char[]? separator, int count) => this.SplitInternal((ReadOnlySpan<char>) separator, count, StringSplitOptions.None);

    /// <summary>Splits a string into substrings based on specified delimiting characters and options.</summary>
    /// <param name="separator">An array of characters that delimit the substrings in this string, an empty array that contains no delimiters, or <see langword="null" />.</param>
    /// <param name="options">A bitwise combination of the enumeration values that specifies whether to trim substrings and include empty substrings.</param>
    /// <exception cref="T:System.ArgumentException">
    /// <paramref name="options" /> is not one of the <see cref="T:System.StringSplitOptions" /> values.</exception>
    /// <returns>An array whose elements contain the substrings in this string that are delimited by one or more characters in <paramref name="separator" />. For more information, see the Remarks section.</returns>
    public string[] Split(char[]? separator, StringSplitOptions options) => this.SplitInternal((ReadOnlySpan<char>) separator, int.MaxValue, options);

    /// <summary>Splits a string into a maximum number of substrings based on specified delimiting characters and, optionally, options.</summary>
    /// <param name="separator">An array of characters that delimit the substrings in this string, an empty array that contains no delimiters, or <see langword="null" />.</param>
    /// <param name="count">The maximum number of substrings to return.</param>
    /// <param name="options">A bitwise combination of the enumeration values that specifies whether to trim substrings and include empty substrings.</param>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    /// <paramref name="count" /> is negative.</exception>
    /// <exception cref="T:System.ArgumentException">
    /// <paramref name="options" /> is not one of the <see cref="T:System.StringSplitOptions" /> values.</exception>
    /// <returns>An array that contains the substrings in this string that are delimited by one or more characters in <paramref name="separator" />. For more information, see the Remarks section.</returns>
    public string[] Split(char[]? separator, int count, StringSplitOptions options) => this.SplitInternal((ReadOnlySpan<char>) separator, count, options);


    #nullable disable
    private unsafe string[] SplitInternal(
      ReadOnlySpan<char> separators,
      int count,
      StringSplitOptions options)
    {
      if (count < 0)
        throw new ArgumentOutOfRangeException(nameof (count), SR.ArgumentOutOfRange_NegativeCount);
      string.CheckStringSplitOptions(options);
      for (; count > 1 && this.Length != 0; count = 1)
      {
        if (separators.IsEmpty)
          options &= ~StringSplitOptions.TrimEntries;
        // ISSUE: untyped stack allocation
        ValueListBuilder<int> sepListBuilder = new ValueListBuilder<int>(new Span<int>((void*) __untypedstackalloc(new IntPtr(512)), 128));
        this.MakeSeparatorList(separators, ref sepListBuilder);
        ReadOnlySpan<int> sepList = sepListBuilder.AsSpan();
        if (sepList.Length != 0)
        {
          string[] strArray = options != StringSplitOptions.None ? this.SplitWithPostProcessing(sepList, new ReadOnlySpan<int>(), 1, count, options) : this.SplitWithoutPostProcessing(sepList, new ReadOnlySpan<int>(), 1, count);
          sepListBuilder.Dispose();
          return strArray;
        }
      }
      string str = this;
      if ((options & StringSplitOptions.TrimEntries) != StringSplitOptions.None && count > 0)
        str = str.Trim();
      if ((options & StringSplitOptions.RemoveEmptyEntries) != StringSplitOptions.None && str.Length == 0)
        count = 0;
      if (count == 0)
        return Array.Empty<string>();
      return new string[1]{ str };
    }


    #nullable enable
    /// <summary>Splits a string into substrings that are based on the provided string separator.</summary>
    /// <param name="separator">A string that delimits the substrings in this string.</param>
    /// <param name="options">A bitwise combination of the enumeration values that specifies whether to trim substrings and include empty substrings.</param>
    /// <returns>An array whose elements contain the substrings from this instance that are delimited by <paramref name="separator" />.</returns>
    public string[] Split(string? separator, StringSplitOptions options = StringSplitOptions.None) => this.SplitInternal(separator ?? string.Empty, (string[]) null, int.MaxValue, options);

    /// <summary>Splits a string into a maximum number of substrings based on a specified delimiting string and, optionally, options.</summary>
    /// <param name="separator">A string that delimits the substrings in this instance.</param>
    /// <param name="count">The maximum number of elements expected in the array.</param>
    /// <param name="options">A bitwise combination of the enumeration values that specifies whether to trim substrings and include empty substrings.</param>
    /// <returns>An array that contains at most <paramref name="count" /> substrings from this instance that are delimited by <paramref name="separator" />.</returns>
    public string[] Split(string? separator, int count, StringSplitOptions options = StringSplitOptions.None) => this.SplitInternal(separator ?? string.Empty, (string[]) null, count, options);

    /// <summary>Splits a string into substrings based on a specified delimiting string and, optionally, options.</summary>
    /// <param name="separator">An array of strings that delimit the substrings in this string, an empty array that contains no delimiters, or <see langword="null" />.</param>
    /// <param name="options">A bitwise combination of the enumeration values that specifies whether to trim substrings and include empty substrings.</param>
    /// <exception cref="T:System.ArgumentException">
    /// <paramref name="options" /> is not one of the <see cref="T:System.StringSplitOptions" /> values.</exception>
    /// <returns>An array whose elements contain the substrings in this string that are delimited by one or more strings in <paramref name="separator" />. For more information, see the Remarks section.</returns>
    public string[] Split(string[]? separator, StringSplitOptions options) => this.SplitInternal((string) null, separator, int.MaxValue, options);

    /// <summary>Splits a string into a maximum number of substrings based on specified delimiting strings and, optionally, options.</summary>
    /// <param name="separator">The strings that delimit the substrings in this string, an empty array that contains no delimiters, or <see langword="null" />.</param>
    /// <param name="count">The maximum number of substrings to return.</param>
    /// <param name="options">A bitwise combination of the enumeration values that specifies whether to trim substrings and include empty substrings.</param>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    /// <paramref name="count" /> is negative.</exception>
    /// <exception cref="T:System.ArgumentException">
    /// <paramref name="options" /> is not one of the <see cref="T:System.StringSplitOptions" /> values.</exception>
    /// <returns>An array whose elements contain the substrings in this string that are delimited by one or more strings in <paramref name="separator" />. For more information, see the Remarks section.</returns>
    public string[] Split(string[]? separator, int count, StringSplitOptions options) => this.SplitInternal((string) null, separator, count, options);


    #nullable disable
    private unsafe string[] SplitInternal(
      string separator,
      string[] separators,
      int count,
      StringSplitOptions options)
    {
      if (count < 0)
        throw new ArgumentOutOfRangeException(nameof (count), SR.ArgumentOutOfRange_NegativeCount);
      string.CheckStringSplitOptions(options);
      bool flag = separator != null;
      if (!flag && (separators == null || separators.Length == 0))
        return this.SplitInternal(new ReadOnlySpan<char>(), count, options);
      for (; count > 1 && this.Length != 0; count = 1)
      {
        if (flag)
        {
          if (separator.Length != 0)
            return this.SplitInternal(separator, count, options);
        }
        else
        {
          // ISSUE: untyped stack allocation
          ValueListBuilder<int> sepListBuilder = new ValueListBuilder<int>(new Span<int>((void*) __untypedstackalloc(new IntPtr(512)), 128));
          // ISSUE: untyped stack allocation
          ValueListBuilder<int> lengthListBuilder = new ValueListBuilder<int>(new Span<int>((void*) __untypedstackalloc(new IntPtr(512)), 128));
          this.MakeSeparatorList(separators, ref sepListBuilder, ref lengthListBuilder);
          ReadOnlySpan<int> sepList = sepListBuilder.AsSpan();
          ReadOnlySpan<int> lengthList = lengthListBuilder.AsSpan();
          if (sepList.Length == 0)
            return new string[1]{ this };
          string[] strArray = options != StringSplitOptions.None ? this.SplitWithPostProcessing(sepList, lengthList, 0, count, options) : this.SplitWithoutPostProcessing(sepList, lengthList, 0, count);
          sepListBuilder.Dispose();
          lengthListBuilder.Dispose();
          return strArray;
        }
      }
      string str = this;
      if ((options & StringSplitOptions.TrimEntries) != StringSplitOptions.None && count > 0)
        str = str.Trim();
      if ((options & StringSplitOptions.RemoveEmptyEntries) != StringSplitOptions.None && str.Length == 0)
        count = 0;
      if (count == 0)
        return Array.Empty<string>();
      return new string[1]{ str };
    }

    private unsafe string[] SplitInternal(string separator, int count, StringSplitOptions options)
    {
      // ISSUE: untyped stack allocation
      ValueListBuilder<int> sepListBuilder = new ValueListBuilder<int>(new Span<int>((void*) __untypedstackalloc(new IntPtr(512)), 128));
      this.MakeSeparatorList(separator, ref sepListBuilder);
      ReadOnlySpan<int> sepList = sepListBuilder.AsSpan();
      if (sepList.Length == 0)
      {
        string str = this;
        if ((options & StringSplitOptions.TrimEntries) != StringSplitOptions.None)
          str = str.Trim();
        if (str.Length == 0 && (options & StringSplitOptions.RemoveEmptyEntries) != StringSplitOptions.None)
          return Array.Empty<string>();
        return new string[1]{ str };
      }
      string[] strArray = options != StringSplitOptions.None ? this.SplitWithPostProcessing(sepList, new ReadOnlySpan<int>(), separator.Length, count, options) : this.SplitWithoutPostProcessing(sepList, new ReadOnlySpan<int>(), separator.Length, count);
      sepListBuilder.Dispose();
      return strArray;
    }

    private string[] SplitWithoutPostProcessing(
      ReadOnlySpan<int> sepList,
      ReadOnlySpan<int> lengthList,
      int defaultLength,
      int count)
    {
      int startIndex = 0;
      int index1 = 0;
      --count;
      int num = sepList.Length < count ? sepList.Length : count;
      string[] strArray = new string[num + 1];
      for (int index2 = 0; index2 < num && startIndex < this.Length; ++index2)
      {
        strArray[index1++] = this.Substring(startIndex, sepList[index2] - startIndex);
        startIndex = sepList[index2] + (lengthList.IsEmpty ? defaultLength : lengthList[index2]);
      }
      if (startIndex < this.Length && num >= 0)
        strArray[index1] = this.Substring(startIndex);
      else if (index1 == num)
        strArray[index1] = string.Empty;
      return strArray;
    }

    private string[] SplitWithPostProcessing(
      ReadOnlySpan<int> sepList,
      ReadOnlySpan<int> lengthList,
      int defaultLength,
      int count,
      StringSplitOptions options)
    {
      int length = sepList.Length;
      string[] array = new string[length < count ? length + 1 : count];
      int start = 0;
      int newSize = 0;
      ReadOnlySpan<char> span;
      for (int index = 0; index < length; ++index)
      {
        span = this.AsSpan(start, sepList[index] - start);
        if ((options & StringSplitOptions.TrimEntries) != StringSplitOptions.None)
          span = span.Trim();
        if (!span.IsEmpty || (options & StringSplitOptions.RemoveEmptyEntries) == StringSplitOptions.None)
          array[newSize++] = span.ToString();
        start = sepList[index] + (lengthList.IsEmpty ? defaultLength : lengthList[index]);
        if (newSize == count - 1)
        {
          if ((options & StringSplitOptions.RemoveEmptyEntries) != StringSplitOptions.None)
          {
            while (++index < length)
            {
              span = this.AsSpan(start, sepList[index] - start);
              if ((options & StringSplitOptions.TrimEntries) != StringSplitOptions.None)
                span = span.Trim();
              if (span.IsEmpty)
                start = sepList[index] + (lengthList.IsEmpty ? defaultLength : lengthList[index]);
              else
                break;
            }
            break;
          }
          break;
        }
      }
      span = this.AsSpan(start);
      if ((options & StringSplitOptions.TrimEntries) != StringSplitOptions.None)
        span = span.Trim();
      if (!span.IsEmpty || (options & StringSplitOptions.RemoveEmptyEntries) == StringSplitOptions.None)
        array[newSize++] = span.ToString();
      Array.Resize<string>(ref array, newSize);
      return array;
    }

    private unsafe void MakeSeparatorList(
      ReadOnlySpan<char> separators,
      ref ValueListBuilder<int> sepListBuilder)
    {
      if (separators.Length == 0)
      {
        for (int index = 0; index < this.Length; ++index)
        {
          if (char.IsWhiteSpace(this[index]))
            sepListBuilder.Append(index);
        }
      }
      else if (separators.Length <= 3)
      {
        char c = separators[0];
        char c2 = separators.Length > 1 ? separators[1] : c;
        char c3 = separators.Length > 2 ? separators[2] : c2;
        if (this.Length >= 16 && Sse41.IsSupported)
        {
          this.MakeSeparatorListVectorized(ref sepListBuilder, c, c2, c3);
        }
        else
        {
          for (int index = 0; index < this.Length; ++index)
          {
            char ch = this[index];
            if ((int) ch == (int) c || (int) ch == (int) c2 || (int) ch == (int) c3)
              sepListBuilder.Append(index);
          }
        }
      }
      else
      {
        uint* charMap = (uint*) &new String.ProbabilisticMap();
        string.InitializeProbabilisticMap(charMap, separators);
        for (int index = 0; index < this.Length; ++index)
        {
          char ch = this[index];
          if (string.IsCharBitSet(charMap, (byte) ch) && string.IsCharBitSet(charMap, (byte) ((uint) ch >> 8)) && separators.Contains<char>(ch))
            sepListBuilder.Append(index);
        }
      }
    }

    private void MakeSeparatorListVectorized(
      ref ValueListBuilder<int> sepListBuilder,
      char c,
      char c2,
      char c3)
    {
      if (!Sse41.IsSupported)
        throw new PlatformNotSupportedException();
      Vector128<byte> mask = Vector128.Create((byte) 0, (byte) 2, (byte) 4, (byte) 6, (byte) 8, (byte) 10, (byte) 12, (byte) 14, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
      Vector128<ushort> right1 = Vector128.Create((ushort) c);
      Vector128<ushort> right2 = Vector128.Create((ushort) c2);
      Vector128<ushort> right3 = Vector128.Create((ushort) c3);
      ref char local = ref MemoryMarshal.GetReference<char>(this.AsSpan());
      int num1 = this.Length & -Vector128<ushort>.Count;
      int num2;
      for (num2 = 0; num2 < num1; num2 += Vector128<ushort>.Count)
      {
        Vector128<ushort> left = ReadVector(ref local, num2);
        Vector128<ushort> right4 = Sse2.CompareEqual(left, right1);
        Vector128<ushort> right5 = Sse2.Or(Sse2.CompareEqual(left, right2), right4);
        Vector128<ushort> vector128 = Sse2.Or(Sse2.CompareEqual(left, right3), right5);
        if (!Sse41.TestZ(vector128, vector128))
        {
          Vector128<byte> vector = Ssse3.Shuffle(Sse2.ShiftRightLogical(vector128.AsUInt64<ushort>(), (byte) 4).AsByte<ulong>(), mask);
          uint uint32_1 = Sse2.ConvertToUInt32(vector.AsUInt32<byte>());
          uint uint32_2 = Sse2.ConvertToUInt32(Sse2.ShiftRightLogical(vector.AsUInt64<byte>(), (byte) 32).AsByte<ulong>().AsUInt32<byte>());
          int num3 = num2;
          while (uint32_1 != 0U)
          {
            if (((int) uint32_1 & 15) != 0)
              sepListBuilder.Append(num3);
            uint32_1 >>= 8;
            ++num3;
          }
          int num4 = num2 + 4;
          while (uint32_2 != 0U)
          {
            if (((int) uint32_2 & 15) != 0)
              sepListBuilder.Append(num4);
            uint32_2 >>= 8;
            ++num4;
          }
        }
      }
      for (; num2 < this.Length; ++num2)
      {
        char ch = Unsafe.Add<char>(ref local, (IntPtr) (long) (uint) num2);
        if ((int) ch == (int) c || (int) ch == (int) c2 || (int) ch == (int) c3)
          sepListBuilder.Append(num2);
      }

      static Vector128<ushort> ReadVector(ref char c0, int offset) => Unsafe.ReadUnaligned<Vector128<ushort>>(ref Unsafe.As<char, byte>(ref Unsafe.Add<char>(ref c0, (IntPtr) (long) (uint) offset)));
    }

    private void MakeSeparatorList(string separator, ref ValueListBuilder<int> sepListBuilder)
    {
      int length = separator.Length;
      for (int index = 0; index < this.Length; ++index)
      {
        if ((int) this[index] == (int) separator[0] && length <= this.Length - index && (length == 1 || this.AsSpan(index, length).SequenceEqual<char>((ReadOnlySpan<char>) separator)))
        {
          sepListBuilder.Append(index);
          index += length - 1;
        }
      }
    }

    private void MakeSeparatorList(
      string[] separators,
      ref ValueListBuilder<int> sepListBuilder,
      ref ValueListBuilder<int> lengthListBuilder)
    {
      for (int index1 = 0; index1 < this.Length; ++index1)
      {
        for (int index2 = 0; index2 < separators.Length; ++index2)
        {
          string separator = separators[index2];
          if (!string.IsNullOrEmpty(separator))
          {
            int length = separator.Length;
            if ((int) this[index1] == (int) separator[0] && length <= this.Length - index1 && (length == 1 || this.AsSpan(index1, length).SequenceEqual<char>((ReadOnlySpan<char>) separator)))
            {
              sepListBuilder.Append(index1);
              lengthListBuilder.Append(length);
              index1 += length - 1;
              break;
            }
          }
        }
      }
    }

    private static void CheckStringSplitOptions(StringSplitOptions options)
    {
      if ((options & ~(StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)) == StringSplitOptions.None)
        return;
      ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidFlag, ExceptionArgument.options);
    }


    #nullable enable
    /// <summary>Retrieves a substring from this instance. The substring starts at a specified character position and continues to the end of the string.</summary>
    /// <param name="startIndex">The zero-based starting character position of a substring in this instance.</param>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    /// <paramref name="startIndex" /> is less than zero or greater than the length of this instance.</exception>
    /// <returns>A string that is equivalent to the substring that begins at <paramref name="startIndex" /> in this instance, or <see cref="F:System.String.Empty" /> if <paramref name="startIndex" /> is equal to the length of this instance.</returns>
    public string Substring(int startIndex) => this.Substring(startIndex, this.Length - startIndex);

    /// <summary>Retrieves a substring from this instance. The substring starts at a specified character position and has a specified length.</summary>
    /// <param name="startIndex">The zero-based starting character position of a substring in this instance.</param>
    /// <param name="length">The number of characters in the substring.</param>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    ///        <paramref name="startIndex" /> plus <paramref name="length" /> indicates a position not within this instance.
    /// 
    /// -or-
    /// 
    /// <paramref name="startIndex" /> or <paramref name="length" /> is less than zero.</exception>
    /// <returns>A string that is equivalent to the substring of length <paramref name="length" /> that begins at <paramref name="startIndex" /> in this instance, or <see cref="F:System.String.Empty" /> if <paramref name="startIndex" /> is equal to the length of this instance and <paramref name="length" /> is zero.</returns>
    public string Substring(int startIndex, int length)
    {
      if (startIndex < 0)
        throw new ArgumentOutOfRangeException(nameof (startIndex), SR.ArgumentOutOfRange_StartIndex);
      if (startIndex > this.Length)
        throw new ArgumentOutOfRangeException(nameof (startIndex), SR.ArgumentOutOfRange_StartIndexLargerThanLength);
      if (length < 0)
        throw new ArgumentOutOfRangeException(nameof (length), SR.ArgumentOutOfRange_NegativeLength);
      if (startIndex > this.Length - length)
        throw new ArgumentOutOfRangeException(nameof (length), SR.ArgumentOutOfRange_IndexLength);
      if (length == 0)
        return string.Empty;
      return startIndex == 0 && length == this.Length ? this : this.InternalSubString(startIndex, length);
    }


    #nullable disable
    private string InternalSubString(int startIndex, int length)
    {
      string str = string.FastAllocateString(length);
      UIntPtr length1 = (UIntPtr) (uint) str.Length;
      Buffer.Memmove<char>(ref str._firstChar, ref Unsafe.Add<char>(ref this._firstChar, (IntPtr) (uint) startIndex), length1);
      return str;
    }


    #nullable enable
    /// <summary>Returns a copy of this string converted to lowercase.</summary>
    /// <returns>A string in lowercase.</returns>
    public string ToLower() => this.ToLower((CultureInfo) null);

    /// <summary>Returns a copy of this string converted to lowercase, using the casing rules of the specified culture.</summary>
    /// <param name="culture">An object that supplies culture-specific casing rules. If <paramref name="culture" /> is <see langword="null" />, the current culture is used.</param>
    /// <returns>The lowercase equivalent of the current string.</returns>
    public string ToLower(CultureInfo? culture) => (culture ?? CultureInfo.CurrentCulture).TextInfo.ToLower(this);

    /// <summary>Returns a copy of this <see cref="T:System.String" /> object converted to lowercase using the casing rules of the invariant culture.</summary>
    /// <returns>The lowercase equivalent of the current string.</returns>
    public string ToLowerInvariant() => TextInfo.Invariant.ToLower(this);

    /// <summary>Returns a copy of this string converted to uppercase.</summary>
    /// <returns>The uppercase equivalent of the current string.</returns>
    public string ToUpper() => this.ToUpper((CultureInfo) null);

    /// <summary>Returns a copy of this string converted to uppercase, using the casing rules of the specified culture.</summary>
    /// <param name="culture">An object that supplies culture-specific casing rules. If <paramref name="culture" /> is <see langword="null" />, the current culture is used.</param>
    /// <returns>The uppercase equivalent of the current string.</returns>
    public string ToUpper(CultureInfo? culture) => (culture ?? CultureInfo.CurrentCulture).TextInfo.ToUpper(this);

    /// <summary>Returns a copy of this <see cref="T:System.String" /> object converted to uppercase using the casing rules of the invariant culture.</summary>
    /// <returns>The uppercase equivalent of the current string.</returns>
    public string ToUpperInvariant() => TextInfo.Invariant.ToUpper(this);

    /// <summary>Removes all leading and trailing white-space characters from the current string.</summary>
    /// <returns>The string that remains after all white-space characters are removed from the start and end of the current string. If no characters can be trimmed from the current instance, the method returns the current instance unchanged.</returns>
    public string Trim() => this.TrimWhiteSpaceHelper(TrimType.Both);

    /// <summary>Removes all leading and trailing instances of a character from the current string.</summary>
    /// <param name="trimChar">A Unicode character to remove.</param>
    /// <returns>The string that remains after all instances of the <paramref name="trimChar" /> character are removed from the start and end of the current string. If no characters can be trimmed from the current instance, the method returns the current instance unchanged.</returns>
    public unsafe string Trim(char trimChar) => this.TrimHelper(&trimChar, 1, TrimType.Both);

    /// <summary>Removes all leading and trailing occurrences of a set of characters specified in an array from the current string.</summary>
    /// <param name="trimChars">An array of Unicode characters to remove, or <see langword="null" />.</param>
    /// <returns>The string that remains after all occurrences of the characters in the <paramref name="trimChars" /> parameter are removed from the start and end of the current string. If <paramref name="trimChars" /> is <see langword="null" /> or an empty array, white-space characters are removed instead. If no characters can be trimmed from the current instance, the method returns the current instance unchanged.</returns>
    public unsafe string Trim(params char[]? trimChars)
    {
      if (trimChars == null || trimChars.Length == 0)
        return this.TrimWhiteSpaceHelper(TrimType.Both);
      fixed (char* trimChars1 = &trimChars[0])
        return this.TrimHelper(trimChars1, trimChars.Length, TrimType.Both);
    }

    /// <summary>Removes all the leading white-space characters from the current string.</summary>
    /// <returns>The string that remains after all white-space characters are removed from the start of the current string. If no characters can be trimmed from the current instance, the method returns the current instance unchanged.</returns>
    public string TrimStart() => this.TrimWhiteSpaceHelper(TrimType.Head);

    /// <summary>Removes all the leading occurrences of a specified character from the current string.</summary>
    /// <param name="trimChar">The Unicode character to remove.</param>
    /// <returns>The string that remains after all occurrences of the <paramref name="trimChar" /> character are removed from the start of the current string. If no characters can be trimmed from the current instance, the method returns the current instance unchanged.</returns>
    public unsafe string TrimStart(char trimChar) => this.TrimHelper(&trimChar, 1, TrimType.Head);

    /// <summary>Removes all the leading occurrences of a set of characters specified in an array from the current string.</summary>
    /// <param name="trimChars">An array of Unicode characters to remove, or <see langword="null" />.</param>
    /// <returns>The string that remains after all occurrences of characters in the <paramref name="trimChars" /> parameter are removed from the start of the current string. If <paramref name="trimChars" /> is <see langword="null" /> or an empty array, white-space characters are removed instead. If no characters can be trimmed from the current instance, the method returns the current instance unchanged.</returns>
    public unsafe string TrimStart(params char[]? trimChars)
    {
      if (trimChars == null || trimChars.Length == 0)
        return this.TrimWhiteSpaceHelper(TrimType.Head);
      fixed (char* trimChars1 = &trimChars[0])
        return this.TrimHelper(trimChars1, trimChars.Length, TrimType.Head);
    }

    /// <summary>Removes all the trailing white-space characters from the current string.</summary>
    /// <returns>The string that remains after all white-space characters are removed from the end of the current string. If no characters can be trimmed from the current instance, the method returns the current instance unchanged.</returns>
    public string TrimEnd() => this.TrimWhiteSpaceHelper(TrimType.Tail);

    /// <summary>Removes all the trailing occurrences of a character from the current string.</summary>
    /// <param name="trimChar">A Unicode character to remove.</param>
    /// <returns>The string that remains after all occurrences of the <paramref name="trimChar" /> character are removed from the end of the current string. If no characters can be trimmed from the current instance, the method returns the current instance unchanged.</returns>
    public unsafe string TrimEnd(char trimChar) => this.TrimHelper(&trimChar, 1, TrimType.Tail);

    /// <summary>Removes all the trailing occurrences of a set of characters specified in an array from the current string.</summary>
    /// <param name="trimChars">An array of Unicode characters to remove, or <see langword="null" />.</param>
    /// <returns>The string that remains after all occurrences of the characters in the <paramref name="trimChars" /> parameter are removed from the end of the current string. If <paramref name="trimChars" /> is <see langword="null" /> or an empty array, Unicode white-space characters are removed instead. If no characters can be trimmed from the current instance, the method returns the current instance unchanged.</returns>
    public unsafe string TrimEnd(params char[]? trimChars)
    {
      if (trimChars == null || trimChars.Length == 0)
        return this.TrimWhiteSpaceHelper(TrimType.Tail);
      fixed (char* trimChars1 = &trimChars[0])
        return this.TrimHelper(trimChars1, trimChars.Length, TrimType.Tail);
    }


    #nullable disable
    private string TrimWhiteSpaceHelper(TrimType trimType)
    {
      int num1 = this.Length - 1;
      int num2 = 0;
      if ((trimType & TrimType.Head) != (TrimType) 0)
      {
        num2 = 0;
        while (num2 < this.Length && char.IsWhiteSpace(this[num2]))
          ++num2;
      }
      if ((trimType & TrimType.Tail) != (TrimType) 0)
      {
        num1 = this.Length - 1;
        while (num1 >= num2 && char.IsWhiteSpace(this[num1]))
          --num1;
      }
      return this.CreateTrimmedString(num2, num1);
    }

    private unsafe string TrimHelper(char* trimChars, int trimCharsLength, TrimType trimType)
    {
      int num1 = this.Length - 1;
      int num2 = 0;
      if ((trimType & TrimType.Head) != (TrimType) 0)
      {
        for (num2 = 0; num2 < this.Length; ++num2)
        {
          char ch = this[num2];
          int index = 0;
          while (index < trimCharsLength && (int) trimChars[index] != (int) ch)
            ++index;
          if (index == trimCharsLength)
            break;
        }
      }
      if ((trimType & TrimType.Tail) != (TrimType) 0)
      {
        for (num1 = this.Length - 1; num1 >= num2; --num1)
        {
          char ch = this[num1];
          int index = 0;
          while (index < trimCharsLength && (int) trimChars[index] != (int) ch)
            ++index;
          if (index == trimCharsLength)
            break;
        }
      }
      return this.CreateTrimmedString(num2, num1);
    }

    private string CreateTrimmedString(int start, int end)
    {
      int length = end - start + 1;
      if (length == this.Length)
        return this;
      return length != 0 ? this.InternalSubString(start, length) : string.Empty;
    }


    #nullable enable
    /// <summary>Returns a value indicating whether a specified substring occurs within this string.</summary>
    /// <param name="value">The string to seek.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="value" /> is <see langword="null" />.</exception>
    /// <returns>
    /// <see langword="true" /> if the <paramref name="value" /> parameter occurs within this string, or if <paramref name="value" /> is the empty string (""); otherwise, <see langword="false" />.</returns>
    public bool Contains(string value)
    {
      if (value == null)
        ThrowHelper.ThrowArgumentNullException(ExceptionArgument.value);
      return SpanHelpers.IndexOf(ref this._firstChar, this.Length, ref value._firstChar, value.Length) >= 0;
    }

    /// <summary>Returns a value indicating whether a specified string occurs within this string, using the specified comparison rules.</summary>
    /// <param name="value">The string to seek.</param>
    /// <param name="comparisonType">One of the enumeration values that specifies the rules to use in the comparison.</param>
    /// <returns>
    /// <see langword="true" /> if the <paramref name="value" /> parameter occurs within this string, or if <paramref name="value" /> is the empty string (""); otherwise, <see langword="false" />.</returns>
    public bool Contains(string value, StringComparison comparisonType) => this.IndexOf(value, comparisonType) >= 0;

    /// <summary>Returns a value indicating whether a specified character occurs within this string.</summary>
    /// <param name="value">The character to seek.</param>
    /// <returns>
    /// <see langword="true" /> if the <paramref name="value" /> parameter occurs within this string; otherwise, <see langword="false" />.</returns>
    public bool Contains(char value) => SpanHelpers.Contains(ref this._firstChar, value, this.Length);

    /// <summary>Returns a value indicating whether a specified character occurs within this string, using the specified comparison rules.</summary>
    /// <param name="value">The character to seek.</param>
    /// <param name="comparisonType">One of the enumeration values that specifies the rules to use in the comparison.</param>
    /// <returns>
    /// <see langword="true" /> if the <paramref name="value" /> parameter occurs within this string; otherwise, <see langword="false" />.</returns>
    public bool Contains(char value, StringComparison comparisonType) => this.IndexOf(value, comparisonType) != -1;

    /// <summary>Reports the zero-based index of the first occurrence of the specified Unicode character in this string.</summary>
    /// <param name="value">A Unicode character to seek.</param>
    /// <returns>The zero-based index position of <paramref name="value" /> if that character is found, or -1 if it is not.</returns>
    public int IndexOf(char value) => SpanHelpers.IndexOf(ref this._firstChar, value, this.Length);

    /// <summary>Reports the zero-based index of the first occurrence of the specified Unicode character in this string. The search starts at a specified character position.</summary>
    /// <param name="value">A Unicode character to seek.</param>
    /// <param name="startIndex">The search starting position.</param>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    /// <paramref name="startIndex" /> is less than 0 (zero) or greater than the length of the string.</exception>
    /// <returns>The zero-based index position of <paramref name="value" /> from the start of the string if that character is found, or -1 if it is not.</returns>
    public int IndexOf(char value, int startIndex) => this.IndexOf(value, startIndex, this.Length - startIndex);

    /// <summary>Reports the zero-based index of the first occurrence of the specified Unicode character in this string. A parameter specifies the type of search to use for the specified character.</summary>
    /// <param name="value">The character to seek.</param>
    /// <param name="comparisonType">An enumeration value that specifies the rules for the search.</param>
    /// <exception cref="T:System.ArgumentException">
    /// <paramref name="comparisonType" /> is not a valid <see cref="T:System.StringComparison" /> value.</exception>
    /// <returns>The zero-based index of <paramref name="value" /> if that character is found, or -1 if it is not.</returns>
    public int IndexOf(char value, StringComparison comparisonType)
    {
      switch (comparisonType)
      {
        case StringComparison.CurrentCulture:
        case StringComparison.CurrentCultureIgnoreCase:
          return CultureInfo.CurrentCulture.CompareInfo.IndexOf(this, value, string.GetCaseCompareOfComparisonCulture(comparisonType));
        case StringComparison.InvariantCulture:
        case StringComparison.InvariantCultureIgnoreCase:
          return CompareInfo.Invariant.IndexOf(this, value, string.GetCaseCompareOfComparisonCulture(comparisonType));
        case StringComparison.Ordinal:
          return this.IndexOf(value);
        case StringComparison.OrdinalIgnoreCase:
          return CompareInfo.Invariant.IndexOf(this, value, CompareOptions.OrdinalIgnoreCase);
        default:
          throw new ArgumentException(SR.NotSupported_StringComparison, nameof (comparisonType));
      }
    }

    /// <summary>Reports the zero-based index of the first occurrence of the specified character in this instance. The search starts at a specified character position and examines a specified number of character positions.</summary>
    /// <param name="value">A Unicode character to seek.</param>
    /// <param name="startIndex">The search starting position.</param>
    /// <param name="count">The number of character positions to examine.</param>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    ///        <paramref name="count" /> or <paramref name="startIndex" /> is negative.
    /// 
    /// -or-
    /// 
    /// <paramref name="startIndex" /> is greater than the length of this string.
    /// 
    /// -or-
    /// 
    /// <paramref name="count" /> is greater than the length of this string minus <paramref name="startIndex" />.</exception>
    /// <returns>The zero-based index position of <paramref name="value" /> from the start of the string if that character is found, or -1 if it is not.</returns>
    public int IndexOf(char value, int startIndex, int count)
    {
      if ((uint) startIndex > (uint) this.Length)
        throw new ArgumentOutOfRangeException(nameof (startIndex), SR.ArgumentOutOfRange_Index);
      if ((uint) count > (uint) (this.Length - startIndex))
        throw new ArgumentOutOfRangeException(nameof (count), SR.ArgumentOutOfRange_Count);
      int num = SpanHelpers.IndexOf(ref Unsafe.Add<char>(ref this._firstChar, startIndex), value, count);
      return num != -1 ? num + startIndex : num;
    }

    /// <summary>Reports the zero-based index of the first occurrence in this instance of any character in a specified array of Unicode characters.</summary>
    /// <param name="anyOf">A Unicode character array containing one or more characters to seek.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="anyOf" /> is <see langword="null" />.</exception>
    /// <returns>The zero-based index position of the first occurrence in this instance where any character in <paramref name="anyOf" /> was found; -1 if no character in <paramref name="anyOf" /> was found.</returns>
    public int IndexOfAny(char[] anyOf) => this.IndexOfAny(anyOf, 0, this.Length);

    /// <summary>Reports the zero-based index of the first occurrence in this instance of any character in a specified array of Unicode characters. The search starts at a specified character position.</summary>
    /// <param name="anyOf">A Unicode character array containing one or more characters to seek.</param>
    /// <param name="startIndex">The search starting position.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="anyOf" /> is <see langword="null" />.</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    ///        <paramref name="startIndex" /> is negative.
    /// 
    /// -or-
    /// 
    /// <paramref name="startIndex" /> is greater than the number of characters in this instance.</exception>
    /// <returns>The zero-based index position of the first occurrence in this instance where any character in <paramref name="anyOf" /> was found; -1 if no character in <paramref name="anyOf" /> was found.</returns>
    public int IndexOfAny(char[] anyOf, int startIndex) => this.IndexOfAny(anyOf, startIndex, this.Length - startIndex);

    /// <summary>Reports the zero-based index of the first occurrence in this instance of any character in a specified array of Unicode characters. The search starts at a specified character position and examines a specified number of character positions.</summary>
    /// <param name="anyOf">A Unicode character array containing one or more characters to seek.</param>
    /// <param name="startIndex">The search starting position.</param>
    /// <param name="count">The number of character positions to examine.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="anyOf" /> is <see langword="null" />.</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    ///        <paramref name="count" /> or <paramref name="startIndex" /> is negative.
    /// 
    /// -or-
    /// 
    /// <paramref name="count" /> + <paramref name="startIndex" /> is greater than the number of characters in this instance.</exception>
    /// <returns>The zero-based index position of the first occurrence in this instance where any character in <paramref name="anyOf" /> was found; -1 if no character in <paramref name="anyOf" /> was found.</returns>
    public int IndexOfAny(char[] anyOf, int startIndex, int count)
    {
      if (anyOf == null)
        throw new ArgumentNullException(nameof (anyOf));
      if ((uint) startIndex > (uint) this.Length)
        throw new ArgumentOutOfRangeException(nameof (startIndex), SR.ArgumentOutOfRange_Index);
      if ((uint) count > (uint) (this.Length - startIndex))
        throw new ArgumentOutOfRangeException(nameof (count), SR.ArgumentOutOfRange_Count);
      if (anyOf.Length != 0 && anyOf.Length <= 5)
      {
        int num = new ReadOnlySpan<char>(ref Unsafe.Add<char>(ref this._firstChar, startIndex), count).IndexOfAny<char>((ReadOnlySpan<char>) anyOf);
        return num != -1 ? num + startIndex : num;
      }
      return anyOf.Length > 5 ? this.IndexOfCharArray(anyOf, startIndex, count) : -1;
    }


    #nullable disable
    private unsafe int IndexOfCharArray(char[] anyOf, int startIndex, int count)
    {
      uint* charMap = (uint*) &new String.ProbabilisticMap();
      string.InitializeProbabilisticMap(charMap, (ReadOnlySpan<char>) anyOf);
      fixed (char* chPtr1 = &this._firstChar)
      {
        char* chPtr2 = chPtr1 + startIndex;
        while (count > 0)
        {
          int searchChar = (int) *chPtr2;
          if (string.IsCharBitSet(charMap, (byte) searchChar) && string.IsCharBitSet(charMap, (byte) (searchChar >> 8)) && string.ArrayContains((char) searchChar, anyOf))
            return (int) (chPtr2 - chPtr1);
          --count;
          ++chPtr2;
        }
        return -1;
      }
    }

    private static unsafe void InitializeProbabilisticMap(uint* charMap, ReadOnlySpan<char> anyOf)
    {
      bool flag = false;
      uint* charMap1 = charMap;
      for (int index = 0; index < anyOf.Length; ++index)
      {
        int num1 = (int) anyOf[index];
        string.SetCharBit(charMap1, (byte) num1);
        int num2 = num1 >> 8;
        if (num2 == 0)
          flag = true;
        else
          string.SetCharBit(charMap1, (byte) num2);
      }
      if (!flag)
        return;
      uint* numPtr = charMap1;
      int num = (int) *numPtr | 1;
      *numPtr = (uint) num;
    }

    private static bool ArrayContains(char searchChar, char[] anyOf)
    {
      for (int index = 0; index < anyOf.Length; ++index)
      {
        if ((int) anyOf[index] == (int) searchChar)
          return true;
      }
      return false;
    }

    private static unsafe bool IsCharBitSet(uint* charMap, byte value) => (*(uint*) ((IntPtr) charMap + (IntPtr) ((long) ((uint) value & 7U) * 4L)) & (uint) (1 << ((int) value >> 3))) > 0U;

    private static unsafe void SetCharBit(uint* charMap, byte value)
    {
      IntPtr num = (IntPtr) charMap + (IntPtr) ((long) ((uint) value & 7U) * 4L);
      *(int*) num = (int) *(uint*) num | 1 << ((int) value >> 3);
    }


    #nullable enable
    /// <summary>Reports the zero-based index of the first occurrence of the specified string in this instance.</summary>
    /// <param name="value">The string to seek.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="value" /> is <see langword="null" />.</exception>
    /// <returns>The zero-based index position of <paramref name="value" /> if that string is found, or -1 if it is not. If <paramref name="value" /> is <see cref="F:System.String.Empty" />, the return value is 0.</returns>
    public int IndexOf(string value) => this.IndexOf(value, StringComparison.CurrentCulture);

    /// <summary>Reports the zero-based index of the first occurrence of the specified string in this instance. The search starts at a specified character position.</summary>
    /// <param name="value">The string to seek.</param>
    /// <param name="startIndex">The search starting position.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="value" /> is <see langword="null" />.</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    /// <paramref name="startIndex" /> is less than 0 (zero) or greater than the length of this string.</exception>
    /// <returns>The zero-based index position of <paramref name="value" /> from the start of the current instance if that string is found, or -1 if it is not. If <paramref name="value" /> is <see cref="F:System.String.Empty" />, the return value is <paramref name="startIndex" />.</returns>
    public int IndexOf(string value, int startIndex) => this.IndexOf(value, startIndex, StringComparison.CurrentCulture);

    /// <summary>Reports the zero-based index of the first occurrence of the specified string in this instance. The search starts at a specified character position and examines a specified number of character positions.</summary>
    /// <param name="value">The string to seek.</param>
    /// <param name="startIndex">The search starting position.</param>
    /// <param name="count">The number of character positions to examine.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="value" /> is <see langword="null" />.</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    ///        <paramref name="count" /> or <paramref name="startIndex" /> is negative.
    /// 
    /// -or-
    /// 
    /// <paramref name="startIndex" /> is greater than the length of this string.
    /// 
    /// -or-
    /// 
    /// <paramref name="count" /> is greater than the length of this string minus <paramref name="startIndex" />.</exception>
    /// <returns>The zero-based index position of <paramref name="value" /> from the start of the current instance if that string is found, or -1 if it is not. If <paramref name="value" /> is <see cref="F:System.String.Empty" />, the return value is <paramref name="startIndex" />.</returns>
    public int IndexOf(string value, int startIndex, int count) => this.IndexOf(value, startIndex, count, StringComparison.CurrentCulture);

    /// <summary>Reports the zero-based index of the first occurrence of the specified string in the current <see cref="T:System.String" /> object. A parameter specifies the type of search to use for the specified string.</summary>
    /// <param name="value">The string to seek.</param>
    /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="value" /> is <see langword="null" />.</exception>
    /// <exception cref="T:System.ArgumentException">
    /// <paramref name="comparisonType" /> is not a valid <see cref="T:System.StringComparison" /> value.</exception>
    /// <returns>The index position of the <paramref name="value" /> parameter if that string is found, or -1 if it is not. If <paramref name="value" /> is <see cref="F:System.String.Empty" />, the return value is 0.</returns>
    public int IndexOf(string value, StringComparison comparisonType) => this.IndexOf(value, 0, this.Length, comparisonType);

    /// <summary>Reports the zero-based index of the first occurrence of the specified string in the current <see cref="T:System.String" /> object. Parameters specify the starting search position in the current string and the type of search to use for the specified string.</summary>
    /// <param name="value">The string to seek.</param>
    /// <param name="startIndex">The search starting position.</param>
    /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="value" /> is <see langword="null" />.</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    /// <paramref name="startIndex" /> is less than 0 (zero) or greater than the length of this string.</exception>
    /// <exception cref="T:System.ArgumentException">
    /// <paramref name="comparisonType" /> is not a valid <see cref="T:System.StringComparison" /> value.</exception>
    /// <returns>The zero-based index position of the <paramref name="value" /> parameter from the start of the current instance if that string is found, or -1 if it is not. If <paramref name="value" /> is <see cref="F:System.String.Empty" />, the return value is <paramref name="startIndex" />.</returns>
    public int IndexOf(string value, int startIndex, StringComparison comparisonType) => this.IndexOf(value, startIndex, this.Length - startIndex, comparisonType);

    /// <summary>Reports the zero-based index of the first occurrence of the specified string in the current <see cref="T:System.String" /> object. Parameters specify the starting search position in the current string, the number of characters in the current string to search, and the type of search to use for the specified string.</summary>
    /// <param name="value">The string to seek.</param>
    /// <param name="startIndex">The search starting position.</param>
    /// <param name="count">The number of character positions to examine.</param>
    /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="value" /> is <see langword="null" />.</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    ///        <paramref name="count" /> or <paramref name="startIndex" /> is negative.
    /// 
    /// -or-
    /// 
    /// <paramref name="startIndex" /> is greater than the length of this instance.
    /// 
    /// -or-
    /// 
    /// <paramref name="count" /> is greater than the length of this string minus <paramref name="startIndex" />.</exception>
    /// <exception cref="T:System.ArgumentException">
    /// <paramref name="comparisonType" /> is not a valid <see cref="T:System.StringComparison" /> value.</exception>
    /// <returns>The zero-based index position of the <paramref name="value" /> parameter from the start of the current instance if that string is found, or -1 if it is not. If <paramref name="value" /> is <see cref="F:System.String.Empty" />, the return value is <paramref name="startIndex" />.</returns>
    public int IndexOf(string value, int startIndex, int count, StringComparison comparisonType)
    {
      switch (comparisonType)
      {
        case StringComparison.CurrentCulture:
        case StringComparison.CurrentCultureIgnoreCase:
          return CultureInfo.CurrentCulture.CompareInfo.IndexOf(this, value, startIndex, count, string.GetCaseCompareOfComparisonCulture(comparisonType));
        case StringComparison.InvariantCulture:
        case StringComparison.InvariantCultureIgnoreCase:
          return CompareInfo.Invariant.IndexOf(this, value, startIndex, count, string.GetCaseCompareOfComparisonCulture(comparisonType));
        case StringComparison.Ordinal:
        case StringComparison.OrdinalIgnoreCase:
          return Ordinal.IndexOf(this, value, startIndex, count, comparisonType == StringComparison.OrdinalIgnoreCase);
        default:
          throw value == null ? (ArgumentException) new ArgumentNullException(nameof (value)) : new ArgumentException(SR.NotSupported_StringComparison, nameof (comparisonType));
      }
    }

    /// <summary>Reports the zero-based index position of the last occurrence of a specified Unicode character within this instance.</summary>
    /// <param name="value">The Unicode character to seek.</param>
    /// <returns>The zero-based index position of <paramref name="value" /> if that character is found, or -1 if it is not.</returns>
    public int LastIndexOf(char value) => SpanHelpers.LastIndexOf(ref this._firstChar, value, this.Length);

    /// <summary>Reports the zero-based index position of the last occurrence of a specified Unicode character within this instance. The search starts at a specified character position and proceeds backward toward the beginning of the string.</summary>
    /// <param name="value">The Unicode character to seek.</param>
    /// <param name="startIndex">The starting position of the search. The search proceeds from <paramref name="startIndex" /> toward the beginning of this instance.</param>
    /// <exception cref="T:System.ArgumentOutOfRangeException">The current instance does not equal <see cref="F:System.String.Empty" />, and <paramref name="startIndex" /> is less than zero or greater than or equal to the length of this instance.</exception>
    /// <returns>The zero-based index position of <paramref name="value" /> if that character is found, or -1 if it is not found or if the current instance equals <see cref="F:System.String.Empty" />.</returns>
    public int LastIndexOf(char value, int startIndex) => this.LastIndexOf(value, startIndex, startIndex + 1);

    /// <summary>Reports the zero-based index position of the last occurrence of the specified Unicode character in a substring within this instance. The search starts at a specified character position and proceeds backward toward the beginning of the string for a specified number of character positions.</summary>
    /// <param name="value">The Unicode character to seek.</param>
    /// <param name="startIndex">The starting position of the search. The search proceeds from <paramref name="startIndex" /> toward the beginning of this instance.</param>
    /// <param name="count">The number of character positions to examine.</param>
    /// <exception cref="T:System.ArgumentOutOfRangeException">The current instance does not equal <see cref="F:System.String.Empty" />, and <paramref name="startIndex" /> is less than zero or greater than or equal to the length of this instance.
    /// 
    /// -or-
    /// 
    /// The current instance does not equal <see cref="F:System.String.Empty" />, and <paramref name="startIndex" /> - <paramref name="count" /> + 1 is less than zero.</exception>
    /// <returns>The zero-based index position of <paramref name="value" /> if that character is found, or -1 if it is not found or if the current instance equals <see cref="F:System.String.Empty" />.</returns>
    public int LastIndexOf(char value, int startIndex, int count)
    {
      if (this.Length == 0)
        return -1;
      if ((uint) startIndex >= (uint) this.Length)
        throw new ArgumentOutOfRangeException(nameof (startIndex), SR.ArgumentOutOfRange_Index);
      if ((uint) count > (uint) (startIndex + 1))
        throw new ArgumentOutOfRangeException(nameof (count), SR.ArgumentOutOfRange_Count);
      int elementOffset = startIndex + 1 - count;
      int num = SpanHelpers.LastIndexOf(ref Unsafe.Add<char>(ref this._firstChar, elementOffset), value, count);
      return num != -1 ? num + elementOffset : num;
    }

    /// <summary>Reports the zero-based index position of the last occurrence in this instance of one or more characters specified in a Unicode array.</summary>
    /// <param name="anyOf">A Unicode character array containing one or more characters to seek.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="anyOf" /> is <see langword="null" />.</exception>
    /// <returns>The index position of the last occurrence in this instance where any character in <paramref name="anyOf" /> was found; -1 if no character in <paramref name="anyOf" /> was found.</returns>
    public int LastIndexOfAny(char[] anyOf) => this.LastIndexOfAny(anyOf, this.Length - 1, this.Length);

    /// <summary>Reports the zero-based index position of the last occurrence in this instance of one or more characters specified in a Unicode array. The search starts at a specified character position and proceeds backward toward the beginning of the string.</summary>
    /// <param name="anyOf">A Unicode character array containing one or more characters to seek.</param>
    /// <param name="startIndex">The search starting position. The search proceeds from <paramref name="startIndex" /> toward the beginning of this instance.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="anyOf" /> is <see langword="null" />.</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">The current instance does not equal <see cref="F:System.String.Empty" />, and <paramref name="startIndex" /> specifies a position that is not within this instance.</exception>
    /// <returns>The index position of the last occurrence in this instance where any character in <paramref name="anyOf" /> was found; -1 if no character in <paramref name="anyOf" /> was found or if the current instance equals <see cref="F:System.String.Empty" />.</returns>
    public int LastIndexOfAny(char[] anyOf, int startIndex) => this.LastIndexOfAny(anyOf, startIndex, startIndex + 1);

    /// <summary>Reports the zero-based index position of the last occurrence in this instance of one or more characters specified in a Unicode array. The search starts at a specified character position and proceeds backward toward the beginning of the string for a specified number of character positions.</summary>
    /// <param name="anyOf">A Unicode character array containing one or more characters to seek.</param>
    /// <param name="startIndex">The search starting position. The search proceeds from <paramref name="startIndex" /> toward the beginning of this instance.</param>
    /// <param name="count">The number of character positions to examine.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="anyOf" /> is <see langword="null" />.</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">The current instance does not equal <see cref="F:System.String.Empty" />, and <paramref name="count" /> or <paramref name="startIndex" /> is negative.
    /// 
    /// -or-
    /// 
    /// The current instance does not equal <see cref="F:System.String.Empty" />, and <paramref name="startIndex" /> minus <paramref name="count" /> + 1 is less than zero.</exception>
    /// <returns>The index position of the last occurrence in this instance where any character in <paramref name="anyOf" /> was found; -1 if no character in <paramref name="anyOf" /> was found or if the current instance equals <see cref="F:System.String.Empty" />.</returns>
    public int LastIndexOfAny(char[] anyOf, int startIndex, int count)
    {
      if (anyOf == null)
        throw new ArgumentNullException(nameof (anyOf));
      if (this.Length == 0)
        return -1;
      if ((uint) startIndex >= (uint) this.Length)
        throw new ArgumentOutOfRangeException(nameof (startIndex), SR.ArgumentOutOfRange_Index);
      if (count < 0 || count - 1 > startIndex)
        throw new ArgumentOutOfRangeException(nameof (count), SR.ArgumentOutOfRange_Count);
      if (anyOf.Length > 1)
        return this.LastIndexOfCharArray(anyOf, startIndex, count);
      return anyOf.Length == 1 ? this.LastIndexOf(anyOf[0], startIndex, count) : -1;
    }


    #nullable disable
    private unsafe int LastIndexOfCharArray(char[] anyOf, int startIndex, int count)
    {
      uint* charMap = (uint*) &new String.ProbabilisticMap();
      string.InitializeProbabilisticMap(charMap, (ReadOnlySpan<char>) anyOf);
      fixed (char* chPtr1 = &this._firstChar)
      {
        char* chPtr2 = chPtr1 + startIndex;
        while (count > 0)
        {
          int searchChar = (int) *chPtr2;
          if (string.IsCharBitSet(charMap, (byte) searchChar) && string.IsCharBitSet(charMap, (byte) (searchChar >> 8)) && string.ArrayContains((char) searchChar, anyOf))
            return (int) (chPtr2 - chPtr1);
          --count;
          --chPtr2;
        }
        return -1;
      }
    }


    #nullable enable
    /// <summary>Reports the zero-based index position of the last occurrence of a specified string within this instance.</summary>
    /// <param name="value">The string to seek.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="value" /> is <see langword="null" />.</exception>
    /// <returns>The zero-based starting index position of <paramref name="value" /> if that string is found, or -1 if it is not.</returns>
    public int LastIndexOf(string value) => this.LastIndexOf(value, this.Length - 1, this.Length, StringComparison.CurrentCulture);

    /// <summary>Reports the zero-based index position of the last occurrence of a specified string within this instance. The search starts at a specified character position and proceeds backward toward the beginning of the string.</summary>
    /// <param name="value">The string to seek.</param>
    /// <param name="startIndex">The search starting position. The search proceeds from <paramref name="startIndex" /> toward the beginning of this instance.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="value" /> is <see langword="null" />.</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">The current instance does not equal <see cref="F:System.String.Empty" />, and <paramref name="startIndex" /> is less than zero or greater than the length of the current instance.
    /// 
    /// -or-
    /// 
    /// The current instance equals <see cref="F:System.String.Empty" />, and <paramref name="startIndex" /> is less than -1 or greater than zero.</exception>
    /// <returns>The zero-based starting index position of <paramref name="value" /> if that string is found, or -1 if it is not found or if the current instance equals <see cref="F:System.String.Empty" />.</returns>
    public int LastIndexOf(string value, int startIndex) => this.LastIndexOf(value, startIndex, startIndex + 1, StringComparison.CurrentCulture);

    /// <summary>Reports the zero-based index position of the last occurrence of a specified string within this instance. The search starts at a specified character position and proceeds backward toward the beginning of the string for a specified number of character positions.</summary>
    /// <param name="value">The string to seek.</param>
    /// <param name="startIndex">The search starting position. The search proceeds from <paramref name="startIndex" /> toward the beginning of this instance.</param>
    /// <param name="count">The number of character positions to examine.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="value" /> is <see langword="null" />.</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    ///        <paramref name="count" /> is negative.
    /// 
    /// -or-
    /// 
    /// The current instance does not equal <see cref="F:System.String.Empty" />, and <paramref name="startIndex" /> is negative.
    /// 
    /// -or-
    /// 
    /// The current instance does not equal <see cref="F:System.String.Empty" />, and <paramref name="startIndex" /> is greater than the length of this instance.
    /// 
    /// -or-
    /// 
    /// The current instance does not equal <see cref="F:System.String.Empty" />, and <paramref name="startIndex" /> - <paramref name="count" />+ 1 specifies a position that is not within this instance.
    /// 
    /// -or-
    /// 
    /// The current instance equals <see cref="F:System.String.Empty" /> and <paramref name="start" /> is less than -1 or greater than zero.
    /// 
    /// -or-
    /// 
    /// The current instance equals <see cref="F:System.String.Empty" /> and <paramref name="count" /> is greater than 1.</exception>
    /// <returns>The zero-based starting index position of <paramref name="value" /> if that string is found, or -1 if it is not found or if the current instance equals <see cref="F:System.String.Empty" />.</returns>
    public int LastIndexOf(string value, int startIndex, int count) => this.LastIndexOf(value, startIndex, count, StringComparison.CurrentCulture);

    /// <summary>Reports the zero-based index of the last occurrence of a specified string within the current <see cref="T:System.String" /> object. A parameter specifies the type of search to use for the specified string.</summary>
    /// <param name="value">The string to seek.</param>
    /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="value" /> is <see langword="null" />.</exception>
    /// <exception cref="T:System.ArgumentException">
    /// <paramref name="comparisonType" /> is not a valid <see cref="T:System.StringComparison" /> value.</exception>
    /// <returns>The zero-based starting index position of the <paramref name="value" /> parameter if that string is found, or -1 if it is not.</returns>
    public int LastIndexOf(string value, StringComparison comparisonType) => this.LastIndexOf(value, this.Length - 1, this.Length, comparisonType);

    /// <summary>Reports the zero-based index of the last occurrence of a specified string within the current <see cref="T:System.String" /> object. The search starts at a specified character position and proceeds backward toward the beginning of the string. A parameter specifies the type of comparison to perform when searching for the specified string.</summary>
    /// <param name="value">The string to seek.</param>
    /// <param name="startIndex">The search starting position. The search proceeds from <paramref name="startIndex" /> toward the beginning of this instance.</param>
    /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="value" /> is <see langword="null" />.</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">The current instance does not equal <see cref="F:System.String.Empty" />, and <paramref name="startIndex" /> is less than zero or greater than the length of the current instance.
    /// 
    /// -or-
    /// 
    /// The current instance equals <see cref="F:System.String.Empty" />, and <paramref name="startIndex" /> is less than -1 or greater than zero.</exception>
    /// <exception cref="T:System.ArgumentException">
    /// <paramref name="comparisonType" /> is not a valid <see cref="T:System.StringComparison" /> value.</exception>
    /// <returns>The zero-based starting index position of the <paramref name="value" /> parameter if that string is found, or -1 if it is not found or if the current instance equals <see cref="F:System.String.Empty" />.</returns>
    public int LastIndexOf(string value, int startIndex, StringComparison comparisonType) => this.LastIndexOf(value, startIndex, startIndex + 1, comparisonType);

    /// <summary>Reports the zero-based index position of the last occurrence of a specified string within this instance. The search starts at a specified character position and proceeds backward toward the beginning of the string for the specified number of character positions. A parameter specifies the type of comparison to perform when searching for the specified string.</summary>
    /// <param name="value">The string to seek.</param>
    /// <param name="startIndex">The search starting position. The search proceeds from <paramref name="startIndex" /> toward the beginning of this instance.</param>
    /// <param name="count">The number of character positions to examine.</param>
    /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="value" /> is <see langword="null" />.</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    ///        <paramref name="count" /> is negative.
    /// 
    /// -or-
    /// 
    /// The current instance does not equal <see cref="F:System.String.Empty" />, and <paramref name="startIndex" /> is negative.
    /// 
    /// -or-
    /// 
    /// The current instance does not equal <see cref="F:System.String.Empty" />, and <paramref name="startIndex" /> is greater than the length of this instance.
    /// 
    /// -or-
    /// 
    /// The current instance does not equal <see cref="F:System.String.Empty" />, and <paramref name="startIndex" /> + 1 - <paramref name="count" /> specifies a position that is not within this instance.
    /// 
    /// -or-
    /// 
    /// The current instance equals <see cref="F:System.String.Empty" /> and <paramref name="start" /> is less than -1 or greater than zero.
    /// 
    /// -or-
    /// 
    /// The current instance equals <see cref="F:System.String.Empty" /> and <paramref name="count" /> is greater than 1.</exception>
    /// <exception cref="T:System.ArgumentException">
    /// <paramref name="comparisonType" /> is not a valid <see cref="T:System.StringComparison" /> value.</exception>
    /// <returns>The zero-based starting index position of the <paramref name="value" /> parameter if that string is found, or -1 if it is not found or if the current instance equals <see cref="F:System.String.Empty" />.</returns>
    public int LastIndexOf(
      string value,
      int startIndex,
      int count,
      StringComparison comparisonType)
    {
      switch (comparisonType)
      {
        case StringComparison.CurrentCulture:
        case StringComparison.CurrentCultureIgnoreCase:
          return CultureInfo.CurrentCulture.CompareInfo.LastIndexOf(this, value, startIndex, count, string.GetCaseCompareOfComparisonCulture(comparisonType));
        case StringComparison.InvariantCulture:
        case StringComparison.InvariantCultureIgnoreCase:
          return CompareInfo.Invariant.LastIndexOf(this, value, startIndex, count, string.GetCaseCompareOfComparisonCulture(comparisonType));
        case StringComparison.Ordinal:
        case StringComparison.OrdinalIgnoreCase:
          return CompareInfo.Invariant.LastIndexOf(this, value, startIndex, count, string.GetCompareOptionsFromOrdinalStringComparison(comparisonType));
        default:
          throw value == null ? (ArgumentException) new ArgumentNullException(nameof (value)) : new ArgumentException(SR.NotSupported_StringComparison, nameof (comparisonType));
      }
    }


    #nullable disable
    [StructLayout(LayoutKind.Explicit, Size = 32)]
    private struct ProbabilisticMap
    {
    }
  }
}
