using System;
using System.Data;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Text;
using static ConsoleApp1.Program;
using System.Runtime.Remoting;
using System.IO.Pipes;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

namespace ConsoleApp1
{
    class Program
    {
        const int INVALID_HANDLE_VALUE = -1;
        const int PAGE_READWRITE = 0x04;
        const string SHARE_MEMORY_NAME = "RC-Rooms-IpcPubSub-Service-Share-Memeory";

        [DllImport("Kernel32.dll", EntryPoint = "CreateFileMapping")]
        private static extern IntPtr CreateFileMapping(IntPtr hFile, //HANDLE hFile,
       UInt32 lpAttributes,//LPSECURITY_ATTRIBUTES lpAttributes,  //0
       UInt32 flProtect,//DWORD flProtect
       UInt32 dwMaximumSizeHigh,//DWORD dwMaximumSizeHigh,
       UInt32 dwMaximumSizeLow,//DWORD dwMaximumSizeLow,
       string lpName//LPCTSTR lpName
       );
        const int FILE_MAP_ALL_ACCESS = 0x0002;
        const int FILE_MAP_WRITE = 0x0002;

        [DllImport("Kernel32.dll", EntryPoint = "MapViewOfFile")]
        private static extern IntPtr MapViewOfFile(
         IntPtr hFileMappingObject,//HANDLE hFileMappingObject,
         UInt32 dwDesiredAccess,//DWORD dwDesiredAccess
         UInt32 dwFileOffsetHight,//DWORD dwFileOffsetHigh,
         UInt32 dwFileOffsetLow,//DWORD dwFileOffsetLow,
         UInt32 dwNumberOfBytesToMap//SIZE_T dwNumberOfBytesToMap
         );

        [DllImport("Kernel32.dll", EntryPoint = "UnmapViewOfFile")]
        private static extern int UnmapViewOfFile(IntPtr lpBaseAddress);

        static Semaphore m_Write;
        static Semaphore m_Read;
        private static IntPtr handle;     //文件句柄
        private static IntPtr addr;       //共享内存地址
        static void Main(string[] args)
        {
            //Console.WriteLine("Hello World!");
            //ShareMemLib shareMemLib = new ShareMemLib();
            //if (shareMemLib.Init(SHARE_MEMORY_NAME, 306084) == 0)
            //{
            //    byte[] bytData = new byte[10240];
            //    int intRet = shareMemLib.Read(ref bytData, 0, bytData.Length);
            //    string stra = Encoding.UTF8.GetString(bytData, 0, bytData.Length);
            //}

            //IntPtr hFile = new IntPtr(INVALID_HANDLE_VALUE);
            //m_Write = new Semaphore(1, 1, "WriteMap");//开始的时候有一个可以写
            //m_Read = new Semaphore(0, 1, "ReadMap");
            //var handle = CreateFileMapping(hFile, 0, PAGE_READWRITE, 0, 306084, SHARE_MEMORY_NAME);
            //addr = MapViewOfFile(handle, FILE_MAP_ALL_ACCESS, 0, 0, 0);
            //m_Read.WaitOne();
            //byte[] byteStr = new byte[100];
            //byteCopy(byteStr, addr);
            //string str = Encoding.Default.GetString(byteStr, 0, byteStr.Length);
            ///////调用数据处理方法 处理读取到的数据
            //m_Write.Release();
            //var pipe = new NamedPipeClientStream(".", "lindexi", PipeDirection.InOut, PipeOptions.None);

            using MemoryMappedFile memoryMappedFile = MemoryMappedFile.CreateFromFile(@"C:\Users\Jim.Jiang\Downloads\mmf\RoomsHost-20220321112105.rcvlog");

            using var stream = memoryMappedFile.CreateViewStream();
            var path = @"C:\Users\Jim.Jiang\Documents\VisualLogger\src\ConsoleApp1\RCRooms_Windows_Binary_Description.json";
            var binaryLoader = BinaryLoader.LoadFromBinaryDescription(path);
            binaryLoader.LoadFromStream(stream);
            var a = binaryLoader.Objects;
            BinaryReader binaryReader = new BinaryReader(stream);
            var logFileHeader = LogFileHeader.LoadFromStream(binaryReader);
            var logSummary = LogSummary.LoadFromStream(binaryReader);
            for (int i = 0; i < logSummary.ItemCount; i++)
            {
                var itemData = LogItem.LoadFromStream(binaryReader);

            }
        }  //不安全的代码在项目生成的选项中选中允许不安全代码
        static unsafe void byteCopy(byte[] dst, IntPtr src)
        {
            fixed (byte* pDst = dst)
            {
                byte* pdst = pDst;
                byte* psrc = (byte*)src;
                while ((*pdst++ = *psrc++) != '\0')
                    ;
            }

        }

