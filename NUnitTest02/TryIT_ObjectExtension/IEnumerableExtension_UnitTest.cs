using NUnit.Framework;
using TryIT.ObjectExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace NUnitTest02.TryIT_ObjectExtension
{
    class IEnumerableExtension_UnitTest
    {
        public class List_Test
        {
            public string Prop1 { get; set; }
            public int Prop2 { get; set; }
            public string Prop3 { get; set; }
            public int? Prop4 { get; set; }
        }

        [Test]
        public void ToDataTable_Test()
        {
            var list = new List<List_Test>
            {
                new List_Test
                {
                   Prop1 = "Prop1 - 1",
                   Prop2 = 2,
                   Prop3 = "Prop3",
                   Prop4 = null
                },
                new List_Test
                {
                   Prop1 = "Prop1 - 2",
                   Prop2 = 2,
                   Prop3 = "Prop3",
                   Prop4 = null
                }
            };

            DataTable dt = list.ToDataTable();

            Assert.AreEqual(2, dt.Rows.Count);
            Assert.AreEqual(4, dt.Columns.Count);
            Assert.AreEqual(list[0].Prop1, dt.Rows[0][0]);
            Assert.AreEqual(list[0].Prop1, dt.Rows[0]["Prop1"]);


            List_Test item = new List_Test
            {
                Prop1 = "Prop1 - 1",
                Prop2 = 2,
                Prop3 = "Prop3",
                Prop4 = null
            };

            DataTable dt2 = item.ToDataTable();
            Assert.AreEqual(1, dt2.Rows.Count);
            Assert.AreEqual(4, dt2.Columns.Count);
            Assert.AreEqual(item.Prop1, dt2.Rows[0][0]);
            Assert.AreEqual(item.Prop1, dt2.Rows[0]["Prop1"]);
        }
    }
}
