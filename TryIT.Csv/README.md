## How to use this library

```
var csvReaderConfig = new TryIT.Csv.CsvReaderConfig
{
    FilePath = @"xxx",
    Delimiter = "|",
    HasHeaderRecord = true,
    SkipFooterRows = 1
};
DataTable dt = TryIT.Csv.Csv.ReadAsDataTable(csvReaderConfig);
```