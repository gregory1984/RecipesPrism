using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using Recipes_Prism.ViewModels;

namespace Recipes_Prism.Helpers
{
    public static class ExcelReports
    {
        public static void MakeSelectedOrderReport(string destinationPath, OrderViewModel order)
        {
            var destinationFile = new FileInfo(destinationPath);

            if (File.Exists(destinationPath))
                File.Delete(destinationPath);

            using (var excel = new ExcelPackage(destinationFile))
            {
                MakeComponentsWorksheet(excel, order);
                MakeProductsWorksheet(excel, order);

                excel.Save();
            }
        }

        private static void MakeComponentsWorksheet(ExcelPackage excel, OrderViewModel order)
        {
            var worksheet = excel.Workbook.Worksheets.Add(order.Name);
            worksheet.Cells.Style.Font.Name = "Segoe UI";

            for (int i = 0; i < 13; i++)
                worksheet.Cells[$"A{i + 1}:F{i + 1}"].Merge = true;

            worksheet.Cells["A1"].Value = "Zakład Produkcyjno Handlowy";
            worksheet.Cells["A1"].Style.Font.Bold = true;
            worksheet.Cells["A1"].Style.Font.Size = 14;

            worksheet.Cells["A2"].Value = "P R O D M A P O L s.c.";
            worksheet.Cells["A2"].Style.Font.Size = 14;
            worksheet.Cells["A2"].Style.Font.Bold = true;

            worksheet.Cells["A3"].Value = "ul. Cmentarna 11/14; 98-270 Złoczew";
            worksheet.Cells["A3"].Style.Font.Italic = true;

            worksheet.Cells["A5"].Value = ":: RECEPTURA MATERIAŁOWA ::";
            worksheet.Cells["A5"].Style.Font.Size = 14;
            worksheet.Cells["A5"].Style.Font.Bold = true;
            worksheet.Cells["A5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            worksheet.Cells["A7"].Value = $"Nazwa zlecenia: {order.Name.Trim()}";
            worksheet.Cells["A7"].Style.Font.Bold = true;
            worksheet.Cells["A7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            worksheet.Cells["A8"].Value = $"Numer zlecenia: {order.OrderNo.Trim()}";
            worksheet.Cells["A8"].Style.Font.Bold = true;
            worksheet.Cells["A8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            worksheet.Cells["A9"].Value = $"Ilość sztuk: {order.ItemCountFormatted}";
            worksheet.Cells["A9"].Style.Font.Bold = true;
            worksheet.Cells["A9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            worksheet.Cells["A10"].Value = $"Data: {order.Date.ToString("yyyy-MM-dd")}";
            worksheet.Cells["A10"].Style.Font.Bold = true;
            worksheet.Cells["A10"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            var components = order.Mounts.Select(m => new
            {
                No = m.No,
                ComponentName = m.ComponentName,
                MeasureName = m.MeasureName,
                MeasureCount = m.MeasureCount,
                ItemCount = m.ItemCount,
                Comments = ""
            });

            var groupedByMeasure = components.GroupBy(m => m.MeasureName).Select(m => new
            {
                MeasureName = m.Key,
                MeasureCount = m.Sum(mount => mount.MeasureCount)
            }).ToList();

            var sumGroupedByMeasure = string.Join(" ", groupedByMeasure.Select(g => "[ " + Math.Round(g.MeasureCount, 2).ToString() + g.MeasureName + " ]"));
            var sumUnitIndependent = Math.Round(components.Sum(m => m.MeasureCount), 2).ToString();

            worksheet.Cells["A11"].Value = $"Razem: {sumGroupedByMeasure}";
            worksheet.Cells["A11"].Style.Font.Bold = true;
            worksheet.Cells["A11"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            worksheet.Cells["A12"].Value = $"Razem bez jednostek: {sumUnitIndependent}";
            worksheet.Cells["A12"].Style.Font.Bold = true;
            worksheet.Cells["A12"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            worksheet.Cells["A14"].Value = "Lp.";
            worksheet.Cells["B14"].Value = "Nazwa składnika";
            worksheet.Cells["C14"].Value = "Miara";
            worksheet.Cells["D14"].Value = "Ilość";
            worksheet.Cells["E14"].Value = "Ilość sztuk";
            worksheet.Cells["F14"].Value = "Uwagi";
            worksheet.Cells["A14:F14"].Style.Font.Bold = true;
            worksheet.Cells["A14"].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            worksheet.Cells["B14"].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            worksheet.Cells["C14"].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            worksheet.Cells["D14"].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            worksheet.Cells["E14"].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            worksheet.Cells["F14"].Style.Border.BorderAround(ExcelBorderStyle.Thin);

            //worksheet.Cells["A15:F15"].AutoFilter = true;

            worksheet.Cells["A15"].LoadFromCollection(components, false, TableStyles.Light1);
            var gridStartLine = 15;
            for (var i = gridStartLine; i < components.Count() + gridStartLine; i++)
            {
                worksheet.Cells[$"A{i}"].Style.Font.Bold = true;
                worksheet.Cells[$"A{i}"].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[$"B{i}"].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[$"C{i}"].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[$"D{i}"].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[$"E{i}"].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[$"F{i}"].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            }

            worksheet.PrinterSettings.TopMargin = 0.25M;
            worksheet.PrinterSettings.BottomMargin = 0.25M;
            worksheet.PrinterSettings.LeftMargin = 1M;
            worksheet.PrinterSettings.RightMargin = 0.25M;
            worksheet.PrinterSettings.Orientation = eOrientation.Portrait;
            worksheet.PrinterSettings.Scale = 82;   //  82% of the real page size.
            worksheet.View.ShowGridLines = false;
            worksheet.Cells.AutoFitColumns();
        }

        private static void MakeProductsWorksheet(ExcelPackage excel, OrderViewModel order)
        {
            var worksheet = excel.Workbook.Worksheets.Add(order.Name + " - produkty");
            worksheet.Cells.Style.Font.Name = "Segoe UI";

            for (int i = 0; i < 13; i++)
                worksheet.Cells[$"A{i + 1}:D{i + 1}"].Merge = true;

            worksheet.Cells["A1"].Value = "Zakład Produkcyjno Handlowy";
            worksheet.Cells["A1"].Style.Font.Bold = true;
            worksheet.Cells["A1"].Style.Font.Size = 14;

            worksheet.Cells["A2"].Value = "P R O D M A P O L s.c.";
            worksheet.Cells["A2"].Style.Font.Size = 14;
            worksheet.Cells["A2"].Style.Font.Bold = true;

            worksheet.Cells["A3"].Value = "ul. Cmentarna 11/14; 98-270 Złoczew";
            worksheet.Cells["A3"].Style.Font.Italic = true;

            worksheet.Cells["A5"].Value = ":: PRODUKTY ::";
            worksheet.Cells["A5"].Style.Font.Size = 14;
            worksheet.Cells["A5"].Style.Font.Bold = true;
            worksheet.Cells["A5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            worksheet.Cells["A7"].Value = $"Nazwa zlecenia: {order.Name.Trim()}";
            worksheet.Cells["A7"].Style.Font.Bold = true;
            worksheet.Cells["A7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            worksheet.Cells["A8"].Value = $"Numer zlecenia: {order.OrderNo.Trim()}";
            worksheet.Cells["A8"].Style.Font.Bold = true;
            worksheet.Cells["A8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            worksheet.Cells["A14"].Value = "Lp.";
            worksheet.Cells["B14"].Value = "Nazwa produktu";
            worksheet.Cells["C14"].Value = "Miara";
            worksheet.Cells["D14"].Value = "Ilość";
            worksheet.Cells["A14:D14"].Style.Font.Bold = true;
            worksheet.Cells["A14"].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            worksheet.Cells["B14"].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            worksheet.Cells["C14"].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            worksheet.Cells["D14"].Style.Border.BorderAround(ExcelBorderStyle.Thin);

            var products = order.Recipes
                .Select(r => new
                {
                    Lp = r.No,
                    ProductName = r.ProductName,
                    MeasureName = r.MeasureName,
                    MeasureCount = r.MeasureCount
                });

            worksheet.Cells["A15"].LoadFromCollection(products, false, TableStyles.Light1);
            int gridStartLine = 15;
            for (var i = gridStartLine; i < products.Count() + gridStartLine; i++)
            {
                worksheet.Cells[$"A{i}"].Style.Font.Bold = true;
                worksheet.Cells[$"A{i}"].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[$"B{i}"].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[$"C{i}"].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[$"D{i}"].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            }

            var groupedByMeasure = products.GroupBy(m => m.MeasureName).Select(m => new
            {
                MeasureName = m.Key,
                MeasureCount = m.Sum(mount => mount.MeasureCount)
            }).ToList();

            var sumGroupedByMeasure = string.Join(" ", groupedByMeasure.Select(g => "[ " + Math.Round(g.MeasureCount, 2).ToString() + g.MeasureName + " ]"));
            var sumUnitIndependent = Math.Round(products.Sum(m => m.MeasureCount), 2).ToString();

            worksheet.Cells["A11"].Value = $"Razem: {sumGroupedByMeasure}";
            worksheet.Cells["A11"].Style.Font.Bold = true;
            worksheet.Cells["A11"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            worksheet.Cells["A12"].Value = $"Razem bez jednostek: {sumUnitIndependent}";
            worksheet.Cells["A12"].Style.Font.Bold = true;
            worksheet.Cells["A12"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            worksheet.PrinterSettings.TopMargin = 0.25M;
            worksheet.PrinterSettings.BottomMargin = 0.25M;
            worksheet.PrinterSettings.LeftMargin = 1M;
            worksheet.PrinterSettings.RightMargin = 0.25M;
            worksheet.PrinterSettings.Orientation = eOrientation.Portrait;
            worksheet.PrinterSettings.Scale = 82;   //  82% of the real page size.
            worksheet.View.ShowGridLines = false;
            worksheet.Cells.AutoFitColumns();
        }
    }
}
