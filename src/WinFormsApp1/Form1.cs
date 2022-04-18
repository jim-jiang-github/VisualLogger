using VisualLogger;
using VisualLogger.Contents;
using VisualLogger.Datas;
using VisualLogger.InterfaceImplModules.LogStreamLoaders;
using VisualLogger.Schemas.Logs;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        static int Count;
        Stream _stream;
        public Form1()
        {
            InitializeComponent();
            MemoryMappedStreamLoader memoryMappedStreamLoader = new MemoryMappedStreamLoader();
            _stream = memoryMappedStreamLoader.LoadLogStream(@"C:\Users\Jim.Jiang\Downloads\WRoomsFeedBack_HostLog_1112e3df-80f9-435d-8b5d-2b7c5a76ee1f_20220407-172316\RoomsHost-20220407165140.rcvlog");

        }


        private void button1_Click(object sender, EventArgs e)
        {
            //12137MB
            //MixStreamReader mixStreamReader = new MixStreamReader(stream);
            //List<StreamCell> sss = new List<StreamCell>();
            //for (int i = 0; i < 111267; i++)
            //{
            //    sss.Add(new StreamCell(mixStreamReader, 0, 0, StreamCellType.Int, null));
            //}
            var logSchemaBinary = LogSchemaBinary.LoadFromJsonFile("LogSchemaBinary.json", out string? ssss);
            if (logSchemaBinary == null)
            {
                return;
            }
            LogContentBinary logContentBinary = new LogContentBinary(_stream, logSchemaBinary);
            var cells = logContentBinary.GetBodyItems("Content");
            LogSource logSource = new LogSource(_stream, logContentBinary.GetItemsTemplate("Content"), cells);
            this.textBox1.Text = LifeCycleViewer.GetLifeCycleInfo();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.WaitForFullGCComplete();
            GC.Collect();

            this.textBox1.Text = LifeCycleViewer.GetLifeCycleInfo();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}