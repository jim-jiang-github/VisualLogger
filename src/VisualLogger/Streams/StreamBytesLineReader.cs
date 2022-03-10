using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Security.Permissions;
using System.Runtime.CompilerServices;
using System.IO;
using Microsoft.CodeAnalysis.Text;

namespace VisualLogger.Streams
{
    // This is designed for character input in a particular Encoding, 
    // whereas the Stream class is designed for byte input and output.  
    // 
    [Serializable]
    [System.Runtime.InteropServices.ComVisible(true)]
    public class StreamBytesLineReader
    {
        // Using a 1K byte buffer and a 4K FileStream buffer works out pretty well
        // perf-wise.  On even a 40 MB text file, any perf loss by using a 4K
        // buffer is negated by the win of allocating a smaller byte[], which 
        // saves construction time.  This does break adaptive buffering,
        // but this is slightly faster.
        internal static int DefaultBufferSize
        {
            get
            {
                return 1024 * 8;
            }
        }

        private const int DefaultFileStreamBufferSize = 4096;
        private const int MinBufferSize = 128;

        private Stream stream;
        private Encoding encoding;
        private Decoder decoder;
        private byte[] byteBuffer;
        private byte[] _preamble;   // Encoding's preamble, which identifies this encoding.
        // Record the number of valid bytes in the byteBuffer, for a few checks.
        private int byteLen;
        // This is used only for preamble detection
        private int bytePos;

        // We will support looking for byte order marks in the stream and trying
        // to decide what the encoding might be from the byte order marks, IF they
        // exist.  But that's all we'll do.  
        private bool _detectEncoding;

        // Whether we must still check for the encoding's given preamble at the
        // beginning of this file.
        private bool _checkPreamble;

        // The intent of this field is to leave open the underlying stream when 
        // disposing of this LineStreamReader1.  A name like _leaveOpen is better, 
        // but this type is serializable, and this field's name was _closable.
        private bool _closable;  // Whether to close the underlying stream.

        internal void InternalBlockCopy(Array src, int srcOffsetBytes, Array dst, int dstOffsetBytes, int byteCount)
        {
            Array.Copy(src, srcOffsetBytes, dst, dstOffsetBytes, byteCount);
        }

        public long Position => stream.Position - byteLen + bytePos;

        public StreamBytesLineReader(Stream stream)
            : this(stream, true)
        {
        }

        public StreamBytesLineReader(Stream stream, bool detectEncodingFromByteOrderMarks)
            : this(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks, DefaultBufferSize, false)
        {
        }
        public StreamBytesLineReader(Stream stream, Encoding encoding)
            : this(stream, encoding, true, DefaultBufferSize, false)
        {
        }

#nullable disable
        public StreamBytesLineReader(Stream stream, Encoding encoding, bool detectEncodingFromByteOrderMarks, int bufferSize, bool leaveOpen)
#nullable enable
        {
            if (stream == null || encoding == null)
                throw new ArgumentNullException((stream == null ? "stream" : "encoding"));
            if (!stream.CanRead)
                throw new ArgumentException("Argument_StreamNotReadable");
            if (bufferSize <= 0)
                throw new ArgumentOutOfRangeException("bufferSize", "ArgumentOutOfRange_NeedPosNum");
            Contract.EndContractBlock();

            Init(stream, encoding, detectEncodingFromByteOrderMarks, bufferSize, leaveOpen);
        }

        private void Init(Stream stream, Encoding encoding, bool detectEncodingFromByteOrderMarks, int bufferSize, bool leaveOpen)
        {
            this.stream = stream;
            this.encoding = encoding;
            decoder = encoding.GetDecoder();
            if (bufferSize < MinBufferSize) bufferSize = MinBufferSize;
            byteBuffer = new byte[bufferSize];
            byteLen = 0;
            bytePos = 0;
            _detectEncoding = detectEncodingFromByteOrderMarks;
            _preamble = encoding.GetPreamble();
            _checkPreamble = (_preamble.Length > 0);
            _closable = !leaveOpen;
            ResetReadLinePosition();
        }

