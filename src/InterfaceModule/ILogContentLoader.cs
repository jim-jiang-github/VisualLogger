using System;
using System.Collections;
using System.Collections.Generic;

namespace InterfaceModule
{
    public interface ILogContentLoader
    {
        LogContent LoadLogContent(string logPath);
    }

    public class LogContent
    {
        public class LogItem
        {
            public IEnumerable<object> Datas { get; }

            public LogItem(IEnumerable<object> datas)
            {
                Datas = datas;
            }
        }
        public string[] ColumnsName { get; }
        public IEnumerable<LogItem> Items { get; }

        public LogContent(string[] columnsName, IEnumerable<LogItem> items)
        {
            ColumnsName = columnsName;
            Items = items;
        }
    }
}
