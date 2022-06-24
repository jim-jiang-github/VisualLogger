using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualLogger.Console
{
    internal enum LogCellBinaryType
    {
        Boolean = -1,
        Byte = -2,
        Char = -3,
        Float = -4,
        Double = -5,
        Decimal = -6,
        UShort = -7,
        Short = -8,
        UInt = -9,
        Int = -10,
        ULong = -11,
        Long = -12,
    }
    public struct TestStruct
    {
        public TestStruct( bool sex, int age, int age1)
        {
            Sex = sex;
            Age = age;
            Age1 = age1;
        }

        public bool Sex { get; set; }
        public int Age { get; set; }
        public int Age1 { get; set; }

        public override string ToString()
        {
            return base.ToString();
        }
    }
    public struct TestRow
    {
        private readonly IEnumerable<TestStruct> _cells;

        public int Index { get; }
        public TestRow(int index, IEnumerable<TestStruct> cells)
        {
            Index = index;
            _cells = cells;
        }
    }
    public struct TestRowWrap
    {
        public Parent Parent { get; set; }
        public TestRow TestRow { get; set; }

        public TestRowWrap(Parent parent, TestRow testRow)
        {
            Parent = parent;
            TestRow = testRow;
        }
        public override string ToString()
        {
            return "xxxxxxxxxx:" + base.ToString();
        }
    }
    public class Parent
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public bool Sex { get; set; }
    }
}
