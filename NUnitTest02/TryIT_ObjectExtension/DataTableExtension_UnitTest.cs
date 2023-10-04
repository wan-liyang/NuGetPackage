using TryIT.ObjectExtension;
using NUnit.Framework;
using System.Collections.Generic;
using System.Data;
using TryIT.ExcelService;

namespace NUnitTest02.TryIT_ObjectExtension
{
    class DataTableExtension_UnitTest
    {
        public class List_Test
        {
            public string Prop1 { get; set; }
            public int Prop2 { get; set; }
            public string Prop3 { get; set; }
            public int? Prop4 { get; set; }
        }

        [Test]
        public void DataTable_ToList_Test()
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Col1");
            dataTable.Columns.Add("Col2");
            dataTable.Columns.Add("Col3");
            dataTable.Columns.Add("Col4");

            DataRow row = dataTable.NewRow();
            row["Col1"] = "abc";
            row["Col2"] = 123;
            row["Col3"] = "def";
            row["Col4"] = 456;
            dataTable.Rows.Add(row);

            DataRow row2 = dataTable.NewRow();
            row2["Col1"] = "hjk";
            row2["Col2"] = 789;
            row2["Col3"] = "lmn";
            dataTable.Rows.Add(row2);

            var keyValues = new Dictionary<string, string> {
                { "Col1", "Prop1"},
                { "Col2", "Prop2"},
                { "Col3", "Prop3"},
                { "Col4", "Prop4"},
            };

            var list = dataTable.ToList<List_Test>(keyValues);
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual("abc", list[0].Prop1);
            Assert.AreEqual(123, list[0].Prop2);
            Assert.AreEqual("def", list[0].Prop3);
            Assert.AreEqual(456, list[0].Prop4);

            Assert.AreEqual("hjk", list[1].Prop1);
            Assert.AreEqual(789, list[1].Prop2);
            Assert.AreEqual("lmn", list[1].Prop3);
            Assert.AreEqual(null, list[1].Prop4);

            // different map sequence
            keyValues = new Dictionary<string, string> {
                { "Col1", "Prop1"},
                { "Col3", "Prop3"},
                { "Col4", "Prop4"},
                { "Col2", "Prop2"},
            };
            list = dataTable.ToList<List_Test>(keyValues);
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual("abc", list[0].Prop1);
            Assert.AreEqual(123, list[0].Prop2);
            Assert.AreEqual("def", list[0].Prop3);
            Assert.AreEqual(456, list[0].Prop4);
        }


        [Test]
        public void DataTable_ToList_Test2()
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Col1");
            dataTable.Columns.Add("Col2");
            dataTable.Columns.Add("Col3");
            dataTable.Columns.Add("Col4");

            DataRow row = dataTable.NewRow();
            row["Col1"] = "abc";
            row["Col2"] = 123;
            row["Col3"] = "def";
            row["Col4"] = 456;
            dataTable.Rows.Add(row);

            DataRow row2 = dataTable.NewRow();
            row2["Col1"] = "hjk";
            row2["Col2"] = 789;
            row2["Col3"] = "lmn";
            row2["Col4"] = DBNull.Value;
            dataTable.Rows.Add(row2);

            var keyValues = new Dictionary<string, string> {
                { "Col1", "Prop1"},
                { "Col2", "Prop2"},
                { "Col3", "Prop3"},
                { "Col4", "Prop4"},
            };

            var list = dataTable.ToList<List_Test>(keyValues);
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual("abc", list[0].Prop1);
            Assert.AreEqual(123, list[0].Prop2);
            Assert.AreEqual("def", list[0].Prop3);
            Assert.AreEqual(456, list[0].Prop4);

