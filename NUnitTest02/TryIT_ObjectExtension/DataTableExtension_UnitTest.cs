using TryIT.ObjectExtension;
using NUnit.Framework;
using System.Collections.Generic;
using System.Data;

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
    }
}
