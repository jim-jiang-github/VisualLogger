using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VisualLogger.InterfaceImplModules.LogContentLoaders.Binary;
using VisualLogger.InterfaceImplModules.LogStreamLoaders;

namespace WinFormsApp1
{
    public class AAAA
    {
        BinaryReader binaryReader;
        ~AAAA()
        {
            Debug.WriteLine($"AAAA");
        }
        public AAAA(BinaryReader binaryReader)
        {
            this.binaryReader = binaryReader;
        }
        public class BBBB
        {
            BinaryReader binaryReader;
            ~BBBB()
            {
                var i = BIndex;
                Debug.WriteLine($"BBBB:{i.Index}");
            }
            public BBBB(BinaryReader binaryReader)
            {
                this.binaryReader = binaryReader;
            }
            public BBBBIndex BIndex { get; set; }
        }
        public class BBBBIndex
        {
            BinaryReader binaryReader;
            ~BBBBIndex()
            {
                var i = Index;
                Debug.WriteLine($"BBBBIndex:{Index}");
            }
            public BBBBIndex(BinaryReader binaryReader)
            {
                this.binaryReader = binaryReader;
            }
            public int Index { get; set; }
        }
        public int AIndex { get; set; }
        public List<BBBB> Bs { get; set; }
    }
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        AAAA.BBBBIndex[] indices;
        private void button1_Click(object sender, EventArgs e)
        {
            var parserFile = @"C:\Users\Jim.Jiang\Documents\VisualLogger\src\ConsoleApp1\RCRooms_Windows_Binary_Parser.json";
            var binaryParser = BinaryContentParser.LoadFromJsonFile(parserFile);
            var logFile = @"C:\Users\Jim.Jiang\Downloads\WRoomsFeedBack_HostLog_88cd1cff-de9c-4a14-9769-d4d512e077e0_20220210-155757\RoomsHost-20220210155123.rcvlog";
            MemoryMappedStreamLoader memoryMappedStreamLoader = new MemoryMappedStreamLoader();
            var stream = memoryMappedStreamLoader.LoadLogStream(logFile);
            BinaryLogLoader binaryLogLoader = BinaryLogLoader.Load(stream, binaryParser);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}