            Assert.AreEqual("hjk", list[1].Prop1);
            Assert.AreEqual(789, list[1].Prop2);
            Assert.AreEqual("lmn", list[1].Prop3);
            Assert.AreEqual(null, list[1].Prop4);
        }

        [Test]
        public void DataTable_ToHtmlString_Test()
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Col1");
            dataTable.Columns.Add("Col2");
            dataTable.Columns.Add("Col3");
            dataTable.Columns.Add("Col4");

            DataRow row = dataTable.NewRow();
            row["Col1"] = "abc";
            row["Col2"] = 123;
            row["Col3"] = "def";
            row["Col4"] = 456;
            dataTable.Rows.Add(row);

            DataTableExtension.ToHtmlString_TableStyle style = new DataTableExtension.ToHtmlString_TableStyle
            {
                Table = "border-collapse: collapse;border: 1px solid;",
                Th = "border: 1px solid;",
                Td = "border: 1px solid;"
            };

            string html = dataTable.ToHtmlString(style);

            Assert.IsTrue(true);
        }

        [Test]
        public void ConvertColumnType_Test()
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Col1", typeof(string));
            dataTable.Columns.Add("Col2", typeof(int));
            dataTable.Columns.Add("Col3", typeof(bool));
            dataTable.Columns.Add("Col4", typeof(DateTime));
            dataTable.Columns.Add("Col5", typeof(decimal));

            DataRow row = dataTable.NewRow();
            row["Col1"] = "123";
            row["Col2"] = 123;
            row["Col3"] = true;
            row["Col4"] = DateTime.Now;
            row["Col5"] = 12.3;
            dataTable.Rows.Add(row);

            // before convert
            Assert.That(dataTable.Columns["Col1"].DataType, Is.EqualTo(typeof(string)));
            Assert.That(dataTable.Columns["Col2"].DataType, Is.EqualTo(typeof(int)));
            Assert.That(dataTable.Columns["Col3"].DataType, Is.EqualTo(typeof(bool)));
            Assert.That(dataTable.Columns["Col4"].DataType, Is.EqualTo(typeof(DateTime)));
            Assert.That(dataTable.Columns["Col5"].DataType, Is.EqualTo(typeof(decimal)));


            // do convert type
            dataTable.ConvertColumnType("Col1", typeof(int));
            dataTable.ConvertColumnType("Col2", typeof(string));
            dataTable.ConvertColumnType("Col3", typeof(string));
            dataTable.ConvertColumnType("Col4", typeof(string));
            dataTable.ConvertColumnType("Col5", typeof(string));

            // after convert
            Assert.That(dataTable.Columns["Col1"].DataType, Is.EqualTo(typeof(int)));
            Assert.That(dataTable.Columns["Col2"].DataType, Is.EqualTo(typeof(string)));
            Assert.That(dataTable.Columns["Col3"].DataType, Is.EqualTo(typeof(string)));
            Assert.That(dataTable.Columns["Col4"].DataType, Is.EqualTo(typeof(string)));
            Assert.That(dataTable.Columns["Col5"].DataType, Is.EqualTo(typeof(string)));
        }

        [Test]
        public void GroupBy_Test()
        {
            //DataTable dt = CreateSampleTable();

            var csvReaderConfig = new TryIT.Csv.CsvReaderConfig
            {
                FilePath = @"",
                Delimiter = "|",
                HasHeaderRecord = true,
                SkipFooterRows = 1
            };
            DataTable dt2 = TryIT.Csv.Csv.ReadAsDataTable(csvReaderConfig);

            var result = dt2.GroupBy(new string[] { "WBSELEMENT", "DOCNO", "DOCITEMNO", "DOCTYPE" });

            Assert.True(result == null);
        }
        DataTable CreateSampleTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("Column1", typeof(string));
            table.Columns.Add("Column2", typeof(int));
            table.Columns.Add("Column3", typeof(double));

            table.Rows.Add("Value1", 1, 1.1);
            table.Rows.Add("Value1", DBNull.Value, 1.1);
            table.Rows.Add("Value2", 2, 2.2);
            table.Rows.Add("Value2", 2, 2.2);
            table.Rows.Add("Value3", 1, 1.1);
            table.Rows.Add("Value3", 1, 1.1);
            table.Rows.Add("Value4", 3, 3.3);
            table.Rows.Add("Value5", 2, 2.2);

            return table;
        }
    }
}