        public class LogFileHeader
        {
            public string Signature { get; }
            public int MagicNumber { get; }
            public int EncryptKey { get; }
            public int SummarySize { get; }
            public int FileMaxSize { get; }
            public int FileCurSize { get; }
            public string Reserved { get; }
            private LogFileHeader(BinaryReader binaryReader)
            {
                Signature = Encoding.UTF8.GetString(binaryReader.ReadBytes(3));

                //self.bom = bytes().join(struct.unpack('2c', buf[3:5]))
                //self.os,self.logVersion,self.encode = struct.unpack('3c', buf[5:8])
                binaryReader.ReadBytes(2 + 3);

                MagicNumber = binaryReader.ReadInt32();
                EncryptKey = binaryReader.ReadInt32();
                SummarySize = binaryReader.ReadInt32();
                FileMaxSize = binaryReader.ReadInt32();
                FileCurSize = binaryReader.ReadInt32();
                Reserved = Encoding.UTF8.GetString(binaryReader.ReadBytes(32));
            }

            public static LogFileHeader LoadFromStream(BinaryReader binaryReader)
            {
                return new LogFileHeader(binaryReader);
            }
        }
        public class LogSummary
        {
            public int TimeZone { get; }
            public long StartTime { get; }
            public uint StartTimeMS { get; }
            public string ProcessName { get; }
            public int ProcessId { get; }
            public int ItemCount { get; }
            public string Reserved { get; }
            private LogSummary(BinaryReader binaryReader)
            {
                //if (platform.system() == 'Windows'):
                //self.timeZone = struct.unpack('1l', buf[0:4])[0]
                //else:
                //self.timeZone = struct.unpack('1l', buf[0:8])[0]
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    TimeZone = binaryReader.ReadInt32();
                    binaryReader.ReadBytes(4);
                }
                else
                {
                    TimeZone = (int)binaryReader.ReadInt64();
                }
                StartTime = binaryReader.ReadInt64();
                StartTimeMS = binaryReader.ReadUInt32();
                ProcessName = Encoding.UTF8.GetString(binaryReader.ReadBytes(256));
                ProcessId = binaryReader.ReadInt32();
                ItemCount = binaryReader.ReadInt32();
                Reserved = Encoding.UTF8.GetString(binaryReader.ReadBytes(32));
                binaryReader.ReadBytes(4); //316 + 4 =320
            }

            public static LogSummary LoadFromStream(BinaryReader binaryReader)
            {
                return new LogSummary(binaryReader);
            }
        }
        public class LogItem
        {
            public long TickOffset { get; }
            public string ModuleName { get; }
            public uint ThreadId { get; }
            public int Level { get; }
            public string Hint { get; }
            public string Msg { get; }
            public string Reserved { get; }
            private LogItem(BinaryReader binaryReader)
            {
                TickOffset = binaryReader.ReadInt64();
                var dataLength = binaryReader.ReadInt32();
                ModuleName = Encoding.UTF8.GetString(binaryReader.ReadBytes(dataLength)).Trim();
                ThreadId = binaryReader.ReadUInt32();
                Level = binaryReader.ReadInt32();
                dataLength = binaryReader.ReadInt32();
                Hint = Encoding.UTF8.GetString(binaryReader.ReadBytes(dataLength)).Trim();
                dataLength = binaryReader.ReadInt32();
                Msg = Encoding.UTF8.GetString(binaryReader.ReadBytes(dataLength)).Trim();
            }

            public static LogItem LoadFromStream(BinaryReader binaryReader)
            {
                return new LogItem(binaryReader);
            }
        }

    }

    public interface ILogLoader<TLogItem> where TLogItem : class, new()
    {
        IEnumerable<TLogItem> LogItems { get; }
        void LoadLogItems();
        void ListenLiveLogItems();
    }


}
