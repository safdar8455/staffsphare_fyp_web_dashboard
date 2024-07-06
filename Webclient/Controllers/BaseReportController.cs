using Ems.BusinessTracker.Common.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using Webclient.Filters;

namespace Webclient.Controllers
{
    public class BaseReportController : Controller
    {
        protected readonly UserSessionModel _userInfo;
        public BaseReportController()
        {
            _userInfo = SessionHelper.GetCurrentUser();
        }


        protected ActionResult ViewReportFormat(LocalReport localReport)
        {
            string reportType = "pdf";
            string mimeType;
            string encoding;
            string fileNameExtension;           
            string deviceInfo =
               "<DeviceInfo>" +
                "  <OutputFormat>PDF</OutputFormat>" +
                "  <PageWidth>8.25in</PageWidth>" +
                "  <PageHeight>11.6in</PageHeight>" +
                "  <MarginTop>0.5in</MarginTop>" +
                "  <MarginLeft>0.5in</MarginLeft>" +
                "  <MarginRight>0.5in</MarginRight>" +
                "  <MarginBottom>0.5in</MarginBottom>" +
                "</DeviceInfo>";
            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;
            //Render the report             
            renderedBytes = localReport.Render(
                reportType,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);
            return File(renderedBytes, mimeType);
        }

        protected ActionResult PrintReportFormat(LocalReport localReport, string topHeaderHeight)
        {
            string reportType = "pdf";
            string mimeType;
            string encoding;
            string fileNameExtension;           
            string deviceInfo =
                "<DeviceInfo>" +
                "  <OutputFormat>PDF</OutputFormat>" +
                "  <PageWidth>8.25in</PageWidth>" +
                "  <PageHeight>11.6in</PageHeight>" +
                "  <MarginTop>" + topHeaderHeight + "in</MarginTop>" +
                "  <MarginLeft>0.75in</MarginLeft>" +
                "  <MarginRight>0.01in</MarginRight>" +
                "  <MarginBottom>0.5in</MarginBottom>" +
                "</DeviceInfo>";
            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;
            //Render the report             
            renderedBytes = localReport.Render(
                reportType,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);

            var doc = new Document();
            var reader = new PdfReader(renderedBytes);
            using (var fs = new FileStream(Server.MapPath("~/Reports/Summary.pdf"), FileMode.Create)) 
            {   
              var stamper = new PdfStamper(reader, fs);
              string Printer = ""; //PrinterName(Convert.ToInt32(Session["localOutletID"]));
              // This is the script for automatically printing the pdf in acrobat viewer
              stamper.JavaScript= "var pp = getPrintParams();pp.interactive =pp.constants.interactionLevel.automatic;pp.printerName = " +Printer + ";print(pp);\r";
              stamper.Close();
            }           
            reader.Close();
            var fss = new FileStream(Server.MapPath("~/Reports/Summary.pdf"), FileMode.Open);
            byte[] bytes = new byte[fss.Length];
            fss.Read(bytes, 0, Convert.ToInt32(fss.Length));
            fss.Close();
            System.IO.File.Delete(Server.MapPath("~/Reports/Summary.pdf"));
            return File(bytes, "application/pdf");
        }

        protected ActionResult ViewReportFormatLandScape(LocalReport localReport)
        {
            string reportType = "pdf";
            string mimeType;
            string encoding;
            string fileNameExtension;
            //The DeviceInfo settings should be changed based on the reportType             
            //http://msdn2.microsoft.com/en-us/library/ms155397.aspx             
            string deviceInfo =
                "<DeviceInfo>" +
                "  <OutputFormat>PDF</OutputFormat>" +
                "  <PageWidth>11.69in</PageWidth>" +
                "  <PageHeight>8.27in</PageHeight>" +
                "  <MarginTop>0.5in</MarginTop>" +
                "  <MarginLeft>0.5in</MarginLeft>" +
                "  <MarginRight>0.5in</MarginRight>" +
                "  <MarginBottom>0.5in</MarginBottom>" +
                "</DeviceInfo>";
            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;
            //Render the report             
            renderedBytes = localReport.Render(
                reportType,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);
            //Response.AddHeader("content-disposition", "attachment; filename=NorthWindCustomers." + fileNameExtension);             
            return File(renderedBytes, mimeType);
        }

        protected void ExportToExcelAsFormated<T>(List<T> list, string reportName, string title)
        {
            var fileName = "filename=" + reportName + ".xls";
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment; " + fileName);
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            WriteHtmlTable(list, Response.Output, title);
            Response.End();
        }

        private void WriteHtmlTable<T>(IEnumerable<T> data, TextWriter output, string title)
        {
            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter htw = new HtmlTextWriter(sw))
                {
                    var table = GetHtmlTableIfExcel(data, htw, title);
                    output.Write(sw.ToString());
                }
            }

        }

        protected void SaveExcelFile<T>(IEnumerable<T> data, string locationPath, string fileName, string title)
        {
            var filePath = Path.Combine(locationPath, fileName);
            var streamWriter = new StreamWriter(filePath);
            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter htw = new HtmlTextWriter(sw))
                {
                    var table = GetHtmlTableIfExcel(data, htw, title);
                    streamWriter.Write(sw.ToString());
                }
            }
            streamWriter.Close();
        }

        private Table GetHtmlTableIfExcel<T>(IEnumerable<T> data, HtmlTextWriter htw, string title)
        {
            Table table = new Table();
            TableRow row = new TableRow();
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));

            foreach (PropertyDescriptor prop in props)
            {
                if (prop.IsBrowsable)
                {
                    TableHeaderCell hcell = new TableHeaderCell();
                    var dName = prop.DisplayName;
                    hcell.Text = string.IsNullOrEmpty(dName) ? prop.Name : dName;
                    hcell.BackColor = System.Drawing.Color.Maroon;
                    hcell.BorderStyle = BorderStyle.Solid;
                    hcell.BorderWidth = Unit.Pixel(1);
                    hcell.BorderColor = System.Drawing.Color.Maroon;
                    hcell.ForeColor = System.Drawing.Color.White;
                    row.Cells.Add(hcell);
                }
            }
            table.Rows.Add(row);


            foreach (T item in data)
            {
                row = new TableRow();
                foreach (PropertyDescriptor prop in props)
                {
                    if (prop.IsBrowsable)
                    {
                        TableCell cell = new TableCell();
                        if (!string.IsNullOrEmpty(prop.Description))
                        {
                            cell.Attributes.CssStyle.Add("mso-number-format", "\\@");
                        }
                        cell.Text = prop.Converter.ConvertToString(prop.GetValue(item));
                        cell.BorderStyle = BorderStyle.Solid;
                        cell.BorderWidth = Unit.Pixel(1);
                        cell.BorderColor = System.Drawing.Color.Gray;
                        row.Cells.Add(cell);
                    }
                    table.Rows.Add(row);
                }
            }
            if (!string.IsNullOrEmpty(title))
            {
                htw.Write(string.Format("<div style='text-align:left;font-size:25px;margin-top:20px;margin-bottom:20px;'>{0}</div>", title));
            }

            table.RenderControl(htw);
            return table;
        }


    }
}
