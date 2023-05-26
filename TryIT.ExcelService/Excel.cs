using OfficeOpenXml;
using System;
using System.Data;
using System.IO;

namespace TryIT.ExcelService
{
    /// <summary>
    /// excel component
    /// </summary>
    public class ExcelComponent : IDisposable
    {
        /// <summary>
        /// excel package after load file
        /// </summary>
        public ExcelPackage ExcelPackage { get; set; }

        /// <summary>
        /// initial excell package from a file
        /// </summary>
        /// <param name="fileNameAndPath"></param>
        /// <exception cref="FileNotFoundException"></exception>
        public ExcelComponent(string fileNameAndPath)
        {
            FileInfo fileInfo = new FileInfo(fileNameAndPath);
            if (File.Exists(fileNameAndPath))
            {
                ExcelPackage = new ExcelPackage(fileInfo);
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
        public bool IsNumberFormat(ExcelRangeBase cell)
        {
            // when talk about format codes, # is only used for number formats. It is not used for text, date, time, or other formats.
            if(cell.Style.Numberformat.Format.Contains("#"))
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
        /// get DataTable from WorkSheet, first row will be Table Header, all column is String type
        /// </summary>
        /// <param name="worksheet"></param>
        /// <returns></returns>
        public DataTable GetDataTable(ExcelWorksheet worksheet)
        {
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

            // adding column to table
            for (int i = 1; i <= maxColumnIndex; i++)
            {
                var headerCell = worksheet.Cells[1, i];
                dt.Columns.Add(headerCell.Text);
            }

            for (int i = 2; i <= worksheet.Dimension.End.Row; i++)
            {
                DataRow dtRow = dt.NewRow();
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
                    if (isEmptyRow && dtRow[newDRColIndex] != null && !string.IsNullOrEmpty(dtRow[newDRColIndex].ToString().Trim()))
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
            if (ExcelPackage != null)
            {
                ExcelPackage.Dispose();
            }
        }
    }
}