        public void Close()
        {
            Dispose(true);
        }

        protected void Dispose(bool disposing)
        {
            // Dispose of our resources if this LineStreamReader1 is closable.
            // Note that Console.In should be left open.
            try
            {
                // Note that Stream.Close() can potentially throw here. So we need to 
                // ensure cleaning up internal resources, inside the finally block.  
                if (!LeaveOpen && disposing && (stream != null))
                    stream.Close();
            }
            finally
            {
                if (!LeaveOpen && (stream != null))
                {
#nullable disable
                    stream = null;
                    encoding = null;
                    decoder = null;
                    byteBuffer = null;
#nullable enable
                }
            }
        }

        private Func<ValueTuple<long, int>?> ReadLinePositionFunc { get; set; }

        public virtual Encoding CurrentEncoding
        {
            get { return encoding; }
        }

        public virtual Stream BaseStream
        {
            get { return stream; }
        }

        internal bool LeaveOpen
        {
            get { return !_closable; }
        }

        // Trims n bytes from the front of the buffer.
        private void CompressBuffer(int n)
        {
            Contract.Assert(byteLen >= n, "CompressBuffer was called with a number of bytes greater than the current buffer length.  Are two threads using this LineStreamReader1 at the same time?");
            InternalBlockCopy(byteBuffer, n, byteBuffer, 0, byteLen - n);
            byteLen -= n;
        }

        private void DetectEncoding()
        {
            if (byteLen < 2)
                return;
            _detectEncoding = false;
            bool changedEncoding = false;
            if (byteBuffer[0] == 0xFE && byteBuffer[1] == 0xFF)
            {
                // Big Endian Unicode

                encoding = new UnicodeEncoding(true, true);
                CompressBuffer(2);
                changedEncoding = true;
            }

            else if (byteBuffer[0] == 0xFF && byteBuffer[1] == 0xFE)
            {
                // Little Endian Unicode, or possibly little endian UTF32
                if (byteLen < 4 || byteBuffer[2] != 0 || byteBuffer[3] != 0)
                {
                    encoding = new UnicodeEncoding(false, true);
                    CompressBuffer(2);
                    changedEncoding = true;
                }
            }

            else if (byteLen >= 3 && byteBuffer[0] == 0xEF && byteBuffer[1] == 0xBB && byteBuffer[2] == 0xBF)
            {
                // UTF-8
                encoding = Encoding.UTF8;
                CompressBuffer(3);
                changedEncoding = true;
            }
            else if (byteLen == 2)
                _detectEncoding = true;
            // Note: in the future, if we change this algorithm significantly,
            // we can support checking for the preamble of the given encoding.

            if (changedEncoding)
            {
                decoder = encoding.GetDecoder();
                ResetReadLinePosition();
            }
        }

        private void ResetReadLinePosition()
        {
            if (encoding.EncodingName == "Unicode (Big-Endian)")
            {
                ReadLinePositionFunc = ReadLinePositionUnicodeBE;
            }
            else if (encoding.EncodingName == "Unicode")
            {
                ReadLinePositionFunc = ReadLinePositionUnicode;
            }
            else
            {
                ReadLinePositionFunc = ReadLinePositionUTF8;
            }
        }

        // DiscardBufferedData tells LineStreamReader1 to throw away its internal
        // buffer contents.  This is useful if the user needs to seek on the
        // underlying stream to a known location then wants the LineStreamReader1
        // to start reading from this new point.  This method should be called
        // very sparingly, if ever, since it can lead to very poor performance.
        // However, it may be the only way of handling some scenarios where 
        // users need to re-read the contents of a LineStreamReader1 a second time.
        public void DiscardBufferedData()
        {
            _checkPreamble = true;
            byteLen = 0;
            // in general we'd like to have an invariant that encoding isn't null. However,
            // for startup improvements for NullStreamReader, we want to delay load encoding. 
            if (encoding != null)
            {
                decoder = encoding.GetDecoder();
            }
        }

