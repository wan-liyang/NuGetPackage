using OfficeOpenXml;
using System;
using System.Data;
using System.IO;
using System.Collections.Generic;

namespace TryIT.EPPlus
{
    /// <summary>
    /// configuration for read excel file for a worksheet
    /// </summary>
    public class ExcelSheetReaderConfig
    {
        /// <summary>
        /// the worksheet index to read data, start from 1
        /// </summary>
        public int SheetIndex { get; set; } = 1;

        /// <summary>
        /// the rows skip from excel while read data, 
        /// <para>defualt 0, it will not skip any row, take row 1 as header, read from row 2 as data</para>
        /// <para>if set to 1, it will skip row 1, take row 2 as header, read from row 3 as data</para>
        /// </summary>
        public int SkipHeaderRows { get; set; } = 0;
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class EPPlusLib : IDisposable
    {
        /// <summary>
        /// excel package instance after load file, this is to allow consumer do customize action as need
        /// </summary>
        public ExcelPackage ExcelPackage
        {
            get
            {
                return _excelPackage;
            }
        }

        /// <summary>
        /// excel package after load file
        /// </summary>
        private ExcelPackage _excelPackage { get; set; }

        /// <summary>
        /// initial excel package from a file
        /// </summary>
        /// <param name="fileNameAndPath"></param>
        /// <exception cref="FileNotFoundException"></exception>
        public EPPlusLib(string fileNameAndPath)
        {
            FileInfo fileInfo = new FileInfo(fileNameAndPath);
            if (File.Exists(fileNameAndPath))
            {
                _excelPackage = new ExcelPackage(fileInfo);
                _excelPackage.Compatibility.IsWorksheets1Based = true;
            }
            else
            {
                string fileName = Path.GetFileName(fileNameAndPath);
                throw new FileNotFoundException($"file '{fileName}' not found.");
            }
        }

        /// <summary>
        /// determine cell is number format
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public static bool IsNumberFormat(ExcelRangeBase cell)
        {
            // when talk about format codes, # is only used for number formats. It is not used for text, date, time, or other formats.
            if (cell.Style.Numberformat.Format.Contains("#"))
            {
                return true;
            }

            int numId = cell.Style.Numberformat.NumFmtID;

            switch (numId)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                case 9:
                case 10:
                case 11:
                case 37:
                case 38:
                case 39:
                case 40:
                case 48:
                case 170: return true;
                default:
                    return false;
            }

            /*
             0 => "General",
            1 => "0",
            2 => "0.00",
            3 => "#,##0",
            4 => "#,##0.00",
            9 => "0%",
            10 => "0.00%",
            11 => "0.00E+00",
            12 => "# ?/?",
            13 => "# ??/??",
            14 => "mm-dd-yy",
            15 => "d-mmm-yy",
            16 => "d-mmm",
            17 => "mmm-yy",
            18 => "h:mm AM/PM",
            19 => "h:mm:ss AM/PM",
            20 => "h:mm",
            21 => "h:mm:ss",
            22 => "m/d/yy h:mm",
            37 => "#,##0 ;(#,##0)",
            38 => "#,##0 ;[Red](#,##0)",
            39 => "#,##0.00;(#,##0.00)",
            40 => "#,##0.00;[Red](#,##0.00)",
            45 => "mm:ss",
            46 => "[h]:mm:ss",
            47 => "mmss.0",
            48 => "##0.0",
            49 => "@",
            170 => _-* #,##0.0_-;\-* #,##0.0_-;_-* "-"??_-;_-@_-
            _ => string.Empty,
             */
        }

        /// <summary>
        /// get data from a worksheet as <see cref="DataTable"/>
        /// </summary>
        /// <param name="sheetReaderConfig"></param>
        /// <returns></returns>
        public DataTable GetDataTable(ExcelSheetReaderConfig sheetReaderConfig)
        {
            var worksheet = _excelPackage.Workbook.Worksheets[sheetReaderConfig.SheetIndex];

            DataTable dt = new DataTable();
            //check if the worksheet is completely empty
            if (worksheet.Dimension == null)
            {
                return dt;
            }

            // get max column index, exclude empty column (all row empty)
            int maxColumnIndex = 0;
            for (int i = worksheet.Dimension.End.Column; i >= 1; i--)
            {
                // if the empty column has format applied, it will be included in worksheet.Dimension.End.Column, use this to skip that column
                if (string.IsNullOrEmpty(worksheet.Cells[1, i].Text))
                    continue;

                for (int j = 1; j <= worksheet.Dimension.End.Row; j++)
                {
                    var cell = worksheet.Cells[j, i];
                    if (!string.IsNullOrEmpty(cell.Text.Trim()))
                    {
                        maxColumnIndex = i;
                        break;
                    }
                }
                if (maxColumnIndex > 0)
                {
                    break;
                }
            }

            /*
                adding column to table, if same column name appear in excel, the next column will be rename with index number
                ColumnName => ColumnName
                ColumnName => ColumnName2            
            */

            // indicate which row to read column name
            int columnRow = sheetReaderConfig.SkipHeaderRows + 1;
            List<string> columns = new List<string>();
            for (int i = 1; i <= maxColumnIndex; i++)
            {
                var columnName = worksheet.Cells[columnRow, i].Text;
                int index = 2;
                while (columns.Contains(columnName))
                {
                    columnName = $"{columnName}{index}";
                    index++;
                }
                dt.Columns.Add(columnName);
                columns.Add(columnName);
            }

            // indicate which row to start read data
            int dataRowStart = columnRow + 1;
            for (int i = dataRowStart; i <= worksheet.Dimension.End.Row; i++)
            {
                DataRow dtRow = dt.NewRow();

                // range of row i (inluding all columns)
                var range = worksheet.Cells[i, 1, i, maxColumnIndex];

                int newDRColIndex = 0;
                // use isEmptyRow to check whether entire row is empty, if yes, then not need add to datatable
                // because if the cell has formula, worksheet will include it in Dimension.End
                bool isEmptyRow = true;

                foreach (var cell in range)
                {
                    newDRColIndex = cell.Start.Column - 1;

                    if (!string.IsNullOrEmpty(cell.Text))
                    {
                        // if cell is number format, get actual value, e.g. cell.Value is 3.1415926, cell.Text is 3.14
                        if (IsNumberFormat(cell))
                        {
                            dtRow[newDRColIndex] = cell.Value.ToString();
                        }
                        else
                        {
                            dtRow[newDRColIndex] = cell.Text.ToString();
                        }
                    }

                    // Once isEmptyRow is false, skip
                    if (isEmptyRow 
                        && dtRow[newDRColIndex] != null 
                        && !string.IsNullOrEmpty(dtRow[newDRColIndex].ToString().Trim()))
                    {
                        isEmptyRow = false;
                    }
                }

                // Do not add empty row.
                if (!isEmptyRow)
                {
                    dt.Rows.Add(dtRow);
                }
            }
            return dt;
        }

        /// <summary>
        /// release resource
        /// </summary>
        public void Dispose()
        {
            if (_excelPackage != null)
            {
                _excelPackage.Dispose();
            }
        }
    }
}
