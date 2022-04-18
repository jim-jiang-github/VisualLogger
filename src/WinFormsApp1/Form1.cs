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

            //FileStreamLoader fileStreamLoader = new FileStreamLoader();
            //_stream = fileStreamLoader.LoadLogStream(@"C:\Users\Jim.Jiang\Downloads\88D77430\log\2022-04-16-154234.906-main.log");
        }

        IEnumerable<StreamCell[]> cells;
        private void button1_Click(object sender, EventArgs e)
        {


            //var logSchema = LogSchemaText.LoadFromJsonFile("LogSchemaText.json", out string? ssss);
            //if (logSchema == null)
            //{
            //    return;
            //}
            //LogContentText logContentText = new LogContentText(_stream, logSchema);
            //var cells = logContentText.GetBodyItems();


            var logSchemaBinary = LogSchemaBinary.LoadFromJsonFile("LogSchemaBinary.json", out string? ssss);
            if (logSchemaBinary == null)
            {
                return;
            }
            LogContentBinary logContentBinary = new LogContentBinary(_stream, logSchemaBinary);
            cells = logContentBinary.GetBodyItems();
            LogSource logSource = new LogSource(_stream, logContentBinary.GetBodyTemplate(), cells);
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