        // Trims the preamble bytes from the byteBuffer. This routine can be called multiple times
        // and we will buffer the bytes read until the preamble is matched or we determine that
        // there is no match. If there is no match, every byte read previously will be available 
        // for further consumption. If there is a match, we will compress the buffer for the 
        // leading preamble bytes
        private bool IsPreamble()
        {
            if (!_checkPreamble)
                return _checkPreamble;

            Contract.Assert(bytePos <= _preamble.Length, "_compressPreamble was called with the current bytePos greater than the preamble buffer length.  Are two threads using this LineStreamReader1 at the same time?");
            int len = (byteLen >= (_preamble.Length)) ? (_preamble.Length - bytePos) : (byteLen - bytePos);

            for (int i = 0; i < len; i++, bytePos++)
            {
                if (byteBuffer[bytePos] != _preamble[bytePos])
                {
                    bytePos = 0;
                    _checkPreamble = false;
                    break;
                }
            }

            Contract.Assert(bytePos <= _preamble.Length, "possible bug in _compressPreamble.  Are two threads using this LineStreamReader1 at the same time?");

            if (_checkPreamble)
            {
                if (bytePos == _preamble.Length)
                {
                    // We have a match
                    CompressBuffer(_preamble.Length);
                    bytePos = 0;
                    _checkPreamble = false;
                    _detectEncoding = false;
                }
            }

            return _checkPreamble;
        }

        internal virtual int ReadBuffer()
        {
            bytePos = 0;
            if (!_checkPreamble)
                byteLen = 0;
            do
            {
                if (_checkPreamble)
                {
                    //byteBuffer = new byte[DefaultBufferSize];
                    //Contract.Assert(bytePos <= _preamble.Length, "possible bug in _compressPreamble.  Are two threads using this LineStreamReader1 at the same time?");
                    int len = stream.Read(byteBuffer, bytePos, byteBuffer.Length - bytePos);
                    //Contract.Assert(len >= 0, "Stream.Read returned a negative number!  This is a bug in your stream class.");

                    if (len == 0)
                    {
                        // EOF but we might have buffered bytes from previous 
                        // attempt to detect preamble that needs to be decoded now
                        if (byteLen > 0)
                        {
                            // Need to zero out the byteLen after we consume these bytes so that we don't keep infinitely hitting this code path
                            bytePos = byteLen = 0;
                        }

                        return byteLen;
                    }

                    byteLen += len;
                }
                else
                {
                    //byteBuffer = new byte[DefaultBufferSize];
                    //Contract.Assert(bytePos == 0, "bytePos can be non zero only when we are trying to _checkPreamble.  Are two threads using this LineStreamReader1 at the same time?");
                    byteLen = stream.Read(byteBuffer, 0, byteBuffer.Length);
                    //Contract.Assert(byteLen >= 0, "Stream.Read returned a negative number!  This is a bug in your stream class.");

                    if (byteLen == 0)  // We're at EOF
                        return byteLen;
                }

                // Check for preamble before detect encoding. This is not to override the
                // user suppplied Encoding for the one we implicitly detect. The user could
                // customize the encoding which we will loose, such as ThrowOnError on UTF8
                if (IsPreamble())
                    continue;

                // If we're supposed to detect the encoding and haven't done so yet,
                // do it.  Note this may need to be called more than once.
                if (_detectEncoding && byteLen >= 2)
                    DetectEncoding();

            } while (byteLen == 0);
            //Console.WriteLine("ReadBuffer called.  chars: "+charLen);
            return byteLen;
        }

