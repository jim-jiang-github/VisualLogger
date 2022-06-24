using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualLogger.Streams
{
    /// <summary>
    /// Can read binary data and string line.
    /// </summary>
    public class MixStreamReader : IDisposable
    {
        private const int BUFFER_SIZE = 3;

        private readonly Stream _input;
        private readonly byte[] _byteBuffer;
        private readonly char[] _charBuffer;
        private readonly Decoder _decoder;
        private readonly Encoding _encoding;
        private int charPos = 0;
        private int charLen = 0;
        private int bytePos = 0;
        private int byteLen = 0;

        public MixStreamReader(Stream input) : this(input, Encoding.UTF8)
        {
        }

        public MixStreamReader(Stream input, Encoding encoding)
        {
            _input = input;
            _byteBuffer = new byte[BUFFER_SIZE];
            _charBuffer = new char[Encoding.UTF8.GetMaxCharCount(BUFFER_SIZE)];
            _decoder = encoding.GetDecoder();
            _encoding = encoding;
        }
        public long BufferPosition => bytePos;
        private int ReadBuffer()
        {
            charLen = 0;
            charPos = 0;
            byteLen = _input.Read(_byteBuffer, 0, _byteBuffer.Length);
            if (byteLen > 0)
            {
                charLen = _decoder.GetChars(_byteBuffer, 0, byteLen, _charBuffer, 0);
            }
            return byteLen;
        }

        public string? ReadLine(bool includeEndOfLine = true)
        {
            //ref: https://referencesource.microsoft.com/#mscorlib/system/io/streamreader.cs,737
            if (charPos == charLen)
            {
                if (ReadBuffer() == 0) return null;
            }
            StringBuilder? sb = null;
            do
            {
                int i = charPos;
                int start;
                int length;
                do
                {
                    char ch = _charBuffer[i];
                    // Note the following common line feed chars:
                    // \n - UNIX   \r\n - DOS   \r - Mac
                    if (ch == '\r' || ch == '\n')
                    {
                        if (sb == null)
                        {
                            sb = new StringBuilder(i + 80);
                        }
                        start = charPos;
                        length = i - charPos + 1;
                        sb.Append(_charBuffer, charPos, i - charPos + 1);
                        charPos = i + 1;
                        int newReadedLength = 0;
                        if (ch == '\r' && (charPos < charLen || (newReadedLength = ReadBuffer()) > 0))
                        {
                            if (_charBuffer[charPos] == '\n')
                            {
                                charPos++;
                                sb.Append('\n');
                            }
                        }
                        string s = sb.ToString();
                        bytePos += _encoding.GetByteCount(s);
                        //bytePos += _encoding.GetByteCount(_charBuffer, start, length);
                        return s;
                    }
                    i++;
                } while (i < charLen);
                i = charLen - charPos;
                if (sb == null) sb = new StringBuilder(i + 80);
                sb.Append(_charBuffer, charPos, i);
            } while (ReadBuffer() > 0);
            string s1 = sb.ToString();
            bytePos += _encoding.GetByteCount(s1);
            return s1;
        }

        public int GetByteCount(ReadOnlySpan<char> chars)
        {
            return _encoding.GetByteCount(chars);
        }

        public void Close()
        {
            Dispose();
        }

        public void Dispose()
        {
            _input?.Close();
        }
    }
}
