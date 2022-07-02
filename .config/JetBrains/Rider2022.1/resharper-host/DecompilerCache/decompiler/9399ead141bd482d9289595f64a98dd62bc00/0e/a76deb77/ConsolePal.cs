// Decompiled with JetBrains decompiler
// Type: System.ConsolePal
// Assembly: System.Console, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9399EAD1-41BD-482D-9289-595F64A98DD6
// Assembly location: /usr/share/dotnet/shared/Microsoft.NETCore.App/6.0.2/System.Console.dll
// XML documentation location: /usr/share/dotnet/packs/Microsoft.NETCore.App.Ref/6.0.2/ref/net6.0/System.Console.xml

using Microsoft.Win32.SafeHandles;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace System
{
  internal static class ConsolePal
  {
    private static int s_cursorVersion;
    private static int s_cursorLeft;
    private static int s_cursorTop;
    private static int s_windowWidth;
    private static int s_windowHeight;
    private static int s_invalidateCachedSettings = 1;
    private static volatile int s_emitAnsiColorCodes = -1;
    private static SyncTextReader s_stdInReader;
    private static ConsoleColor s_trackedForegroundColor = ~ConsoleColor.Black;
    private static ConsoleColor s_trackedBackgroundColor = ~ConsoleColor.Black;
    private static bool s_everReceivedCursorPositionResponse;
    private static bool s_firstCursorPositionRequest = true;
    private static readonly int[] _consoleColorToAnsiCode = new int[16]
    {
      0,
      4,
      2,
      6,
      1,
      5,
      3,
      7,
      8,
      12,
      10,
      14,
      9,
      13,
      11,
      15
    };
    private static readonly string[,] s_fgbgAndColorStrings = new string[2, 16];
    private static volatile bool s_initialized;
    internal static byte s_posixDisableValue;
    private static byte s_veraseCharacter;
    internal static byte s_veolCharacter;
    internal static byte s_veol2Character;
    internal static byte s_veofCharacter;

    public static Stream OpenStandardInput() => (Stream) new ConsolePal.UnixConsoleStream(SafeFileHandleHelper.Open((Func<SafeFileHandle>) (() => Interop.Sys.Dup(Interop.Sys.FileDescriptors.STDIN_FILENO))), FileAccess.Read, !Console.IsInputRedirected);

    public static Stream OpenStandardOutput() => (Stream) new ConsolePal.UnixConsoleStream(SafeFileHandleHelper.Open((Func<SafeFileHandle>) (() => Interop.Sys.Dup(Interop.Sys.FileDescriptors.STDOUT_FILENO))), FileAccess.Write);

    public static Stream OpenStandardError() => (Stream) new ConsolePal.UnixConsoleStream(SafeFileHandleHelper.Open((Func<SafeFileHandle>) (() => Interop.Sys.Dup(Interop.Sys.FileDescriptors.STDERR_FILENO))), FileAccess.Write);

    public static Encoding InputEncoding => ConsolePal.GetConsoleEncoding();

    public static Encoding OutputEncoding => ConsolePal.GetConsoleEncoding();

    internal static SyncTextReader StdInReader
    {
      get
      {
        return Volatile.Read<SyncTextReader>(ref ConsolePal.s_stdInReader) ?? EnsureInitialized();

        static SyncTextReader EnsureInitialized()
        {
          ConsolePal.EnsureConsoleInitialized();
          SyncTextReader synchronizedTextReader = SyncTextReader.GetSynchronizedTextReader((TextReader) new System.IO.StdInReader(Console.InputEncoding, (int) byte.MaxValue));
          Interlocked.CompareExchange<SyncTextReader>(ref ConsolePal.s_stdInReader, synchronizedTextReader, (SyncTextReader) null);
          return ConsolePal.s_stdInReader;
        }
      }
    }

    internal static TextReader GetOrCreateReader()
    {
      if (!Console.IsInputRedirected)
        return (TextReader) ConsolePal.StdInReader;
      Stream stream = ConsolePal.OpenStandardInput();
      return (TextReader) SyncTextReader.GetSynchronizedTextReader(stream == Stream.Null ? (TextReader) StreamReader.Null : (TextReader) new StreamReader(stream, Console.InputEncoding, false, 4096, true));
    }

    public static bool KeyAvailable => ConsolePal.StdInReader.KeyAvailable;

    public static ConsoleKeyInfo ReadKey(bool intercept)
    {
      if (Console.IsInputRedirected)
        throw new InvalidOperationException(SR.InvalidOperation_ConsoleReadKeyOnFile);
      bool previouslyProcessed;
      ConsoleKeyInfo consoleKeyInfo = ConsolePal.StdInReader.ReadKey(out previouslyProcessed);
      if (!intercept && !previouslyProcessed && consoleKeyInfo.KeyChar != char.MinValue)
        Console.Write(consoleKeyInfo.KeyChar);
      return consoleKeyInfo;
    }

    public static bool TreatControlCAsInput
    {
      get
      {
        if (Console.IsInputRedirected)
          return false;
        ConsolePal.EnsureConsoleInitialized();
        return Interop.Sys.GetSignalForBreak() == 0;
      }
      set
      {
        if (Console.IsInputRedirected)
          return;
        ConsolePal.EnsureConsoleInitialized();
        if (Interop.Sys.SetSignalForBreak(Convert.ToInt32(!value)) == 0)
          throw Interop.GetExceptionForIoErrno(Interop.Sys.GetLastErrorInfo());
      }
    }

    public static ConsoleColor ForegroundColor
    {
      get => ConsolePal.s_trackedForegroundColor;
      set => ConsolePal.RefreshColors(ref ConsolePal.s_trackedForegroundColor, value);
    }

    public static ConsoleColor BackgroundColor
    {
      get => ConsolePal.s_trackedBackgroundColor;
      set => ConsolePal.RefreshColors(ref ConsolePal.s_trackedBackgroundColor, value);
    }

    public static void ResetColor()
    {
      TextWriter textWriter = Console.Out;
      bool lockTaken = false;
      try
      {
        Monitor.Enter((object) textWriter, ref lockTaken);
        ConsolePal.s_trackedForegroundColor = ~ConsoleColor.Black;
        ConsolePal.s_trackedBackgroundColor = ~ConsoleColor.Black;
        ConsolePal.WriteResetColorString();
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit((object) textWriter);
      }
    }

    public static bool NumberLock => throw new PlatformNotSupportedException();

    public static bool CapsLock => throw new PlatformNotSupportedException();

    public static int CursorSize
    {
      get => 100;
      set => throw new PlatformNotSupportedException();
    }

    public static string Title
    {
      get => throw new PlatformNotSupportedException();
      set
      {
        if (Console.IsOutputRedirected)
          return;
        string title = ConsolePal.TerminalFormatStrings.Instance.Title;
        if (string.IsNullOrEmpty(title))
          return;
        ConsolePal.WriteStdoutAnsiString(TermInfo.ParameterizedStrings.Evaluate(title, (TermInfo.ParameterizedStrings.FormatParam) value), false);
      }
    }

    public static void Beep()
    {
      if (Console.IsOutputRedirected)
        return;
      ConsolePal.WriteStdoutAnsiString(ConsolePal.TerminalFormatStrings.Instance.Bell, false);
    }

    public static void Beep(int frequency, int duration) => throw new PlatformNotSupportedException();

    public static void Clear()
    {
      if (Console.IsOutputRedirected)
        return;
      ConsolePal.WriteStdoutAnsiString(ConsolePal.TerminalFormatStrings.Instance.Clear);
    }

    public static void SetCursorPosition(int left, int top)
    {
      if (Console.IsOutputRedirected)
        return;
      TextWriter textWriter = Console.Out;
      bool lockTaken = false;
      try
      {
        Monitor.Enter((object) textWriter, ref lockTaken);
        int left1;
        int top1;
        if (ConsolePal.TryGetCachedCursorPosition(out left1, out top1) && left == left1 && top == top1)
          return;
        string cursorAddress = ConsolePal.TerminalFormatStrings.Instance.CursorAddress;
        if (!string.IsNullOrEmpty(cursorAddress))
          ConsolePal.WriteStdoutAnsiString(TermInfo.ParameterizedStrings.Evaluate(cursorAddress, (TermInfo.ParameterizedStrings.FormatParam) top, (TermInfo.ParameterizedStrings.FormatParam) left));
        ConsolePal.SetCachedCursorPosition(left, top);
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit((object) textWriter);
      }
    }

    private static void SetCachedCursorPosition(int left, int top, int? version = null)
    {
      int num;
      if (version.HasValue)
      {
        int? nullable = version;
        int cursorVersion = ConsolePal.s_cursorVersion;
        num = nullable.GetValueOrDefault() == cursorVersion & nullable.HasValue ? 1 : 0;
      }
      else
        num = 1;
      if (num != 0)
      {
        ConsolePal.s_cursorLeft = left;
        ConsolePal.s_cursorTop = top;
        ++ConsolePal.s_cursorVersion;
      }
      else
        ConsolePal.InvalidateCachedCursorPosition();
    }

    private static void InvalidateCachedCursorPosition()
    {
      ConsolePal.s_cursorLeft = -1;
      ++ConsolePal.s_cursorVersion;
    }

    private static bool TryGetCachedCursorPosition(out int left, out int top)
    {
      ConsolePal.CheckTerminalSettingsInvalidated();
      bool cachedCursorPosition = ConsolePal.s_cursorLeft >= 0;
      if (cachedCursorPosition)
      {
        left = ConsolePal.s_cursorLeft;
        top = ConsolePal.s_cursorTop;
      }
      else
      {
        left = 0;
        top = 0;
      }
      return cachedCursorPosition;
    }

    public static int BufferWidth
    {
      get => ConsolePal.WindowWidth;
      set => throw new PlatformNotSupportedException();
    }

    public static int BufferHeight
    {
      get => ConsolePal.WindowHeight;
      set => throw new PlatformNotSupportedException();
    }

    public static void SetBufferSize(int width, int height) => throw new PlatformNotSupportedException();

    public static int LargestWindowWidth => ConsolePal.WindowWidth;

    public static int LargestWindowHeight => ConsolePal.WindowHeight;

    public static int WindowLeft
    {
      get => 0;
      set => throw new PlatformNotSupportedException();
    }

    public static int WindowTop
    {
      get => 0;
      set => throw new PlatformNotSupportedException();
    }

    public static int WindowWidth
    {
      get
      {
        int width;
        ConsolePal.GetWindowSize(out width, out int _);
        return width;
      }
      set => throw new PlatformNotSupportedException();
    }

    public static int WindowHeight
    {
      get
      {
        int height;
        ConsolePal.GetWindowSize(out int _, out height);
        return height;
      }
      set => throw new PlatformNotSupportedException();
    }

    private static void GetWindowSize(out int width, out int height)
    {
      TextWriter textWriter = Console.Out;
      bool lockTaken = false;
      try
      {
        Monitor.Enter((object) textWriter, ref lockTaken);
        ConsolePal.CheckTerminalSettingsInvalidated();
        if (ConsolePal.s_windowWidth == -1)
        {
          Interop.Sys.WinSize winSize;
          if (Interop.Sys.GetWindowSize(out winSize) == 0)
          {
            ConsolePal.s_windowWidth = (int) winSize.Col;
            ConsolePal.s_windowHeight = (int) winSize.Row;
          }
          else
          {
            ConsolePal.s_windowWidth = ConsolePal.TerminalFormatStrings.Instance.Columns;
            ConsolePal.s_windowHeight = ConsolePal.TerminalFormatStrings.Instance.Lines;
          }
        }
        width = ConsolePal.s_windowWidth;
        height = ConsolePal.s_windowHeight;
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit((object) textWriter);
      }
    }

    public static void SetWindowPosition(int left, int top) => throw new PlatformNotSupportedException();

    public static void SetWindowSize(int width, int height) => throw new PlatformNotSupportedException();

    public static bool CursorVisible
    {
      get => throw new PlatformNotSupportedException();
      set
      {
        if (Console.IsOutputRedirected)
          return;
        ConsolePal.WriteStdoutAnsiString(value ? ConsolePal.TerminalFormatStrings.Instance.CursorVisible : ConsolePal.TerminalFormatStrings.Instance.CursorInvisible);
      }
    }

    public static (int Left, int Top) GetCursorPosition()
    {
      int left;
      int top;
      ConsolePal.TryGetCursorPosition(out left, out top);
      return (left, top);
    }

    internal static unsafe bool TryGetCursorPosition(
      out int left,
      out int top,
      bool reinitializeForRead = false)
    {
      left = top = 0;
      if (Console.IsInputRedirected || Console.IsOutputRedirected)
        return false;
      TextWriter textWriter1 = Console.Out;
      bool lockTaken1 = false;
      int cursorVersion;
      try
      {
        Monitor.Enter((object) textWriter1, ref lockTaken1);
        if (ConsolePal.TryGetCachedCursorPosition(out left, out top))
          return true;
        cursorVersion = ConsolePal.s_cursorVersion;
      }
      finally
      {
        if (lockTaken1)
          Monitor.Exit((object) textWriter1);
      }
      int start = 0;
      // ISSUE: untyped stack allocation
      Span<byte> dst = new Span<byte>((void*) __untypedstackalloc(new IntPtr(256)), 256);
      SyncTextReader stdInReader = ConsolePal.StdInReader;
      bool lockTaken2 = false;
      try
      {
        Monitor.Enter((object) stdInReader, ref lockTaken2);
        Interop.Sys.InitializeConsoleBeforeRead(ConsolePal.s_everReceivedCursorPositionResponse ? (byte) 1 : (byte) 0, ConsolePal.s_firstCursorPositionRequest ? (byte) 100 : (byte) 10);
        try
        {
          ConsolePal.WriteStdoutAnsiString("\u001B[6n", false);
          System.IO.StdInReader inner = ConsolePal.StdInReader.Inner;
          int foundPos1;
          int foundPos2;
          if (!AppendToStdInReaderUntil((byte) 27, inner, dst, ref start, out int _) || !BufferUntil((byte) 91, inner, ref dst, ref start, out int _) || !BufferUntil((byte) 59, inner, ref dst, ref start, out foundPos1) || !BufferUntil((byte) 82, inner, ref dst, ref start, out foundPos2))
          {
            TransferBytes((ReadOnlySpan<byte>) dst.Slice(start), inner);
            return false;
          }
          int num = dst.Slice(0, foundPos1).LastIndexOf<byte>((byte) 91);
          int length = dst.Slice(0, num).LastIndexOf<byte>((byte) 27);
          TransferBytes((ReadOnlySpan<byte>) dst.Slice(0, length), inner);
          TransferBytes((ReadOnlySpan<byte>) dst.Slice(length + 1, num - (length + 1)), inner);
          ReadRowOrCol(num, foundPos1, inner, (ReadOnlySpan<byte>) dst, ref top);
          ReadRowOrCol(foundPos1, foundPos2, inner, (ReadOnlySpan<byte>) dst, ref left);
          ConsolePal.s_everReceivedCursorPositionResponse = true;
        }
        finally
        {
          if (reinitializeForRead)
            Interop.Sys.InitializeConsoleBeforeRead();
          else
            Interop.Sys.UninitializeConsoleAfterRead();
          ConsolePal.s_firstCursorPositionRequest = false;
        }
      }
      finally
      {
        if (lockTaken2)
          Monitor.Exit((object) stdInReader);
      }
      TextWriter textWriter2 = Console.Out;
      bool lockTaken3 = false;
      try
      {
        Monitor.Enter((object) textWriter2, ref lockTaken3);
        ConsolePal.SetCachedCursorPosition(left, top, new int?(cursorVersion));
        return true;
      }
      finally
      {
        if (lockTaken3)
          Monitor.Exit((object) textWriter2);
      }

      static unsafe bool BufferUntil(
        byte toFind,
        System.IO.StdInReader src,
        ref Span<byte> dst,
        ref int dstPos,
        out int foundPos)
      {
        byte num;
        while (src.ReadStdin(&num, 1) == 1)
        {
          if (dstPos == dst.Length)
          {
            byte[] destination = new byte[dst.Length * 2];
            dst.CopyTo((Span<byte>) destination);
            dst = (Span<byte>) destination;
          }
          dst[dstPos++] = num;
          if ((int) num == (int) toFind)
          {
            foundPos = dstPos - 1;
            return true;
          }
        }
        foundPos = -1;
        return false;
      }

      static unsafe bool AppendToStdInReaderUntil(
        byte toFind,
        System.IO.StdInReader reader,
        Span<byte> foundByteDst,
        ref int foundByteDstPos,
        out int foundPos)
      {
        byte reference;
        while (reader.ReadStdin(&reference, 1) == 1)
        {
          if ((int) reference == (int) toFind)
          {
            foundPos = foundByteDstPos;
            foundByteDst[foundByteDstPos++] = reference;
            return true;
          }
          reader.AppendExtraBuffer(MemoryMarshal.CreateReadOnlySpan<byte>(ref reference, 1));
        }
        foundPos = -1;
        return false;
      }

      static void ReadRowOrCol(
        int startExclusive,
        int endExclusive,
        System.IO.StdInReader reader,
        ReadOnlySpan<byte> source,
        ref int result)
      {
        int num = 0;
        for (int index = startExclusive + 1; index < endExclusive; ++index)
        {
          byte reference = source[index];
          if (ConsolePal.IsDigit(reference))
          {
            try
            {
              num = checked (num * 10 + (int) reference - 48);
            }
            catch (OverflowException ex)
            {
            }
          }
          else
            reader.AppendExtraBuffer(MemoryMarshal.CreateReadOnlySpan<byte>(ref reference, 1));
        }
        if (num < 1)
          return;
        result = num - 1;
      }

      static void TransferBytes(ReadOnlySpan<byte> src, System.IO.StdInReader dst)
      {
        for (int start = 0; start < src.Length; ++start)
          dst.AppendExtraBuffer(src.Slice(start, 1));
      }
    }

    public static void MoveBufferArea(
      int sourceLeft,
      int sourceTop,
      int sourceWidth,
      int sourceHeight,
      int targetLeft,
      int targetTop,
      char sourceChar,
      ConsoleColor sourceForeColor,
      ConsoleColor sourceBackColor)
    {
      throw new PlatformNotSupportedException();
    }

    private static bool IsDigit(byte c) => c >= (byte) 48 && c <= (byte) 57;

    private static bool IsHandleRedirected(SafeFileHandle fd) => !Interop.Sys.IsATty(fd);

    public static bool IsInputRedirectedCore() => ConsolePal.IsHandleRedirected(Interop.Sys.FileDescriptors.STDIN_FILENO);

    public static bool IsOutputRedirectedCore() => ConsolePal.IsHandleRedirected(Interop.Sys.FileDescriptors.STDOUT_FILENO);

    public static bool IsErrorRedirectedCore() => ConsolePal.IsHandleRedirected(Interop.Sys.FileDescriptors.STDERR_FILENO);

    private static Encoding GetConsoleEncoding()
    {
      Encoding encodingFromCharset = EncodingHelper.GetEncodingFromCharset();
      return encodingFromCharset == null ? Encoding.Default : encodingFromCharset.RemovePreamble();
    }

    public static void SetConsoleInputEncoding(Encoding enc)
    {
    }

    public static void SetConsoleOutputEncoding(Encoding enc)
    {
    }

    private static void RefreshColors(ref ConsoleColor toChange, ConsoleColor value)
    {
      if ((value & ~ConsoleColor.White) != ConsoleColor.Black && value != ~ConsoleColor.Black)
        throw new ArgumentException(SR.Arg_InvalidConsoleColor);
      TextWriter textWriter = Console.Out;
      bool lockTaken = false;
      try
      {
        Monitor.Enter((object) textWriter, ref lockTaken);
        toChange = value;
        ConsolePal.WriteResetColorString();
        if (ConsolePal.s_trackedForegroundColor != ~ConsoleColor.Black)
          ConsolePal.WriteSetColorString(true, ConsolePal.s_trackedForegroundColor);
        if (ConsolePal.s_trackedBackgroundColor == ~ConsoleColor.Black)
          return;
        ConsolePal.WriteSetColorString(false, ConsolePal.s_trackedBackgroundColor);
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit((object) textWriter);
      }
    }

    private static void WriteSetColorString(bool foreground, ConsoleColor color)
    {
      if (!ConsolePal.EmitAnsiColorCodes)
        return;
      int index1 = foreground ? 0 : 1;
      int index2 = (int) color;
      string fgbgAndColorString = ConsolePal.s_fgbgAndColorStrings[index1, index2];
      if (fgbgAndColorString != null)
      {
        ConsolePal.WriteStdoutAnsiString(fgbgAndColorString);
      }
      else
      {
        string format = foreground ? ConsolePal.TerminalFormatStrings.Instance.Foreground : ConsolePal.TerminalFormatStrings.Instance.Background;
        if (string.IsNullOrEmpty(format))
          return;
        int maxColors = ConsolePal.TerminalFormatStrings.Instance.MaxColors;
        if (maxColors <= 0)
          return;
        int num = ConsolePal._consoleColorToAnsiCode[index2] % maxColors;
        string str = TermInfo.ParameterizedStrings.Evaluate(format, (TermInfo.ParameterizedStrings.FormatParam) num);
        ConsolePal.WriteStdoutAnsiString(str);
        ConsolePal.s_fgbgAndColorStrings[index1, index2] = str;
      }
    }

    private static void WriteResetColorString()
    {
      if (!ConsolePal.EmitAnsiColorCodes)
        return;
      ConsolePal.WriteStdoutAnsiString(ConsolePal.TerminalFormatStrings.Instance.Reset);
    }

    private static bool EmitAnsiColorCodes
    {
      get
      {
        int emitAnsiColorCodes1 = ConsolePal.s_emitAnsiColorCodes;
        if (emitAnsiColorCodes1 != -1)
          return Convert.ToBoolean(emitAnsiColorCodes1);
        bool emitAnsiColorCodes2;
        if (!Console.IsOutputRedirected)
        {
          emitAnsiColorCodes2 = Environment.GetEnvironmentVariable("NO_COLOR") == null;
        }
        else
        {
          string environmentVariable = Environment.GetEnvironmentVariable("DOTNET_SYSTEM_CONSOLE_ALLOW_ANSI_COLOR_REDIRECTION");
          emitAnsiColorCodes2 = environmentVariable != null && (environmentVariable == "1" || environmentVariable.Equals("true", StringComparison.OrdinalIgnoreCase));
        }
        ConsolePal.s_emitAnsiColorCodes = Convert.ToInt32(emitAnsiColorCodes2);
        return emitAnsiColorCodes2;
      }
    }

    public static bool TryGetSpecialConsoleKey(
      char[] givenChars,
      int startIndex,
      int endIndex,
      out ConsoleKeyInfo key,
      out int keyLength)
    {
      int val1 = endIndex - startIndex;
      if (val1 > 0)
      {
        char givenChar = givenChars[startIndex];
        if ((int) givenChar != (int) ConsolePal.s_posixDisableValue && (int) givenChar == (int) ConsolePal.s_veraseCharacter)
        {
          key = new ConsoleKeyInfo(givenChar, ConsoleKey.Backspace, false, false, false);
          keyLength = 1;
          return true;
        }
      }
      int minKeyFormatLength = ConsolePal.TerminalFormatStrings.Instance.MinKeyFormatLength;
      if (val1 >= minKeyFormatLength)
      {
        for (int length = Math.Min(val1, ConsolePal.TerminalFormatStrings.Instance.MaxKeyFormatLength); length >= minKeyFormatLength; --length)
        {
          ReadOnlyMemory<char> key1 = new ReadOnlyMemory<char>(givenChars, startIndex, length);
          if (ConsolePal.TerminalFormatStrings.Instance.KeyFormatToConsoleKey.TryGetValue(key1, out key))
          {
            keyLength = key1.Length;
            return true;
          }
        }
      }
      key = new ConsoleKeyInfo();
      keyLength = 0;
      return false;
    }

    internal static void EnsureConsoleInitialized()
    {
      if (ConsolePal.s_initialized)
        return;
      ConsolePal.EnsureInitializedCore();
    }

    private static void EnsureInitializedCore()
    {
      TextWriter textWriter = Console.Out;
      bool lockTaken = false;
      try
      {
        Monitor.Enter((object) textWriter, ref lockTaken);
        if (ConsolePal.s_initialized)
          return;
        if (!Interop.Sys.InitializeTerminalAndSignalHandling())
          throw new Win32Exception();
        if (!Console.IsOutputRedirected)
        {
          string keypadXmit = ConsolePal.TerminalFormatStrings.Instance.KeypadXmit;
          if (keypadXmit != null)
            Interop.Sys.SetKeypadXmit(keypadXmit);
        }
        if (!Console.IsInputRedirected)
        {
          // ISSUE: method pointer
          // ISSUE: cast to a function pointer type
          Interop.Sys.SetTerminalInvalidationHandler((__FnPtr<void ()>) __methodptr(InvalidateTerminalSettings));
          Interop.Sys.ControlCharacterNames[] controlCharacterNames = new Interop.Sys.ControlCharacterNames[4]
          {
            Interop.Sys.ControlCharacterNames.VERASE,
            Interop.Sys.ControlCharacterNames.VEOL,
            Interop.Sys.ControlCharacterNames.VEOL2,
            Interop.Sys.ControlCharacterNames.VEOF
          };
          byte[] controlCharacterValues = new byte[controlCharacterNames.Length];
          Interop.Sys.GetControlCharacters(controlCharacterNames, controlCharacterValues, controlCharacterNames.Length, out ConsolePal.s_posixDisableValue);
          ConsolePal.s_veraseCharacter = controlCharacterValues[0];
          ConsolePal.s_veolCharacter = controlCharacterValues[1];
          ConsolePal.s_veol2Character = controlCharacterValues[2];
          ConsolePal.s_veofCharacter = controlCharacterValues[3];
        }
        ConsolePal.s_initialized = true;
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit((object) textWriter);
      }
    }

    internal static unsafe int Read(SafeFileHandle fd, Span<byte> buffer)
    {
      fixed (byte* buffer1 = &buffer.GetPinnableReference())
        return Interop.CheckIo(Interop.Sys.Read((SafeHandle) fd, buffer1, buffer.Length), (string) null, false, (Func<Interop.ErrorInfo, Interop.ErrorInfo>) null);
    }

    internal static unsafe void Write(
      SafeFileHandle fd,
      ReadOnlySpan<byte> buffer,
      bool mayChangeCursorPosition = true)
    {
      ConsolePal.EnsureConsoleInitialized();
      fixed (byte* numPtr1 = &buffer.GetPinnableReference())
      {
        byte* numPtr2 = numPtr1;
        int length = buffer.Length;
        while (length > 0)
        {
          int cursorVersion = mayChangeCursorPosition ? Volatile.Read(ref ConsolePal.s_cursorVersion) : -1;
          int count = Interop.Sys.Write((SafeHandle) fd, numPtr2, length);
          if (count < 0)
          {
            Interop.ErrorInfo lastErrorInfo = Interop.Sys.GetLastErrorInfo();
            if (lastErrorInfo.Error == Interop.Error.EPIPE)
              break;
            if (lastErrorInfo.Error != Interop.Error.EAGAIN)
              throw Interop.GetExceptionForIoErrno(lastErrorInfo);
            int num = (int) Interop.Sys.Poll((SafeHandle) fd, Interop.PollEvents.POLLOUT, -1, out Interop.PollEvents _);
          }
          else
          {
            if (mayChangeCursorPosition)
              ConsolePal.UpdatedCachedCursorPosition(numPtr2, count, cursorVersion);
            length -= count;
            numPtr2 += count;
          }
        }
      }
    }

    private static unsafe void UpdatedCachedCursorPosition(
      byte* bufPtr,
      int count,
      int cursorVersion)
    {
      TextWriter textWriter = Console.Out;
      bool lockTaken = false;
      try
      {
        Monitor.Enter((object) textWriter, ref lockTaken);
        int top;
        int left;
        if (cursorVersion != ConsolePal.s_cursorVersion || !ConsolePal.TryGetCachedCursorPosition(out left, out top) || count > (int) byte.MaxValue)
        {
          ConsolePal.InvalidateCachedCursorPosition();
        }
        else
        {
          int height;
          int width;
          ConsolePal.GetWindowSize(out width, out height);
          for (int index = 0; index < count; ++index)
          {
            byte num = bufPtr[index];
            if (num < (byte) 127 && num >= (byte) 32)
            {
              IncrementX();
            }
            else
            {
              switch (num)
              {
                case 8:
                  if (left > 0)
                  {
                    left--;
                    continue;
                  }
                  continue;
                case 10:
                  left = 0;
                  IncrementY();
                  continue;
                case 13:
                  left = 0;
                  continue;
                default:
                  ConsolePal.InvalidateCachedCursorPosition();
                  return;
              }
            }
          }
          ConsolePal.SetCachedCursorPosition(left, top, new int?(cursorVersion));

          void IncrementY()
          {
            top++;
            if (top < height)
              return;
            top = height - 1;
          }

          void IncrementX()
          {
            left++;
            if (left < width)
              return;
            left = 0;
            IncrementY();
          }
        }
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit((object) textWriter);
      }
    }

    private static void CheckTerminalSettingsInvalidated()
    {
      ConsolePal.EnsureConsoleInitialized();
      if (Interlocked.CompareExchange(ref ConsolePal.s_invalidateCachedSettings, 0, 1) != 1)
        return;
      ConsolePal.InvalidateCachedCursorPosition();
      ConsolePal.s_windowWidth = -1;
    }

    [UnmanagedCallersOnly]
    private static void InvalidateTerminalSettings() => Volatile.Write(ref ConsolePal.s_invalidateCachedSettings, 1);

    internal static unsafe void WriteStdoutAnsiString(string value, bool mayChangeCursorPosition = true)
    {
      if (string.IsNullOrEmpty(value))
        return;
      Span<byte> span = new Span<byte>();
      Span<byte> buffer;
      if (value.Length <= 256)
      {
        int maxByteCount = Encoding.UTF8.GetMaxByteCount(value.Length);
        // ISSUE: untyped stack allocation
        Span<byte> bytes1 = new Span<byte>((void*) __untypedstackalloc((IntPtr) (uint) maxByteCount), maxByteCount);
        int bytes2 = Encoding.UTF8.GetBytes((ReadOnlySpan<char>) value, bytes1);
        buffer = bytes1.Slice(0, bytes2);
      }
      else
        buffer = (Span<byte>) Encoding.UTF8.GetBytes(value);
      TextWriter textWriter = Console.Out;
      bool lockTaken = false;
      try
      {
        Monitor.Enter((object) textWriter, ref lockTaken);
        ConsolePal.Write(Interop.Sys.FileDescriptors.STDOUT_FILENO, (ReadOnlySpan<byte>) buffer, mayChangeCursorPosition);
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit((object) textWriter);
      }
    }

    internal sealed class TerminalFormatStrings
    {
      private static readonly Lazy<ConsolePal.TerminalFormatStrings> s_instance = new Lazy<ConsolePal.TerminalFormatStrings>((Func<ConsolePal.TerminalFormatStrings>) (() => new ConsolePal.TerminalFormatStrings(TermInfo.Database.ReadActiveDatabase())));
      public readonly string Foreground;
      public readonly string Background;
      public readonly string Reset;
      public readonly int MaxColors;
      public readonly int Columns;
      public readonly int Lines;
      public readonly string CursorVisible;
      public readonly string CursorInvisible;
      public readonly string Title;
      public readonly string Bell;
      public readonly string Clear;
      public readonly string CursorAddress;
      public readonly string CursorLeft;
      public readonly string ClrEol;
      public readonly Dictionary<ReadOnlyMemory<char>, ConsoleKeyInfo> KeyFormatToConsoleKey = new Dictionary<ReadOnlyMemory<char>, ConsoleKeyInfo>((IEqualityComparer<ReadOnlyMemory<char>>) new ConsolePal.ReadOnlyMemoryContentComparer());
      public readonly int MaxKeyFormatLength;
      public readonly int MinKeyFormatLength;
      public readonly string KeypadXmit;

      public static ConsolePal.TerminalFormatStrings Instance => ConsolePal.TerminalFormatStrings.s_instance.Value;

      public TerminalFormatStrings(TermInfo.Database db)
      {
        if (db == null)
          return;
        this.KeypadXmit = db.GetString(TermInfo.WellKnownStrings.KeypadXmit);
        this.Foreground = db.GetString(TermInfo.WellKnownStrings.SetAnsiForeground);
        this.Background = db.GetString(TermInfo.WellKnownStrings.SetAnsiBackground);
        this.Reset = db.GetString(TermInfo.WellKnownStrings.OrigPairs) ?? db.GetString(TermInfo.WellKnownStrings.OrigColors);
        this.Bell = db.GetString(TermInfo.WellKnownStrings.Bell);
        this.Clear = db.GetString(TermInfo.WellKnownStrings.Clear);
        this.Columns = db.GetNumber(TermInfo.WellKnownNumbers.Columns);
        this.Lines = db.GetNumber(TermInfo.WellKnownNumbers.Lines);
        this.CursorVisible = db.GetString(TermInfo.WellKnownStrings.CursorVisible);
        this.CursorInvisible = db.GetString(TermInfo.WellKnownStrings.CursorInvisible);
        this.CursorAddress = db.GetString(TermInfo.WellKnownStrings.CursorAddress);
        this.CursorLeft = db.GetString(TermInfo.WellKnownStrings.CursorLeft);
        this.ClrEol = db.GetString(TermInfo.WellKnownStrings.ClrEol);
        this.Title = ConsolePal.TerminalFormatStrings.GetTitle(db);
        int number = db.GetNumber(TermInfo.WellKnownNumbers.MaxColors);
        this.MaxColors = number >= 16 ? 16 : (number >= 8 ? 8 : 0);
        this.AddKey(db, TermInfo.WellKnownStrings.KeyF1, ConsoleKey.F1);
        this.AddKey(db, TermInfo.WellKnownStrings.KeyF2, ConsoleKey.F2);
        this.AddKey(db, TermInfo.WellKnownStrings.KeyF3, ConsoleKey.F3);
        this.AddKey(db, TermInfo.WellKnownStrings.KeyF4, ConsoleKey.F4);
        this.AddKey(db, TermInfo.WellKnownStrings.KeyF5, ConsoleKey.F5);
        this.AddKey(db, TermInfo.WellKnownStrings.KeyF6, ConsoleKey.F6);
        this.AddKey(db, TermInfo.WellKnownStrings.KeyF7, ConsoleKey.F7);
        this.AddKey(db, TermInfo.WellKnownStrings.KeyF8, ConsoleKey.F8);
        this.AddKey(db, TermInfo.WellKnownStrings.KeyF9, ConsoleKey.F9);
        this.AddKey(db, TermInfo.WellKnownStrings.KeyF10, ConsoleKey.F10);
        this.AddKey(db, TermInfo.WellKnownStrings.KeyF11, ConsoleKey.F11);
        this.AddKey(db, TermInfo.WellKnownStrings.KeyF12, ConsoleKey.F12);
        this.AddKey(db, TermInfo.WellKnownStrings.KeyF13, ConsoleKey.F13);
        this.AddKey(db, TermInfo.WellKnownStrings.KeyF14, ConsoleKey.F14);
        this.AddKey(db, TermInfo.WellKnownStrings.KeyF15, ConsoleKey.F15);
        this.AddKey(db, TermInfo.WellKnownStrings.KeyF16, ConsoleKey.F16);
        this.AddKey(db, TermInfo.WellKnownStrings.KeyF17, ConsoleKey.F17);
        this.AddKey(db, TermInfo.WellKnownStrings.KeyF18, ConsoleKey.F18);
        this.AddKey(db, TermInfo.WellKnownStrings.KeyF19, ConsoleKey.F19);
        this.AddKey(db, TermInfo.WellKnownStrings.KeyF20, ConsoleKey.F20);
        this.AddKey(db, TermInfo.WellKnownStrings.KeyF21, ConsoleKey.F21);
        this.AddKey(db, TermInfo.WellKnownStrings.KeyF22, ConsoleKey.F22);
        this.AddKey(db, TermInfo.WellKnownStrings.KeyF23, ConsoleKey.F23);
        this.AddKey(db, TermInfo.WellKnownStrings.KeyF24, ConsoleKey.F24);
        this.AddKey(db, TermInfo.WellKnownStrings.KeyBackspace, ConsoleKey.Backspace);
        this.AddKey(db, TermInfo.WellKnownStrings.KeyBackTab, ConsoleKey.Tab, true, false, false);
        this.AddKey(db, TermInfo.WellKnownStrings.KeyBegin, ConsoleKey.Home);
        this.AddKey(db, TermInfo.WellKnownStrings.KeyClear, ConsoleKey.Clear);
        this.AddKey(db, TermInfo.WellKnownStrings.KeyDelete, ConsoleKey.Delete);
        this.AddKey(db, TermInfo.WellKnownStrings.KeyDown, ConsoleKey.DownArrow);
        this.AddKey(db, TermInfo.WellKnownStrings.KeyEnd, ConsoleKey.End);
        this.AddKey(db, TermInfo.WellKnownStrings.KeyEnter, ConsoleKey.Enter);
        this.AddKey(db, TermInfo.WellKnownStrings.KeyHelp, ConsoleKey.Help);
        this.AddKey(db, TermInfo.WellKnownStrings.KeyHome, ConsoleKey.Home);
        this.AddKey(db, TermInfo.WellKnownStrings.KeyInsert, ConsoleKey.Insert);
        this.AddKey(db, TermInfo.WellKnownStrings.KeyLeft, ConsoleKey.LeftArrow);
        this.AddKey(db, TermInfo.WellKnownStrings.KeyPageDown, ConsoleKey.PageDown);
        this.AddKey(db, TermInfo.WellKnownStrings.KeyPageUp, ConsoleKey.PageUp);
        this.AddKey(db, TermInfo.WellKnownStrings.KeyPrint, ConsoleKey.Print);
        this.AddKey(db, TermInfo.WellKnownStrings.KeyRight, ConsoleKey.RightArrow);
        this.AddKey(db, TermInfo.WellKnownStrings.KeyScrollForward, ConsoleKey.PageDown, true, false, false);
        this.AddKey(db, TermInfo.WellKnownStrings.KeyScrollReverse, ConsoleKey.PageUp, true, false, false);
        this.AddKey(db, TermInfo.WellKnownStrings.KeySBegin, ConsoleKey.Home, true, false, false);
        this.AddKey(db, TermInfo.WellKnownStrings.KeySDelete, ConsoleKey.Delete, true, false, false);
        this.AddKey(db, TermInfo.WellKnownStrings.KeySHome, ConsoleKey.Home, true, false, false);
        this.AddKey(db, TermInfo.WellKnownStrings.KeySelect, ConsoleKey.Select);
        this.AddKey(db, TermInfo.WellKnownStrings.KeySLeft, ConsoleKey.LeftArrow, true, false, false);
        this.AddKey(db, TermInfo.WellKnownStrings.KeySPrint, ConsoleKey.Print, true, false, false);
        this.AddKey(db, TermInfo.WellKnownStrings.KeySRight, ConsoleKey.RightArrow, true, false, false);
        this.AddKey(db, TermInfo.WellKnownStrings.KeyUp, ConsoleKey.UpArrow);
        this.AddPrefixKey(db, "kLFT", ConsoleKey.LeftArrow);
        this.AddPrefixKey(db, "kRIT", ConsoleKey.RightArrow);
        this.AddPrefixKey(db, "kUP", ConsoleKey.UpArrow);
        this.AddPrefixKey(db, "kDN", ConsoleKey.DownArrow);
        this.AddPrefixKey(db, "kDC", ConsoleKey.Delete);
        this.AddPrefixKey(db, "kEND", ConsoleKey.End);
        this.AddPrefixKey(db, "kHOM", ConsoleKey.Home);
        this.AddPrefixKey(db, "kNXT", ConsoleKey.PageDown);
        this.AddPrefixKey(db, "kPRV", ConsoleKey.PageUp);
        if (this.KeyFormatToConsoleKey.Count <= 0)
          return;
        this.MaxKeyFormatLength = int.MinValue;
        this.MinKeyFormatLength = int.MaxValue;
        foreach (KeyValuePair<ReadOnlyMemory<char>, ConsoleKeyInfo> keyValuePair in this.KeyFormatToConsoleKey)
        {
          ReadOnlyMemory<char> key = keyValuePair.Key;
          if (key.Length > this.MaxKeyFormatLength)
          {
            key = keyValuePair.Key;
            this.MaxKeyFormatLength = key.Length;
          }
          key = keyValuePair.Key;
          if (key.Length < this.MinKeyFormatLength)
          {
            key = keyValuePair.Key;
            this.MinKeyFormatLength = key.Length;
          }
        }
      }

      private static string GetTitle(TermInfo.Database db)
      {
        string str1 = db.GetString(TermInfo.WellKnownStrings.ToStatusLine);
        string str2 = db.GetString(TermInfo.WellKnownStrings.FromStatusLine);
        if (str1 != null && str2 != null)
          return str1 + "%p1%s" + str2;
        string str3 = db.Term;
        if (str3 == null)
          return string.Empty;
        if (str3.StartsWith("xterm", StringComparison.Ordinal))
          str3 = "xterm";
        switch (str3)
        {
          case "aixterm":
          case "dtterm":
          case "linux":
          case "rxvt":
          case "xterm":
            return "\u001B]0;%p1%s\a";
          case "cygwin":
            return "\u001B];%p1%s\a";
          case "konsole":
            return "\u001B]30;%p1%s\a";
          case "screen":
            return "\u001Bk%p1%s\u001B";
          default:
            return string.Empty;
        }
      }

      private void AddKey(TermInfo.Database db, TermInfo.WellKnownStrings keyId, ConsoleKey key) => this.AddKey(db, keyId, key, false, false, false);

      private void AddKey(
        TermInfo.Database db,
        TermInfo.WellKnownStrings keyId,
        ConsoleKey key,
        bool shift,
        bool alt,
        bool control)
      {
        ReadOnlyMemory<char> key1 = db.GetString(keyId).AsMemory();
        if (key1.IsEmpty)
          return;
        this.KeyFormatToConsoleKey[key1] = new ConsoleKeyInfo(char.MinValue, key, shift, alt, control);
      }

      private void AddPrefixKey(TermInfo.Database db, string extendedNamePrefix, ConsoleKey key)
      {
        this.AddKey(db, extendedNamePrefix + "3", key, false, true, false);
        this.AddKey(db, extendedNamePrefix + "4", key, true, true, false);
        this.AddKey(db, extendedNamePrefix + "5", key, false, false, true);
        this.AddKey(db, extendedNamePrefix + "6", key, true, false, true);
        this.AddKey(db, extendedNamePrefix + "7", key, false, false, true);
      }

      private void AddKey(
        TermInfo.Database db,
        string extendedName,
        ConsoleKey key,
        bool shift,
        bool alt,
        bool control)
      {
        ReadOnlyMemory<char> key1 = db.GetExtendedString(extendedName).AsMemory();
        if (key1.IsEmpty)
          return;
        this.KeyFormatToConsoleKey[key1] = new ConsoleKeyInfo(char.MinValue, key, shift, alt, control);
      }
    }

    private sealed class UnixConsoleStream : ConsoleStream
    {
      private readonly SafeFileHandle _handle;
      private readonly bool _useReadLine;

      internal UnixConsoleStream(SafeFileHandle handle, FileAccess access, bool useReadLine = false)
        : base(access)
      {
        this._handle = handle;
        this._useReadLine = useReadLine;
      }

      protected override void Dispose(bool disposing)
      {
        if (disposing)
          this._handle.Dispose();
        base.Dispose(disposing);
      }

      public override int Read(Span<byte> buffer) => !this._useReadLine ? ConsolePal.Read(this._handle, buffer) : ConsolePal.StdInReader.ReadLine(buffer);

      public override void Write(ReadOnlySpan<byte> buffer) => ConsolePal.Write(this._handle, buffer);

      public override void Flush()
      {
        if (this._handle.IsClosed)
          throw System.IO.Error.GetFileNotOpen();
        base.Flush();
      }
    }

    private sealed class ReadOnlyMemoryContentComparer : IEqualityComparer<ReadOnlyMemory<char>>
    {
      public bool Equals(ReadOnlyMemory<char> x, ReadOnlyMemory<char> y) => x.Span.SequenceEqual<char>(y.Span);

      public int GetHashCode(ReadOnlyMemory<char> obj) => string.GetHashCode(obj.Span);
    }
  }
}