        // Reads a line. A line is defined as a sequence of characters followed by
        // a carriage return ('\r'), a line feed ('\n'), or a carriage return
        // immediately followed by a line feed. The resulting string does not
        // contain the terminating carriage return and/or line feed. The returned
        // value is null if the end of the input stream has been reached.
        //
        public ValueTuple<long, int>? ReadLinePosition()
        {
            return ReadLinePositionFunc.Invoke();
        }
        private ValueTuple<long, int>? ReadLinePositionUnicodeBE()
        {
            if (stream == null)
                throw new ObjectDisposedException(null, "ObjectDisposed_ReaderClosed");

            if (bytePos == byteLen)
            {
#nullable disable
                if (ReadBuffer() == 0) return null;
#nullable enable
            }

            long start = Position;
            do
            {
                int i = bytePos;
                do
                {
                    byte b = byteBuffer[i];
                    if (b == 0x0)
                    {
                        bytePos = i + 1;
                        if ((bytePos < byteLen || ReadBuffer() > 0) && (byteBuffer[bytePos] == 0xD || byteBuffer[bytePos] == 0XA))
                        {
                            bytePos++;
                            if ((bytePos < byteLen || ReadBuffer() > 0) && byteBuffer[bytePos] == 0x0)
                            {
                                if ((++bytePos < byteLen || ReadBuffer() > 0) && byteBuffer[bytePos] == 0xA)
                                {
                                    bytePos++;
                                }
                                else
                                {
                                    bytePos--;
                                }
                            }
                            return new ValueTuple<long, int>(start, (int)(Position - start));
                        }
                    }
                    i++;
                } while (i < byteLen);
            } while (ReadBuffer() > 0);
            return new ValueTuple<long, int>(start, (int)(Position - start));
        }
        private ValueTuple<long, int>? ReadLinePositionUnicode()
        {
            if (stream == null)
                throw new ObjectDisposedException(null, "ObjectDisposed_ReaderClosed");

            if (bytePos == byteLen)
            {
#nullable disable
                if (ReadBuffer() == 0) return null;
#nullable enable
            }

            long start = Position;
            do
            {
                int i = bytePos;
                do
                {
                    byte b = byteBuffer[i];
                    if (b == 0xD || b == 0XA)
                    {
                        bytePos = i + 1;
                        if ((bytePos < byteLen || ReadBuffer() > 0) && byteBuffer[bytePos] == 0x0)
                        {
                            bytePos++;
                            if (b == 0xD && (bytePos < byteLen || ReadBuffer() > 0) && byteBuffer[bytePos] == 0xA)
                            {
                                if ((++bytePos < byteLen || ReadBuffer() > 0) && byteBuffer[bytePos] == 0x0)
                                {
                                    bytePos++;
                                }
                            }
                            return new ValueTuple<long, int>(start, (int)(Position - start));
                        }
                    }
                    i++;
                } while (i < byteLen);
            } while (ReadBuffer() > 0);
            return new ValueTuple<long, int>(start, (int)(Position - start));
        }
        private ValueTuple<long, int>? ReadLinePositionUTF8()
        {
            if (stream == null)
                throw new ObjectDisposedException(null, "ObjectDisposed_ReaderClosed");

            if (bytePos == byteLen)
            {
#nullable disable
                if (ReadBuffer() == 0) return null;
#nullable enable
            }

            long start = Position;
            do
            {
                int i = bytePos;
                do
                {
                    byte b = byteBuffer[i];
                    if (b == 0xD || b == 0XA)
                    {
                        bytePos = i + 1;
                        if (b == 0xD && (bytePos < byteLen || ReadBuffer() > 0) && byteBuffer[bytePos] == 0XA)
                        {
                            bytePos++;
                        }
                        return new ValueTuple<long, int>(start, (int)(Position - start));
                    }
                    i++;
                } while (i < byteLen);
            } while (ReadBuffer() > 0);
            return new ValueTuple<long, int>(start, (int)(Position - start));
        }

        public string? ReadString(long position, int length)
        {
            if (length <= 0)
            {
                return null;
            }
            if (position + length > stream.Length)
            {
                return null;
            }
            byte[] bBuffer = new byte[length];
            var lastPosition = stream.Position;
            stream.Seek(position, SeekOrigin.Begin);
            int bLen = stream.Read(bBuffer, 0, length);
            stream.Seek(lastPosition, SeekOrigin.Begin);
            var result = encoding.GetString(bBuffer, 0, bLen);
            return result;
        }

        public string? ReadString(ValueTuple<long, int>? linePosition)
        {
            if (linePosition == null)
            {
                return null;
            }
            return ReadString(linePosition.Value.Item1, linePosition.Value.Item2);
        }
    }
}
