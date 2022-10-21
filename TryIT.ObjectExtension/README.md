# ObjectExtionsion

the extension method for general ASP.NET type

## DataTableExtension

`IsNullOrEmpty()` : Indicator whether current table is `null` or `empty`

`ConvertColumnType()` : convert existing column data type to new type, e.g. from `Int` to `String`, if needs convert to ```System.Data.SqlTypes.SqlBinary```, the source value will process as string value ```Encoding.UTF8.GetBytes(value.ToString())```