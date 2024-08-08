using TryIT.ObjectExtension;
using NUnit.Framework;
using System.Collections.Generic;
using System.Data;
using TryIT.ExcelService;
using System.Diagnostics;

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

        public enum EnumTest
        {
            None,
            FirstEnum,
            SecondEnum,
        }

        public class List_Test2
        {
            public string Prop1 { get; set; }
            public int Prop2 { get; set; }
            public string Prop3 { get; set; }
            public EnumTest Prop4 { get; set; }
        }

        [Test]
        public void DataTable_ToList_WithEnum()
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
            row["Col4"] = "FirstEnum";
            dataTable.Rows.Add(row);

            DataRow row2 = dataTable.NewRow();
            row2["Col1"] = "hjk";
            row2["Col2"] = 789;
            row2["Col3"] = "lmn";
            row2["Col4"] = "SecondEnum";
            dataTable.Rows.Add(row2);

            DataRow row3 = dataTable.NewRow();
            row3["Col1"] = "456";
            row3["Col2"] = 456;
            row3["Col3"] = "aaaa";
            row3["Col4"] = "";
            dataTable.Rows.Add(row3);

            var keyValues = new Dictionary<string, string> {
                { "Col1", "Prop1"},
                { "Col2", "Prop2"},
                { "Col3", "Prop3"},
                { "Col4", "Prop4"},
            };

            var list = dataTable.ToList<List_Test2>(keyValues);

            Assert.AreEqual(EnumTest.FirstEnum, list.First().Prop4);
            Assert.AreEqual(EnumTest.SecondEnum, list.Where(p => p.Prop2 == 789).First().Prop4);
            Assert.AreEqual(EnumTest.None, list.Where(p => p.Prop2 == 456).First().Prop4);

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

        [Test]
        public void ExcludeRows1()
        {
            // Initialize DataTables with dynamic columns
            DataTable dt1 = new DataTable();
            dt1.Columns.Add("Column1", typeof(string));
            dt1.Columns.Add("Column2", typeof(int));
            dt1.Columns.Add("Column3", typeof(bool));

            DataTable dt2 = new DataTable();
            dt2.Columns.Add("Column_1", typeof(string));
            dt2.Columns.Add("Column_2", typeof(int));
            dt2.Columns.Add("Column_3", typeof(bool));

            // Add some test data
            dt1.Rows.Add("Value1", 1, true);
            dt1.Rows.Add("Value2", 2, false);
            dt1.Rows.Add("Value4", 2, false);

            dt2.Rows.Add("Value1", 1, true);
            dt2.Rows.Add("Value2", 2, false);
            dt2.Rows.Add("Value3", 3, true);

            Dictionary<string, string> keysMap = new Dictionary<string, string>
            {
                { "Column1", "Column_1"}
            };

            // Act
            DataTable resultTable = dt1.ExcludeRows(dt2, keysMap);

            // Assert
            Assert.That(resultTable.Rows.Count, Is.EqualTo(1));

            //Assert.AreEqual("Value3", resultTable.Rows[0]["Column1"]);
            //Assert.AreEqual(3, resultTable.Rows[0]["Column2"]);
            //Assert.AreEqual(true, resultTable.Rows[0]["Column3"]);
        }

        [Test]
        public void ExcludeRows2()
        {
            // Initialize DataTables with dynamic columns
            DataTable dt1 = new DataTable();
            dt1.Columns.Add("Column1", typeof(string));
            dt1.Columns.Add("Column2", typeof(int));
            dt1.Columns.Add("Column3", typeof(bool));

            DataTable dt2 = new DataTable();
            dt2.Columns.Add("Column_1", typeof(string));
            dt2.Columns.Add("Column_2", typeof(int));
            dt2.Columns.Add("Column_3", typeof(bool));

            // Add some test data
            dt1.Rows.Add("Value1", 1, true);
            dt1.Rows.Add("Value2", 2, false);
            dt1.Rows.Add("Value4", 2, false);

            dt2.Rows.Add("Value1", 1, true);
            dt2.Rows.Add("Value2", 3, false);
            dt2.Rows.Add("Value3", 3, true);

            Dictionary<string, string> keysMap = new Dictionary<string, string>
            {
                { "Column1", "Column_1"},
                { "Column2", "Column_2"}
            };

            // Act
            DataTable resultTable = dt1.ExcludeRows(dt2, keysMap);

            // Assert
            Assert.That(resultTable.Rows.Count, Is.EqualTo(2));

            //Assert.AreEqual("Value3", resultTable.Rows[0]["Column1"]);
            //Assert.AreEqual(3, resultTable.Rows[0]["Column2"]);
            //Assert.AreEqual(true, resultTable.Rows[0]["Column3"]);
        }

        [Test]
        public void ExcludeRows3()
        {
            // Create two DataTables with a million rows each
            DataTable dt1 = CreateDataTable(100000);
            DataTable dt2 = CreateDataTable(100000);

            // Exclude rows from dt1 that are present in dt2
            Dictionary<string, string> keysMap = new Dictionary<string, string>
            {
                { "Column1", "Column1"},
                { "Column2", "Column2"}
            };

            Stopwatch stopwatch = Stopwatch.StartNew();
            DataTable filteredDt = dt1.ExcludeRows(dt2, keysMap);
            stopwatch.Stop();

            Console.WriteLine($"Filtered DataTable has {filteredDt.Rows.Count} rows.");
            Console.WriteLine($"Execution time: {stopwatch.ElapsedMilliseconds} ms");
        }

        static DataTable CreateDataTable(int rowCount)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Column1", typeof(int));
            dt.Columns.Add("Column2", typeof(string));
            dt.Columns.Add("Column3", typeof(DateTime));

            Random random = new Random();
            for (int i = 0; i < rowCount; i++)
            {
                dt.Rows.Add(i, Guid.NewGuid().ToString(), DateTime.Now.AddDays(random.Next(-365, 365)));
            }


            dt.Rows.Add(100001, null, null);
            dt.Rows.Add(100002, "aaa", null);

            return dt;
        }

        [Test]
        public void ExcludeRows4()
        {
            Dictionary<string, string> keysMap = new Dictionary<string, string>
            {
                { "Column1", "Column1"},
                { "Column2", "Column2"}
            };

            // Create two DataTables with a million rows each
            DataTable dt1 = CreateDataTable(100000);
            DataTable dt2 = CreateDataTable(100000);

            Stopwatch stopwatch = Stopwatch.StartNew();

            // Create a dictionary to store rows from dt2
            var dt2RowsDictionary = dt2.AsEnumerable()
                                       .ToDictionary(row => GetCompositeKey(row, keysMap), row => row);

            // Get the rows from dt1 that are not present in dt2
            var rowsToInclude = dt1.AsEnumerable()
                                   .Where(row1 => !dt2RowsDictionary.ContainsKey(GetCompositeKey(row1, keysMap)))
                                   .CopyToDataTable();

            stopwatch.Stop();

            Console.WriteLine($"Filtered DataTable has {rowsToInclude.Rows.Count} rows.");
            Console.WriteLine($"Execution time: {stopwatch.ElapsedMilliseconds} ms");

        }

        // Helper method to create a composite key from a row and a dictionary of column mappings
        static string GetCompositeKey(DataRow row, Dictionary<string, string> keysMap)
        {
            return string.Join("_", keysMap.Select(kvp => row.Field<object>(kvp.Key).ToString().GetHashCode()));
        }



        [Test]
        public void ToList_StringToGuid()
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Col1", typeof(string));

            dataTable.Rows.Add(Guid.NewGuid().ToString());

            var list = dataTable.ToList<TableToList>();

            Assert.IsTrue(list.Count == 1);
        }
        class TableToList
        {
            public Guid Col1 { get; set; }
        }



        [Test]
        public void ToList_GuidToString()
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Col1", typeof(Guid));

            dataTable.Rows.Add(Guid.NewGuid());

            var list = dataTable.ToList<TableToList2>();

            Assert.IsTrue(list.Count == 1);
        }

        class TableToList2
        {
            public string Col1 { get; set; }
        }
    }
}
