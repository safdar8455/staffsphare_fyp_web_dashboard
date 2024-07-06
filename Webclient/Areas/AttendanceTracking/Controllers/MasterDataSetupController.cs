using Ems.BusinessTracker.Common;
using Microsoft.Reporting.WebForms;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Ems.AttendanceTracking.Models;
using Ems.AttendanceTracking.Services;
using Webclient.Controllers;
using Webclient.Filters;
using Webclient.Helpers;
using Webclient.Models;

namespace Webclient.Areas.AttendanceTracking.Controllers
{
    [SessionHelper]
    public class MasterDataSetupController : BaseReportController
    {
        private readonly SetupInputHelpBusiness _setupBusiness;
        public MasterDataSetupController()
        {
            _setupBusiness = new SetupInputHelpBusiness();
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GenerateQrCode()
        {
            return View();
        }
        public ActionResult Create()
        {
            return PartialView();
        }

        [HttpGet]
        public JsonResult GetAll(GridSettings grid)
        {

            var query = _setupBusiness.GetAll().AsQueryable();
            var listOfFilteredData = FilterHelper.JQGridFilter(query, grid).AsQueryable();
            var listOfPagedData = FilterHelper.JQGridPageData(listOfFilteredData, grid);
            var count = listOfFilteredData.Count();
            var result = new
            {
                total = (int)Math.Ceiling((double)count / grid.PageSize),
                page = grid.PageIndex,
                records = count,
                rows = listOfPagedData
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetAllInputHelpType()
        {
            var textValueInputHelpList = Enum.GetValues(typeof(InputHelpType)).Cast<InputHelpType>().Select(v => new NameValueModel
            {
                Name = EnumUtility.GetDescriptionFromEnumValue((InputHelpType)v),
                Value = (int)v,

            }).ToList().OrderBy(c => c.Name);
            return Json(textValueInputHelpList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GenerateQrCodeToPdf(string model)
        {
            List<string> EmployeeIdList;
            EmployeeIdList = model.Split(',').ToList();
            int pdfSerialNo = 1;

            var empList = new AttendanceReportBusiness().GetQrCodeEmployee();

            var qrCodeModel = new List<EmployeeExportModel>();

            foreach (var item in EmployeeIdList)
            {
                var data = empList.Where(x => x.Id == Convert.ToInt64(item)).FirstOrDefault();

                data.PdfSerialNo = pdfSerialNo;
                if (string.IsNullOrEmpty(data.QrCodeNo))
                {
                    data.QrCodeNo = item;
                    _setupBusiness.UpdateQrCode(data);
                }

                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(data.QrCodeNo, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);
                using (Bitmap bitMap = qrCode.GetGraphic(20))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        bitMap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        byte[] byteImage = ms.ToArray();
                        data.QrCode = byteImage;
                    }
                }
                pdfSerialNo += 1;

                string imageUrl = data.ImagePath;

                if (!string.IsNullOrEmpty(data.ImageFileName))
                {
                    using (var webClient = new WebClient())
                    {
                        try
                        {
                            data.EmployeeImageData = webClient.DownloadData(new Uri(Server.MapPath(imageUrl)).AbsoluteUri);
                        }
                        catch (Exception e)
                        {

                        }
                    }
                }

                qrCodeModel.Add(data);
            }
            return QrCodePdf(qrCodeModel);
        }

        private ActionResult QrCodePdf(List<EmployeeExportModel> printableLists)
        {
            int reminder = 0;
            int totalRecords = printableLists.Count();
            if ((totalRecords % 2) == 1)
                reminder += 1;
            int perDataSetRecord = (totalRecords / 2) + reminder;

            var localReport = new LocalReport { ReportPath = Server.MapPath("~/Areas/AttendanceTracking/Reports/EmployeeQrCodeReport.rdlc") };

            localReport.DataSources.Add(new ReportDataSource("DataSet1", printableLists.Take(perDataSetRecord)));
            localReport.DataSources.Add(new ReportDataSource("DataSet2", printableLists.Skip(perDataSetRecord).Take(perDataSetRecord + 2)));

            return ViewReportFormat(localReport);
        }
        [HttpPost]
        public ActionResult SaveInputHelp(SetupInputHelpModel model)
        {
            return Json(_setupBusiness.Save(model));
        }
        [HttpGet]
        public JsonResult GetInputHelp(int id)
        {
            return Json(_setupBusiness.GetAll().FirstOrDefault(x => x.Id == id), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult DeleteInputHelp(int id)
        {
            var response = _setupBusiness.DeleteInputHelp(id);
            return Json(response, JsonRequestBehavior.AllowGet);
        }

    }
}
