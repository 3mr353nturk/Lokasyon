using AWSServerless_Google_Geocoding_Mvc.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace AWSServerless_Google_Geocoding_Mvc.Controllers
{
    [ControlLogin]
    public class HomeController : Controller
    {
        LocationDBEntities _dbContext = new LocationDBEntities();

        [Route("anasayfa")]
        public ActionResult Index()
        {
            return View();
        }

        //[System.Web.Http.Authorize]
        [System.Web.Mvc.HttpPost]
        public ActionResult InsertMap(string Address, string Latitude, string Longitude)
        {
            Map map = new Map();

            // Insert record
            if (map.Id == 0)
            {
                map.Address = Address;
                map.Latitude = Latitude;
                map.Longitude = Longitude;
                _dbContext.Map.Add(map);
                _dbContext.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            else
            {
                // Update
                var updateRow = _dbContext.Map.Where(x => x.Address == Address).FirstOrDefault();
                updateRow.Address = Address;
                updateRow.Latitude = Latitude;
                updateRow.Longitude = Longitude;
                _dbContext.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult Upload(FormCollection formCollection)
        {

            var mapList = new List<AWSServerless_Google_Geocoding_Mvc.Models.Map>();
            if (Request != null)
            {
                HttpPostedFileBase file = Request.Files["FileUpload"];
                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    string fileName = file.FileName;
                    string fileContentType = file.ContentType;
                    byte[] fileBytes = new byte[file.ContentLength];
                    var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
                    using (var package = new ExcelPackage(file.InputStream))
                    {
                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                        var currentSheet = package.Workbook.Worksheets;
                        var workSheet = currentSheet.First();
                        var noOfCol = workSheet.Dimension.End.Column;
                        var noOfRow = workSheet.Dimension.End.Row;
                        for (int rowIterator = 1; rowIterator <= noOfRow; rowIterator++)
                        {
                            var map = new AWSServerless_Google_Geocoding_Mvc.Models.Map();

                            map.UserID = null;
                            map.Name = null;
                            map.Latitude = workSheet.Cells[rowIterator, 3].Value.ToString();
                            map.Longitude = workSheet.Cells[rowIterator, 4].Value.ToString();
                            map.Address = workSheet.Cells[rowIterator, 5].Value.ToString();

                            mapList.Add(map);
                        }
                    }
                }
            }
            using (LocationDBEntities db = new LocationDBEntities())
            {
                foreach (var item in mapList)
                {
                    db.Map.Add(item);
                }
                db.SaveChanges();

            }
            return RedirectToAction("Index", "Home");
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult UploadMultipleFiles(IEnumerable<HttpPostedFileBase> FileUpload)
        //{
        //    int count = 0;

        //    if (FileUpload != null)
        //    {
        //        foreach (var file in FileUpload)
        //        {
        //            if (file != null && file.ContentLength > 0)
        //            {
        //                var mapList = new List<AWSServerless_Google_Geocoding_Mvc.Models.Map>();
        //                var postedFile = Request.Files[0];

        //                StreamReader sr = new StreamReader(postedFile.InputStream);
        //                StringBuilder sb = new StringBuilder();
        //                //DataTable dt = new DataTable();
        //                //DataRow dr;
        //                string s;
        //                int j = 0;

        //                while (!sr.EndOfStream)
        //                {
        //                    while ((s = sr.ReadLine()) != null)
        //                    {
        //                        //Ignore first row as it consists of headers
        //                        if (j > 0)
        //                        {
        //                            //string[] str = s.Split(',');

        //                            using (var package = new ExcelPackage(file.InputStream))
        //                            {
        //                                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        //                                var currentSheet = package.Workbook.Worksheets;
        //                                var workSheet = currentSheet.First();
        //                                var noOfCol = workSheet.Dimension.End.Column;
        //                                var noOfRow = workSheet.Dimension.End.Row;
        //                                for (int rowIterator = 1; rowIterator <= noOfRow; rowIterator++)
        //                                {
        //                                    var map = new AWSServerless_Google_Geocoding_Mvc.Models.Map();

        //                                    map.UserID = null;
        //                                    map.Name = null;
        //                                    map.Latitude = workSheet.Cells[rowIterator, 3].Value.ToString();
        //                                    map.Longitude = workSheet.Cells[rowIterator, 4].Value.ToString();
        //                                    map.Address = workSheet.Cells[rowIterator, 5].Value.ToString();

        //                                    mapList.Add(map);
        //                                }
        //                            }
        //                        }
        //                        j++;
        //                    }
        //                }
        //                // Save to database
        //                using (LocationDBEntities db = new LocationDBEntities())
        //                {
        //                    foreach (var item in mapList)
        //                    {
        //                        db.Map.Add(item);
        //                    }
        //                    db.SaveChanges();
        //                }
        //                sr.Close();
        //                count++;
        //            }
        //        }
        //    }
        //    return RedirectToAction("Index", "Home");
        //}

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}