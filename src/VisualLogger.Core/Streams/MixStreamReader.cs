using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualLogger.Core.Streams
{
    /// <summary>
    /// Can read binary data and string line.
    /// </summary>
    public class MixStreamReader : BinaryReader
    {
        private const int BUFFER_SIZE = 1024;

        private readonly Stream _input;
        private readonly byte[] _byteBuffer;
        private readonly char[] _charBuffer;
        private readonly Decoder _decoder;
        private int charPos = 0;
        private int charLen = 0;

        public MixStreamReader(Stream input) : this(input, Encoding.UTF8)
        {
        }

        public MixStreamReader(Stream input, Encoding encoding) : base(input, encoding)
        {
            _input = input;
            _byteBuffer = new byte[BUFFER_SIZE];
            _charBuffer = new char[Encoding.UTF8.GetMaxCharCount(BUFFER_SIZE)];
            _decoder = encoding.GetDecoder();
        }

        public long BufferPosition => BaseStream.Position - charLen + charPos;

        private int ReadBuffer()
        {
            charLen = 0;
            charPos = 0;
            var byteLen = _input.Read(_byteBuffer, 0, _byteBuffer.Length);
            if (byteLen > 0)
            {
                charLen += _decoder.GetChars(_byteBuffer, 0, byteLen, _charBuffer, 0);
            }
            return byteLen;
        }

        public string? ReadLine(bool includeEndOfLine = false)
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
                do
                {
                    char ch = _charBuffer[i];
                    if (ch == '\0') //Skip '\0'
                    {
                        charPos = i + 1;
                    }
                    else
                    {
                        // Note the following common line feed chars:
                        // \n - UNIX   \r\n - DOS   \r - Mac
                        if (ch == '\r' || ch == '\n')
                        {
                            string s;
                            int startPos = charPos;
                            int length = i - charPos;
                            length += includeEndOfLine ? 1 : 0;
                            charPos = i + 1;
                            if (ch == '\r' && (charPos < charLen || ReadBuffer() > 0))
                            {
                                if (_charBuffer[charPos] == '\n') charPos++;
                                length += includeEndOfLine ? 1 : 0;
                            }
                            if (sb != null)
                            {
                                sb.Append(_charBuffer, startPos, length);
                                s = sb.ToString();
                            }
                            else
                            {
                                s = new string(_charBuffer, startPos, length);
                            }
                            return s;
                        }
                    }
                    i++;
                } while (i < charLen);
                i = charLen - charPos;
                if (sb == null) sb = new StringBuilder(i + 80);
                sb.Append(_charBuffer, charPos, i);
            } while (ReadBuffer() > 0);
            return sb.ToString();
        }
    }
}
