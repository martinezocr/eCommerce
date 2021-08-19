using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Data;
using System.IO;

namespace eCommerce.Web.Helpers
{
    /// <summary>
    /// Clase para el manejo de excels
    /// </summary>
    public class Excel
    {
        /// <summary>
        /// mimetype xlsx
        /// </summary>
        public const string MIME_XLSX = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        /// <summary>
        /// genera el excel a partir de un datatable y devuelve el array de bytes
        /// </summary>
        /// <param name="data">datatable con los datos para generar el excel</param>
        /// <param name="sheetname">nombre que se le va a poner a la hoja del excel</param>
        /// <returns></returns>
        internal static byte[] ToExcel(DataTable data, string sheetname)
        {
            XSSFWorkbook wb = new XSSFWorkbook();

            IFont font = wb.CreateFont();
            font.FontHeightInPoints = (short)16;
            font.FontName = "Calibri";
            font.Color = IndexedColors.White.Index;
            font.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

            XSSFCellStyle c1Style = (XSSFCellStyle)wb.CreateCellStyle();
            byte[] rgb = new byte[3] { 103, 57, 141 };
            c1Style.SetFillForegroundColor(new XSSFColor(rgb));

            c1Style.FillPattern = FillPattern.SolidForeground;
            c1Style.SetFont(font);

            XSSFSheet sheet = wb.CreateSheet(sheetname) as XSSFSheet;
            //creo la cabecera
            IRow header = sheet.CreateRow(0);
            for (int i = 0; i < data.Columns.Count; i++)
            {
                ICell cellHeader = header.CreateCell(i);
                cellHeader.SetCellValue(data.Columns[i].ColumnName);
                cellHeader.CellStyle = c1Style;
            }
            //contenido
            for (int i = 0; i < data.Rows.Count; i++)
            {
                IRow rows = sheet.CreateRow(i + 1);
                for (int j = 0; j < data.Columns.Count; j++)
                {
                    ICell cellName = rows.CreateCell(j);
                    cellName.SetCellValue(data.Rows[i][j].ToString());
                }
            }
            if (sheet.PhysicalNumberOfRows > 0)
                sheet.GetRow(0).Cells.ForEach(p => sheet.AutoSizeColumn(p.ColumnIndex)); //ajuste de columnas

            //Devuelve excell
            using var exportData = new MemoryStream();
            wb.Write(exportData);
            return exportData.ToArray();
        }
    }
}
