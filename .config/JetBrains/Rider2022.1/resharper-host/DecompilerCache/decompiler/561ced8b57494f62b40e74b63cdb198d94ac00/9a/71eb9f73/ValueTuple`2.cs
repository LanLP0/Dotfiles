// Decompiled with JetBrains decompiler
// Type: System.ValueTuple`2
// Assembly: System.Private.CoreLib, Version=6.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e
// MVID: 561CED8B-5749-4F62-B40E-74B63CDB198D
// Assembly location: /usr/share/dotnet/shared/Microsoft.NETCore.App/6.0.2/System.Private.CoreLib.dll
// XML documentation location: /usr/share/dotnet/packs/Microsoft.NETCore.App.Ref/6.0.2/ref/net6.0/System.Runtime.xml

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;


#nullable enable
namespace System
{
  /// <summary>Represents a value tuple with 2 components.</summary>
  /// <typeparam name="T1">The type of the value tuple's first element.</typeparam>
  /// <typeparam name="T2">The type of the value tuple's second element.</typeparam>
  [TypeForwardedFrom("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
  [Serializable]
  [StructLayout(LayoutKind.Auto)]
  public struct ValueTuple<T1, T2> : 
    IEquatable<(T1, T2)>,
    IStructuralEquatable,
    IStructuralComparable,
    IComparable,
    IComparable<(T1, T2)>,
    IValueTupleInternal,
    ITuple
  {
    /// <summary>Gets the value of the current <see cref="T:System.ValueTuple`2" /> instance's first element.</summary>
    public T1 Item1;
    /// <summary>Gets the value of the current <see cref="T:System.ValueTuple`2" /> instance's second element.</summary>
    public T2 Item2;

    /// <summary>Initializes a new <see cref="T:System.ValueTuple`2" /> instance.</summary>
    /// <param name="item1">The value tuple's first element.</param>
    /// <param name="item2">The value tuple's second element.</param>
    public ValueTuple(T1 item1, T2 item2)
    {
      this.Item1 = item1;
      this.Item2 = item2;
    }

    /// <summary>Returns a value that indicates whether the current <see cref="T:System.ValueTuple`2" /> instance is equal to a specified object.</summary>
    /// <param name="obj">The object to compare with this instance.</param>
    /// <returns>
    /// <see langword="true" /> if the current instance is equal to the specified object; otherwise, <see langword="false" />.</returns>
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is (T1, T2) other && this.Equals(other);

    /// <summary>Returns a value that indicates whether the current <see cref="T:System.ValueTuple`2" /> instance is equal to a specified <see cref="T:System.ValueTuple`2" /> instance.</summary>
    /// <param name="other">The value tuple to compare with this instance.</param>
    /// <returns>
    /// <see langword="true" /> if the current instance is equal to the specified tuple; otherwise, <see langword="false" />.</returns>
    public bool Equals((T1, T2) other) => EqualityComparer<T1>.Default.Equals(this.Item1, other.Item1) && EqualityComparer<T2>.Default.Equals(this.Item2, other.Item2);


    #nullable disable
    /// <summary>Returns a value that indicates whether the current <see cref="T:System.ValueTuple`2" /> instance is equal to a specified object based on a specified comparison method.</summary>
    /// <param name="other">The object to compare with this instance.</param>
    /// <param name="comparer">An object that defines the method to use to evaluate whether the two objects are equal.</param>
    /// <returns>
    /// <see langword="true" /> if the current instance is equal to the specified objects; otherwise, <see langword="false" />.</returns>
    bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer) => other is (T1, T2) valueTuple && comparer.Equals((object) this.Item1, (object) valueTuple.Item1) && comparer.Equals((object) this.Item2, (object) valueTuple.Item2);

    /// <summary>Compares the current <see cref="T:System.ValueTuple`2" /> instance to a specified object by using a specified comparer and returns an integer that indicates whether the current object is before, after, or in the same position as the specified object in the sort order.</summary>
    /// <param name="other">The object to compare with the current instance.</param>
    /// <returns>A signed integer that indicates the relative position of this instance and <paramref name="obj" /> in the sort order, as shown in the following table.
    /// 
    /// <list type="table"><listheader><term> Value</term><description> Description</description></listheader><item><term> A negative integer</term><description> This instance precedes <paramref name="other" />.</description></item><item><term> Zero</term><description> This instance and <paramref name="other" /> have the same position in the sort order.</description></item><item><term> A positive integer</term><description> This instance follows <paramref name="other" />.</description></item></list></returns>
    int IComparable.CompareTo(object other)
    {
      if (other != null)
      {
        if (other is (T1, T2) other1)
          return this.CompareTo(other1);
        ThrowHelper.ThrowArgumentException_TupleIncorrectType((object) this);
      }
      return 1;
    }


    #nullable enable
    /// <summary>Compares the current <see cref="T:System.ValueTuple`2" /> instance to a specified <see cref="T:System.ValueTuple`2" /> instance.</summary>
    /// <param name="other">The tuple to compare with this instance.</param>
    /// <returns>A signed integer that indicates the relative position of this instance and <paramref name="other" /> in the sort order, as shown in the following table.
    /// 
    /// <list type="table"><listheader><term> Value</term><description> Description</description></listheader><item><term> A negative integer</term><description> This instance precedes <paramref name="other" />.</description></item><item><term> Zero</term><description> This instance and <paramref name="other" /> have the same position in the sort order.</description></item><item><term> A positive integer</term><description> This instance follows <paramref name="other" />.</description></item></list></returns>
    public int CompareTo((T1, T2) other)
    {
      int num = Comparer<T1>.Default.Compare(this.Item1, other.Item1);
      return num != 0 ? num : Comparer<T2>.Default.Compare(this.Item2, other.Item2);
    }


    #nullable disable
    /// <summary>Compares the current <see cref="T:System.ValueTuple`2" /> instance to a specified object by using a specified comparer and returns an integer that indicates whether the current object is before, after, or in the same position as the specified object in the sort order.</summary>
    /// <param name="other">The object to compare with the current instance.</param>
    /// <param name="comparer">An object that provides custom rules for comparison.</param>
    /// <returns>A signed integer that indicates the relative position of this instance and <paramref name="other" /> in the sort order, as shown in the following table.
    /// 
    /// <list type="table"><listheader><term> Value</term><description> Description</description></listheader><item><term> A negative integer</term><description> This instance precedes <paramref name="other" />.</description></item><item><term> Zero</term><description> This instance and <paramref name="other" /> have the same position in the sort order.</description></item><item><term> A positive integer</term><description> This instance follows <paramref name="other" />.</description></item></list></returns>
    int IStructuralComparable.CompareTo(object other, IComparer comparer)
    {
      if (other != null)
      {
        if (other is (T1, T2) valueTuple)
        {
          int num = comparer.Compare((object) this.Item1, (object) valueTuple.Item1);
          return num != 0 ? num : comparer.Compare((object) this.Item2, (object) valueTuple.Item2);
        }
        ThrowHelper.ThrowArgumentException_TupleIncorrectType((object) this);
      }
      return 1;
    }

    /// <summary>Calculates the hash code for the current <see cref="T:System.ValueTuple`2" /> instance.</summary>
    /// <returns>The hash code for the current <see cref="T:System.ValueTuple`2" /> instance.</returns>
    public override int GetHashCode()
    {
      // ISSUE: explicit reference operation
      ref T1 local1 = @this.Item1;
      int num1;
      if ((object) default (T1) == null)
      {
        T1 obj = local1;
        ref T1 local2 = ref obj;
        if ((object) obj == null)
        {
          num1 = 0;
          goto label_4;
        }
        else
          local1 = ref local2;
      }
      num1 = local1.GetHashCode();
label_4:
      // ISSUE: explicit reference operation
      ref T2 local3 = @this.Item2;
      int num2;
      if ((object) default (T2) == null)
      {
        T2 obj = local3;
        ref T2 local4 = ref obj;
        if ((object) obj == null)
        {
          num2 = 0;
          goto label_8;
        }
        else
          local3 = ref local4;
      }
      num2 = local3.GetHashCode();
label_8:
      return HashCode.Combine<int, int>(num1, num2);
    }

    /// <summary>Calculates the hash code for the current <see cref="T:System.ValueTuple`2" /> instance by using a specified computation method.</summary>
    /// <param name="comparer">An object whose <see cref="M:System.Collections.IEqualityComparer.GetHashCode(System.Object)" /> method calculates the hash code of the current <see cref="T:System.ValueTuple`2" /> instance.</param>
    /// <returns>A 32-bit signed integer hash code.</returns>
    int IStructuralEquatable.GetHashCode(IEqualityComparer comparer) => this.GetHashCodeCore(comparer);

    private int GetHashCodeCore(IEqualityComparer comparer) => HashCode.Combine<int, int>(comparer.GetHashCode((object) this.Item1), comparer.GetHashCode((object) this.Item2));

    int IValueTupleInternal.GetHashCode(IEqualityComparer comparer) => this.GetHashCodeCore(comparer);


    #nullable enable
    /// <summary>Returns a string that represents the value of this <see cref="T:System.ValueTuple`2" /> instance.</summary>
    /// <returns>The string representation of this <see cref="T:System.ValueTuple`2" /> instance.</returns>
    public override string ToString()
    {
      string[] strArray = new string[5]
      {
        "(",
        null,
        null,
        null,
        null
      };
      // ISSUE: explicit reference operation
      ref T1 local1 = @this.Item1;
      string str1;
      if ((object) default (T1) == null)
      {
        T1 obj = local1;
        ref T1 local2 = ref obj;
        if ((object) obj == null)
        {
          str1 = (string) null;
          goto label_4;
        }
        else
          local1 = ref local2;
      }
      str1 = local1.ToString();
label_4:
      strArray[1] = str1;
      strArray[2] = ", ";
      // ISSUE: explicit reference operation
      ref T2 local3 = @this.Item2;
      string str2;
      if ((object) default (T2) == null)
      {
        T2 obj = local3;
        ref T2 local4 = ref obj;
        if ((object) obj == null)
        {
          str2 = (string) null;
          goto label_8;
        }
        else
          local3 = ref local4;
      }
      str2 = local3.ToString();
label_8:
      strArray[3] = str2;
      strArray[4] = ")";
      return string.Concat(strArray);
    }


    #nullable disable
    string IValueTupleInternal.ToStringEnd()
    {
      // ISSUE: explicit reference operation
      ref T1 local1 = @this.Item1;
      string str1;
      if ((object) default (T1) == null)
      {
        T1 obj = local1;
        ref T1 local2 = ref obj;
        if ((object) obj == null)
        {
          str1 = (string) null;
          goto label_4;
        }
        else
          local1 = ref local2;
      }
      str1 = local1.ToString();
label_4:
      // ISSUE: explicit reference operation
      ref T2 local3 = @this.Item2;
      string str2;
      if ((object) default (T2) == null)
      {
        T2 obj = local3;
        ref T2 local4 = ref obj;
        if ((object) obj == null)
        {
          str2 = (string) null;
          goto label_8;
        }
        else
          local3 = ref local4;
      }
      str2 = local3.ToString();
label_8:
      return str1 + ", " + str2 + ")";
    }

    /// <summary>Gets the number of elements in the <see langword="ValueTuple" />.</summary>
    /// <returns>2, the number of elements in a <see cref="T:System.ValueTuple`2" /> object.</returns>
    int ITuple.Length => 2;


    #nullable enable
    /// <summary>Gets the value of the specified <see langword="ValueTuple" /> element.</summary>
    /// <param name="index">The index of the specified <see langword="ValueTuple" /> element. <paramref name="index" /> can range from 0 to 1.</param>
    /// <exception cref="T:System.IndexOutOfRangeException">
    /// <paramref name="index" /> is less than 0 or greater than 1.</exception>
    /// <returns>The value of the <see langword="ValueTuple" /> element at the specified position.</returns>
    object? ITuple.this[int index]
    {
      get
      {
        if (index == 0)
          return (object) this.Item1;
        if (index == 1)
          return (object) this.Item2;
        throw new IndexOutOfRangeException();
      }
    }
  }
}
