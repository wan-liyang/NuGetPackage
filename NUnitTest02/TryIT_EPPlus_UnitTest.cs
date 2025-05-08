using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using TryIT.EPPlus;

namespace NUnitTest02;

public class TryIT_EPPlus_UnitTest
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        EPPlusLib ePPlusLib = new EPPlusLib(@"");

        var table = ePPlusLib.GetDataTable(new ExcelSheetReaderConfig
        {
            SheetIndex = 1,
            SkipHeaderRows = 1
        });

        Assert.IsTrue(table.Rows.Count > 0);
    }
}
