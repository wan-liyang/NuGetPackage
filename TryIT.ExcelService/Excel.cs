using OfficeOpenXml;
using System;
using System.Data;
using System.IO;

namespace TryIT.ExcelService
{
    public class ExcelComponent : IDisposable
    {
        public ExcelPackage ExcelPackage { get; set; }

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
                        dtRow[newDRColIndex] = cell.Text.ToString();
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

        public void Dispose()
        {
            if (ExcelPackage != null)
            {
                ExcelPackage.Dispose();
            }
        }
    }
}
